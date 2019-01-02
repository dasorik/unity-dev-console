using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace DeveloperConsole
{
	public class ConsoleManager : MonoBehaviour
	{
		[Header("General")]
		[SerializeField] KeyCode consoleKey = KeyCode.BackQuote;
		[SerializeField, Range(0.05f, 1f)] float heightPercent = 0.33f;
		[SerializeField] int maxStorage = 200;
		[SerializeField] int maxCommandHistory = 100;
		[SerializeField] char executionSymbol = '>';

		[Header("SetUp")]
		[SerializeField] RectTransform mainPanel;
		[SerializeField] Text textTemplate;
		[SerializeField] InputField commandText;
		[SerializeField] RectTransform outputList;
		[SerializeField] RectTransform suggestionList;
		[SerializeField] Text executionSymbolText;

		[Header("Colors")]
		[SerializeField] Color defaultColor = Color.white;
		[SerializeField] Color errorColor = new Color(1f, 0.224f, 0.18f, 1f);
		[SerializeField] Color warningColor = new Color(1f, 0.918f, 0f, 1f);
		[SerializeField] Color successColor = new Color(0.584f, 1f, 0f, 1f);

		bool active = false;
		Text[] output;
		int index = 0;
		int focusFrame = 0;

		public int MaxCommandHistory => maxCommandHistory;
		public char ExecutionSymbol => executionSymbol;

		void Awake()
		{
			commandText.onValueChanged.AddListener(OnCommandChanged);

			output = new Text[maxStorage];
			executionSymbolText.text = executionSymbol.ToString();
			
			DontDestroyOnLoad(gameObject);
			EnableConsole(false);
		}

		void Update()
		{
			if (Input.GetKeyDown(consoleKey))
				EnableConsole(!active);

			focusFrame++;

			if (commandText.isFocused)
				focusFrame = 0;

			if (Input.GetKeyDown(KeyCode.Return))
			{
				if (focusFrame <= 1)
				{
					if (!string.IsNullOrEmpty(commandText.text))
						OnSubmit(commandText.text);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (!string.IsNullOrEmpty(commandText.text))
				{
					var potentialCommands = DeveloperConsole.GetPotentialCommands(commandText.text);

					if (potentialCommands.Length > 0)
						SetCommandText(potentialCommands[0].fullname);
				}
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				var command = DeveloperConsole.GetPreviousCommand(true);

				if (!string.IsNullOrEmpty(command))
					SetCommandText(command);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				var command = DeveloperConsole.GetPreviousCommand(false);

				if (!string.IsNullOrEmpty(command))
					SetCommandText(command);
			}
		}

		void SetCommandText(string command)
		{
			commandText.text = command;
			commandText.caretPosition = command.Length;
		}

		void EnableConsole(bool active)
		{
			this.active = active;
			mainPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * heightPercent);
			mainPanel.gameObject.SetActive(active);

			if (active)
				commandText.Select();
		}

		void AddLog(string text, Color color)
		{
			Text textComponent = GameObject.Instantiate(textTemplate.gameObject).GetComponent<Text>();
			textComponent.gameObject.SetActive(true);
			textComponent.transform.SetParent(outputList);
			textComponent.text = text;
			textComponent.color = color;

			if (output[index] != null)
				GameObject.Destroy(output[index].gameObject);

			output[index] = textComponent;
			index = (index + 1) % maxStorage;
		}

		public void AddLog(string text)
		{
			AddLog(text, defaultColor);
		}

		public void AddErrorLog(string text)
		{
			AddLog(text, errorColor);
		}

		public void AddWarningLog(string text)
		{
			AddLog(text, warningColor);
		}

		public void AddSuccessLog(string text)
		{
			AddLog(text, successColor);
		}

		void OnCommandChanged(string command)
		{
			foreach (Transform child in suggestionList.transform)
				GameObject.Destroy(child.gameObject);

			var matches = Regex.Matches(command, @"^[A-Za-z][A-za-z0-9-_]*(?=\s)");
			if (matches.Count > 0)
			{
				var parameterLists = DeveloperConsole.GetPotentialParameters(matches[0].Value);

				for (int i = 0; i < parameterLists.Length; i++)
					AddSuggestion(parameterLists[i]);

				suggestionList.gameObject.SetActive(parameterLists.Length > 0);
			}
			else
			{
				var commands = DeveloperConsole.GetPotentialCommands(command);
				
				for (int i = 0; i < commands.Length; i++)
				{
					string helpText = string.IsNullOrEmpty(commands[i].helpText) ? "<i>No help text provided</i>" : commands[i].helpText;
					string text = string.Format("<color=yellow>{0}</color> - {1}", commands[i].fullname, helpText);
					AddSuggestion(text);
				}

				suggestionList.gameObject.SetActive(commands.Length > 0);
			}
		}

		void AddSuggestion(string text)
		{
			Text textComponent = GameObject.Instantiate(textTemplate.gameObject).GetComponent<Text>();
			textComponent.gameObject.SetActive(true);
			textComponent.transform.SetParent(suggestionList);
			textComponent.text = text;
		}

		void OnSubmit(string command)
		{
			DeveloperConsole.Run(command);
			
			commandText.text = string.Empty;
			commandText.ActivateInputField();
			commandText.Select();

			suggestionList.gameObject.SetActive(false);
		}
	}
}