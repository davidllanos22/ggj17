using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

	public Vector3 deltaPos;

	// Use this for initialization
	void Start () {
		Vector3 pos = transform.position;

		transform.position += deltaPos;

		transform.LookAt (pos);
	}
}
