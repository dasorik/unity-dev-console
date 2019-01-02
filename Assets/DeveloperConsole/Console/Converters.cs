using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using DeveloperConsole.Utilities;

namespace DeveloperConsole
{
	public interface IConverter
	{
		bool CanConvert(string parameterString, out string parameter, out string subString);
		object Convert(string value);
	}


	public abstract class BaseConverter : IConverter
	{
		protected const string BOUNDRY_TERMINATOR = @"(?=$|\s)";

		protected string matchPattern;

		public virtual bool CanConvert(string parameterString, out string parameter, out string subString)
		{
			var match = Regex.Match(parameterString, matchPattern);

			if (match.Length > 0)
			{
				parameter = match.Value;
				subString = parameterString.Remove(0, parameter.Length);
				return true;
			}

			parameter = string.Empty;
			subString = parameterString;
			return false;
		}

		public abstract object Convert(string value);

		object IConverter.Convert(string value)
		{
			return Convert(value);
		}
	}


	public class StringConverter : BaseConverter
	{
		string extractionPattern;

		public StringConverter()
		{
			matchPattern = $@"^\""(?:\\.|[^\""])*\""{BOUNDRY_TERMINATOR}";
			Debug.Log(matchPattern);
			extractionPattern = "(?<=[\"]).*(?=[\"])";
		}

		public override object Convert(string value)
		{
			return Regex.Match(value, extractionPattern).Value;
		}
	}


	public class BoolConverter : BaseConverter
	{
		public BoolConverter()
		{
			matchPattern = @"^(\b[Ff]alse\b|\b[Tt]rue\b)";
		}

		public override object Convert(string value)
		{
			return value.ToLower() == "true" ? true : false;
		}
	}


	public class CharConverter : BaseConverter
	{
		string extractionPattern;

		public CharConverter()
		{
			matchPattern = $@"^[\'].[\']{BOUNDRY_TERMINATOR}";
			extractionPattern = @"(?<=\').(?=\')";
		}

		public override object Convert(string value)
		{
			return System.Convert.ToChar(Regex.Match(value, extractionPattern));
		}
	}


	public class IntConverter : BaseConverter
	{
		public IntConverter()
		{
			matchPattern = $@"^-?\d+{BOUNDRY_TERMINATOR}";
		}

		public override object Convert(string value)
		{
			return System.Convert.ToInt32(value);
		}
	}


	public class FloatConverter : BaseConverter
	{
		public FloatConverter()
		{
			matchPattern = $@"^-?\d+\.\d+{BOUNDRY_TERMINATOR}";
		}

		public override object Convert(string value)
		{
			return System.Convert.ToSingle(value);
		}
	}


	public abstract class EnclosedBracketConverter : BaseConverter
	{
		string splitPattern;
		protected int matchCount;

		protected EnclosedBracketConverter(string prefix, char bracketLeft, char bracketRight, int requiredMatches, bool escapeBrackets)
		{
			this.matchCount = requiredMatches;

			string internalMatches = string.Empty;
			for (int i = 0; i < requiredMatches; i++)
			{
				internalMatches += "[^,]*";
				if (i != requiredMatches - 1)
					internalMatches += ",";
			}

			string escape = escapeBrackets ? "\\" : string.Empty;
			matchPattern = $@"^{prefix}{escape}{bracketLeft}{internalMatches}{escape}{bracketRight}{BOUNDRY_TERMINATOR}";
			splitPattern = @"-?\d+(\.?\d+)?";
		}

		float[] GetValues(string value)
		{
			var collection = Regex.Matches(value, splitPattern);
			float[] values = new float[collection.Count];

			for (int i = 0; i < collection.Count; i++)
			{
				values[i] = System.Convert.ToSingle(collection[i].Value);
			}

			return values;
		}

		public bool CanConvert(string value)
		{
			return Regex.IsMatch(value, matchPattern);
		}

		public override object Convert(string value)
		{
			return OnConvert(GetValues(value));
		}

		protected abstract object OnConvert(float[] values);
	}


	public class Vector2Converter : EnclosedBracketConverter
	{
		public Vector2Converter() : base(string.Empty, '(', ')', 2, true) { }

		protected override object OnConvert(float[] values)
		{
			float x = values[0];
			float y = values[1];

			return new Vector2(x, y);
		}
	}


	public class Vector3Converter : EnclosedBracketConverter
	{
		public Vector3Converter() : base(string.Empty, '(', ')', 3, true) { }

		protected override object OnConvert(float[] values)
		{
			float x = values[0];
			float y = values[1];
			float z = values[2];

			return new Vector3(x, y, z);
		}
	}


	public class Vector4Converter : EnclosedBracketConverter
	{
		public Vector4Converter() : base(string.Empty, '(', ')', 4, true) { }

		protected override object OnConvert(float[] values)
		{
			float x = values[0];
			float y = values[1];
			float z = values[2];
			float w = values[3];

			return new Vector4(x, y, z, w);
		}
	}


	public class RGBColorConverter : EnclosedBracketConverter
	{
		public RGBColorConverter() : base("rgb", '(', ')', 3, true) { }

		protected override object OnConvert(float[] values)
		{
			float r = values[0];
			float g = values[1];
			float b = values[2];

			return new Color(r, g, b, 1f);
		}
	}


	public class RGBAColorConverter : EnclosedBracketConverter
	{
		public RGBAColorConverter() : base("rgba", '(', ')', 4, true) { }

		protected override object OnConvert(float[] values)
		{
			float r = values[0];
			float g = values[1];
			float b = values[2];
			float a = values[3];

			return new Color(r, g, b, a);
		}
	}


	public class HTMLColorConverter : BaseConverter
	{
		public HTMLColorConverter()
		{
			matchPattern = $@"^#([0-9A-Fa-f]{{6}}|[0-9A-Fa-f]{{8}}){BOUNDRY_TERMINATOR}";
		}

		public override bool CanConvert(string parameterString, out string parameter, out string subString)
		{
			return base.CanConvert(parameterString, out parameter, out subString) || IsLiteralColor(parameterString, out parameter, out subString);
		}

		bool IsLiteralColor(string parameterString, out string parameter, out string subString)
		{
			string colorName = parameterString.Split(' ')[0];
			if (Colors.IsLiteralColor(colorName))
			{
				parameter = colorName;
				subString = parameterString.Remove(0, colorName.Length);
				return true;
			}

			parameter = string.Empty;
			subString = parameterString;
			return false;
		}

		public override object Convert(string value)
		{
			Color color;
			ColorUtility.TryParseHtmlString(value, out color);

			return color;
		}
	}

	public class GameObjectConverter : BaseConverter
	{
		public GameObjectConverter()
		{
			matchPattern = $@"^\{{.+?\}}{BOUNDRY_TERMINATOR}";
		}

		public override object Convert(string value)
		{
			string gameObjectName = value.Substring(1, value.Length - 2);
			var gameObject = GameObject.Find(gameObjectName);

			if (gameObject == null)
				DeveloperConsole.LogError($"Unable to find a game object with the name '{gameObjectName}'");

			return gameObject;
		}
	}

	public class NullConverter : BaseConverter
	{
		public NullConverter()
		{
			matchPattern = @"^\b[Nn]ull\b";
		}

		public override object Convert(string value)
		{
			return null;
		}
	}
}