//***********************************************************************************************
//** AnimalRagdollTest.cs                                                                       
                  
//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCrafters
{
	public class AnimalRagdollTest : MonoBehaviour
	{
		public float explodeForce = 700f;

		GameObject explosionPoint;
		Rigidbody rb;

		Vector3 clickPos;

		void Start()
		{
			explosionPoint = new GameObject("ExplosionPoint");
			rb = explosionPoint.AddComponent<Rigidbody>();
			rb.isKinematic = true;
		}

		void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 100f))
				{
					clickPos = hit.point;
					Explode();
				}
			}
		}

		void Explode()
		{
			Collider[] cols = Physics.OverlapSphere(clickPos, 5f);
			foreach (Collider col in cols)
			{
				Rigidbody rigid = col.GetComponent<Rigidbody>();
				if (rigid != null)
				{
					rigid.AddExplosionForce(explodeForce, clickPos, 5f, 3f);
				}
			}
		}
	}
}