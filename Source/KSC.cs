using KSP.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StockAlarmClockDisabler
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	public class KSC : MonoBehaviour
	{
		public void Awake()
		{
			if (StockAlarmClockDisabler.FirstRun)
			{
				GameEvents.onLevelWasLoaded.Add(onLevelLoaded);
			}
		}

		private void onLevelLoaded(GameScenes scene)
		{
			if(scene != GameScenes.SPACECENTER)
			{
				return;
			}
			GameEvents.onLevelWasLoaded.Remove(onLevelLoaded);
			SettingsManager.InitializeSettings();
			OptionWindow.Show();
		}
	}
}
