using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static StockAlarmClockDisabler.OptionWindow;

namespace StockAlarmClockDisabler
{
	public static class SettingsManager
	{
		public static string GetConfigPath()
		{
			string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return Path.Combine(directory, "..", "PluginData", "Settings.cfg");
		}
		public static void InitializeSettings()
		{
			string path = GetConfigPath();
			if (!File.Exists(path))
			{
				StockAlarmClockDisabler.FirstRun = true;
				Debug.Log("[StockAlarmClockDisabler] Running first run.");
				SaveSettings();
				return;
			}

			try
			{
				ConfigNode fileNode = ConfigNode.Load(path);
				ConfigNode node = fileNode?.GetNode("StockAlarmClockDisablerSettings");
				if (node != null && node.HasValue("replaceStockAppLauncher"))
				{
					if (node.HasValue("replaceStockAppLauncher"))
					{
						bool.TryParse(node.GetValue("replaceStockAppLauncher"), out StockAlarmClockDisabler.setting_replaceStock); ;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"[StockAlarmClockDisabler] Error loading config: {e.Message}");
			}
		}

		public static void SaveSettings()
		{
			try
			{
				string path = GetConfigPath();
				string directory = Path.GetDirectoryName(path);

				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				ConfigNode fileNode = new ConfigNode();
				ConfigNode settingsNode = fileNode.AddNode("StockAlarmClockDisablerSettings");

				// Saves the mode as text (e.g., "StockAppLauncher" or "BlizzyToolbar")
				settingsNode.AddValue("replaceStockAppLauncher", StockAlarmClockDisabler.setting_replaceStock);

				fileNode.Save(path);
			}
			catch (Exception e)
			{
				Debug.LogError($"[StockAlarmClockDisabler] Error saving config: {e.Message}");
			}
		}

	}
}
