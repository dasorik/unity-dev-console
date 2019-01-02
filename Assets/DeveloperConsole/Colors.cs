using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole.Utilities
{
	public static class Colors
	{
		static readonly string[] htmlLiteralColors = new string[]
		{
			"red", "cyan", "blue", "darkblue", "lightblue", "purple", "yellow", "lime", "fuchsia", "white", "silver",
			"grey", "black", "orange", "brown", "maroon", "green", "olive", "navy", "teal", "aqua", "magenta"
		};

		public static bool IsLiteralColor(string color)
		{
			for (int i = 0; i < htmlLiteralColors.Length; i++)
			{
				if (htmlLiteralColors[i] == color)
					return true;
			}

			return false;
		}
	}
}