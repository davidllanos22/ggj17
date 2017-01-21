using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

	public Vector3 deltaPos;

	Vector3 basePos;

	void Start() {
		basePos = transform.position;
	}

	// Use this for initialization
	void Update () {
		transform.position = basePos + deltaPos;

		transform.LookAt (basePos);
	}
}
