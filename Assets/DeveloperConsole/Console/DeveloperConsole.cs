#if DEBUG || DEVELOPER_CONSOLE_ENABLED

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using DeveloperConsole.Extensions;
using DeveloperConsole.Utilities;

namespace DeveloperConsole
{
	public enum MessageType
	{
		Info,
		Warning,
		Error,
		Success
	}

	public class DeveloperConsole
	{
		struct ConsoleVariable
		{
			public readonly string name;

			public ConsoleVariable(string name)
			{
				this.name = name;
			}

			public static implicit operator string(ConsoleVariable name) => name.name;
			public static implicit operator ConsoleVariable(string name) => new ConsoleVariable(name);
		}

		class CommandInfo
		{
			public readonly string helpText;
			public readonly string fullName;
			public readonly MethodInfo method;
			public readonly int parameterCount;
			public readonly ParameterInfo[] parameters;

			public CommandInfo(MethodInfo method)
			{
				this.method = method;

				CommandAttribute attribute;
				if (method.HasAttribute(false, out attribute))
				{
					this.helpText = attribute.HelpText;
					this.fullName = ConsoleHelper.GetCommandFullName(method, attribute);
				}
				else
				{
					throw new System.Exception("Tried to add a command for a method that does not have a [Command] attribute");
				}

				this.parameters = method.GetParameters();
				this.parameterCount = parameters.Length;
			}

			public Type ReturnType { get { return method.ReturnType; } }

			public object Invoke(params object[] args)
			{
				return method.Invoke(null, args);
			}
		}

		public struct CommandMatch
		{
			public readonly string fullname;
			public readonly string helpText;
			public readonly int partialDistance;
			public readonly int fullDistance;

			public CommandMatch(string fullname, string helpText, int partialDistance, int fullDistance)
			{
				this.fullname = fullname;
				this.helpText = helpText;
				this.partialDistance = partialDistance;
				this.fullDistance = fullDistance;
			}
		}

		class VariableConverter : BaseConverter
		{
			public VariableConverter()
			{
				matchPattern = @"^[a-zA-Z][a-zA-Z0-9]*";
			}

			public override bool CanConvert(string parameterString, out string parameter, out string subString)
			{
				return base.CanConvert(parameterString, out parameter, out subString)
					&& ValidateVariableName(parameter);
			}

			public override object Convert(string value)
			{
				return new ConsoleVariable(value);
			}
		}


		const int MATCH_THRESHOLD = 3;

		static List<IConverter> converters;
		static IConverter variableConverter = new VariableConverter();

		static Dictionary<string, List<CommandInfo>> commands;
		static Dictionary<string, object> variables = new Dictionary<string, object>();

		static LinkedList<string> previousCommands = new LinkedList<string>();
		static int commandHistoryIndex = -1;

		static ConsoleManager manager;


		static DeveloperConsole()
		{
			manager = GameObject.FindObjectOfType<ConsoleManager>();

			if (manager == null)
			{
				GameObject managerObject = GameObject.Instantiate(Resources.Load<GameObject>("DeveloperConsole"));
				manager = managerObject.GetComponent<ConsoleManager>();
			}
			
			LoadCommands();
			LoadConverters();
		}

		public static void Open()
		{

		}

		public static void Close()
		{

		}

		public static void Log(string message, MessageType messageType)
		{
			switch (messageType)
			{
				case MessageType.Info:
					Log(message); break;
				case MessageType.Error:
					LogError(message); break;
				case MessageType.Warning:
					LogWarning(message); break;
				case MessageType.Success:
					LogSuccess(message); break;
			}
		}

		public static void Log(string message)
		{
			manager.AddLog(message);
		}

		public static void LogError(string message)
		{
			manager.AddErrorLog(message);
		}

		public static void LogWarning(string message)
		{
			manager.AddWarningLog(message);
		}

		public static void LogSuccess(string message)
		{
			manager.AddSuccessLog(message);
		}

		static bool ValidateVariableName(string key, bool verbose = true)
		{
			string lowerKey = key.ToLower();

			if (lowerKey == "null")
			{
				if (verbose)
					LogError($"Value '{key}' cannot be used as a variable name. This is a special keyword");

				return false;
			}
			else if (lowerKey == "true" || lowerKey == "false")
			{
				if (verbose)
					LogError($"Value '{key}' cannot be used as a variable name. This is a boolean constant");

				return false;
			}
			else if (Colors.IsLiteralColor(key))
			{
				if (verbose)
					LogError($"Value '{key}' cannot be used as a variable name. This is a color literal");

				return false;
			}

			return true;
		}

		static void SetVariable(string key, object value)
		{
			if (ValidateVariableName(key))
			{
				variables.AddOrUpdateValue(key, value);
				LogSuccess($"Variable '{key}' has been set to value '{(value?.ToString() ?? "null")}'");
			}
		}

		static void RemoveVariable(string key)
		{
			if (ValidateVariableName(key))
			{
				if (variables.ContainsKey(key))
				{
					variables.Remove(key);
					LogSuccess($"Successfully removed variable '{key}'");
				}
				else
				{
					LogError($"Unable to remove variable '{key}', since it was never set");
				}
			}
		}

		static object GetVariable(string key)
		{
			if (ValidateVariableName(key))
			{
				if (variables.ContainsKey(key))
				{
					return variables[key];
				}
				else
				{
					LogError($"'{key}' has not been set");
				}
			}

			return null;
		}

		public static void Run(string commandString)
		{
			commandHistoryIndex = -1;
			previousCommands.AddFirst(commandString);

			while (previousCommands.Count > manager.MaxCommandHistory)
				previousCommands.RemoveLast();

			Log($"<b>{manager.ExecutionSymbol}</b> {commandString}");

			string commandName = commandString.Split(' ')[0];
			string parameterString = commandString.Remove(0, commandName.Length);

			List<CommandInfo> commandPool;

			if (!commands.TryGetValue(commandName, out commandPool))
			{
				LogError($"Unable to run command '{commandName}'. No such command exists");
				return;
			}

			object[] parameters;
			if (!Parse(parameterString, out parameters))
				return;

			var matchedCommands = new List<CommandInfo>();
			int maxScore = int.MinValue;
			int tempScore;

			foreach (var potentialCommand in commandPool)
			{
				if (TryGetMatchScore(potentialCommand, parameters, out tempScore))
				{
					if (tempScore > maxScore)
					{
						maxScore = tempScore;
						matchedCommands.Clear();
					}

					if (tempScore == maxScore)
						matchedCommands.Add(potentialCommand);
				}
			}

			if (matchedCommands.Count > 1)
			{
				LogError($"Unable to run command '{commandName}'. Could not distinguish between overloaded variations (conflicting commands: '{string.Join(", ", matchedCommands.Select(c => GetParameters(c)))}')");
				return;
			}

			if (matchedCommands.Count == 0)
			{
				LogError($"Unable to run command '{commandName}'. One of your parameters may be formatted incorrectly, or the number of supplied parameters did not match any available overloads");
				return;
			}

			try
			{
				for (int i = 0; i < matchedCommands[0].parameterCount; i++)
				{
					Type parameterObjectType = parameters[i]?.GetType();
					Type parameterMethodType = matchedCommands[0].parameters[i].ParameterType;

					if (parameterObjectType == typeof(ConsoleVariable) && parameterMethodType != typeof(ConsoleVariable))
					{
						string varName = ((ConsoleVariable)parameters[i]).name;

						if (variables.ContainsKey(varName))
							parameters[i] = variables[varName];
						else
						{
							LogError($"Variable '{varName}' has not been set, variables cannot be used before they are set");
							return;
						}
					}
				}

				matchedCommands[0].Invoke(parameters);
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
					LogError($"{ex.Message} | {ex.InnerException.Message}");
				else
					LogError(ex.Message);
			}
		}
		
		static bool TryGetMatchScore(CommandInfo command, object[] parameters, out int score)
		{
			score = 0;
			
			// Discard any matched with a different number of parameters
			if (command.parameterCount != parameters.Length)
				return false;

			for (int i = 0; i < command.parameterCount; i++)
			{
				Type parameterObjectType = parameters[i]?.GetType();
				Type parameterMethodType = command.parameters[i].ParameterType;
				
				if (parameterObjectType == typeof(ConsoleVariable) && parameterMethodType != typeof(ConsoleVariable))
				{
					string varName = ((ConsoleVariable)parameters[i]).name;

					if (variables.ContainsKey(varName))
						parameterObjectType = variables[varName]?.GetType();
				}

				// Exact matches are ranked higher
				if (parameterMethodType == parameterObjectType)
					score += 2;

				// Partial matches are ranked slightly lower, but still valid
				else if (parameterMethodType.IsAssignableFrom(parameterObjectType))
					score += 1;
				
				// Could be valid in certain circumstances
				else if (parameterObjectType == null && !parameterMethodType.IsPrimitive)
					score += 0;

				else
					return false;
			}

			return true;
		}

		static bool Parse(string parameterString, out object[] parameters)
		{
			var parameterList = new List<object>();
			
			string splitParameter;
			string tempParameterString;

			while (!string.IsNullOrWhiteSpace(parameterString))
			{
				parameterString = parameterString.TrimStart();

				foreach (IConverter converter in converters)
				{
					if (converter.CanConvert(parameterString, out splitParameter, out tempParameterString))
					{
						try
						{
							parameterList.Add(converter.Convert(splitParameter));
						}
						catch (Exception ex)
						{
							LogError(ex.Message);
						}

						goto nextParameter;
					}
				}

				LogError($"'{parameterString}' was unable to be resolved to a value, one of your parameters may be malformed");
				parameters = new object[0];
				return false;

				nextParameter:;
				parameterString = tempParameterString;
			}

			parameters = parameterList.ToArray();
			return true;
		}

		static bool IsVariableType(Type type)
		{
			return type == typeof(ConsoleVariable);
		}

		public static string GetPreviousCommand(bool forward)
		{
			commandHistoryIndex += forward ? 1 : -1;
			commandHistoryIndex = Mathf.Clamp(commandHistoryIndex, 0, previousCommands.Count - 1);

			return previousCommands.Skip(commandHistoryIndex).FirstOrDefault() ?? string.Empty;
		}

		static void LoadCommands()
		{
			commands = new Dictionary<string, List<CommandInfo>>();

			Type[] assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
			Type[] commandLibraries = assemblyTypes.Where(t => typeof(ICommandLibrary).IsAssignableFrom(t)).ToArray();

			foreach (var library in commandLibraries)
			{
				Debug.Log($"Loading Commands: {library.Name}");
				var methods = library.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.HasAttribute<CommandAttribute>(false));
				
				foreach (var method in methods)
				{
					CommandInfo command = new CommandInfo(method);

					if (!method.IsStatic)
					{
						LogError($"Unable to load command '{command.fullName}'. Please ensure the [Command] attribute is only placed on static methods");
						continue;
					}

					List<CommandInfo> commandList;
					if (!commands.TryGetValue(command.fullName, out commandList))
					{
						commandList = new List<CommandInfo>();
						commands.Add(command.fullName, commandList);
					}

					commandList.Add(command);
				}
			}
		}

		static void LoadConverters()
		{
			converters = new List<IConverter>();

			converters.Add(new NullConverter());
			converters.Add(new FloatConverter());
			converters.Add(new StringConverter());
			converters.Add(new BoolConverter());
			converters.Add(new CharConverter());
			converters.Add(new IntConverter());
			converters.Add(new Vector2Converter());
			converters.Add(new Vector3Converter());
			converters.Add(new Vector4Converter());
			converters.Add(new RGBColorConverter());
			converters.Add(new RGBAColorConverter());
			converters.Add(new HTMLColorConverter());
			converters.Add(new GameObjectConverter());

			converters.Add(new VariableConverter());
		}

		public static CommandMatch[] GetAllCommands()
		{
			List<CommandMatch> list = new List<CommandMatch>();
			foreach (var commandPair in commands)
				list.Add(new CommandMatch(commandPair.Value[0].fullName, commandPair.Value[0].helpText, 0, 0));

			var ordered = list.OrderBy(match => match.fullname).ThenBy(match => match.fullname);
			return ordered.ToArray();
		}

		public static CommandMatch[] GetPotentialCommands(string partialCommand)
		{
			List<CommandMatch> list = new List<CommandMatch>();

			foreach (var commandPair in commands)
			{
				int fullDistance = partialCommand.LevenshteinDistanceTo(commandPair.Key);
				int partialDistance = partialCommand.LevenshteinDistanceTo(commandPair.Key.Substring(0, Mathf.Min(commandPair.Key.Length, partialCommand.Length + 1)));

				if (partialDistance < MATCH_THRESHOLD)
					list.Add(new CommandMatch(commandPair.Value[0].fullName, commandPair.Value[0].helpText, partialDistance, fullDistance));
			}

			var ordered = list.OrderBy(match => match.partialDistance).ThenBy(match => match.fullDistance);
			return ordered.ToArray();
		}

		public static string[] GetPotentialParameters(string commandName)
		{
			if (!commands.ContainsKey(commandName))
				return new string[0];

			List<string> commandLists = new List<string>();
			foreach (var command in commands[commandName])
				commandLists.Add(GetParameters(command));

			return commandLists.ToArray();
		}

		static string GetParameters(CommandInfo command)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("<color=yellow>");
			builder.Append(command.fullName);
			builder.Append("</color>");
			builder.Append(' ');

			for (int i = 0; i < command.parameterCount; i++)
			{
				builder.Append("[<color=cyan>");
				builder.Append(command.parameters[i].Name);
				builder.Append("</color>: ");
				builder.Append(command.parameters[i].ParameterType.Name);
				builder.Append(']');

				if (i < command.parameterCount - 1)
					builder.Append(' ');
			}

			return builder.ToString();
		}

		class VariableCommands : ICommandLibrary
		{
			//[Command("set", "Set a value that can be used again later")]
			//static void Set(ConsoleVariable key, ConsoleVariable value)
			//{
			//	DeveloperConsole.SetVariable(key, DeveloperConsole.GetVariable(value));
			//}

			[Command("set", "Set a value that can be used again later")]
			static void Set(ConsoleVariable key, object value)
			{
				DeveloperConsole.SetVariable(key, value);
			}

			[Command("unset", "Remove a previously set value")]
			static void Unset(ConsoleVariable key)
			{
				DeveloperConsole.RemoveVariable(key);
			}

			[Command("peek", "logs the contents of a variable to the console")]
			static void Peek(ConsoleVariable key)
			{
				var value = DeveloperConsole.GetVariable(key);

				if (value != null)
					DeveloperConsole.Log(value.ToString());
			}
		}
	}
}

#endif