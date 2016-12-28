using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Globalization;


public class GameManager : MonoBehaviour {

	private string GetUpdateGameURL = "https://sleepy-temple-82721.herokuapp.com/api_v1/game/";
	private string UpdateGameURL;
	private string CheckForProjectilesURL;

	public Transform goodStartingPos;
	public Transform badStartingPos;

	public GameObject playerPrefab;
	public GameObject opponentPrefab;
	public GameObject projectilePrefab;
	
	private GameObject playerInstance;
	private GameObject opponentInstance;
	private GameObject playerShip;
	private GameObject opponentShip;
	public GameObject goodShip;
	public GameObject badShip;

	private DataStorer dataHog;
	private bool otherPlayerLoaded;
	private bool ranOnce;

	private bool throttleNetwork;
		/*
			public string playerName;
			public string spawnPlayerAs;
			public int playerID;
			public int opponentID;
			public int gameID;
			public bool firstInGame;
		*/


	void Awake () {

		throttleNetwork = false;
		ranOnce = false;

		dataHog = GameObject.Find ("DataHogCube").GetComponent<DataStorer>();

		if (dataHog.spawnPlayerAs == "good") {
			Debug.Log("swawning as good guy");
			playerInstance = (GameObject) GameObject.Instantiate (playerPrefab, goodStartingPos.position, Quaternion.identity);	
			opponentInstance = (GameObject) GameObject.Instantiate(opponentPrefab, badStartingPos.position, Quaternion.identity);

			playerShip = goodShip;
			playerShip.GetComponentInChildren<ShipMover>().playerShip = goodShip;
			playerShip.GetComponentInChildren<ShipMover>().enabled = true;
			opponentShip = badShip;
		} else {
			Debug.Log("swawning as bad guy");
			playerInstance = (GameObject) GameObject.Instantiate (playerPrefab, badStartingPos.position, Quaternion.identity);
			opponentInstance = (GameObject) GameObject.Instantiate(opponentPrefab, goodStartingPos.position, Quaternion.identity);
			playerShip = badShip;
			playerShip.GetComponentInChildren<ShipMover>().playerShip = badShip;
			playerShip.GetComponentInChildren<ShipMover>().enabled = true;
			opponentShip = goodShip;
		}

		GetUpdateGameURL += dataHog.gameID + "/";
		UpdateGameURL = GetUpdateGameURL + "update/";
		CheckForProjectilesURL = (GetUpdateGameURL + "new_projectiles/" + dataHog.playerID + "/"); 

		for (int i = 0; i < GameObject.FindGameObjectsWithTag("Cannon").Length; i++) {
			GameObject.FindGameObjectsWithTag("Cannon")[i].GetComponent<FireCannon>().createProjectileURL = (GetUpdateGameURL + "projectile/" + dataHog.playerID + "/");
		}


	}
	
	// Update is called once per frame
	void Update () {
		//check if there are 2 players?
		if (dataHog.firstInGame) {

			StartCoroutine (checkIfGameFullYet ());
			lockMovement ();
		} else if (!ranOnce) {
			unlockMovement ();
//			shipText += dataHog.
			Debug.Log ("Running game with 2 players");
			ranOnce = true;
		} else {


			if (!throttleNetwork) {
				StartCoroutine(checkForNewProjectiles());
				StartCoroutine(mainGamePositionUpdater());

			}

		}
	}

	void lockMovement() {
		playerInstance.GetComponent<CharacterController> ().enabled = false;
		playerInstance.GetComponent<PlayerMovement> ().enabled = false;
		playerShip.GetComponentsInChildren<ShipMover> () [0].enabled = false;
		
	}

	void unlockMovement(){
		playerInstance.GetComponent<CharacterController> ().enabled = true;
		playerInstance.GetComponent<PlayerMovement> ().enabled = true;
		playerShip.GetComponentsInChildren<ShipMover> () [0].enabled = true;

	}

	IEnumerator checkForNewProjectiles() {
		WWW getRequest = new WWW(CheckForProjectilesURL);
		yield return getRequest;
		if (!string.IsNullOrEmpty (getRequest.error)) {
			Debug.Log (getRequest.error);
		} else {
			var structuredPostData = JSON.Parse(getRequest.text); //.Replace("null", "\"null\"")

			for (int i = 0; i < Int32.Parse(structuredPostData["length"].Value); i++) {
				Debug.Log("Shooting new Cannon");

				Vector3 spawnPos = new Vector3(
					float.Parse(structuredPostData["results"][i]["start_location_x"].Value, CultureInfo.InvariantCulture),
				    float.Parse(structuredPostData["results"][i]["start_location_y"].Value, CultureInfo.InvariantCulture),
				    float.Parse(structuredPostData["results"][i]["start_location_z"].Value, CultureInfo.InvariantCulture));
				Debug.Log(spawnPos);

				Vector3 forceToAdd = new Vector3(
					float.Parse(structuredPostData["results"][i]["firing_force_x"].Value, CultureInfo.InvariantCulture),
					float.Parse(structuredPostData["results"][i]["firing_force_y"].Value, CultureInfo.InvariantCulture),
					float.Parse(structuredPostData["results"][i]["firing_force_z"].Value, CultureInfo.InvariantCulture));
				Debug.Log(forceToAdd);
				GameObject newProjectile = (GameObject) GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
				newProjectile.rigidbody.AddForce(forceToAdd);
				Debug.Log("______");
			}
			//need to do with 2 clients so i can see how the data is rendered


		}
		
	
	}

	IEnumerator mainGamePositionUpdater() {
		throttleNetwork = true;
	
		WWWForm form = new WWWForm ();
		//send player coordinates to server
		//player
		if (dataHog.spawnPlayerAs == "good") {
			//player
			form.AddField ("good_guy_position_x", playerInstance.transform.position.x.ToString());
			form.AddField ("good_guy_position_y", playerInstance.transform.position.y.ToString());
			form.AddField ("good_guy_position_z", playerInstance.transform.position.z.ToString());
		
			form.AddField ("good_guy_rotation_x", playerInstance.transform.localRotation.x.ToString());
			form.AddField ("good_guy_rotation_y", playerInstance.transform.localRotation.y.ToString());
			form.AddField ("good_guy_rotation_z", playerInstance.transform.localRotation.z.ToString());
		
			//ship
			form.AddField ("good_ship_position_x", playerShip.transform.position.x.ToString());
			form.AddField ("good_ship_position_y", playerShip.transform.position.y.ToString());
			form.AddField ("good_ship_position_z", playerShip.transform.position.z.ToString());
		
			form.AddField ("good_ship_rotation_x", playerShip.transform.localRotation.x.ToString());
			form.AddField ("good_ship_rotation_y", playerShip.transform.localRotation.y.ToString());
			form.AddField ("good_ship_rotation_z", playerShip.transform.localRotation.z.ToString());
		} else {
			//ship
			form.AddField ("bad_guy_position_x", playerInstance.transform.position.x.ToString());
			form.AddField ("bad_guy_position_y", playerInstance.transform.position.y.ToString());
			form.AddField ("bad_guy_position_z", playerInstance.transform.position.z.ToString());
			
			form.AddField ("bad_guy_rotation_x", playerInstance.transform.localRotation.x.ToString());
			form.AddField ("bad_guy_rotation_y", playerInstance.transform.localRotation.y.ToString());
			form.AddField ("bad_guy_rotation_z", playerInstance.transform.localRotation.z.ToString());
			
			//ship
			form.AddField ("bad_ship_position_x", playerShip.transform.position.x.ToString());
			form.AddField ("bad_ship_position_y", playerShip.transform.position.y.ToString());
			form.AddField ("bad_ship_position_z", playerShip.transform.position.z.ToString());
			
			form.AddField ("bad_ship_rotation_x", playerShip.transform.localRotation.x.ToString());
			form.AddField ("bad_ship_rotation_y", playerShip.transform.localRotation.y.ToString());
			form.AddField ("bad_ship_rotation_z", playerShip.transform.localRotation.z.ToString());
		
		}

		//Dictionary<string, string> headers = new Dictionary<string, string>();
		//headers.Add("X-HTTP-Method-Override", "PUT");

		WWW postRequest = new WWW(UpdateGameURL, form);
		yield return postRequest;
		if (!string.IsNullOrEmpty (postRequest.error)) {
			Debug.Log (postRequest.error);
		} else {
			//update oponent position via the response
			var structuredPostData = JSON.Parse(postRequest.text.Replace("null", "\"null\""));

			//main positional reset
			if (dataHog.spawnPlayerAs == "good") {

				float tempX = float.Parse(structuredPostData["bad_guy_position_x"].Value, CultureInfo.InvariantCulture);
				float tempY = float.Parse(structuredPostData["bad_guy_position_y"].Value, CultureInfo.InvariantCulture);
				float tempZ = float.Parse(structuredPostData["bad_guy_position_z"].Value, CultureInfo.InvariantCulture);
				opponentInstance.transform.position = new Vector3(tempX,tempY,tempZ);
				
				float tempx = float.Parse(structuredPostData["bad_guy_rotation_x"].Value, CultureInfo.InvariantCulture);
				float tempy = float.Parse(structuredPostData["bad_guy_rotation_y"].Value, CultureInfo.InvariantCulture);
				float tempz = float.Parse(structuredPostData["bad_guy_rotation_z"].Value, CultureInfo.InvariantCulture);
				opponentInstance.transform.localRotation = new Quaternion(tempx, tempy, tempz, 0f);

				float shipTempX = float.Parse(structuredPostData["bad_ship_position_x"].Value, CultureInfo.InvariantCulture);
				float shipTempY = float.Parse(structuredPostData["bad_ship_position_y"].Value, CultureInfo.InvariantCulture);
				float shipTempZ = float.Parse(structuredPostData["bad_ship_position_z"].Value, CultureInfo.InvariantCulture);
				opponentShip.transform.position = new Vector3(shipTempX, shipTempY, shipTempZ);

				float shiptempx = float.Parse(structuredPostData["bad_ship_rotation_x"].Value, CultureInfo.InvariantCulture);
				float shiptempy = float.Parse(structuredPostData["bad_ship_rotation_y"].Value, CultureInfo.InvariantCulture);
				float shiptempz = float.Parse(structuredPostData["bad_ship_rotation_z"].Value, CultureInfo.InvariantCulture);
				opponentInstance.transform.localRotation = new Quaternion(shiptempx, shiptempy, shiptempz, 0f);

			} else {
				float tempX = float.Parse(structuredPostData["good_guy_position_x"].Value, CultureInfo.InvariantCulture);
				float tempY = float.Parse(structuredPostData["good_guy_position_y"].Value, CultureInfo.InvariantCulture);
				float tempZ = float.Parse(structuredPostData["good_guy_position_z"].Value, CultureInfo.InvariantCulture);
				opponentInstance.transform.position = new Vector3(tempX,tempY,tempZ);
				
				float tempx = float.Parse(structuredPostData["good_guy_rotation_x"].Value, CultureInfo.InvariantCulture);
				float tempy = float.Parse(structuredPostData["good_guy_rotation_y"].Value, CultureInfo.InvariantCulture);
				float tempz = float.Parse(structuredPostData["good_guy_rotation_z"].Value, CultureInfo.InvariantCulture);
				opponentInstance.transform.localRotation = new Quaternion(tempx, tempy, tempz, 0f);
				
				float shipTempX = float.Parse(structuredPostData["good_ship_position_x"].Value, CultureInfo.InvariantCulture);
				float shipTempY = float.Parse(structuredPostData["good_ship_position_y"].Value, CultureInfo.InvariantCulture);
				float shipTempZ = float.Parse(structuredPostData["good_ship_position_z"].Value, CultureInfo.InvariantCulture);
				opponentShip.transform.position = new Vector3(shipTempX, shipTempY, shipTempZ);
				
				float shiptempx = float.Parse(structuredPostData["good_ship_rotation_x"].Value, CultureInfo.InvariantCulture);
				float shiptempy = float.Parse(structuredPostData["good_ship_rotation_y"].Value, CultureInfo.InvariantCulture);
				float shiptempz = float.Parse(structuredPostData["good_ship_rotation_z"].Value, CultureInfo.InvariantCulture);
				opponentInstance.transform.localRotation = new Quaternion(shiptempx, shiptempy, shiptempz, 0f);
			}
			throttleNetwork = false;
				
			
			
		}

	}

	IEnumerator checkIfGameFullYet() {
		//only runs if the player is the first one in the game
		WWW getRequest = new WWW(GetUpdateGameURL);
		yield return getRequest;
		if (!string.IsNullOrEmpty (getRequest.error)) {
			Debug.Log (getRequest.error);
		} else {
			// found game
			var structuredPostData = JSON.Parse(getRequest.text.Replace("null", "\"null\"")); // have to otherswise exception raised
			if (dataHog.spawnPlayerAs == "good") {
				//look at bad guy data, if null pass
				if (!structuredPostData["bad_guy"].Value.Contains("null")) {
					//Debug.Log(structuredPostData);

					dataHog.opponentID = Int32.Parse(structuredPostData["bad_guy"].Value);
					dataHog.firstInGame = false;
				}


			} else {
				//spawn player as bad
				if (!structuredPostData["good_guy"].Value.Contains("null")) {

					dataHog.opponentID = Int32.Parse(structuredPostData["good_guy"].Value);
					dataHog.firstInGame = false;
				}

			}


		}


	}
}
