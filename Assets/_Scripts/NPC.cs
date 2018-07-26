using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

	[SerializeField]
	private Animator anim;

	void Start(){
		if (anim == null) {
			anim = GetComponent <Animator> ();
		}
	}

	public void SetAnim(float joy, float pain, float fear){



	}
}
