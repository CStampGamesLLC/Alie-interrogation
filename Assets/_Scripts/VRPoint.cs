﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRPoint : MonoBehaviour {

	public Text text;

	void Update(){

		RaycastHit hit;

		if (Physics.Raycast (new Ray (transform.position, transform.forward), out hit, 100f)){

			text.text = hit.transform.name;
			Debug.DrawRay (transform.position, transform.forward);
		
		}

	}
}
