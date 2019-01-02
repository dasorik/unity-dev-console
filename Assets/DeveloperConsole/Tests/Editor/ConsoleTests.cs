using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit;
using NUnit.Framework;
using DeveloperConsole;

public class ConsoleTests
{
	string substring, parameter;

	void CheckConversionPasses<T>(IConverter converter, string testString, string expectedParameter = "", string expectedSubstring = "", T expectedValue = default(T))
	{
		Assert.IsTrue(converter.CanConvert(testString, out parameter, out substring), $"{converter.GetType().Name} could not convert string '{testString}'");
		Assert.AreEqual(expectedParameter, parameter);
		Assert.AreEqual(expectedSubstring, substring);

		T value = (T)converter.Convert(parameter);
		Assert.AreEqual(expectedValue, value);
	}

	void CheckConversionFails(IConverter converter, string testString)
	{
		Assert.IsFalse(converter.CanConvert(testString, out parameter, out substring));
	}


	// String

	[Test]
	public void CheckStringConverterWhole()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"This is a test string\"";

		CheckConversionPasses(converter, testString, "\"This is a test string\"", string.Empty, "This is a test string");
	}

	[Test]
	public void CheckStringConverterSingle()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"A test string with extra parameters\" (0,0,0) value";

		CheckConversionPasses(converter, testString, "\"A test string with extra parameters\"", " (0,0,0) value", "A test string with extra parameters");
	}

	[Test]
	public void CheckStringConverterMultiple()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"A test string with another string present\" \"A second test string\"";

		CheckConversionPasses(converter, testString, "\"A test string with another string present\"", " \"A second test string\"", "A test string with another string present");
	}

	[Test]
	public void CheckStringConverterQuotesInside()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"This test string has \\\" inside of it\" \"A second test string\"";

		CheckConversionPasses(converter, testString, "\"This test string has \\\" inside of it\"", " \"A second test string\"", "This test string has \\\" inside of it");
	}

	[Test]
	public void CheckStringConverterQuotesInsideAlt()
	{
		StringConverter converter = new StringConverter();
		var testString = "\"This test string has\"inside of it\" \"A second test string\"";

		CheckConversionFails(converter, testString);
	}


	// Bool

	[Test]
	public void BoolConverterWholeFalseLowerCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "false";

		CheckConversionPasses(converter, testString, "false", string.Empty, false);
	}

	[Test]
	public void BoolConverterWholeFalseUpperCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "False";

		CheckConversionPasses(converter, testString, "False", string.Empty, false);
	}

	[Test]
	public void BoolConverterWholeTrueLowerCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "true";

		CheckConversionPasses(converter, testString, "true", string.Empty, true);
	}

	[Test]
	public void BoolConverterWholeTrueUpperCase()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "True";

		CheckConversionPasses(converter, testString, "True", string.Empty, true);
	}

	[Test]
	public void BoolConverterWholeTrueCapitals()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "TRUE";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void BoolConverterWholeFalseCapitals()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "FALSE";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void BoolConverterWholeRandom()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "(*&$(*)&(HJ uHIUHUI";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedStart()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "falseoidhzfIUHi4509uw9h8d";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedMiddle()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "oidhzfIUHfalsei4509uw9h8d";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedEnd()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "oidhzfIUHi4509uw9h8dfalse";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void BoolConverterEmbeddedMultiple()
	{
		BoolConverter converter = new BoolConverter();
		var testString = "falseoidhzfIUHfalsei4509uw9h8dfalse";

		CheckConversionFails(converter, testString);
	}


	// Char

	[Test]
	public void CharConverterWhole()
	{
		CharConverter converter = new CharConverter();
		var testString = "'a'";

		CheckConversionPasses(converter, testString, "'a'", string.Empty, 'a');
	}

	[Test]
	public void CharConverterWholeMultiChar()
	{
		CharConverter converter = new CharConverter();
		var testString = "'ab'";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void CharConverterMultiple()
	{
		CharConverter converter = new CharConverter();
		var testString = "'q' 34.3 'b'";

		CheckConversionPasses(converter, testString, "'q'", " 34.3 'b'", 'q');
	}

	[Test]
	public void CharConverterString()
	{
		CharConverter converter = new CharConverter();
		var testString = "\"a\"";

		CheckConversionFails(converter, testString);
	}


	// Int

	[Test]
	public void IntConverterWhole()
	{
		IntConverter converter = new IntConverter();
		string testString = "346";

		CheckConversionPasses(converter, testString, "346", string.Empty, 346);
	}

	[Test]
	public void IntConverterWholeDecimal()
	{
		IntConverter converter = new IntConverter();
		string testString = "4563.23";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void IntConverterMalformedPre()
	{
		IntConverter converter = new IntConverter();
		string testString = "a873";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void IntConverterMalformedPost()
	{
		IntConverter converter = new IntConverter();
		string testString = "873d";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void IntConverterMultiple()
	{
		IntConverter converter = new IntConverter();
		string testString = "8743 45873";

		CheckConversionPasses(converter, testString, "8743", " 45873", 8743);
	}


	// Float

	[Test]
	public void FloatConverterWhole()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "234.26";

		CheckConversionPasses(converter, testString, "234.26", string.Empty, 234.26f);
	}

	[Test]
	public void FloatConverterPartial()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "234.26 \"Test String\"";

		CheckConversionPasses(converter, testString, "234.26", " \"Test String\"", 234.26f);
	}

	[Test]
	public void FloatConverterMultiple()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "45423.8 \"Test String\" 546.2";

		CheckConversionPasses(converter, testString, "45423.8", " \"Test String\" 546.2", 45423.8f);
	}

	[Test]
	public void FloatConverterWholeMalformedPre()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "s45423.8";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void FloatConverterWholeMalformedPost()
	{
		FloatConverter converter = new FloatConverter();
		string testString = "45423.8dfjgh";

		CheckConversionFails(converter, testString);
	}


	// Vector2

	[Test]
	public void Vector2ConverterWholeNoSpacesInt()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(1,2)";

		CheckConversionPasses(converter, testString, "(1,2)", string.Empty, new Vector2(1, 2));
	}

	[Test]
	public void Vector2ConverterWholeSpacesInt()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(6, 2)";

		CheckConversionPasses(converter, testString, "(6, 2)", string.Empty, new Vector2(6, 2));
	}

	[Test]
	public void Vector2ConverterWholeNoSpacesFloat()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5,3.4)";

		CheckConversionPasses(converter, testString, "(5,3.4)", string.Empty, new Vector2(5, 3.4f));
	}

	[Test]
	public void Vector2ConverterWholeSpacesFloat()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5, 3.4)";

		CheckConversionPasses(converter, testString, "(5, 3.4)", string.Empty, new Vector2(5, 3.4f));
	}

	[Test]
	public void Vector2ConverterWholeMalformedPre()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "fd(5,3.4)";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void Vector2ConverterWholeMalformedPost()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5,3.4)6u5rf";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void Vector2ConverterMultiple()
	{
		Vector2Converter converter = new Vector2Converter();
		string testString = "(5,3.4) (1,2,3)";

		CheckConversionPasses(converter, testString, "(5,3.4)", " (1,2,3)", new Vector2(5, 3.4f));
	}


	// Vector3

	[Test]
	public void Vector3ConverterWholeNoSpacesInt()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(1,2,3)";

		CheckConversionPasses(converter, testString, "(1,2,3)", string.Empty, new Vector3(1, 2, 3));
	}

	[Test]
	public void Vector3ConverterWholeSpacesInt()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(6, 2, 7)";

		CheckConversionPasses(converter, testString, "(6, 2, 7)", string.Empty, new Vector3(6, 2, 7));
	}

	[Test]
	public void Vector3ConverterWholeNoSpacesFloat()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,3.4,345.21)";

		CheckConversionPasses(converter, testString, "(5,3.4,345.21)", string.Empty, new Vector3(5, 3.4f, 345.21f));
	}

	[Test]
	public void Vector3ConverterWholeSpacesFloat()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5, 3.4, 673.245)";

		CheckConversionPasses(converter, testString, "(5, 3.4, 673.245)", string.Empty, new Vector3(5, 3.4f, 673.245f));
	}

	[Test]
	public void Vector3ConverterWholeRandomSpaces()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5, 3.4,673.245)";

		CheckConversionPasses(converter, testString, "(5, 3.4,673.245)", string.Empty, new Vector3(5, 3.4f, 673.245f));
	}

	[Test]
	public void Vector3ConverterWholeMultipleSpaces()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,    3.4,673.245)";

		CheckConversionPasses(converter, testString, "(5,    3.4,673.245)", string.Empty, new Vector3(5, 3.4f, 673.245f));
	}

	[Test]
	public void Vector3ConverterWholeMalformedPre()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "fd(5,3.4,674.3)";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void Vector3ConverterWholeMalformedPost()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(5,3.4,85.3)6u5rf";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void Vector3ConverterMultiple()
	{
		Vector3Converter converter = new Vector3Converter();
		string testString = "(67.2,3.4,85.3) (1,2,3)";

		CheckConversionPasses(converter, testString, "(67.2,3.4,85.3)", " (1,2,3)", new Vector3(67.2f, 3.4f, 85.3f));
	}
	

	// Vector4

	[Test]
	public void Vector4ConverterWholeNoSpacesInt()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(1,2,3,234)";

		CheckConversionPasses(converter, testString, "(1,2,3,234)", string.Empty, new Vector4(1, 2, 3, 234));
	}

	[Test]
	public void Vector4ConverterWholeSpacesInt()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(6, 2, 7, 78)";

		CheckConversionPasses(converter, testString, "(6, 2, 7, 78)", string.Empty, new Vector4(6, 2, 7, 78));
	}

	[Test]
	public void Vector4ConverterWholeNoSpacesFloat()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,3.4,345.21,6.21)";

		CheckConversionPasses(converter, testString, "(5,3.4,345.21,6.21)", string.Empty, new Vector4(5, 3.4f, 345.21f, 6.21f));
	}

	[Test]
	public void Vector4ConverterWholeSpacesFloat()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(64.3, 3, 673.245, 4.32)";

		CheckConversionPasses(converter, testString, "(64.3, 3, 673.245, 4.32)", string.Empty, new Vector4(64.3f, 3, 673.245f, 4.32f));
	}

	[Test]
	public void Vector4ConverterWholeRandomSpaces()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5.2, 3.4,673, 4.32)";

		CheckConversionPasses(converter, testString, "(5.2, 3.4,673, 4.32)", string.Empty, new Vector4(5.2f, 3.4f, 673, 4.32f));
	}

	[Test]
	public void Vector4ConverterWholeMultipleSpaces()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,    3.4,673.245, 43.32)";

		CheckConversionPasses(converter, testString, "(5,    3.4,673.245, 43.32)", string.Empty, new Vector4(5, 3.4f, 673.245f, 43.32f));
	}

	[Test]
	public void Vector4ConverterWholeMalformedPre()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "fd(5,3.4,674.3,445.32)";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void Vector4ConverterWholeMalformedPost()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,3.4,85.3,4.32)6u5rf";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void Vector4ConverterMultiple()
	{
		Vector4Converter converter = new Vector4Converter();
		string testString = "(5,3.4,85.3,4) (1,2,3)";

		CheckConversionPasses(converter, testString, "(5,3.4,85.3,4)", " (1,2,3)", new Vector4(5, 3.4f, 85.3f, 4));
	}


	// RGB Color

	public void RGBColorConverterWhole()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1)";

		CheckConversionPasses(converter, testString, "rgb(0.2,0.3,1)", string.Empty, new Color(0.2f, 0.3f, 1f));
	}

	public void RGBColorConverterMultiple()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1) rgb(1,0.245,0.7)";

		CheckConversionPasses(converter, testString, "rgb(0.2,0.3)", " rgb(1,0.245,0.7)", new Color(0.2f, 0.3f, 1f));
	}

	public void RGBColorConverterRGBA()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,0.2)";

		CheckConversionFails(converter, testString);
	}

	public void RGBColorConverterWholeMalformedPre()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "srgb(0.2,0.3,1)";

		CheckConversionFails(converter, testString);
	}

	public void RGBColorConverterWholeMalformedPost()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1)s";

		CheckConversionFails(converter, testString);
	}


	// RGBA Color

	public void RGBAColorConverterWhole()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,1)";

		CheckConversionPasses(converter, testString, "rgba(0.2,0.3,1)", string.Empty, new Color(0.2f, 0.3f, 1f, 1f));
	}

	public void RGBAColorConverterMultiple()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,1) rgba(1,0.245,0.7,1)";

		CheckConversionPasses(converter, testString, "rgba(0.2,0.3,1)", " rgba(1,0.245,0.7,1)", new Color(0.2f, 0.3f, 1f));
	}

	public void RGBAColorConverterWholeRGB()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1)";

		CheckConversionFails(converter, testString);
	}

	public void RGBAColorConverterWholeRGBAlt()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgb(0.2,0.3,1)";

		CheckConversionFails(converter, testString);
	}

	public void RGBAColorConverterWholeMalformedPre()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "srgba(0.2,0.3,1,1)";

		CheckConversionFails(converter, testString);
	}

	public void RGBAColorConverterWholeMalformedPost()
	{
		RGBColorConverter converter = new RGBColorConverter();
		string testString = "rgba(0.2,0.3,1,1)s";

		CheckConversionFails(converter, testString);
	}


	// HTML Color

	[Test]
	public void HTMLColorConverterWholeLiteral()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "red";

		CheckConversionPasses(converter, testString, "red", string.Empty, Color.red);
	}

	[Test]
	public void HTMLColorConverterWholeLiteralMalformedPre()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "bred";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeLiteralMalformedPost()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "redish";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBUpperCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#FF4FE2";

		CheckConversionPasses(converter, testString, "#FF4FE2", string.Empty, new Color(1f, 79f/255f, 226f/255f));
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBLowerCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#fe74ab";

		CheckConversionPasses(converter, testString, "#fe74ab", string.Empty, new Color(254f/255f, 116f/255f, 171f/255f));
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBHybrid()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2";

		CheckConversionPasses(converter, testString, "#Ff4Fe2", string.Empty, new Color(1f, 79f/255f, 226f/255f));
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBAUpperCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#FF4FE2FF";

		CheckConversionPasses(converter, testString, "#FF4FE2FF", string.Empty, new Color(1f, 79f/255f, 226f/255f, 1f));
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBALowerCase()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#fe74abff";

		CheckConversionPasses(converter, testString, "#fe74abff", string.Empty, new Color(254f/255f, 116f/255f, 171f/255f, 1f));
	}

	[Test]
	public void HTMLColorConverterWholeHexRGBAHybrid()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2fF";

		CheckConversionPasses(converter, testString, "#Ff4Fe2fF", string.Empty, new Color(1f, 79f/255f, 226f/255f, 1f));
	}

	[Test]
	public void HTMLColorConverterWholeHex7Chars()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2f";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeHex9Chars()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Fe2ff2";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void HTMLColorConverterWholeNonHexChars()
	{
		HTMLColorConverter converter = new HTMLColorConverter();
		string testString = "#Ff4Qe2ff";

		CheckConversionFails(converter, testString);
	}


	// GameObject

	[Test]
	public void GameObjectConverterWholeSingleWord()
	{
		GameObject gameObject = new GameObject("TestObject");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{TestObject}";

		CheckConversionPasses(converter, testString, "{TestObject}", string.Empty, gameObject);
	}

	[Test]
	public void GameObjectConverterWholeMultiWord()
	{
		GameObject gameObject = new GameObject("Test Object");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object}";

		CheckConversionPasses(converter, testString, "{Test Object}", string.Empty, gameObject);
	}

	[Test]
	public void GameObjectConverterNull()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{NonExistentObject}";

		CheckConversionPasses<GameObject>(converter, testString, "{NonExistentObject}", string.Empty, null);
	}

	[Test]
	public void GameObjectConverterPartialSingleWord()
	{
		GameObject gameObject = new GameObject("TestObject1");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{TestObject1} (1,2,3,4)";

		CheckConversionPasses(converter, testString, "{TestObject1}", " (1,2,3,4)", gameObject);
	}

	[Test]
	public void GameObjectConverterPartialMultiWord()
	{
		GameObject gameObject = new GameObject("Test Object2");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object2} \"Test String\"";

		CheckConversionPasses(converter, testString, "{Test Object2}", " \"Test String\"", gameObject);
	}

	[Test]
	public void GameObjectConverterPartialMultiple()
	{
		GameObject gameObject = new GameObject("Test Object3");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object3} \"Test String\" {AnotherTest Object}";

		CheckConversionPasses(converter, testString, "{Test Object3}", " \"Test String\" {AnotherTest Object}", gameObject);
	}

	[Test]
	public void GameObjectConverterWholeMalformedPre()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "sd{Test Object}";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void GameObjectConverterWholeMalformedPost()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test Object}s";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void GameObjectConverterInvalid()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "TestObject";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void GameObjectConverterWithCurlyBraceInside()
	{
		GameObject gameObject = new GameObject("Test{Object4");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test{Object4}";

		CheckConversionPasses(converter, testString, "{Test{Object4}", string.Empty, gameObject);
	}

	[Test]
	public void GameObjectConverterWithMultipleCurlyBraceInside()
	{
		GameObject gameObject = new GameObject("Test{Obje}ct5");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test{Obje}ct5}";

		CheckConversionPasses(converter, testString, "{Test{Obje}ct5}", string.Empty, gameObject);
	}

	[Test]
	public void GameObjectConverterWithMultipleCurlyBraceInsideMultiple()
	{
		GameObject gameObject = new GameObject("Test{Obje}ct6");

		GameObjectConverter converter = new GameObjectConverter();
		string testString = "{Test{Obje}ct6} {Other}}";

		CheckConversionPasses(converter, testString, "{Test{Obje}ct6}", " {Other}}", gameObject);
	}

	[Test]
	public void GameObjectConverterCurvedBrackets()
	{
		GameObjectConverter converter = new GameObjectConverter();
		string testString = "(TestObject)";

		CheckConversionFails(converter, testString);
	}


	// Null

	[Test]
	public void NullConverterWholeLowerCase()
	{
		NullConverter converter = new NullConverter();
		string testString = "null";

		CheckConversionPasses<object>(converter, testString, "null", string.Empty, null);
	}

	[Test]
	public void NullConverterWholeUpperCase()
	{
		NullConverter converter = new NullConverter();
		string testString = "Null";

		CheckConversionPasses<object>(converter, testString, "Null", string.Empty, null);
	}

	[Test]
	public void NullConverterPartial()
	{
		NullConverter converter = new NullConverter();
		string testString = "null (1,3.5,50.214)";

		CheckConversionPasses<object>(converter, testString, "null", " (1,3.5,50.214)", null);
	}

	[Test]
	public void NullConverterMalformedPre()
	{
		NullConverter converter = new NullConverter();
		string testString = "qnull";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void NullConverterMalformedPost()
	{
		NullConverter converter = new NullConverter();
		string testString = "nulll";

		CheckConversionFails(converter, testString);
	}

	[Test]
	public void NullConverterMultiple()
	{
		NullConverter converter = new NullConverter();
		string testString = "null null";

		CheckConversionPasses<object>(converter, testString, "null", " null", null);
	}
}