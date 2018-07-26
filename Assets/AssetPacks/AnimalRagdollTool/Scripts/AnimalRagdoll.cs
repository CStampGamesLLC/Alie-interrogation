//***********************************************************************************************
//** AnimalRagdoll.cs                                                                       

//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCrafters
{
	[System.Serializable]
	public class AnimalRagdoll : MonoBehaviour
	{
		public BaseRagdollValues baseValues = new BaseRagdollValues();

		[HideInInspector, SerializeField]
		AnimalRagdollRig[] rigs;

		[HideInInspector]
		public AnimalRagdollRig[] Rigs
		{
			get { return rigs; }
		}

		public string configName;

		Animator anim;
		Rigidbody rb;
		Collider animalCollider;

		void Start()
		{
			anim = GetComponent<Animator>();
			rb = GetComponent<Rigidbody>();
			animalCollider = GetComponent<Collider>();

			GrabRigs();
		}

		/// <summary>
		/// Activates or deactivates a ragdoll.
		/// </summary>
		/// <param name="activate">True to activate.</param>
		public void ActivateRagdoll(bool activate)
		{
			foreach (AnimalRagdollRig ar in rigs)
			{
				if (ar.rigDescription.col != null)
					ar.rigDescription.col.enabled = activate;
				if (ar.rigDescription.rb != null)
					ar.rigDescription.rb.isKinematic = !activate;
			}

			if (anim != null)
				anim.enabled = !activate;
			if (rb != null)
				rb.isKinematic = activate;
			if (animalCollider != null)
				animalCollider.enabled = !activate;
		}

		/// <summary>
		/// grabs the AnimalRagdollRig components in the children.
		/// </summary>
		public void GrabRigs()
		{
			rigs = GetComponentsInChildren<AnimalRagdollRig>();
		}

#if UNITY_EDITOR
		public void RemoveRigs()
		{
			for (int i = 0; i < rigs.Length; i++)
			{
				RigDescription rd = rigs[i].rigDescription;
				if (rd.joint != null)
					DestroyImmediate(rd.joint);
				if (rd.col != null)
					DestroyImmediate(rd.col);
				if (rd.rb != null)
					DestroyImmediate(rd.rb);
				DestroyImmediate(rigs[i]);
			}
			DestroyImmediate(this);
		}
	}
#endif
}