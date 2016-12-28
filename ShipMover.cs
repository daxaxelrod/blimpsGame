using UnityEngine;
using System.Collections;

public class ShipMover : MonoBehaviour {

	public float speed;
	public readonly float MAX_SPEED = 10;

	public GameObject steeringWheelPad;
	public GameObject playerShip;
	//public GameObject trackingBehind;
	//public GameObject trackingForward;

	private GameObject player;

	private bool hitTrigger;
	private bool hitSteeringWheel;

	// Use this for initialization
	void OnTriggerEnter(Collider player) {

		if (player.gameObject.name == "Player") {

			if (hitSteeringWheel == false) {
				gameObject.GetComponent<AudioSource> ().Play();
			}
			hitSteeringWheel = true;
			
			Debug.Log("Entered the Steering wheel");
			steeringWheelPad.transform.renderer.material.color = Color.green;
			
			hitTrigger = true;
		
		}

	}
	
	void OnTriggerExit(Collider player) {
		steeringWheelPad.transform.renderer.material.color = Color.white;
		hitTrigger = false;
	}

	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		speed = 5;
		//hitSteeringWheel = false;
		hitSteeringWheel = true;
		hitTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (hitTrigger) {

			player.transform.parent = playerShip.transform;

			if (Input.GetKey(KeyCode.Period)) {
				playerShip.transform.Rotate(0f, 1.0f, 0f);
			}
			if (Input.GetKey(KeyCode.M)) {
				playerShip.transform.Rotate(0f, -1.0f, 0f);

			}
			if (Input.GetKey(KeyCode.K)) {
				//FUCK IT ITS JUST GOING FORWARD
				//Vector3 moveDir = playerShip.transform.forward; //trackingForward.transform.position - trackingBehind.transform.position;
				//player.transform.Translate(Vector3.forward *  Time.deltaTime * speed);

				playerShip.transform.Translate(Vector3.forward * Time.deltaTime * speed);

			}
			
			if (Input.GetKey(KeyCode.Comma)) {
				//Vector3 moveDir = trackingForward.transform.position - trackingBehind.transform.position;
				//Vector3 moveDir = playerShip.transform.forward;
				playerShip.transform.Translate(-Vector3.forward * Time.deltaTime * speed);

			}

			player.transform.parent = null;

		}
	}

}
