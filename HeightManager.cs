using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;


public class HeightManager : MonoBehaviour {


	public AudioClip youLose;
	private bool playedOnce;
	// Use this for initialization
	void Start () {
		playedOnce = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y <= 2.0f && !playedOnce) {
			AudioSource source = gameObject.GetComponent<AudioSource>();
			source.clip = youLose;
			source.playOnAwake = false;
			source.loop = false;
			gameObject.GetComponent<AudioSource>().Play();
			playedOnce = true;

			StartCoroutine(WaitAndRedirect());

		}


	
	}

	IEnumerator WaitAndRedirect() {

		yield return new WaitForSeconds(5);

		//SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		//Application.LoadLevel("MainMenu");
	}
}
