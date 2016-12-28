using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;

	private Vector3 moveDirection = Vector3.zero;
	public Camera camera;


	// Use this for initialization
	void Start () {
		camera = Camera.main;
	
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = gameObject.GetComponent<CharacterController> ();
		if (controller.isGrounded) {
			/*
			Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			if (dir != Vector3.zero) {
				transform.forward += Vector3.Normalize(dir) * speed;
			} */

			moveDirection = new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));

			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			/*if (Input.GetKeyDown(KeyCode.Space)){
				moveDirection.y = jumpSpeed;
			}*/
		}

		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);

	}
}
