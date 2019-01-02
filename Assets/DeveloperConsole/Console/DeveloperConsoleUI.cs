using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
	public class DeveloperConsoleUI : MonoBehaviour
	{
		[SerializeField]
		RectTransform consoleRect;

		[SerializeField]
		RectTransform logText;
		[SerializeField]
		RectTransform commandText;

		[SerializeField]
		RectTransform autocompleteBox;
		[SerializeField]
		RectTransform autocompleteText;
	}
}