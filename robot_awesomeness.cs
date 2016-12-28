using UnityEngine;
using System.Collections;

public class robot_awesomeness : MonoBehaviour {

	public GameObject objthingy;
	Vector3 letsGoUp;


	// Use this for initialization
	void Start () {
		objthingy = gameObject;
		letsGoUp = new Vector3(0f,20f,0f);

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.C)) {
			// how to debug
////			Debug.Log(objthingy.GetComponent<>());

			/// Moving via transforms
			Vector3 letsGoUp = new Vector3(0f,1f,0f);
//			gameObject.transform.Translate(letsGoUp);

			//Physics
			Rigidbody rb = gameObject.GetComponent<Rigidbody>();
			rb.AddForce(letsGoUp);

		}

		if (Input.GetKey (KeyCode.Z)) {
			
			Rigidbody rb2 = GameObject.Find("TempCube").GetComponent<Rigidbody>();
			rb2.AddForce(letsGoUp);
			
			
		}	
	
	}
}
