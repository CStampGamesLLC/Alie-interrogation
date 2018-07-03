using System.Collections;
using System.Collections.Generic;
using Oculus;
using UnityEngine;

public class SnapToChair : MonoBehaviour {

	[SerializeField]
	private Transform chair;

	[SerializeField]
	private Vector3 offset;

	void Start(){
		SnapToChairPos ();
	}

	void SnapToChairPos(){
		
		Vector3 newPos = chair.position;
		newPos += offset;

		transform.position = newPos;

	}

}
