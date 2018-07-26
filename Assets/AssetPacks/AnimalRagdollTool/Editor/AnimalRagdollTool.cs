//***********************************************************************************************
//** AnimalRagdollTool.cs                                                                       

//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BattleCrafters
{
	[CustomEditor(typeof(AnimalRagdollTool))]
	[System.Serializable]
	public class AnimalRagdollTool : EditorWindow
	{
		GameObject selectedObject;

		List<RigDescription> rigsToRemove = new List<RigDescription>();

		static RigEditor rigEditor;
		static NewRigEditor newRigEditor;
		[SerializeField]
		public AnimalRagdoll animalRagdoll;
		int rigCountReq = 0;
		Vector2 scrollPos;
		Texture2D helpButtonImage;
		Texture2D backButtonImage;
		Texture2D gearButtonImage;
		Texture2D globalButtonImage;
		Texture2D lockButtonImage;
		Texture2D unlockButtonImage;
		bool showHelp = false;
		bool showRigConfig = false;
		RigDescription globalRig = new RigDescription();

		public AnimalRagdollConfig config;
		string configName;
		string[] configs;
		int configIndex = 0;
		int copyConfigIndex = 0;
		int configIndexLast = 0;

		Color barColor = new Color(0.14117f, 0.4549f, 0.86666f, 1f);

		static AnimalRagdollTool window;
		[MenuItem("BattleCrafters/Animal Ragdoll Tool")]
		static void Init()
		{
			window = GetWindow<AnimalRagdollTool>(false, "Animal Ragdoll Tool");
			window.Show();
		}

		void Awake()
		{
			UpdateConfigList();
			LoadConfig();

			helpButtonImage = (Texture2D)Resources.Load("button_help", typeof(Texture2D));
			backButtonImage = (Texture2D)Resources.Load("button_back", typeof(Texture2D));
			gearButtonImage = (Texture2D)Resources.Load("button_gear", typeof(Texture2D));
			globalButtonImage = (Texture2D)Resources.Load("button_global", typeof(Texture2D));
			lockButtonImage = (Texture2D)Resources.Load("button_locked", typeof(Texture2D));
			unlockButtonImage = (Texture2D)Resources.Load("button_unlocked", typeof(Texture2D));
		}

		void Update()
		{
			if (selectedObject == null)
			{
				selectedObject = Selection.activeObject as GameObject;
			}

			if (rigsToRemove.Count > 0)
			{
				foreach (RigDescription rig in rigsToRemove)
				{
					config.rigs.Remove(rig);
				}
				rigsToRemove.Clear();
				Repaint();
			}
		}

		void OnDestroy()
		{
			if (rigEditor != null)
				rigEditor.Close();
		}

		void OnFocus()
		{
			if (Selection.activeGameObject != null && Selection.activeGameObject.activeSelf)
			{
				AnimalRagdoll ar = Selection.activeGameObject.GetComponent<AnimalRagdoll>();
				if (ar != null)// && ar != animalRagdoll)
				{
					animalRagdoll = ar;
					if (animalRagdoll != null)
					{
						GetRagdollData();
					}
				}
			}
		}

		void OnGUI()
		{
			EditorGUILayout.BeginVertical();

			EditorGUILayout.HelpBox("By Eric P Smith.  ©2018, Battle Crafters LLC. V1.0", MessageType.None);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Visit http://www.battlecrafters.com", GUILayout.Height(30f)))
			{
				Application.OpenURL("http://www.battlecrafters.com");
			}
			Texture2D tex = helpButtonImage;
			if (showHelp)
				tex = backButtonImage;
			if (GUILayout.Button(tex, GUILayout.Width(30f), GUILayout.Height(30f)))
			{
				showHelp = !showHelp;
				ResetConfigEditor();
			}
			EditorGUILayout.EndHorizontal();

			GUI.color = barColor;
			EditorGUILayout.HelpBox("", MessageType.None);
			GUI.color = Color.white;

			if (configs != null)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Configuration " + ((config.locked) ? "(locked)" : ""), EditorStyles.boldLabel);
				configIndex = EditorGUILayout.Popup("", configIndex, configs, GUILayout.Height(30f));
				EditorGUILayout.EndVertical();
				if (configIndex != configIndexLast)
				{
					LoadConfig();
					configIndexLast = configIndex;
				}
				if (GUILayout.Button(((config.locked) ? lockButtonImage : unlockButtonImage), GUILayout.Width(30f), GUILayout.Height(30f)))
				{
					ToggleConfigLock();
				}
				if (GUILayout.Button(globalButtonImage, GUILayout.Width(30f), GUILayout.Height(30f)))
				{
					EditGlobalRig();
				}
				if (GUILayout.Button(gearButtonImage, GUILayout.Width(30f), GUILayout.Height(30f)))
				{
					showRigConfig = !showRigConfig;
				}
				EditorGUILayout.EndHorizontal();
			}

			GUI.color = barColor;
			EditorGUILayout.HelpBox("", MessageType.None);
			GUI.color = Color.white;

			GUI.enabled = !config.locked && !Application.isPlaying;
			config.baseValues.totalMass = EditorGUILayout.FloatField("Total Mass", config.baseValues.totalMass);
			config.baseValues.distributeMass = EditorGUILayout.Toggle("Distribute Mass Evenly", config.baseValues.distributeMass);
			config.baseValues.lateralAngle = EditorGUILayout.Slider("Lateral Angle", config.baseValues.lateralAngle, 0f, 180f);
			config.baseValues.bendAngle = EditorGUILayout.Slider("Bend Angle", config.baseValues.bendAngle, 0f, 180f);
			config.baseValues.coordSystem = (CoordinateSystem)EditorGUILayout.EnumPopup("Coordinate System", config.baseValues.coordSystem);
			GUI.enabled = true;

			GUI.color = barColor;
			EditorGUILayout.HelpBox("", MessageType.None);
			GUI.color = Color.white;

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.box);
			EditorGUILayout.BeginVertical(GUILayout.Width(position.width - 24f));

			if (config.baseValues.totalMass < rigCountReq + 1)
				config.baseValues.totalMass = rigCountReq + 1;

			if (showRigConfig)
			{
				DrawConfigEditor();
			}
			else
			{
				ResetConfigEditor();
				if (showHelp)
				{
					DrawHelp();
				}
				else
				{
					bool canCreate = true;
					foreach (RigDescription rig in config.rigs)
					{
						if (rig.source == null && rig.required)
							canCreate = false;
					}

					EditorGUILayout.BeginHorizontal();
					GUI.enabled = canCreate && (animalRagdoll == null) && !Application.isPlaying && config.rigs.Count > 0;
					if (GUILayout.Button("Create"))
					{
						CreateRagdoll();
					}
					GUI.enabled = true;

					GUI.enabled = !Application.isPlaying;
					if (GUILayout.Button("Reset"))
					{
						if (EditorUtility.DisplayDialog("Reset Configuration", "You are about to reset this configuration and you will lose all changes.\n\nAre you sure?", "Reset", "Cancel"))
							ResetConfig();
					}
					GUI.enabled = true;

					GUI.enabled = (animalRagdoll != null) && !Application.isPlaying;
					if (GUILayout.Button("Remove"))
					{
						if (EditorUtility.DisplayDialog("Remove Ragdoll", "You are about to remove the ragdoll from the current object.\n\nAre you sure?", "Remove", "Cancel"))
							RemoveRagdoll();
					}
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();

					for (int i = 0; i < config.categories.Count; i++)
					{
						EditorGUILayout.Separator();

						GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
						myStyle.richText = true;
						myStyle.alignment = TextAnchor.UpperLeft;
						myStyle.fontSize = 10;
						GUI.color = new Color(1f, 1f, 0.4f);
						EditorGUILayout.LabelField("<b>" + config.categories[i] + "</b>", myStyle);
						GUI.color = Color.white;
						foreach (RigDescription rig in config.rigs)
						{
							if (rig.category >= config.categories.Count)
								rig.category = 0;
							if (rig.category == i)
								DrawField(rig);
						}
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUI.enabled = !config.locked && !Application.isPlaying;
						if (GUILayout.Button("+", GUILayout.Width(24f)))
						{
							Vector2 clickpos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
							newRigEditor = GetWindow<NewRigEditor>("New Rig");
							newRigEditor.tool = this;
							newRigEditor.minSize = newRigEditor.maxSize = new Vector2(300f, 250f);
							newRigEditor.position = new Rect(clickpos.x - (newRigEditor.minSize.x / 2f), clickpos.y - (newRigEditor.minSize.y / 2f), newRigEditor.minSize.x, newRigEditor.minSize.y);
							newRigEditor.rigDescription.category = i;
							newRigEditor.Show();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = true;
					}
				}
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// Applies the changes from the Global Rig to all rigs in the selected configuration.
		/// </summary>
		/// <param name="changed">An array of the changed values from the RigEditor.</param>
		public void ApplyGlobalRig(bool[] changed)
		{
			for (int i = 0; i < config.rigs.Count; i++)
			{
				if (changed[0])
					config.rigs[i].label = globalRig.label;
				if (changed[1])
					config.rigs[i].category = globalRig.category;
				if (changed[2])
					config.rigs[i].connectsToInt = globalRig.connectsToInt;
				if (changed[3])
					config.rigs[i].runsToInt = globalRig.runsToInt;
				if (changed[4])
					config.rigs[i].required = globalRig.required;
				if (changed[5])
					config.rigs[i].colType = globalRig.colType;
				if (changed[6])
					config.rigs[i].direction = globalRig.direction;
				if (changed[7])
					config.rigs[i].bendType = globalRig.bendType;
				if (changed[8])
					config.rigs[i].hasRB = globalRig.hasRB;
				if (changed[9])
					config.rigs[i].hasJoint = globalRig.hasJoint;
				if (changed[10])
					config.rigs[i].reverseLateralAngle = globalRig.reverseLateralAngle;

				if (animalRagdoll != null)
				{
					animalRagdoll.Rigs[i].rigDescription.CopyFrom(config.rigs[i]);
				}
			}
		}

		/// <summary>
		/// Create and adds a new rig to the configuration.
		/// </summary>
		/// <param name="rig">The newly created rig from the NewRigEditor.</param>
		public void ApplyNewRig(RigDescription rig)
		{
			config.rigs.Add(rig);
			Repaint();
		}

		/// <summary>
		/// Creates the ragdoll when the 'Create' button is pressed.
		/// </summary>
		void CreateRagdoll()
		{
			//count the required rigs with rigidbodies to calculate mass
			rigCountReq = 0;
			foreach (RigDescription rig in config.rigs)
			{
				if (rig.required && rig.hasRB)
					rigCountReq++;
			}

			Transform rootObject = config.rigs[0].source.root;
			if (rootObject != null)
			{
				animalRagdoll = rootObject.GetComponent<AnimalRagdoll>();
				if (animalRagdoll == null)
					animalRagdoll = rootObject.gameObject.AddComponent<AnimalRagdoll>();
				animalRagdoll.baseValues.totalMass = config.baseValues.totalMass;
				animalRagdoll.baseValues.distributeMass = config.baseValues.distributeMass;
				animalRagdoll.baseValues.lateralAngle = config.baseValues.lateralAngle;
				animalRagdoll.baseValues.bendAngle = config.baseValues.bendAngle;
				animalRagdoll.baseValues.coordSystem = config.baseValues.coordSystem;
				animalRagdoll.configName = config.properName;
			}

			foreach (RigDescription rig in config.rigs)
			{
				if (rig.source != null)
				{
					if (rig.ragdollRig == null)
						rig.ragdollRig = rig.source.gameObject.AddComponent<AnimalRagdollRig>();
					rig.ragdollRig.rigDescription = rig;

					Transform connectedTo = null;
					RigDescription connectedToRig = null;
					if (rig.connectsToInt > 0)
						connectedToRig = config.rigs[rig.connectsToInt - 1];
					if (connectedToRig != null)
						connectedTo = connectedToRig.source;

					Transform runsTo = null;
					RigDescription runsToRig = null;
					if (rig.runsToInt > 0)
						runsToRig = config.rigs[rig.runsToInt - 1];
					if (runsToRig != null)
						runsTo = runsToRig.source;

					rig.connectedTo = connectedTo;
					rig.runsTo = runsTo;

					if (rig.hasRB)
					{
						rig.rb = rig.source.gameObject.AddComponent<Rigidbody>();
						rig.rb.mass = ((config.baseValues.distributeMass) ? config.baseValues.totalMass / rigCountReq : 1f);
					}
				}
			}

			foreach (RigDescription rig in config.rigs)
			{
				if (rig.source != null)
				{
					if (rig.hasJoint)
						AddJoint(rig);

					if (rig.source != null)
						AddCollider(rig);
				}
			}

			GetRigByName("Pelvis").rb.mass = config.baseValues.totalMass / rigCountReq;
			//rigs[0].rb.mass = totalMass / rigCountReq;

			animalRagdoll.GrabRigs();
		}

		/// <summary>
		/// Creates a new rig and adds it to the selected configuration.
		/// </summary>
		/// <param name="category">The category index.</param>
		/// <param name="label">The name of the rig.</param>
		/// <param name="coltype">The collider type of the rig.</param>
		/// <param name="bendtype">The bend type of the rig.</param>
		/// <param name="hasrb">True if the rig will have a Rigidbody component.</param>
		/// <param name="hasjoint">True if the rig will have a ConfigurableJoint component.</param>
		/// <param name="reverselateral">True to reverse the lateral angle.</param>
		/// <param name="req">True if this rigs transform must be set.</param>
		void CreateRig(int category, string label, ColliderTypes coltype, BendTypes bendtype, bool hasrb, bool hasjoint, bool reverselateral, bool req = true)
		{
			RigDescription rig = new RigDescription();
			rig.category = category;
			rig.label = label;
			rig.required = req;
			rig.connectsToInt = 0;
			rig.runsToInt = 0;
			rig.colType = coltype;
			rig.bendType = bendtype;
			rig.hasRB = hasrb;
			rig.hasJoint = hasjoint;
			rig.reverseLateralAngle = reverselateral;
			config.AddRig(rig);
		}

		/// <summary>
		/// Creates a new rig and adds it to the selected configuration.
		/// </summary>
		/// <param name="category">The category index.</param>
		/// <param name="label">The name of the rig.</param>
		void CreateRig(int category, string label)
		{
			RigDescription rig = new RigDescription();
			rig.category = category;
			rig.label = label;
			rig.required = true;
			rig.connectsToInt = 0;
			rig.runsToInt = 0;
			rig.colType = ColliderTypes.Box;
			rig.bendType = BendTypes.None;
			rig.hasRB = false;
			rig.hasJoint = false;
			rig.reverseLateralAngle = false;
			config.rigs.Add(rig);
		}

		/// <summary>
		/// Adds a collider the the given rig.
		/// </summary>
		/// <param name="rig">The rig to add the collider to according to its settings.</param>
		void AddCollider(RigDescription rig)
		{
			if (rig.source == null)
				return;

			if (rig.colType == ColliderTypes.Box)
			{
				float dist = 0.3f;
				Transform transTo = rig.connectedTo;
				if (transTo == null)
					transTo = rig.runsTo;
				dist = (rig.source.position - transTo.position).magnitude;
				rig.col = rig.source.gameObject.AddComponent<BoxCollider>();
				((BoxCollider)rig.col).size = new Vector3(dist, dist, dist);
			}
			else if (rig.colType == ColliderTypes.Capsule)
			{
				rig.col = rig.source.gameObject.AddComponent<CapsuleCollider>();
				CapsuleCollider cap = (CapsuleCollider)rig.col;
				cap.direction = (int)rig.direction;
				Transform transTo = rig.runsTo;
				if (transTo == null)
					transTo = rig.connectedTo;
				float dist = (rig.source.position - transTo.position).magnitude;
				cap.height = dist;
				cap.radius = dist * 0.3f;
				switch (rig.direction)
				{
					case CapsuleDirection.XAxis:
						cap.center = new Vector3(-(dist * 0.5f), 0f, 0f);
						break;
					case CapsuleDirection.YAxis:
						cap.center = new Vector3(0f, -(dist * 0.5f), 0f);
						break;
					case CapsuleDirection.ZAxis:
						cap.center = new Vector3(0f, 0f, -(dist * 0.5f));
						break;
				}
			}
			else if (rig.colType == ColliderTypes.Sphere || rig.colType == ColliderTypes.SphereCentered)
			{
				Transform transTo = rig.connectedTo;
				if (transTo == null)
					transTo = rig.runsTo;
				rig.col = rig.source.gameObject.AddComponent<SphereCollider>();
				SphereCollider cap = (SphereCollider)rig.col;
				float dist = (rig.source.position - transTo.position).magnitude;
				cap.radius = dist * ((rig.colType == ColliderTypes.SphereCentered) ? 0.5f : 1f);
				if (rig.colType == ColliderTypes.Sphere)
				{
					switch (rig.direction)
					{
						case CapsuleDirection.XAxis:
							cap.center = new Vector3(-(dist * 0.5f), 0f, 0f);
							break;
						case CapsuleDirection.YAxis:
							cap.center = new Vector3(0f, -(dist * 0.5f), 0f);
							break;
						case CapsuleDirection.ZAxis:
							cap.center = new Vector3(0f, 0f, -(dist * 0.5f));
							break;
					}
				}
			}
		}

		/// <summary>
		/// Adds a ConfigurableJoint to the given rig.
		/// </summary>
		/// <param name="rig">The rig to add to.</param>
		void AddJoint(RigDescription rig)
		{
			//rig.joint = trans.gameObject.AddComponent<CharacterJoint>();
			rig.joint = rig.source.gameObject.AddComponent<ConfigurableJoint>();
			rig.joint.connectedBody = config.rigs[rig.connectsToInt - 1].rb;
			//rig.joint.enableProjection = true;
			rig.joint.enablePreprocessing = false;

			rig.joint.xMotion = ConfigurableJointMotion.Locked;
			rig.joint.yMotion = ConfigurableJointMotion.Locked;
			rig.joint.zMotion = ConfigurableJointMotion.Locked;

			rig.joint.angularXMotion = ConfigurableJointMotion.Limited;
			rig.joint.angularYMotion = ConfigurableJointMotion.Locked;
			rig.joint.angularZMotion = ConfigurableJointMotion.Limited;

			SoftJointLimit limit = new SoftJointLimit();

			switch (rig.bendType)
			{
				case BendTypes.None:
					rig.joint.axis = (config.baseValues.coordSystem == CoordinateSystem.Y_Up ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 0f, 1f));
					float latangle = config.baseValues.lateralAngle;
					if (!rig.reverseLateralAngle)
						latangle = (180f - config.baseValues.lateralAngle);
					limit.limit = -latangle;
					rig.joint.lowAngularXLimit = limit;
					limit = rig.joint.highAngularXLimit;
					limit.limit = latangle;
					rig.joint.highAngularXLimit = limit;
					break;
				case BendTypes.Forwards:
					rig.joint.axis = (config.baseValues.coordSystem == CoordinateSystem.Y_Up ? new Vector3(1f, 0f, 0f) : new Vector3(0f, 1f, 0f));
					limit.limit = config.baseValues.bendAngle;
					rig.joint.angularZLimit = limit;
					break;
				case BendTypes.Backwards:
					rig.joint.axis = (config.baseValues.coordSystem == CoordinateSystem.Y_Up ? new Vector3(-1f, 0f, 0f) : new Vector3(0f, -1f, 0f));
					limit.limit = config.baseValues.bendAngle;
					rig.joint.angularZLimit = limit;
					break;
			}
		}

		/// <summary>
		/// Removed the rig from the currently selected object.
		/// </summary>
		void RemoveRagdoll()
		{
			if (animalRagdoll != null)
			{
				animalRagdoll.RemoveRigs();
			}
		}

		/// <summary>
		/// Gets the data from the selected AnimalRagdoll component and sets it up in the tool.
		/// </summary>
		void GetRagdollData()
		{
			configIndex = GetConfigIndex(animalRagdoll.configName);
			foreach (AnimalRagdollRig rig in animalRagdoll.Rigs)
			{
				RigDescription thisRig = GetRigByName(rig.rigDescription.label);
				if (thisRig != null)
				{
					thisRig.CopyFrom(rig.rigDescription);
				}
			}
			config.baseValues.totalMass = animalRagdoll.baseValues.totalMass;
			config.baseValues.distributeMass = animalRagdoll.baseValues.distributeMass;
			config.baseValues.lateralAngle = animalRagdoll.baseValues.lateralAngle;
			config.baseValues.bendAngle = animalRagdoll.baseValues.bendAngle;
			config.baseValues.coordSystem = animalRagdoll.baseValues.coordSystem;
		}

		/// <summary>
		/// Gets a rig by a given name.
		/// </summary>
		/// <param name="rigname">The name of the rig to retrieve.</param>
		/// <returns></returns>
		RigDescription GetRigByName(string rigname)
		{
			foreach (RigDescription rig in config.rigs)
			{
				if (rig.label.ToLower() == rigname.ToLower())
					return rig;
			}
			return null;
		}

		/// <summary>
		/// Gets the index of the rig in the config.rigs list.
		/// </summary>
		/// <param name="rigname">The name of the rig to retrieve.</param>
		/// <returns></returns>
		int GetRigIndex(string rigname)
		{
			for (int i = 0; i < config.rigs.Count; i++)
			{
				if (config.rigs[i].label.ToLower() == rigname.ToLower())
				{
					return i + 1;
				}
			}
			return 0;
		}

		/// <summary>
		/// Draws the rigs tool controls in the editor GUI.
		/// </summary>
		/// <param name="rig">The rig to draw.</param>
		void DrawField(RigDescription rig)
		{
			EditorGUILayout.BeginHorizontal();
			rig.source = (Transform)EditorGUILayout.ObjectField(rig.label, rig.source, typeof(Transform), true);

			GUI.enabled = rig.label != "Pelvis" && !config.locked;
			if (GUILayout.Button("X", GUILayout.Width(24f)))
			{
				rigsToRemove.Add(rig);
			}
			GUI.enabled = true;

			if (GUILayout.Button("...", GUILayout.Width(24f)))
			{
				Vector2 clickpos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				rigEditor = GetWindow<RigEditor>("Edit Rig");
				rigEditor.tool = this;
				rigEditor.minSize = rigEditor.maxSize = new Vector2(300f, 230f);
				rigEditor.position = new Rect(clickpos.x - (rigEditor.minSize.x / 2f), clickpos.y - (rigEditor.minSize.y / 2f), rigEditor.minSize.x, rigEditor.minSize.y);
				rigEditor.Show();
				rigEditor.rigDescription = rig;
			}
			EditorGUILayout.EndHorizontal();

		}

		/// <summary>
		/// Open the global rig editor.
		/// </summary>
		void EditGlobalRig()
		{
			Vector2 clickpos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
			rigEditor = GetWindow<RigEditor>("Global Rig");
			rigEditor.tool = this;
			rigEditor.minSize = rigEditor.maxSize = new Vector2(300f, 150f);
			rigEditor.position = new Rect(clickpos.x - (rigEditor.minSize.x / 2f), clickpos.y - (rigEditor.minSize.y / 2f), rigEditor.minSize.x, rigEditor.minSize.y);
			rigEditor.Show();
			rigEditor.rigDescription = globalRig;
			rigEditor.isGlobal = true;
		}

		/// <summary>
		/// Reads in the configuration files into a list.
		/// </summary>
		void UpdateConfigList()
		{
			AssetDatabase.Refresh();
			configs = null;
			UnityEngine.Object[] obj = Resources.LoadAll<AnimalRagdollConfig>("Configurations");
			configs = new string[obj.Length];
			for (int i = 0; i < obj.Length; i++)
			{
				configs[i] = ((AnimalRagdollConfig)obj[i]).properName;
			}
		}

		/// <summary>
		/// Retrieves a configuration index by properName.
		/// </summary>
		/// <param name="configname">The name of the configuration.</param>
		/// <returns></returns>
		int GetConfigIndex(string configname)
		{
			for (int i = 0; i < configs.Length; i++)
			{
				if (configname == configs[i])
					return i;
			}
			return 0;
		}

		/// <summary>
		/// Resets the configuration.
		/// </summary>
		void ResetConfig()
		{
			foreach (RigDescription rig in config.rigs)
			{
				rig.source = null;
				rig.connectedTo = null;
				rig.runsTo = null;
			}
			LoadConfig();
		}

		/// <summary>
		/// Loads the configuration into the tool as determined by the configName.
		/// </summary>
		void LoadConfig()
		{
			configName = configs[configIndex];
			config = (AnimalRagdollConfig)Resources.Load("Configurations/" + configName);

			rigCountReq = 0;
			foreach (RigDescription rig in config.rigs)
			{
				if (rig.required && rig.hasRB)
					rigCountReq++;
			}
		}

		/// <summary>
		/// Saves the current configuration.
		/// </summary>
		void SaveConfig()
		{
			if (EditorUtility.DisplayDialog("Save Configuration", "You are about to save the current rig settings to the active configuration. (" + configName + ")", "Save", "Cancel"))
			{
				config = (AnimalRagdollConfig)Resources.Load("Configurations/" + configName);
				if (config == null)
				{
					config = CreateInstance<AnimalRagdollConfig>();
					AssetDatabase.CreateAsset(config, "Assets/AnimalRagdollTool/Resources/Configurations/" + configName + ".asset");
				}
				EditorUtility.SetDirty(config);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				config.properName = configName;
			}
		}

		/// <summary>
		/// Creates a new configuration.
		/// </summary>
		void NewConfig()
		{
			config = CreateInstance<AnimalRagdollConfig>();
			AssetDatabase.CreateAsset(config, "Assets/AnimalRagdollTool/Resources/Configurations/" + configName + ".asset");
			EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			config.properName = configName;
		}

		/// <summary>
		/// Locks or unlocks the current configuration.
		/// </summary>
		void ToggleConfigLock()
		{
			if (config.locked)
			{
				if (EditorUtility.DisplayDialog("Unlock Configuration", configName + " is currently locked to prevent editing of this configuration." +
					"  If you unlock it, changes you make will affect the source configuration.\n\nMake a copy instead by clicking the gear icon and click New Config.", "Unlock", "Cancel"))
				{
					config.locked = false;
				}
			}
			else
			{
				if (EditorUtility.DisplayDialog("Lock Configuration", configName + " will be locked. If you lock it, you will not be able to make changes until it is unlocked.", "Lock", "Cancel"))
				{
					config.locked = true;
				}
			}
		}

		/// <summary>
		/// Draws the help text to the help panel.
		/// </summary>
		void DrawHelp()
		{
			string percent = string.Format("<b><i>{0:##.#}</i></b>", (config.baseValues.totalMass - rigCountReq));
			string percent2 = string.Format("<b><i>{0:#.0}</i></b>", (config.baseValues.totalMass / rigCountReq));
			string helptext =
				"<color=#2474DD><b>Zerk's Animal Ragdoll Tool</b></color>\n\n" +
				"<color=#DDDDDD>This tool was designed to allow you to create ragdolls on animal models.  " +
				"This tool uses the ConfigurableJoint and not the CharacterJoint." +
				"</color>\n\n" +
				AddHelpLine("Configuration", "The currently selected configuration.  When selecting a new configuration, it will automatically be loaded into the tool.") +
				AddHelpLine("Total Mass", "Total mass is the amount of rigidbody mass that will be applied to all rigigbodies of the ragdoll.") +
				AddHelpLine("Distribute Mass Evenly", "If unchecked, only the pelvis will be given a mass of " + percent + " from the Total Mass value " +
				"while other rigidbodies will be given a mass value of 1.  " +
				"Otherwise, if checked, a mass of " + percent2 + " from the Total Mass will be distributed to the mass of all required rigidbodies of the ragdoll.") +
				AddHelpLine("Lateral Angle", "The angle at which ragdoll limbs will collapse outward and inword of the body.") +
				AddHelpLine("Bend Angle", "The angle at which joints will bend forward and backwards of the body.") +
				AddHelpLine("Coordinate System", " Sets which coordinate system will be used to determine joint and collider settings.  Some artists may use a 'left handed' (where Z is up) coordinate system when modeling which may also depend on the modeling tool they use.") +
				AddHelpLine("Ragdoll Panel", "This is where you will set all the transforms used in the animal ragdoll.  The panel consists of Categories and Rigs.  The options buttons at top will perform certain actions on the animal.  They are:") +
				AddHelpLine("   - Create", "Create the ragdoll.  This option will remain disabled until all the required rig transforms have been set.") +
				AddHelpLine("   - Reset", "This option will reset the rigs in the tool erasing all the transform fields and connections.  It will also reload the current configuration.") +
				AddHelpLine("   - Remove", "This button will remove the ragdoll from the animal model.  It can only be used when a ragdoll has been created.") +
				AddHelpLine("Transform Field", "This is a pointer to the transform that will be used for the specific ragdoll joint and can originate from either a project prefab or from the heirarchy.") +
				AddHelpLine("Remove Rig", "The button denoted by the 'X' will remove this rig from the configuration.") +
				AddHelpLine("Add Rig", "The button denoted by a '+' at the bottom of the category will bring up a panel allowing you to create a new rig.") +
				AddHelpLine("Rig Options", "The button denoted by '...' next to each transform field will bring up a dialog of options that you can change.  They are:") +
				AddHelpLine("   - Name", "This is the name of the rig.  It is used for cross referrence and labeling.  Make sure each one is unique.") +
				AddHelpLine("   - Category", "The category this rig belongs to for sorting and organization.  You can create or delete categories from the Gear icon.") +
				AddHelpLine("   - Comes From", "Specifies which rig that this rig is attached to and given the 'Connected Body' value in the joint.") +
				AddHelpLine("   - Goes To", "Specifies which rig that this rig points towards.") +
				AddHelpLine("   - Required", "If checked, then the transform field must be populated before the ragdoll can be generated.") +
				AddHelpLine("   - Collider Type", "The type of collider that will be placed on the rig.") +
				AddHelpLine("   - Collider Direction", "Sets the direction the collider will be offset from it's transform.") +
				AddHelpLine("   - Bend Type", "The direction at which limbs will bend.  None is free motion and will use the Lateral Angle for the joint constraint while Forwards and Backwards bend respectively as determined by the Bend Angle value and Coordinate System.") +
				AddHelpLine("   - Has Rigidbody", "If checked, a rigidbody will be placed on this rig.") +
				AddHelpLine("   - Has Joint", "If checked, a ConfigurableJoint will be added to the rig and will be configured according to the Lateral Angle, Bend Angle, Bend Type and Coordinate System.") +
				AddHelpLine("   - Reverse Lateral Angle", "Reverses the lateral angle to allow bending in the opposite direction.") +
				AddHelpLine("Lock Configuration", "This button denoted by the paddle lock icon, will lock or unlock the configuration respectively.  This is to prevent you from changing values to the configuration.  It is recommended to make a copy of the configuration before editing.") +
				AddHelpLine("Global Rig", "The button up top denoted by the global icon is the global rig configuration.  Changing values here and clicking Apply will apply all changed values to all rigs int the configuration.") +
				AddHelpLine("Options", "The button denoted by the gear icon will allow you to perform other functionality in the tool.  They are:") +
				AddHelpLine("   - New Config", "Alows you to create a new configuration from scratch or from another configuration.  To copy a new configuration, select the configuration up top that you want to copy, enter a new name and then click the 'Copy' button.") +
				AddHelpLine("   - Categories", "This option allows you to edit the categories in the configuration be it adding, changing or removing them.") +
				AddHelpLine("   - Copy Category Rigs", "Will allow you to copy all the rigs from one category to another.  Copied rig names will be appended with the 'new' keyword but will still have the original 'Comes From' and 'Goes To' settings that you will need to change.") +
				AddHelpLine("   - Save Config", "This will save the current configuration to the project.  You cannot save locked configurations.") +
				AddHelpLine("   - Delete Config", "This option will delete the current configuration from the project.  This cannot be undone and you cannot delete locked configurations.") +
				//AddHelpLine("", "") +
				"";
			GUIStyle myStyle = GUI.skin.GetStyle("TextArea");
			myStyle.richText = true;
			GUI.enabled = false;
			EditorGUILayout.TextArea(helptext, myStyle);
			GUI.enabled = true;
		}

		/// <summary>
		/// Renders a line of help text to the editor GUI.
		/// </summary>
		/// <param name="label">The label of the help text.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		string AddHelpLine(string label, string text)
		{
			return "<color=#BBBB00><b>" + label + ": </b></color><color=#DDDDDD>" + text + "</color>\n\n";
		}

		bool showNewConfig = false;
		bool showNewCategory = false;
		bool showCopyCategory = false;
		string inputText;
		int fromCat;
		int toCat;
		/// <summary>
		/// Draws the editor GUI for the configuration panel.
		/// </summary>
		void DrawConfigEditor()
		{
			EditorGUILayout.BeginHorizontal();
			GUI.enabled = !Application.isPlaying;
			if (GUILayout.Button("New Config", GUILayout.Width((position.width - 30f) / 2f)))
			{
				showNewConfig = true;
				showNewCategory = false;
				showCopyCategory = false;
				inputText = "";
			}
			GUI.enabled = true;

			GUI.enabled = !Application.isPlaying && !config.locked;
			if (GUILayout.Button("Save Config", GUILayout.Width((position.width - 30f) / 2f)))
			{
				SaveConfig();
				ResetConfigEditor();
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUI.enabled = !Application.isPlaying;
			if (GUILayout.Button("Categories", GUILayout.Width((position.width - 30f) / 2f)))
			{
				showNewCategory = true;
				showNewConfig = false;
				showCopyCategory = false;
				inputText = "";
			}
			GUI.enabled = true;

			GUI.enabled = !Application.isPlaying && !config.locked;
			if (GUILayout.Button("Delete Config", GUILayout.Width((position.width - 30f) / 2f)))
			{
				if (EditorUtility.DisplayDialog("Delete Configuration", "You are about to completely delete the configuration (" + configName + ") from your system.  This action cannot be undone.\n\nAre you sure?", "Delete", "Cancel"))
				{
					string path = AssetDatabase.GetAssetPath(config);
					AssetDatabase.DeleteAsset(path);
					UpdateConfigList();
					configIndex = 0;
					LoadConfig();
					configIndexLast = configIndex;
				}
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUI.enabled = !Application.isPlaying;
			if (GUILayout.Button("Copy Category Rigs", GUILayout.Width((position.width - 30f) / 2f)))
			{
				showNewCategory = false;
				showNewConfig = false;
				showCopyCategory = true;
				inputText = "";
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();

			if (showNewConfig)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox("Enter a name for the new configuration.  The text entered here will also be used for the configurations file name.  " +
					"For copying from another configuration, you must enter a new unique name.", MessageType.Info);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("New Configuration Name", EditorStyles.boldLabel);
				GUI.SetNextControlName("Name");
				inputText = EditorGUILayout.TextField("", inputText);
				EditorGUILayout.LabelField("Copy From", EditorStyles.boldLabel);
				//copyConfigIndex = EditorGUILayout.Popup("", copyConfigIndex, configs, GUILayout.Height(30f));
				EditorGUILayout.LabelField(config.properName);
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
				if (GUILayout.Button("Create"))
				{
					configName = inputText;
					NewConfig();
					config.categories.Add("Body");
					CreateRig(0, "Pelvis", ColliderTypes.SphereCentered, BendTypes.None, true, false, false);
					SaveConfig();
					UpdateConfigList();
					configIndex = GetConfigIndex(configName);
					LoadConfig();
					ResetConfigEditor();
				}
				if (GUILayout.Button("Done"))
				{
					ResetConfigEditor();
				}
				GUI.enabled = !string.IsNullOrEmpty(inputText);
				if (GUILayout.Button("Copy"))
				{
					configName = inputText;
					AnimalRagdollConfig oldconfig = config;
					NewConfig();
					config.CopyFrom(oldconfig);
					SaveConfig();
					UpdateConfigList();
					configIndex = GetConfigIndex(configName);
					LoadConfig();
					ResetConfigEditor();
				}
				GUI.enabled = true;
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();

				if (string.IsNullOrEmpty(inputText))
					EditorGUI.FocusTextInControl("Name");
			}

			if (showNewCategory)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox("Enter a name for the new category.  Catgegories are used for rig sorting.", MessageType.Info);
				GUI.enabled = !Application.isPlaying && !config.locked;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("New Category Name", EditorStyles.boldLabel);
				GUI.SetNextControlName("Name");
				inputText = EditorGUILayout.TextField("", inputText);
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
				if (GUILayout.Button("Create"))
				{
					config.categories.Add(inputText);
					ResetConfigEditor();
				}
				if (GUILayout.Button("Done"))
				{
					ResetConfigEditor();
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				GUI.enabled = true;

				EditorGUILayout.Separator();

				for (int i = 0; i < config.categories.Count; i++)
				{
					GUI.enabled = !Application.isPlaying && !config.locked;
					EditorGUILayout.BeginHorizontal();
					config.categories[i] = EditorGUILayout.TextField("", config.categories[i]);
					if (GUILayout.Button("X", GUILayout.Width(24f)))
					{
						config.categories.RemoveAt(i);
					}
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}

				//if (string.IsNullOrEmpty(inputText))
				//	EditorGUI.FocusTextInControl("Name");
			}

			if (showCopyCategory)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox("This will copy the rigs from one category to another category.", MessageType.Info);
				GUI.enabled = !Application.isPlaying && !config.locked;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				fromCat = EditorGUILayout.Popup("Copy From", fromCat, config.categories.ToArray());
				toCat = EditorGUILayout.Popup("Copy To", toCat, config.categories.ToArray());
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
				if (GUILayout.Button("Copy"))
				{
					List<RigDescription> newrigs = new List<RigDescription>();
					foreach (RigDescription rig in config.rigs)
					{
						if (rig.category == fromCat)
						{
							RigDescription newrig = new RigDescription();
							newrig.CopyFrom(rig);
							newrig.label += " new";
							newrig.category = toCat;
							newrigs.Add(newrig);
						}
					}
					foreach (RigDescription rig in newrigs)
					{
						config.rigs.Add(rig);
					}
					ResetConfigEditor();
				}
				if (GUILayout.Button("Done"))
				{
					ResetConfigEditor();
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				GUI.enabled = true;

				EditorGUILayout.Separator();
			}
		}

		/// <summary>
		/// Resets the configuration editor panel.
		/// </summary>
		void ResetConfigEditor()
		{
			showRigConfig = false;
			showNewConfig = false;
			showNewCategory = false;
			showCopyCategory = false;
		}
	}
}