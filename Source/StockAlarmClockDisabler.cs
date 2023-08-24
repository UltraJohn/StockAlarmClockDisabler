using HarmonyLib;
using KSP.UI.Screens;
using KSP.UI.Screens.Mapview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StockAlarmClockDisabler
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class StockAlarmClockDisabler : MonoBehaviour
    {
		public void Awake()
		{
			// NOTE: A Harmony patcher should be placed in a run once Startup addon. The patch is kept between scene changes.
			var harmony = new Harmony("UltraJohn.Mods.StockAlarmClockDisabler");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		[HarmonyPatch(typeof(MapNode), "Init")]
		class Patch1
		{
			static void Postfix(ref bool ___showAddAlarmButton)
			{
				___showAddAlarmButton = false;
			}
		}

		[HarmonyPatch(typeof(AlarmClockApp), "GetAppScenes")]
		class Patch2
		{
			static void Postfix(ref ApplicationLauncher.AppScenes __result)
			{
				__result = ApplicationLauncher.AppScenes.NEVER;
			}
		}

		[HarmonyPatch(typeof(AlarmClockApp), "OnAppInitialized")]
		class Patch3
		{
			static bool Prefix(ref AlarmClockApp __instance)
			{
				if(AlarmClockApp.Instance != null)
				{
					Destroy(AlarmClockApp.Instance);
				}
				Destroy(__instance);
				return false;
			}
		}

	}
}
