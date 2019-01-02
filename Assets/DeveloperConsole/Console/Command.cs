using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class CommandAttribute : Attribute
	{
		public string CommandName { get; private set; }
		public string HelpText { get; private set; }

		public CommandAttribute(string commandName, string helptext = "")
		{
			this.CommandName = commandName;
			this.HelpText = helptext;
		}
	}

	
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class FunctionAttribute : Attribute
	{

	}
	

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PropertyAttribute : Attribute
	{

	}


	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class CommandGroupAttribute : Attribute
	{
		public string GroupName { get; private set; }

		public CommandGroupAttribute(string groupName)
		{
			this.GroupName = groupName;
		}
	}
}