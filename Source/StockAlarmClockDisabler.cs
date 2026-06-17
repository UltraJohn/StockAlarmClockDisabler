using CommNet.Network;
using HarmonyLib;
using KSP.IO;
using KSP.UI.Screens;
using KSP.UI.Screens.Mapview;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StockAlarmClockDisabler
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class StockAlarmClockDisabler : MonoBehaviour
    {
		public static bool FirstRun = false;
		public static bool setting_replaceStock = true;
		public void Awake()
		{
			LoadSettings();
			// NOTE: A Harmony patcher should be placed in a run once Startup addon. The patch is kept between scene changes.
			var harmony = new Harmony("UltraJohn.Mods.StockAlarmClockDisabler");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		void LoadSettings()
		{
			SettingsManager.InitializeSettings();
		}

		[HarmonyPatch(typeof(MapNode), "Init")]
		class Patch1
		{
			static void Postfix(ref bool ___showAddAlarmButton)
			{
				___showAddAlarmButton = false;
			}
		}

		static Assembly GetKACAssembly()
		{
			AssemblyLoader.LoadedAssembly list = AssemblyLoader.loadedAssemblies.FirstOrDefault(x => x.dllName.Contains("KerbalAlarmClock"));
			if (list != null)
			{
				Assembly assembly = list.assembly;
				if (assembly != null)
				{
					return assembly;
				}
			}
			return null;
		}
		
		//Disables the button. If KAC is installed, keep the button and override.
		[HarmonyPatch(typeof(AlarmClockApp), "OnAppInitialized")]
		class Patch3
		{
			static bool Prefix(ref AlarmClockApp __instance)
			{
				Assembly assembly = GetKACAssembly();
				if (assembly != null)
				{
					if (setting_replaceStock)
					{
						if (__instance != null)
						{
							// Call KerbalAlarmClock.KACToolbarAPI.OverrideStockToolbar:
							Type type = assembly.GetType("KerbalAlarmClock.KACToolbarAPI");
							MethodInfo methodinfo_overridestock = type.GetMethod("OverrideStockToolbar", BindingFlags.Public | BindingFlags.Static);
							methodinfo_overridestock.Invoke(null, new object[] { true });

							// Get the functions for the AppLauncher button:
							MethodInfo methodinfo_OnHover = type.GetMethod("onAppLaunchHoverOn", BindingFlags.Public | BindingFlags.Static);
							Callback onHover = (Callback)Delegate.CreateDelegate(typeof(Callback), null, methodinfo_OnHover);
							__instance.appLauncherButton.onHover = onHover;

							MethodInfo methodinfo_OnHoverOff = type.GetMethod("onAppLaunchHoverOff", BindingFlags.Public | BindingFlags.Static);
							Callback onHoverOff = (Callback)Delegate.CreateDelegate(typeof(Callback), null, methodinfo_OnHoverOff);
							__instance.appLauncherButton.onHoverOut = onHoverOff;

							MethodInfo methodinfo_OnToggleOn = type.GetMethod("onAppLaunchToggleOn", BindingFlags.Public | BindingFlags.Static);
							Callback onToggleOn = (Callback)Delegate.CreateDelegate(typeof(Callback), null, methodinfo_OnToggleOn);
							__instance.appLauncherButton.onTrue = onToggleOn;

							MethodInfo methodinfo_OnToggleOff = type.GetMethod("onAppLaunchToggleOff", BindingFlags.Public | BindingFlags.Static);
							Callback onToggleOff = (Callback)Delegate.CreateDelegate(typeof(Callback), null, methodinfo_OnToggleOff);
							__instance.appLauncherButton.onFalse = onToggleOff;


							__instance.appLauncherButton.onEnable = btnOnEnable;
							__instance.appLauncherButton.onDisable = btnOnDisable;
							//__instance.appLauncherButton.SetTexture(Resources.texAppLaunchIcon);


							// Check if the button is active, then set stock toolbar button to true, so that it correctly reflects if the window is active or not.
							Type typeKAC = assembly.GetType("KerbalAlarmClock.KerbalAlarmClock");
							if (typeKAC != null)
							{
								FieldInfo instanceField = typeKAC.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
								object kacInstance = instanceField?.GetValue(null);
								
								if (kacInstance != null)
								{
									PropertyInfo windowVisibleProp = typeKAC.GetProperty("WindowVisibleByActiveScene", BindingFlags.Public | BindingFlags.Instance);
									if (windowVisibleProp != null)
									{
										bool isWindowVisible = (bool)windowVisibleProp.GetValue(kacInstance);
										if(isWindowVisible)
										{
											__instance.appLauncherButton.SetTrue();
										}
									}
								}
							}
						}
						return false;
					}
					else
					{
						// Call KerbalAlarmClock.KACToolbarAPI.OverrideStockToolbar:
						Type type = assembly.GetType("KerbalAlarmClock.KACToolbarAPI");
						MethodInfo methodinfo_overridestock = type.GetMethod("OverrideStockToolbar", BindingFlags.Public | BindingFlags.Static);
						methodinfo_overridestock.Invoke(null, new object[] { false });

						if (AlarmClockApp.Instance != null)
						{
							Destroy(AlarmClockApp.Instance);
						}
						Destroy(__instance);
						return false;
					}
				}
				return false;
			}
		}

		public static void SetToolbarMode()
		{
			try
			{
				Assembly assembly = GetKACAssembly();
				Type type = assembly.GetType("KerbalAlarmClock.KACToolbarAPI");
				MethodInfo methodinfo_overridestock = type.GetMethod("OverrideStockToolbar", BindingFlags.Public | BindingFlags.Static);
				methodinfo_overridestock.Invoke(null, new object[] { false });
			}catch(Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			
		}

		static void btnOnEnable()
		{
			Debug.Log("StockAlarmClockDisabler: btnOnEnable testing. If you read this, please report this to the mod author");
		}
		static void btnOnDisable()
		{
			Debug.Log("StockAlarmClockDisabler: btnOnDisable testing. If you read this, please report this to the mod author");
		}

		//Disables stock functionality.
		[HarmonyPatch(typeof(AlarmClockScenario), "OnAwake")]
		class Patch4
		{
			static bool Prefix(ref AlarmClockScenario __instance)
			{
				if (AlarmClockScenario.Instance != null)
				{
					Destroy(AlarmClockScenario.Instance);
				}
				Destroy(__instance);
				return false;
			}
		}

		[HarmonyPatch(typeof(AlarmClockScenario), "OnLoad")]
		class Patch5
		{
			static bool Prefix(ref AlarmClockScenario __instance)
			{
				if (AlarmClockScenario.Instance != null)
				{
					Destroy(AlarmClockScenario.Instance);
				}
				if(__instance != null)
				{
					Destroy(__instance);
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(AlarmClockScenario), "OnSave")]
		class Patch6
		{
			static bool Prefix(ref AlarmClockScenario __instance)
			{
				if (AlarmClockScenario.Instance != null)
				{
					Destroy(AlarmClockScenario.Instance);
				}
				if (__instance != null)
				{
					Destroy(__instance);
				}
				return false;
			}
		}

		//Seemingly does nothing. Disabled for now.
		/*
		[HarmonyPatch(typeof(AlarmClockApp), "GetAppScenes")]
		class Patch2
		{
			static void Postfix(ref ApplicationLauncher.AppScenes __result)
			{
				Assembly assembly = AssemblyLoader.loadedAssemblies.First(x => x.name == "KerbalAlarmClock").assembly;
				if (assembly == null)
				{
					__result = ApplicationLauncher.AppScenes.NEVER;
				}
			}
		}
		*/


	}
}
