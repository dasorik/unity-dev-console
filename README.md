# unity-dev-console
A handy little developer console for quickly testing your game

## QuickStart
Drag the prefab 'DeveloperConsole' into the scene (Under "Assets/DeveloperConsole/"). Hit play and click the '~' key (this is the default button), You should now be able to run commands.
Type `spawn-prefab "TestSphere" (0,0,0)` into the console and hit enter/return. A sphere with the name "TestSphere" should now appear in your scene at the origin of the world.
This developer console comes prepackaged with some generic commands, type `help` into the console for a full list of available commands

## Adding Commands
To add a command to this pool, create a new class that inherits from `ICommandLibrary`. Now simply create a static method and decorate it with the `[Command]` attribute. You then need to give the command a name (ensure this doesn't contain spaces) and a description. More than one command can share the same name, and the console will simply resolve the correct command to use based on the parameters passed to it. Use the command library class to group related commands, since this will eventually be used to enable/disable commands at runtime

```cs
public class MyCommands : ICommandLibrary
{
    [Command("test-command", "This is a test command!")]
    static void Test()
    {
        DeveloperConsole.LogSuccess("Test");
    }

    [Command("destroy-lights", "Destroys all lights in the scene")]
    static void DestroyLights()
    {
        var lights = GameObject.FindObjectOfType<Light>();

        foreach (var light in lights)
            GameObject.Destroy(light.gameObject);
    }
}
```

To give a command parameters, simply add parameters to the method. The following could be run as:<br>
`test-command-with-params "Test" 23.4`.

At present there are a limited number of valid types for parameters (see next section below)

```cs
public class MyCommands : ICommandLibrary
{
    [Command("test-command-with-params", "This is a test command! (With parameters)")]
    static void TestWithParams(string stringValue, float floatValue)
    {
        DeveloperConsole.LogSuccess($"stringValue: {stringvalue}, floatValue: {floatValue}");
    }
}
```

## Syntax
The following types can be used as parameters. When using commands in the console that require parameters, refer to this table for how to define objects of the requested parameter types

Support can be added for additional types by creating an `IConverter` that resolves to the requested type. This will however require modifying DeveloperConsole.cs to add them to the list of available converters. (Official support for external converters will be added soon)

Type | Format | Examples
---- | ------ | --------
Float | \*.\*\* | 2.3, 23.53, 1.0, -12.354
Int | \* | 1, 454, 23
String | "\*" | "Example String", "Test"
Char | '\*' | 'a', 'b', 'c'
Bool | [Tr]rue, [Ff]alse | true, false, True, False
Vector2 | (\*,\*) | (1,2), (4.3, 4)
Vector3 | (\*,\*,\*) | (1,2,3), (4.3, 4, -1)
Vector4 | (\*,\*,\*,\*) | (1,2,3,4), (4.3, 4, -1, 2.78)
Color | rgba(\*,\*,\*,\*)<br>rgb(\*,\*,\*)<br>#\*\*\*\*\*\*<br>#\*\*\*\*\*\*\*\*<br>HTML Literal | rgba(0.2, 0.5, 0.4, 1)<br>rgb(0.2, 0.5, 0.4)<br>#FF5D33<br>#FF5D33FF<br>red, blue, green etc
GameObject | {\*} | {TestSphere} {Player}
Null | [Nn]null | null, Null

* Valid HTML colour literals are the following:
```
"red", "cyan", "blue", "darkblue", "lightblue", "purple", "yellow", "lime", "fuchsia", "white", "silver",
"grey", "black", "orange", "brown", "maroon", "green", "olive", "navy", "teal", "aqua", "magenta"
```

## Variables
Variables can be set that are able to be used lated within that session. To set a variable, enter `set {VarName} {Value}` into the console (ie. `set name "Tony"`). A variable can be set to any value, and can be set more than once. A variable cannot be a colour literal (see above), or the following: `true, false, null`.

Call `unset {VarName}` to remove the variable and the stored value.

You can use a variable in place of any regular parameter, providing it has been set with a value and that value could be used in the command context.

So for example, Instead of writing<br>
`spawn-prefab "TestSphere" (1,2,3)`<br>
you can instead write (in sequence)<br>
`set spawnpos (1,2,3)`<br>
`spawn-prefab "TestSphere" spawnpos`<br>
for the same effect. `spawnpos` will now have the value of a Vector3 with the value (1,2,3) for the rest of the session

Conversely<br>
`set spawnpos "London"`<br>
`spawn-prefab "TestSphere" spawnpos`<br>
will not work, because `"London"` is not a Vector3 as expected by the `spawn-prefab` command

## Additional Notes
* The console remembers the previous commands entered. Hit the up/down arrow to navigate through old commands