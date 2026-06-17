using KSP.UI.Screens.DebugToolbar.Screens.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StockAlarmClockDisabler
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	public class OptionWindow : MonoBehaviour
	{
		public static OptionWindow Instance;
		private Rect windowRect = new Rect(150, 150, 360, 180);
		private int windowID;
		private bool isVisible = false;

		// Labels matching our ToolbarMode Enum order
		private readonly string[] modeLabels = new string[]
		{
			"Stock AppLauncher",
			"Toolbar Controller"
		};

		private readonly string[] modeDescriptions = new string[]
		{
			"Replaces the Stock app launcher button with KAC.",
			"Uses Toolbar Controller to handle the app button placement. Select this if you want to use Blizzy toolbar."
		};

		public static void Show()
		{
			Instance.isVisible = true;
		}

		private void Awake()
		{
			windowID = Guid.NewGuid().GetHashCode();
			Instance = this;
		}

		private void OnGUI()
		{
			if (!isVisible) return;

			GUI.skin = HighLogic.Skin;

			windowRect = GUILayout.Window(
				windowID,
				windowRect,
				DrawWindowContent,
				"Stock Alarm Clock Disabler Options",
				GUILayout.ExpandHeight(true)
			);
		}

		private void DrawWindowContent(int id)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(10);


			GUILayout.Label("Select Kerbal Alarm Clock Button Display Mode:", HighLogic.Skin.label);
			GUILayout.Space(5);

			int currentSelection = StockAlarmClockDisabler.setting_replaceStock ? 0 : 1;


			int newSelection = currentSelection;

			GUIStyle descriptionStyle = new GUIStyle(HighLogic.Skin.label);
			descriptionStyle.fontStyle = FontStyle.Normal;
			descriptionStyle.normal.textColor = Color.yellow;
			descriptionStyle.wordWrap = true;

			// Grab our current states
			bool isStockSelected = StockAlarmClockDisabler.setting_replaceStock;
			bool isToolbarSelected = !StockAlarmClockDisabler.setting_replaceStock;

			// --- OPTION 1: STOCK APPLAUNCHER ---
			bool newStockSelected = GUILayout.Toggle(isStockSelected, " " + modeLabels[0], HighLogic.Skin.toggle);

			GUILayout.BeginHorizontal();
			GUILayout.Space(22); // Indent descriptions past the radio check circle
			GUILayout.Label(modeDescriptions[0], descriptionStyle);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			// --- OPTION 2: TOOLBAR CONTROLLER ---
			bool newToolbarSelected = GUILayout.Toggle(isToolbarSelected, " " + modeLabels[1], HighLogic.Skin.toggle);

			GUILayout.BeginHorizontal();
			GUILayout.Space(22);
			GUILayout.Label(modeDescriptions[1], descriptionStyle);
			GUILayout.EndHorizontal();

			// If the user clicked the Stock button when it wasn't already active
			if (newStockSelected && !isStockSelected)
			{
				StockAlarmClockDisabler.setting_replaceStock = true;
				Debug.Log($"[StockAlarmClockDisabler] Mode changed to: Stock AppLauncher");
				SettingsManager.SaveSettings();
			}
			// If the user clicked the Toolbar button when it wasn't already active
			else if (newToolbarSelected && !isToolbarSelected)
			{
				StockAlarmClockDisabler.setting_replaceStock = false;
				Debug.Log($"[StockAlarmClockDisabler] Mode changed to: Toolbar Controller");
				SettingsManager.SaveSettings();
			}

			GUILayout.Space(15);

			// 3. Close Button
			if (GUILayout.Button("Save & Close"))
			{
				isVisible = false;
				if (!StockAlarmClockDisabler.setting_replaceStock)
				{
					StockAlarmClockDisabler.SetToolbarMode();
				}
			}

			GUILayout.EndVertical();

			// Drag behavior
			GUI.DragWindow();
		}
	}
}
