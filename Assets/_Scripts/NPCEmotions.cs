using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Laughter isn't implemented here since the smiley faces
// are just a proof of concept

public class NPCEmotions : MonoBehaviour {

	public const string JOY = "joy";
	public const string FEAR = "fear";
	public const string LAUGHTER = "laughter";
	public const string PAIN = "pain";

	[SerializeField]
	Animator anim;

	GameObject currentFace;

	public Dictionary <string, float> emotions;

	// Use this for initialization
	void Start () {
		
		emotions = new Dictionary<string, float> ();
		emotions.Add ("joy", 10);
		emotions.Add("fear", 10);
		emotions.Add("laughter", 10);
		emotions.Add("pain", 10);

	}

	public void AddFear(float fear){
		UpdateEmotions (FEAR, emotions [FEAR] += fear);
	}
	public void AddPain(float pain){
		UpdateEmotions (PAIN, emotions [PAIN] += pain);
	}
	public void AddJoy(float joy){
		UpdateEmotions (JOY, emotions [JOY] += joy);
	}
	public void AddLaughter(float laughter){
		UpdateEmotions (LAUGHTER, emotions [LAUGHTER] += laughter);
	}

	void UpdateEmotions(string key, float value){

		//clamps the new value between 0 and 100;
		emotions [key] = Mathf.Clamp (value, 0f, 100f);;

		Debug.Log (key.ToLower () + " = " + value);

		OnUpdateEmotions ();

	}

	//emotion update callback:
	//where visible changes to the npc are made
	void OnUpdateEmotions(){

		Debug.Log ("Updating emotions");

		//happy
		if (emotions [FEAR] < 50f && emotions [PAIN] < 50f) {
			if (emotions [JOY] > 20f) {
				anim.SetFloat ("blend", .2f); //happy
				if (emotions [JOY] > 50f) {
					anim.SetFloat ("blend", .1f); //pleased
					if (emotions [JOY] > 70f) {
						anim.SetFloat ("blend", 0f); //in love
					}
				}
			} 
		}

		//scared
		if ((emotions [JOY] / emotions [FEAR]) < 2 && emotions [FEAR] > emotions[PAIN]) { 
			if (emotions [FEAR] > 10f) {
				anim.SetFloat ("blend", .3f); //cringe
				if (emotions [FEAR] > 30f) {
					anim.SetFloat ("blend", .4f); //worried
					if (emotions [FEAR] > 50f) {
						anim.SetFloat ("blend", .5f); //scared
						if (emotions [FEAR] > 70f) {
							anim.SetFloat ("blend", .55f); //doomed
						}
					}
				}
			}
		}

		//hurt
		if ((emotions [JOY] / emotions [PAIN]) < 2 && emotions [PAIN] > emotions [FEAR]) { 
			if (emotions [PAIN] > 10f) {
				anim.SetFloat ("blend", .5f); //sad
				if (emotions [PAIN] > 20f) {
					anim.SetFloat ("blend", .55f); //shocked
					if (emotions [PAIN] > 40f) {
						anim.SetFloat ("blend", .6f); //hurt
						if (emotions [PAIN] > 60f) {
							anim.SetFloat ("blend", .7f); //tearing
							if (emotions [PAIN] > 80f) {
								anim.SetFloat ("blend", 1f); //weaping
							}
						}
					}
				}
			}
		}
			
	}
		
}
