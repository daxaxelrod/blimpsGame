using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using SimpleJSON;


//using UnityEngine.SceneManagement;


public class LoadPlayground : MonoBehaviour {


	private string playerCreationURL= "https://sleepy-temple-82721.herokuapp.com/api_v1/players/";
	//private string gameListCreateURL= "https://sleepy-temple-82721.herokuapp.com/api_v1/games/";
	private string playerSearchingForGameURL = "https://sleepy-temple-82721.herokuapp.com/api_v1/searching/";

	public GameObject dataHogCube;
	public InputField nameInputField;
	public Text updaterText;

	public string playerName;
	public int gameNumber;

	
	// Use this for initialization
	void Start () {
	}

	IEnumerator FindOpenGame(int playerId) {
		// custom route
		// posts with searching player id
		// returns a game pk and which side the player is on
		WWWForm form = new WWWForm ();
		form.AddField("player_id", playerId);
		WWW postRequest = new WWW(playerSearchingForGameURL, form);
		yield return postRequest;
		if (!string.IsNullOrEmpty (postRequest.error)) {
			Debug.Log (postRequest.error);
		} else {
			// found game
			// store game pk and which side the player is on
			// load new scene with the player on the correct side

			Debug.Log("Found Game for the player!");

			var structuredPostData = JSON.Parse(postRequest.text.Replace("null", "\"null\"")); // not handled in simpleJSON

			DataStorer hogDataStore = dataHogCube.GetComponent<DataStorer>();
			// int goodId = structuredPostData["good_guy"].Value;
			// int badId = structuredPostData["bad_guy"].Value;

			// 4 possible states // eg "good_guy":"4", "bad_guy":"null" //
			string wrappedIdString = playerId.ToString();
			if (structuredPostData["good_guy"].Value == wrappedIdString && structuredPostData["bad_guy"].Value.Replace("\"", "") == "null") {
				//player is good guy and is first in game
				hogDataStore.spawnPlayerAs = "good";
				hogDataStore.opponentID = -1;
				hogDataStore.firstInGame = true;

			} else if (structuredPostData["bad_guy"].Value == wrappedIdString && structuredPostData["good_guy"].Value.Replace("\"", "") == "null") {
				//player is bad guy and first in game
				hogDataStore.spawnPlayerAs = "bad";
				hogDataStore.opponentID = -1;
				hogDataStore.firstInGame = true;
			} else if (structuredPostData["good_guy"].Value == wrappedIdString) {
				//opponent is bad and last in game
				hogDataStore.spawnPlayerAs = "good";
				hogDataStore.opponentID = Int32.Parse(structuredPostData["bad_guy"].Value.Replace("\"", "")); 
//				hogDataStore.opponentName = structuredPostData["bad_guy_name"].Value.Replace("\"", "");
				hogDataStore.firstInGame = false;

			} else if (structuredPostData["bad_guy"].Value == wrappedIdString) {
				//opponent is good and last in game
				hogDataStore.spawnPlayerAs = "bad";
				hogDataStore.opponentID = Int32.Parse(structuredPostData["good_guy"].Value.Replace("\"", ""));
//				hogDataStore.opponentName = structuredPostData["good_guy_name"].Value.Replace("\"", "");
				hogDataStore.firstInGame = false;
			} else {
				
				Debug.Log(structuredPostData["good_guy"].Value);
				Debug.Log(structuredPostData["bad_guy"].Value);

				Debug.Log(structuredPostData["good_guy"].Value.GetType());
				Debug.Log(structuredPostData["bad_guy"].Value.GetType());
				Debug.Log(playerId);

				Debug.Log(structuredPostData["good_guy"].Value == wrappedIdString);
				Debug.Log(structuredPostData["bad_guy"].Value == wrappedIdString);

				Debug.LogError("SOMETHING BROKE");

			}

			hogDataStore.playerID = playerId;
			hogDataStore.gameID = Int32.Parse(structuredPostData["id"]);
			hogDataStore.playerName = playerName;


			DontDestroyOnLoad(dataHogCube);

			Application.LoadLevel("ShipsMult");




		}


	
	}
	
	IEnumerator CreateOrGetPlayer() {

		// New web form
		WWWForm form = new WWWForm();
		playerName = nameInputField.text;
		form.AddField("name", playerName);
		
		// Upload to api
		WWW postRequest = new WWW(playerCreationURL, form);
		yield return postRequest;
		if (!string.IsNullOrEmpty(postRequest.error)) {
			Debug.Log(postRequest.error);
		}
		else {
			Debug.Log("Finished Creating Player");
			//JsonValue structuredPostData = JsonValue.Parse(postRequest.text);
			var structuredPostData = JSON.Parse(postRequest.text);
			updaterText.text = "Welcome " + playerName + "! Finding an open game for you! \n Just a Second...";
			StartCoroutine(FindOpenGame(Int32.Parse(structuredPostData["id"])));



		}
	}


	void Update () {

		if (Input.GetKeyDown(KeyCode.Return)) {
			StartCoroutine(CreateOrGetPlayer());

		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			//SceneManager.LoadScene("Ships", LoadSceneMode.Single);
			Application.LoadLevel("Ships");

		}
	
	}
}
