﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class RaceManager : MonoBehaviour {

	public GameObject[] gates;
	public int numberOfLaps = 3;
	public GameObject timeCanvas;
	public GameObject scoreCanvas;
	public Text lapText;
	public Text timeText;
	public GameObject drone;

	private bool raceStarted = false;
	private int currentGate = 0;
	private int currentLap = 0;
	private double startTime;
	private double endTime;

	void Start () {
		// Register gates;
		for (int i=0; i<gates.Length; i++) {
			gates[i].GetComponent<RaceGate>().SetRaceManager(this);
		}
		// Init race
		StopRace();
		lapText.text = "Waiting...";
		SetTextColor (Color.red);
	}

	public void StopRace() {
		Debug.Log ("Stop Race");
		for (int i=0; i<gates.Length; i++) {
			gates [i].GetComponent<RaceGate>().DisableGate ();
		}
		currentGate = 0;
		currentLap = 0;
	}

	public void StartRace() {
		Debug.Log ("Start Race");
		if (gates.Length > 0) {
			gates [(currentGate+1)%gates.Length].GetComponent<RaceGate> ().setNextGate ();
			gates [currentGate].GetComponent<RaceGate> ().EnableGate ();
			raceStarted = true;
			startTime = PhotonNetwork.time;
			lapText.text = "Lap: 1/" + numberOfLaps;
			ResetMyScore();
		}
	}
	
	public void NextGate () {
		Debug.Log ("Next Gate");
		currentGate = (currentGate + 1) % gates.Length;
		gates [(currentGate+1)%gates.Length].GetComponent<RaceGate> ().setNextGate ();
		gates[currentGate].GetComponent<RaceGate>().EnableGate();
		if (currentGate == 1) {
			NextLap ();
		}
	}

	void NextLap() {
		// Save score
		if (currentLap > 0) {
			PhotonNetwork.player.SetLap (currentLap);
			PhotonNetwork.player.SetTime ((float)(PhotonNetwork.time - startTime));
		}

		// End of race
		if (currentLap == numberOfLaps) {
			// Change UI
			SetTextColor(Color.green);
			lapText.text = "Finished!";
			// End race
			raceStarted = false;
			gates [currentGate].GetComponent<RaceGate>().DisableGate ();
		} else {
			// New lap
			currentLap++;
			lapText.text = "Lap: " + currentLap + "/" + numberOfLaps;
		}
	}

	public void SetLapText(string s) {
		lapText.text = s;
	}

	public void SetTimeText(string s) {
		timeText.text = s;
	}

	public void SetTextColor(Color c) {
		lapText.color = c;
		timeText.color = c;
	}

	public void ResetMyScore() {
		PhotonNetwork.player.SetLap (0);
		PhotonNetwork.player.SetTime (0);
	}

	void Update() {
		if (raceStarted) {
			double duration = PhotonNetwork.time - startTime;
			TimeSpan timeSpan = TimeSpan.FromSeconds(duration);
			SetTimeText(String.Format("{0:D2}:{1:D2}:{2:D3}",timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
		}

		if (Input.GetButtonDown("Score") && !scoreCanvas.activeSelf) {
			scoreCanvas.SetActive (true);
		}
		if (Input.GetButtonUp("Score") && scoreCanvas.activeSelf) {
			scoreCanvas.SetActive(false);
		}
	}
}
