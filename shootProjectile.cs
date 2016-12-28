using UnityEngine;
using System.Collections;

public class shootProjectile : MonoBehaviour {

	public GameObject projectile;

	public float shootForce = 4000F;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//if (Input.GetKeyDown (KeyCode.F)) {
		//	GameObject newBullet = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
		//	newBullet.rigidbody.AddForce(newBullet.transform.forward * shootForce);
		
		//}
	
	}
}
