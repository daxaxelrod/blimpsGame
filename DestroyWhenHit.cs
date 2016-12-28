using UnityEngine;
using System.Collections;

public class DestroyWhenHit : MonoBehaviour {

	private bool hasBeenHit;

	void OnCollisionEnter(Collision projectile) {

		if (!hasBeenHit) {
			hasBeenHit = true;
			gameObject.transform.renderer.material.color = Color.white;
		} else {
			Destroy(gameObject);
		}
	
	}

	// Use this for initialization
	void Start () {
		hasBeenHit = false;
	
	}

	void Update () {
	
	}
}
