using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCorrection : MonoBehaviour {

	Animator anim;

	void Start(){

		anim = GetComponent <Animator> ();

	}

	public void CorrectRotation(){

		anim.SetTrigger ("normalize");

	}
}
