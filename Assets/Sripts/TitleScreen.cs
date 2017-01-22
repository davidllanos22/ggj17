﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {


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

		if ((Input.GetAxis ("Swim1") > 0.2f && Input.GetAxis ("Swim2") > 0.2f && Input.GetAxis ("Swim3") > 0.2f && Input.GetAxis ("Swim4") > 0.2f) || Input.GetKeyDown (KeyCode.Space)) {
		
		}
	}
}