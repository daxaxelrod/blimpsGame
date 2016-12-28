using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;


public class FireCannon : MonoBehaviour {

	public string createProjectileURL;

	public GameObject projectile;
	public GameObject cannonballLoader;
	public GameObject cannonCap;
	private int cannonPower = 20;


	private Vector3 fireDirection;

	private float cannonMultipler;
	private bool hitTrigger = false;
	//cannon Sounds
	public GameObject cannonSoundManagerOBJ;
	private AudioClip[] cannonSounds;

	IEnumerator LogProjectileFire(Vector3 creationLocation, Vector3 firingForce) {

		WWWForm form = new WWWForm ();
		//send player coordinates to server
		//player

		//cannonball spot
		form.AddField ("start_location_x", creationLocation.x.ToString());
		form.AddField ("start_location_y", creationLocation.y.ToString());
		form.AddField ("start_location_z", creationLocation.z.ToString());
		//firing force
		form.AddField ("firing_force_x", firingForce.x.ToString());
		form.AddField ("firing_force_y", firingForce.y.ToString());
		form.AddField ("firing_force_z", firingForce.z.ToString());

		
		WWW postRequest = new WWW(createProjectileURL, form);
		yield return postRequest;
		if (!string.IsNullOrEmpty (postRequest.error)) {
			Debug.Log (postRequest.error);
		} else {
			//update oponent position via the response
			var structuredPostData = JSON.Parse(postRequest.text.Replace("null", "\"null\""));
			Debug.Log(structuredPostData);
			Debug.Log("CannonBall Created");

			//main positional reset

			
			
		}

	}

	public void fireCannon(Vector3 heading, int power) {
		GameObject newProjectile = (GameObject) GameObject.Instantiate(projectile, 
		                                                               cannonballLoader.transform.position,
		                                                               Quaternion.identity);

		Vector3 firingForce = heading * (power * 10);
		newProjectile.rigidbody.AddForce (firingForce);
		Debug.Log ("Fired the projectile");
		StartCoroutine (LogProjectileFire(cannonballLoader.transform.position,
		                                  firingForce));
	}

	void OnTriggerEnter(Collider player) {
		Debug.Log("Entered the cannon");
		if(player.gameObject.name.Contains("Player")) {
			hitTrigger = true;
		}
	}

	void OnTriggerExit(Collider player) {
		if (player.gameObject.name.Contains ("Player")) {
			hitTrigger = false;
		}
	}

	// Use this for initialization
	void Start () {
		cannonMultipler = 1;

		cannonSounds = cannonSoundManagerOBJ.GetComponent<CannonSoundManager>().cannonSounds;

	}
	
	// Update is called once per frame
	void Update () {

		fireDirection = cannonCap.transform.position - cannonballLoader.transform.position;

		if(hitTrigger && Input.GetKey(KeyCode.Space)) {
			cannonMultipler += 0.25f;
		}

		if (hitTrigger && Input.GetKeyUp(KeyCode.Space)) {
			System.Random rnd = new System.Random();
			int r = rnd.Next(cannonSounds.Length);
			AudioSource cannon_audio = gameObject.GetComponent<AudioSource>();
			cannon_audio.clip = cannonSounds[r];
			cannon_audio.Play();

			Debug.Log("Firing cannon");
			int cannonWithMultipler = (int)(cannonPower * cannonMultipler);
			fireCannon(fireDirection, cannonWithMultipler);
			cannonMultipler = 1;

		}
	
	}
}
