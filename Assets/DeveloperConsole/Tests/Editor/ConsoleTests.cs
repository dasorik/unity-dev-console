using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit;
using NUnit.Framework;
using DeveloperConsole;

public class ConsoleTests
{
	string substring, parameter;

	void CheckConverter(bool shouldMatch, IConverter converter, string testString, string expectedParameter = "", string expectedSubstring = "")
	{
		if (shouldMatch)
		{
			Assert.IsTrue(converter.CanConvert(testString, out parameter, out substring), $"{converter.GetType().Name} could not convert string '{testString}'");
			Assert.AreEqual(expectedParameter, parameter);
			Assert.AreEqual(expectedSubstring, substring);
		}
		else
		{
			Assert.IsFalse(converter.CanConvert(testString, out parameter, out substring));
		}
	}


	// String

	[Test]
	public void CheckStringConverterWhole()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"This is a test string\"";

		CheckConverter(true, converter, testString, "\"This is a test string\"", string.Empty);
	}

	[Test]
	public void CheckStringConverterSingle()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"A test string with extra parameters\" (0,0,0) value";

		CheckConverter(true, converter, testString, "\"A test string with extra parameters\"", " (0,0,0) value");
	}

	[Test]
	public void CheckStringConverterMultiple()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"A test string with another string present\" \"A second test string\"";

		CheckConverter(true, converter, testString, "\"A test string with another string present\"", " \"A second test string\"");
	}

	[Test]
	public void CheckStringConverterQuotesInside()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"This test string has \\\" inside of it\" \"A second test string\"";

		CheckConverter(true, converter, testString, "\"This test string has \\\" inside of it\"", " \"A second test string\"");
	}

	[Test]
	public void CheckStringConverterQuotesInsideAlt()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"This test string has\"inside of it\" \"A second test string\"";

		CheckConverter(false, converter, testString);
	}


	// Bool

	[Test]
	public void BoolConverterWholeFalseLowerCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "false";

		CheckConverter(true, converter, testString, "false", string.Empty);
	}

	[Test]
	public void BoolConverterWholeFalseUpperCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "False";

		CheckConverter(true, converter, testString, "False", string.Empty);
	}

	[Test]
	public void BoolConverterWholeTrueLowerCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "true";

		CheckConverter(true, converter, testString, "true", string.Empty);
	}

	[Test]
	public void BoolConverterWholeTrueUpperCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "True";

		CheckConverter(true, converter, testString, "True", string.Empty);
	}

	[Test]
	public void BoolConverterWholeTrueCapitals()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "TRUE";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void BoolConverterWholeFalseCapitals()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "FALSE";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void BoolConverterWholeRandom()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "(*&$(*)&(HJ uHIUHUI";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedStart()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "falseoidhzfIUHi4509uw9h8d";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedMiddle()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "oidhzfIUHfalsei4509uw9h8d";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedEnd()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "oidhzfIUHi4509uw9h8dfalse";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedMultiple()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "falseoidhzfIUHfalsei4509uw9h8dfalse";

		CheckConverter(false, converter, testString);
	}


	// Char

	[Test]
	public void CharConverterWhole()
	{
		CharConverter converter = new CharConverter();
		var testString = "'a'";

		CheckConverter(true, converter, testString, "'a'", string.Empty);
	}

	[Test]
	public void CharConverterWholeMultiChar()
	{
		CharConverter converter = new CharConverter();
		var testString = "'ab'";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void CharConverterMultiple()
	{
		CharConverter converter = new CharConverter();
		var testString = "'a' 34.3 'b'";

		CheckConverter(true, converter, testString, "'a'", " 34.3 'b'");
	}

	[Test]
	public void CharConverterString()
	{
		CharConverter converter = new CharConverter();
		var testString = "\"a\"";

		CheckConverter(false, converter, testString);
	}


	// Int

	[Test]
	public void IntConverterWhole()
	{
		IntConverter converter = new IntConverter();
		string testString = "346";

		CheckConverter(true, converter, testString, "346", string.Empty);
	}

	[Test]
	public void IntConverterWholeDecimal()
	{
		IntConverter converter = new IntConverter();
		string testString = "4563.23";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void IntConverterMalformedPre()
	{
		IntConverter converter = new IntConverter();
		string testString = "a873";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void IntConverterMalformedPost()
	{
		IntConverter converter = new IntConverter();
		string testString = "873d";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void IntConverterMultiple()
	{
		IntConverter converter = new IntConverter();
		string testString = "8743 45873";

		CheckConverter(true, converter, testString, "8743", " 45873");
	}


	// Float

	[Test]
	public void FloatConverterWhole()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "234.26";

		CheckConverter(true, converter, testString, "234.26", string.Empty);
	}

	[Test]
	public void FloatConverterPartial()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "234.26 \"Test String\"";

		CheckConverter(true, converter, testString, "234.26", " \"Test String\"");
	}

	[Test]
	public void FloatConverterMultiple()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "45423.8 \"Test String\" 546.2";

		CheckConverter(true, converter, testString, "45423.8", " \"Test String\" 546.2");
	}

	[Test]
	public void FloatConverterWholeMalformedPre()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "s45423.8";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void FloatConverterWholeMalformedPost()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "45423.8dfjgh";

		CheckConverter(false, converter, testString);
	}


	// Vector2

	[Test]
	public void Vector2ConverterWholeNoSpacesInt()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(1,2)";

		CheckConverter(true, converter, testString, "(1,2)", string.Empty);
	}

	[Test]
	public void Vector2ConverterWholeSpacesInt()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(6, 2)";

		CheckConverter(true, converter, testString, "(6, 2)", string.Empty);
	}

	[Test]
	public void Vector2ConverterWholeNoSpacesFloat()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5,3.4)";

		CheckConverter(true, converter, testString, "(5,3.4)", string.Empty);
	}

	[Test]
	public void Vector2ConverterWholeSpacesFloat()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5, 3.4)";

		CheckConverter(true, converter, testString, "(5, 3.4)", string.Empty);
	}

	[Test]
	public void Vector2ConverterWholeMalformedPre()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "fd(5,3.4)";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void Vector2ConverterWholeMalformedPost()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5,3.4)6u5rf";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void Vector2ConverterMultiple()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5,3.4) (1,2,3)";

		CheckConverter(true, converter, testString, "(5,3.4)", " (1,2,3)");
	}


	// Vector3

	[Test]
	public void Vector3ConverterWholeNoSpacesInt()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(1,2,3)";

		CheckConverter(true, converter, testString, "(1,2,3)", string.Empty);
	}

	[Test]
	public void Vector3ConverterWholeSpacesInt()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(6, 2, 7)";

		CheckConverter(true, converter, testString, "(6, 2, 7)", string.Empty);
	}

	[Test]
	public void Vector3ConverterWholeNoSpacesFloat()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,3.4,345.21)";

		CheckConverter(true, converter, testString, "(5,3.4,345.21)", string.Empty);
	}

	[Test]
	public void Vector3ConverterWholeSpacesFloat()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5, 3.4, 673.245)";

		CheckConverter(true, converter, testString, "(5, 3.4, 673.245)", string.Empty);
	}

	[Test]
	public void Vector3ConverterWholeRandomSpaces()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5, 3.4,673.245)";

		CheckConverter(true, converter, testString, "(5, 3.4,673.245)", string.Empty);
	}

	[Test]
	public void Vector3ConverterWholeMultipleSpaces()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,    3.4,673.245)";

		CheckConverter(true, converter, testString, "(5,    3.4,673.245)", string.Empty);
	}

	[Test]
	public void Vector3ConverterWholeMalformedPre()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "fd(5,3.4,674.3)";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void Vector3ConverterWholeMalformedPost()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,3.4,85.3)6u5rf";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void Vector3ConverterMultiple()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,3.4,85.3) (1,2,3)";

		CheckConverter(true, converter, testString, "(5,3.4,85.3)", " (1,2,3)");
	}
	

	// Vector4

	[Test]
	public void Vector4ConverterWholeNoSpacesInt()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(1,2,3,234)";

		CheckConverter(true, converter, testString, "(1,2,3,234)", string.Empty);
	}

	[Test]
	public void Vector4ConverterWholeSpacesInt()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(6, 2, 7, 78)";

		CheckConverter(true, converter, testString, "(6, 2, 7, 78)", string.Empty);
	}

	[Test]
	public void Vector4ConverterWholeNoSpacesFloat()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,3.4,345.21,6.21)";

		CheckConverter(true, converter, testString, "(5,3.4,345.21,6.21)", string.Empty);
	}

	[Test]
	public void Vector4ConverterWholeSpacesFloat()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5, 3.4, 673.245, 4.32)";

		CheckConverter(true, converter, testString, "(5, 3.4, 673.245, 4.32)", string.Empty);
	}

	[Test]
	public void Vector4ConverterWholeRandomSpaces()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5, 3.4,673.245, 4.32)";

		CheckConverter(true, converter, testString, "(5, 3.4,673.245, 4.32)", string.Empty);
	}

	[Test]
	public void Vector4ConverterWholeMultipleSpaces()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,    3.4,673.245, 43.32)";

		CheckConverter(true, converter, testString, "(5,    3.4,673.245, 43.32)", string.Empty);
	}

	[Test]
	public void Vector4ConverterWholeMalformedPre()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "fd(5,3.4,674.3,445.32)";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void Vector4ConverterWholeMalformedPost()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,3.4,85.3,4.32)6u5rf";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void Vector4ConverterMultiple()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,3.4,85.3,4.32) (1,2,3)";

		CheckConverter(true, converter, testString, "(5,3.4,85.3,4.32)", " (1,2,3)");
	}


	// RGB Color

	public void RGBColorConverterWhole()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1)";

		CheckConverter(true, converter, testString, "rgb(0.2,0.3,1)", string.Empty);
	}

	public void RGBColorConverterMultiple()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1) rgb(1,0.245,0.7)";

		CheckConverter(true, converter, testString, "rgb(0.2,0.3)", " rgb(1,0.245,0.7)");
	}

	public void RGBColorConverterRGBA()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,0.2)";

		CheckConverter(false, converter, testString);
	}

	public void RGBColorConverterWholeMalformedPre()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "srgb(0.2,0.3,1)";

		CheckConverter(false, converter, testString);
	}

	public void RGBColorConverterWholeMalformedPost()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1)s";

		CheckConverter(false, converter, testString);
	}


	// RGBA Color

	public void RGBAColorConverterWhole()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,1)";

		CheckConverter(true, converter, testString, "rgba(0.2,0.3,1)", string.Empty);
	}

	public void RGBAColorConverterMultiple()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,1) rgba(1,0.245,0.7,1)";

		CheckConverter(true, converter, testString, "rgba(0.2,0.3,1)", " rgba(1,0.245,0.7,1)");
	}

	public void RGBAColorConverterWholeRGB()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1)";

		CheckConverter(false, converter, testString);
	}

	public void RGBAColorConverterWholeRGBAlt()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1)";

		CheckConverter(false, converter, testString);
	}

	public void RGBAColorConverterWholeMalformedPre()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "srgba(0.2,0.3,1,1)";

		CheckConverter(false, converter, testString);
	}

	public void RGBAColorConverterWholeMalformedPost()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,1)s";

		CheckConverter(false, converter, testString);
	}


	// HTML Color

	[Test]
	public void HTMLColorConverterWholeLiteral()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "red";

		CheckConverter(true, converter, testString, "red", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeLiteralMalformedPre()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "bred";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeLiteralMalformedPost()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "redish";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBUpperCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#FF4FE2";

		CheckConverter(true, converter, testString, "#FF4FE2", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBLowerCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#fe74ab";

		CheckConverter(true, converter, testString, "#fe74ab", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBHybrid()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2";

		CheckConverter(true, converter, testString, "#Ff4Fe2", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBAUpperCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#FF4FE2FF";

		CheckConverter(true, converter, testString, "#FF4FE2FF", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBALowerCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#fe74abff";

		CheckConverter(true, converter, testString, "#fe74abff", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBAHybrid()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2fF";

		CheckConverter(true, converter, testString, "#Ff4Fe2fF", string.Empty);
	}

	[Test]
	public void HTMLColorConverterWholeHex7Chars()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2f";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeHex9Chars()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2ff2";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeNonHexChars()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Qe2ff";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void GameObjectConverterWholeSingleWord()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{TestObject}";

		CheckConverter(true, converter, testString, "{TestObject}", string.Empty);
	}


	// GameObject

	[Test]
	public void GameObjectConverterWholeMultiWord()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object}";

		CheckConverter(true, converter, testString, "{Test Object}", string.Empty);
	}

	[Test]
	public void GameObjectConverterPartialSingleWord()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{TestObject} (1,2,3,4)";

		CheckConverter(true, converter, testString, "{TestObject}", " (1,2,3,4)");
	}

	[Test]
	public void GameObjectConverterPartialMultiWord()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object} \"Test String\"";

		CheckConverter(true, converter, testString, "{Test Object}", " \"Test String\"");
	}

	[Test]
	public void GameObjectConverterPartialMultiple()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object} \"Test String\" {AnotherTest Object}";

		CheckConverter(true, converter, testString, "{Test Object}", " \"Test String\" {AnotherTest Object}");
	}

	[Test]
	public void GameObjectConverterWholeMalformedPre()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "sd{Test Object}";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void GameObjectConverterWholeMalformedPost()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object}s";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void GameObjectConverterInvalid()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "TestObject";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void GameObjectConverterWithCurlyBraceInside()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test{Object}";

		CheckConverter(true, converter, testString, "{Test{Object}", string.Empty);
	}

	[Test]
	public void GameObjectConverterWithMultipleCurlyBraceInside()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test{Obje}ct}";

		CheckConverter(true, converter, testString, "{Test{Obje}ct}", string.Empty);
	}

	[Test]
	public void GameObjectConverterWithMultipleCurlyBraceInsideMultiple()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test{Obje}ct} {Other}}";

		CheckConverter(true, converter, testString, "{Test{Obje}ct}", " {Other}}");
	}

	[Test]
	public void GameObjectConverterCurvedBrackets()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "(TestObject)";

		CheckConverter(false, converter, testString);
	}


	// Null

	[Test]
	public void NullConverterWholeLowerCase()
	{
		NullConverter converter = new NullConverter();
		string testString = "null";

		CheckConverter(true, converter, testString, "null", string.Empty);
	}

	[Test]
	public void NullConverterWholeUpperCase()
	{
		NullConverter converter = new NullConverter();
		string testString = "Null";

		CheckConverter(true, converter, testString, "Null", string.Empty);
	}

	[Test]
	public void NullConverterPartial()
	{
		NullConverter converter = new NullConverter();
		string testString = "null (1,3.5,50.214)";

		CheckConverter(true, converter, testString, "null", " (1,3.5,50.214)");
	}

	[Test]
	public void NullConverterMalformedPre()
	{
		NullConverter converter = new NullConverter();
		string testString = "qnull";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void NullConverterMalformedPost()
	{
		NullConverter converter = new NullConverter();
		string testString = "nulll";

		CheckConverter(false, converter, testString);
	}

	[Test]
	public void NullConverterMultiple()
	{
		NullConverter converter = new NullConverter();
		string testString = "null null";

		CheckConverter(true, converter, testString, "null", " null");
	}
}