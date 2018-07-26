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
	[CustomEditor(typeof(NewRigEditor))]
	public class NewRigEditor : EditorWindow
	{
		public AnimalRagdollTool tool;

		public RigDescription rigDescription = new RigDescription();

		string[] rigList;

		static RigEditor window;
		static void Init()
		{
			window = GetWindow<RigEditor>(false, "New Rig Editor");
			window.Show();
		}

		void OnGUI()
		{
			Event e = Event.current;
			if (e.isKey)
			{
				if (e.keyCode == KeyCode.Escape)
					Close();

				if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
				{
					CreateNewRig();
					Close();
				}
			}

			GetRigList();

			EditorGUILayout.Separator();

			GUI.SetNextControlName("Name");
			rigDescription.label = EditorGUILayout.TextField("Name", rigDescription.label);
			rigDescription.category = EditorGUILayout.Popup("Category", rigDescription.category, tool.config.categories.ToArray());
			rigDescription.connectsToInt = EditorGUILayout.Popup("Comes From", rigDescription.connectsToInt, rigList);
			rigDescription.runsToInt = EditorGUILayout.Popup("Goes To", rigDescription.runsToInt, rigList);
			rigDescription.required = EditorGUILayout.Toggle("Required", rigDescription.required);
			rigDescription.colType = (ColliderTypes)EditorGUILayout.EnumPopup("Collider Type", rigDescription.colType);
			rigDescription.direction = (CapsuleDirection)EditorGUILayout.EnumPopup("Collider Direction", rigDescription.direction);
			rigDescription.bendType = (BendTypes)EditorGUILayout.EnumPopup("Bend Type", rigDescription.bendType);
			rigDescription.hasRB = EditorGUILayout.Toggle("Has RigidBody", rigDescription.hasRB);
			rigDescription.hasJoint = EditorGUILayout.Toggle("Has Joint", rigDescription.hasJoint);
			rigDescription.reverseLateralAngle = EditorGUILayout.Toggle("Reverse Lateral Angle", rigDescription.reverseLateralAngle);

			GUI.enabled = !string.IsNullOrEmpty(rigDescription.label);
			if (GUILayout.Button("Create"))
			{
				CreateNewRig();
				Close();
			}
			GUI.enabled = true;

			if (GUILayout.Button("Close"))
			{
				Close();
			}

			if (string.IsNullOrEmpty(rigDescription.label))
				EditorGUI.FocusTextInControl("Name");
		}

		/// <summary>
		/// Creates a new rig and adds it to the current configuration.
		/// </summary>
		void CreateNewRig()
		{
			RigDescription newrig = new RigDescription();
			newrig.CopyFrom(rigDescription);
			tool.ApplyNewRig(newrig);
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