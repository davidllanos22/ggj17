﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

	public DataBetweenScenes data;

	int readyPlayers;
	float readyTime = 0;
	int readyCount = 0;
	public UnityEngine.UI.Image[] readies;
	public Sprite offSprite;
	public Sprite[] onSprites;

	public float speed = 1f;
	public float displacement = 10f;
	public float height = 1f;
	public float period = 2f;

	float time = 0f;
	float baseY = 0f;

	// Use this for initialization
	void Start () {
		time = 0f;
		baseY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		Vector3 pos;
		pos.z = transform.position.z;

		pos.x = (time * speed) % (displacement * 4f);
		if (pos.x > displacement * 2f) {
			pos.x = 4f * displacement - pos.x;
		}

		pos.x -= displacement;

		pos.y = baseY + Mathf.Sin (time * Mathf.PI * 2f / period) * height;

		transform.position = pos;

		#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
		string axis = "MSwim";
		#else
		string axis = "Swim";
		#endif


		readyPlayers = 0;
		for (int i = 0; i < 4; ++i) {
			if (Input.GetAxis (axis + (i + 1).ToString ()) > 0.2f) {
				++readyPlayers;
				readies [i].sprite = onSprites[i];
				data.playerPlaying [i] = true;
			} else {
				readies [i].sprite = offSprite;
				data.playerPlaying [i] = false;
			}
		}
		if (readyPlayers != readyCount) {
			readyTime = 0f;
		}

		if (readyPlayers >= 2) {
			readyTime += Time.deltaTime;
		} else {
			readyTime = 0;
		}

		readyCount = readyPlayers;
			

		if (readyTime >= 3f || Input.GetKeyDown (KeyCode.Space)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene (1);
			this.enabled = false;
		}


	}
}
