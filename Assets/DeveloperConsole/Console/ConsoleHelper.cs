using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using DeveloperConsole.Extensions;

namespace DeveloperConsole
{
	public static class ConsoleHelper
	{
		public static string GetCommandFullName(MethodInfo command, CommandAttribute attribute)
		{
			Type library = command.DeclaringType;
			string path = attribute.CommandName;

			CommandGroupAttribute groupAttribute;
			if (library.HasAttribute(false, out groupAttribute))
				path = string.Format("{0}.{1}", groupAttribute.GroupName, path);

			return path;
		}
	}
}