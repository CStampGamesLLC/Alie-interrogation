using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class VRUIElement : MonoBehaviour {



	public virtual void OnHover(){
	
		if (GetComponent <Image> ()) {

			if (GetComponent <Image>().color != Color.grey){
				GetComponent<Image>().color= Color.grey;
			}

		}

	}

	public virtual void OnClick(){

	}

	public virtual void OnHold(){

	}

}

