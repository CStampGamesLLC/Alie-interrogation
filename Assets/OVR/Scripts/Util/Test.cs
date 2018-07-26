using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public GameObject dude;

	[ContextMenu("Test")]
	public void Testies(){
		Instantiate (dude);
	}
}
