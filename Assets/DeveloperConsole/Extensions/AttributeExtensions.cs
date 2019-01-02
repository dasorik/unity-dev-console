using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;

namespace DeveloperConsole.Extensions
{
	public static class AttributeExtensions
	{

		public static T GetAttribute<T>(this MemberInfo info, bool inherit)
			where T : class
		{
			object[] attributes = info.GetCustomAttributes(typeof(T), inherit);

			if (attributes != null && attributes.Length > 0)
				return attributes[0] as T;

			return null;
		}


		public static bool HasAttribute<T>(this MemberInfo info, bool inherit)
			where T : class
		{
			object[] attributes = info.GetCustomAttributes(typeof(T), inherit);
			return attributes != null && attributes.Length > 0;
		}


		static bool GetHasAttribute<T>(object[] attributes, out T attribute)
			where T : class
		{
			if (attributes != null && attributes.Length > 0)
			{
				attribute = attributes[0] as T;
				return true;
			}

			attribute = null;
			return false;
		}


		static bool GetHasAttributes<T>(object[] attributes, out T[] attributeList)
			where T : class
		{
			if (attributes != null && attributes.Length > 0)
			{
				attributeList = attributes.Cast<T>().ToArray();
				return true;
			}

			attributeList = null;
			return false;
		}


		public static bool HasAttribute<T>(this MemberInfo info, bool inherit, out T attribute)
			where T : class
		{
			object[] attributes = info.GetCustomAttributes(typeof(T), inherit);
			return GetHasAttribute(attributes, out attribute);
		}


		public static bool HasAttributes<T>(this MemberInfo info, bool inherit, out T[] attributeList)
			where T : class
		{
			object[] attributes = info.GetCustomAttributes(typeof(T), inherit);
			return GetHasAttributes(attributes, out attributeList);
		}

	}
}