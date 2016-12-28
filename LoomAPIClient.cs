using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Net;
using System.Text;
using System;

public class LoomAPIClient : MonoBehaviour
{
	WWW callToPythonServer;
	HttpWebResponse response;
	 




	public HttpWebRequest PostFile(string url, string file)
	{
		string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);


//		httpWebRequest.Headers.Add("file_type", "mp3");
		httpWebRequest.Headers.Add("language", "es-419"); // es-419 en

		httpWebRequest.Headers.Add("language2", "wav");

		httpWebRequest.Headers.Add("language3", "44100");
		Debug.Log (httpWebRequest.Headers.ToString ());

		httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;

		httpWebRequest.Method = "POST";
//		httpWebRequest.KeepAlive = true;
		Stream memStream = new System.IO.MemoryStream();
		byte[] boundarybytes =System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary     +"\r\n");
		//string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition:  form-data; name=\"{0}\";\r\n\r\n{1}";
		string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";
		memStream.Write(boundarybytes, 0, boundarybytes.Length);


			string header = string.Format(headerTemplate, "file" + 0, file);
			//string header = string.Format(headerTemplate, "uplTheFile", files[i]);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
			memStream.Write(headerbytes, 0, headerbytes.Length);
			FileStream fileStream = new FileStream(file, FileMode.Open,
			                                       FileAccess.Read);
			byte[] buffer = new byte[1024];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				memStream.Write(buffer, 0, bytesRead);
			}
			memStream.Write(boundarybytes, 0, boundarybytes.Length);
			fileStream.Close();

		httpWebRequest.ContentLength = memStream.Length;
		Stream requestStream = httpWebRequest.GetRequestStream();
		memStream.Position = 0;
		byte[] tempBuffer = new byte[memStream.Length];
		memStream.Read(tempBuffer, 0, tempBuffer.Length);
		memStream.Close();
		requestStream.Write(tempBuffer, 0, tempBuffer.Length);
		requestStream.Close();



		return httpWebRequest;
	}




											// must create temp file first and reference that
	public HttpWebResponse GetSpeechToText(string pathToSpeechFile) {
		//take in path and serialized that and posts it


//		//have to turn data into byte array
//		BinaryFormatter binFormatter = new BinaryFormatter();
//		MemoryStream mStream = new MemoryStream ();
//		binFormatter.Serialize (mStream, payloadData); 
//		byte[] payloadByteArray = new byte[mStream.Length];
		
		string speechURL = "http://ec2-52-201-247-146.compute-1.amazonaws.com:8000/speech/v1/analyze/";
										// payloadByteArray
//		callToPythonServer = new WWW(speechURL, soundBytes, headerDictionary);

		


		try {
			HttpWebRequest callToPythonServer = PostFile(speechURL, pathToSpeechFile);

			HttpWebResponse response = (HttpWebResponse)callToPythonServer.GetResponse ();

			Debug.Log(response.StatusDescription);
			Stream dataStreamOfResponses = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStreamOfResponses);
			string responseFromServer = reader.ReadToEnd();
			Debug.Log(responseFromServer);
			Debug.Log("End response from server");
			
			return response;


		} catch (WebException wex) {
			Debug.Log(wex);
			Debug.Log(new StreamReader(wex.Response.GetResponseStream()).ReadToEnd());
//			print(pageContent);

			WebRequest callToPythonServer = WebRequest.Create(speechURL);
			return (HttpWebResponse)callToPythonServer.GetResponse();

		}


	


	}




	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log ("Listening for STT call");

		if (Input.GetKeyUp(KeyCode.LeftShift)) {
			Debug.Log("sending google call");
											// pull from 
			response = GetSpeechToText("Assets/PlayerConver1.wav");
		}



		if (response != null) {

			Debug.Log("Upload progress {}");
			Debug.Log(response.StatusDescription);
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			Debug.Log(responseFromServer);
			Debug.Log("End response from server");

//
//			reader.Close();
//			dataStream.Close();
//			response.Close();
//
//
			response = null;

		}}

}