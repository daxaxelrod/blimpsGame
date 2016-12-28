using UnityEngine;
using System.Collections;

public class detectCollision : MonoBehaviour {




	void OnTriggerEnter(Collider other) {
		Debug.Log (other.gameObject.name);
		gameObject.transform.Translate (new Vector3 (0f, 0.1f, 0f));
//		Destroy(other.gameObject);
	}




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
