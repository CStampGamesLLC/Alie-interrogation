using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGlove : Item {

	Animator anim;

	[SerializeField]
	GameObject avatar;
	[SerializeField]
	GameObject defaultHand;

	void Start(){
		anim = GetComponent <Animator> ();
	}

	public override void SpecialPickup ()
	{

		Destroy (GetComponent <Rigidbody> ());
		transform.SetParent (avatar.transform);
		defaultHand.SetActive (false);
		transform.parent.GetComponent<OvrAvatar> ().HandRight = gameObject.AddComponent <OvrAvatarHand> ();

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
