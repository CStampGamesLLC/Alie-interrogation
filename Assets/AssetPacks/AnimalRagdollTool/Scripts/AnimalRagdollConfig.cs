//***********************************************************************************************
//** AnimalRagdollConfig.cs                                                                       
                  
//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCrafters
{
	[System.Serializable]
	public class AnimalRagdollConfig : ScriptableObject
	{
		public string properName = "Quadruped (4 legs)";

		//[HideInInspector]
		public bool locked = false;

		[HideInInspector]
		public BaseRagdollValues baseValues;

		[HideInInspector]
		public List<string> categories = new List<string>();

		[HideInInspector]
		public List<RigDescription> rigs = new List<RigDescription>();

		/// <summary>
		/// Adds a rig to this configuration.
		/// </summary>
		/// <param name="rig">The rig to add.</param>
		public void AddRig(RigDescription rig)
		{
			rigs.Add(rig);
		}

		/// <summary>
		/// Copies one configuration to another.
		/// </summary>
		/// <param name="config">The configuration to copy.</param>
		public void CopyFrom(AnimalRagdollConfig config)
		{
			baseValues.CopyFrom(config.baseValues);

			categories.Clear();
			foreach (string cat in config.categories)
			{
				categories.Add(cat);
			}

			rigs.Clear();
			foreach (RigDescription rig in config.rigs)
			{
				rigs.Add(rig);
			}
		}
	}
}