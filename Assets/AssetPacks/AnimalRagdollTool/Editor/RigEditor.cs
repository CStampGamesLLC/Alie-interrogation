//***********************************************************************************************
//** RigEditor.cs                                                                       

//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BattleCrafters
{
	[CustomEditor(typeof(RigEditor))]
	public class RigEditor : EditorWindow
	{
		public AnimalRagdollTool tool;

		public RigDescription rigDescription;
		public bool isGlobal = false;

		string[] rigList;

		bool[] changed = new bool[11];

		static RigEditor window;
		static void Init()
		{
			window = GetWindow<RigEditor>(false, "Rig Editor");
			window.Show();
		}

		void OnGUI()
		{
			Event e = Event.current;
			if (e.isKey)
			{
				if (e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
				{
					tool.Repaint();
					Close();
				}
			}

			GetRigList();

			if (!isGlobal)
			{
				GUI.enabled = rigDescription.label != "Pelvis";
				GUI.SetNextControlName("Name");
				EditorGUI.BeginChangeCheck();
				rigDescription.label = EditorGUILayout.TextField(((changed[0]) ? "*" : "") + "Name", rigDescription.label);
				if (EditorGUI.EndChangeCheck())
					changed[0] = true;
				GUI.enabled = !tool.config.locked;

				EditorGUI.BeginChangeCheck();
				rigDescription.category = EditorGUILayout.Popup(((changed[1]) ? "*" : "") + "Category", rigDescription.category, tool.config.categories.ToArray());
				if (EditorGUI.EndChangeCheck())
					changed[1] = true;

				EditorGUI.BeginChangeCheck();
				rigDescription.connectsToInt = EditorGUILayout.Popup(((changed[2]) ? "*" : "") + "Comes From", rigDescription.connectsToInt, rigList);
				if (EditorGUI.EndChangeCheck())
					changed[2] = true;

				EditorGUI.BeginChangeCheck();
				rigDescription.runsToInt = EditorGUILayout.Popup(((changed[3]) ? "*" : "") + "Goes To", rigDescription.runsToInt, rigList);
				if (EditorGUI.EndChangeCheck())
					changed[3] = true;
			}
			EditorGUI.BeginChangeCheck();
			rigDescription.required = EditorGUILayout.Toggle(((changed[4]) ? "*" : "") + "Required", rigDescription.required);
			if (EditorGUI.EndChangeCheck())
				changed[4] = true;

			EditorGUI.BeginChangeCheck();
			rigDescription.colType = (ColliderTypes)EditorGUILayout.EnumPopup(((changed[5]) ? "*" : "") + "Collider Type", rigDescription.colType);
			if (EditorGUI.EndChangeCheck())
				changed[5] = true;

			EditorGUI.BeginChangeCheck();
			rigDescription.direction = (CapsuleDirection)EditorGUILayout.EnumPopup(((changed[6]) ? "*" : "") + "Collider Direction", rigDescription.direction);
			if (EditorGUI.EndChangeCheck())
				changed[6] = true;

			EditorGUI.BeginChangeCheck();
			rigDescription.bendType = (BendTypes)EditorGUILayout.EnumPopup(((changed[7]) ? "*" : "") + "Bend Type", rigDescription.bendType);
			if (EditorGUI.EndChangeCheck())
				changed[7] = true;

			EditorGUI.BeginChangeCheck();
			rigDescription.hasRB = EditorGUILayout.Toggle(((changed[8]) ? "*" : "") + "Has RigidBody", rigDescription.hasRB);
			if (EditorGUI.EndChangeCheck())
				changed[8] = true;

			EditorGUI.BeginChangeCheck();
			rigDescription.hasJoint = EditorGUILayout.Toggle(((changed[9]) ? "*" : "") + "Has Joint", rigDescription.hasJoint);
			if (EditorGUI.EndChangeCheck())
				changed[9] = true;

			EditorGUI.BeginChangeCheck();
			rigDescription.reverseLateralAngle = EditorGUILayout.Toggle(((changed[10]) ? "*" : "") + "Reverse Lateral Angle", rigDescription.reverseLateralAngle);
			if (EditorGUI.EndChangeCheck())
				changed[10] = true;

			rigDescription.dirty = (changed[0] || changed[1] || changed[2] || changed[3] || changed[4] || changed[5] || changed[6] ||
				changed[7] || changed[8] || changed[9] || changed[10]);
			GUI.enabled = true;
			if (GUILayout.Button((rigDescription.dirty) ? "Apply" : "Close"))
			{
				if (isGlobal && rigDescription.dirty)
				{
					tool.ApplyGlobalRig(changed);
				}
				tool.Repaint();
				Close();
			}

			if (string.IsNullOrEmpty(rigDescription.label))
				EditorGUI.FocusTextInControl("Name");
		}

		/// <summary>
		/// Gets a list of rig names to use in the editor.
		/// </summary>
		void GetRigList()
		{
			rigList = new string[tool.config.rigs.Count + 1];
			rigList[0] = "None";
			for (int i = 0; i < tool.config.rigs.Count; i++)
			{
				rigList[i + 1] = tool.config.rigs[i].label;
			}
		}
	}
}