using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeveloperConsole
{
	public class UnityConsoleCommands : ICommandLibrary
	{

		[Command("help", "Displays a list of available commands")]
		static void Help()
		{
			DeveloperConsole.LogSuccess("Displaying a list of available commands");

			foreach (var command in DeveloperConsole.GetAllCommands())
				DeveloperConsole.Log("<color=yellow>" + command.fullname + "</color> - " + command.helpText, MessageType.Info);
		}

		[Command("timescale", "Sets the current timescale of the game")]
		static void TimeScale(float timeScale)
		{
			if (timeScale < 0)
			{
				DeveloperConsole.LogError("Timescale must be greater than or equal to 0");
				return;
			}

			Time.timeScale = timeScale;
			DeveloperConsole.LogSuccess(string.Format("Change successful, timescale is now {0}", timeScale));
		}


		[Command("set-fixed-delta-time", "Sets the current fixed delta time")]
		static void FixedDeltaTime(float fixedDeltaTime)
		{
			if (fixedDeltaTime <= 0)
			{
				DeveloperConsole.LogError("Fixed delta time value must be greater than 0");
				return;
			}

			Time.fixedDeltaTime = fixedDeltaTime;
			DeveloperConsole.LogSuccess(string.Format("Change successful, fixedDeltaTime is now {0}", fixedDeltaTime));
		}


		[Command("show-time", "Displays the current in-game time")]
		static void ShowTime()
		{
			DeveloperConsole.Log(Time.time.ToString());
		}


		[Command("show-realtime", "Displays the current in-game time")]
		static void ShowRealtime()
		{
			DeveloperConsole.Log(Time.realtimeSinceStartup.ToString());
		}

		[Command("show-framecount", "Displays the current in-game time")]
		static void ShowFramecound()
		{
			DeveloperConsole.Log(Time.frameCount.ToString());
		}


		[Command("gravity-x", "Sets the x component of gravity")]
		static void GravityX(float gravityX)
		{
			Physics.gravity = new Vector3(gravityX, Physics.gravity.y, Physics.gravity.z);
			DeveloperConsole.LogSuccess(string.Format("Change successful, gravity is now {0}", Physics.gravity));
		}


		[Command("gravity-y", "Sets the y component of gravity")]
		static void GravityY(float gravityY)
		{
			Physics.gravity = new Vector3(Physics.gravity.x, gravityY, Physics.gravity.z);
			DeveloperConsole.LogSuccess(string.Format("Change successful, gravity is now {0}", Physics.gravity));
		}


		[Command("gravity-z", "Sets the z component of gravity")]
		static void GravityZ(float gravityZ)
		{
			Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y, gravityZ);
			DeveloperConsole.LogSuccess(string.Format("Change successful, gravity is now {0}", Physics.gravity));
		}


		[Command("gravity", "Sets the gravitational force")]
		static void Gravity(float x, float y, float z)
		{
			Physics.gravity = new Vector3(x, y, z);
			DeveloperConsole.LogSuccess(string.Format("Change successful, gravity is now {0}", Physics.gravity));
		}


		[Command("gravity", "Sets the gravitational force")]
		static void Gravity(Vector3 gravity)
		{
			Physics.gravity = gravity;
			DeveloperConsole.LogSuccess(string.Format("Change successful, gravity is now {0}", Physics.gravity));
		}


		[Command("invert-gravity", "Inverts the current gravity vector")]
		static void InvertGravity()
		{
			Physics.gravity = -Physics.gravity;
			DeveloperConsole.LogSuccess(string.Format("Gravity Inverted, New Gravity {0}", Physics.gravity));
		}

		[Command("spawn-prefab", "Loads a prefab from the resources folder")]
		static void SpawnPrefab(string prefabPath, Vector3 position)
		{
			SpawnPrefab(prefabPath, position, string.Empty);
		}

		[Command("spawn-prefab", "Loads a prefab from the resources folder, can be given a unique name for debugging purposes")]
		static void SpawnPrefab(string prefabPath, Vector3 position, string name)
		{
			var prefab = Resources.Load<GameObject>(prefabPath);

			if (prefab == null)
			{
				DeveloperConsole.LogError($"Unable to find gameobject at path '{prefabPath}'");
				return;
			}

			var instantiatedPrefab = GameObject.Instantiate(prefab, position, Quaternion.identity);

			if (!string.IsNullOrEmpty(name))
				instantiatedPrefab.name = name;

			DeveloperConsole.LogSuccess($"Loaded prefab '{prefabPath}'");
		}

		[Command("destroy", "Destroy a game object")]
		static void DestroyGameObject(GameObject target)
		{
			if (target == null)
			{
				DeveloperConsole.LogError("Could not destroy target, target was null");
				return;
			}

			GameObject.Destroy(target);
			DeveloperConsole.LogSuccess("GameObject destroyed");
		}
	}
}