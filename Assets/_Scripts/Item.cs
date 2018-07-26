using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]//[RequireComponent(typeof(Rigidbody))]
public abstract class Item : MonoBehaviour {

	//The last item that was grabbed
	public static Item lastGrabbed;

	//factored into stat modifications instead of changing stat mods
	protected float strength = 1.0f;

	public float joyMod = 1;
	public float laughterMod = 1;
	public float fearMod = 1;
	public float painMod = 1;

	protected Rigidbody rb;
	protected new AudioSource audio;

	NPCEmotions NPC;

	bool justHit = false;

	[HideInInspector]
	public bool hitting = false;
	public Vector3 hitDirection;

	public AudioClip[] soundEffects;

	void Awake(){

		//reference rigidbody and audiosource

		if (GetComponent <Rigidbody> () == null) {
			rb = gameObject.AddComponent<Rigidbody> ();
		} else {
			rb = GetComponent <Rigidbody> ();
		}

		if (GetComponent <AudioSource> () == null) {
			audio = gameObject.AddComponent<AudioSource> ();
		} else {
			audio = GetComponent <AudioSource> ();
		}

	}

	void OnCollisionEnter(Collision col){

		if (justHit)
			return;

		NPC = col.transform.root.GetComponent<NPCEmotions> ();

		strength = Mathf.Abs(Mathf.Max (rb.velocity.x, rb.velocity.y));

		audio.volume = (strength / 50);

		//Debug.Log ("Just hit: " + justHit);

		//hits a player
		if (NPC != null) {

			Debug.Log ("Hit " + NPC.name + " with " + name); 

			justHit = true;
			StartCoroutine (JustHit ());

			audio.clip = soundEffects [0];
			audio.Play ();

			AffectNPC (NPC);

		} else if (col.transform.tag == "Surface") { //hit surface

			StopPassThrough (col.transform.position);

			justHit = true;
			StartCoroutine (JustHit ());

			audio.clip = soundEffects [1];
			audio.Play ();

		} else { //hit something else

		//	Debug.Log ("Hit " + col.transform.name + " with " + name);

		}

	}

	void OnCollisionStay(Collision col){

		if (col.transform.CompareTag("Surface"))
			StopPassThrough (col.transform.position);

	}

	void OnCollisionExit(Collision col){
		hitting = false;
	}

	public virtual void Cast(){

		//something that would be used to detect things like pointing a flash light,
		//threatening with a knife, or offering a donut using raycasts (ray tracing)
		//even things like feathers can use short range ray casts

	}

	//meant to be overwritten by specific item and still lacks 
	//levels of abstraction. Might need to be redesigned to accomodate
	//items that Cast() can't
	public virtual void AffectNPC(NPCEmotions NPC){

		NPC.AddPain (strength * painMod);
		NPC.AddFear (strength * fearMod);
		NPC.AddJoy (strength * joyMod);
		NPC.AddLaughter (strength * laughterMod);

	}

	public virtual void SpecialPickup(){

		//if the item is supposed to do something special when grabbed

	}

	public virtual void SpecialDrop(){
		
		//if the item is supposed to do something special when grabbed
		
	}

	IEnumerator JustHit(){

		while (true) {

//			Debug.Log ("waiting");

			yield return new WaitForSeconds (1f);

			justHit = false;
//			Debug.Log ("can hit again");

		}

	}

	private void StopPassThrough(Vector3 colPos){

		hitting = true;

		Vector3 heading = colPos - transform.position;

		float distance = heading.magnitude;
		hitDirection = heading / distance;;

	}

	private Vector3 ClampToCol(Vector3 newPos){

		if (hitDirection.x > 0) {
			newPos.x = Mathf.Clamp (newPos.x, float.MinValue, transform.position.x);
		} else if (hitDirection.x < 0) {
			newPos.x = Mathf.Clamp (newPos.x, transform.position.x, float.MaxValue);
		}
		if (hitDirection.y > 0) {
			newPos.y = Mathf.Clamp (newPos.y, float.MinValue, transform.position.y);
		} else if (hitDirection.y < 0) {
			newPos.y = Mathf.Clamp (newPos.y, transform.position.y, float.MaxValue);
		}
		if (hitDirection.z > 0) {
			newPos.z = Mathf.Clamp (newPos.z, float.MinValue, transform.position.z);
		} else if (hitDirection.z < 0) {
			newPos.z = Mathf.Clamp (newPos.z, transform.position.z, float.MaxValue);
		}

		return newPos;

	}

	public void OrientGrabbedObj(Quaternion handRot, Vector3 handPos){

		Vector3 offset;
		Transform snapToPoint = GetComponent<OVRGrabbable> ().snapToPoint;

		if (snapToPoint == null) {
			offset = Vector3.zero;
		} else {
			offset = snapToPoint.position - snapToPoint.parent.position;
		}

		Vector3 newPos = handPos - offset;

		if (hitting) {
			newPos = ClampToCol (newPos);
		}

		rb.MovePosition (newPos);
		rb.MoveRotation (handRot);

	}

}
