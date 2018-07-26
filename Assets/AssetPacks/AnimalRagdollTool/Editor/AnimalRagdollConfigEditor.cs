//***********************************************************************************************
//** AnimalRagdollConfigEditor.cs                                                                       
                  
//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCrafters
{
	public class AnimalRagdollConfigEditor : MonoBehaviour
	{
		public static AnimalRagdollConfigEditor Instance;
		
		void Awake()
		{
			if (Instance != null)
				Destroy(gameObject);
			else
				Instance = this;
		}
	
		void Start()
		{
			
		}
		
		void Update()
		{
			
		}
	}
}