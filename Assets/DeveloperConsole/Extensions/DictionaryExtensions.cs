using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DeveloperConsole.Extensions
{
	public static class DictionaryExtension
	{
		public static V GetOrAddValue<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, defaultValue);
				return dictionary[key];
			}

			return dictionary[key];
		}

		public static void AddOrUpdateValue<K, V>(this Dictionary<K, V> dictionary, K key, V value)
		{
			if (!dictionary.ContainsKey(key))
				dictionary.Add(key, value);

			else
				dictionary[key] = value;
		}

		public static V GetValueOr<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue, bool useDefaultOnNull = false)
		{
			if (!dictionary.ContainsKey(key))
				return defaultValue;

			V value = dictionary[key];
			return useDefaultOnNull && value == null ? defaultValue : value;
		}
	}
}