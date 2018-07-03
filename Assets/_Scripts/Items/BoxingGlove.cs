using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGlove : Item {

	Animator anim;

	[SerializeField]
	GameObject rightHand;
	GameObject rightHandRenderPart;

	void Start(){
		anim = GetComponent <Animator> ();
	}

	public override void SpecialPickup ()
	{

		transform.SetParent (rightHand.transform);
		rightHandRenderPart = transform.parent.Find ("hand_right_renderPart_0").gameObject;
		rightHandRenderPart.SetActive (false);

		//Debug.Break ();

	}

	public override void SpecialDrop ()
	{

	}

	void Update(){

		float dir = Input.GetAxis ("Mouse ScrollWheel");

		if (dir > 0f) {
			anim.SetFloat ("Blend", Mathf.Clamp (anim.GetFloat ("Blend") + 0.1f, 0, 1));
		} else if (dir < 0f) {
			anim.SetFloat ("Blend", Mathf.Clamp (anim.GetFloat ("Blend") - 0.1f, 0, 1));
		}

	}

}
