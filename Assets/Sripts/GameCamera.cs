using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

	public Vector3 deltaPos;

	public float minCameraWidth = 5f;
	public float cameraMarginSides = 4f;

	Vector3 middlePos;
	Vector2 gameWidth;
	Vector2 delta;

	GameObject[] players;

	public float cameraAngle = 45f;

	float ratio;
	float wFOV;

	public void Init(GameObject[] players, Vector2 gameWidth) {

		this.players = players;
		this.gameWidth = gameWidth;
		ratio = (float)Screen.width / (float)Screen.height;
		wFOV = Camera.main.fieldOfView * ratio;
	}

	// Use this for initialization
	void Update () {

		Vector2 min = new Vector2 (Mathf.Infinity, Mathf.Infinity);
		Vector2 max = new Vector2 (Mathf.NegativeInfinity, Mathf.NegativeInfinity);

		middlePos = Vector3.zero;
		for (int i = 0; i < players.Length; ++i) {
			middlePos += players[i].transform.position;

			min.x = Mathf.Min (min.x, players [i].transform.position.x - cameraMarginSides);
			max.x = Mathf.Max (max.x, players [i].transform.position.x + cameraMarginSides);
			min.y = Mathf.Min (min.y, players [i].transform.position.z - cameraMarginSides);
			max.y = Mathf.Max (max.y, players [i].transform.position.z + cameraMarginSides);
		}

		min.y = Mathf.Max (0f, min.y);
		max.y = Mathf.Min (max.y, gameWidth.y + cameraMarginSides);

		delta = max - min;
		Vector3 middle = (max + min) / 2f;

		middlePos.x = middle.x;
		middlePos.y = 0;
		middlePos.z = middle.y;

		// Moure la camera

		float r = delta.x / delta.y;

		float hFov = Camera.main.fieldOfView / 2f;
		float beta = 180 - cameraAngle - hFov;

		float distance = 0f;
		if (r > 1) { // Wrong
			distance = (Mathf.Sin(beta * Mathf.Deg2Rad)/Mathf.Sin(hFov * Mathf.Deg2Rad)) * delta.y/2f;
		} else {
			distance = (delta.x / 2f) / Mathf.Tan (wFOV/2f * Mathf.Deg2Rad);
		}

		deltaPos.y = Mathf.Sin(cameraAngle * Mathf.Deg2Rad) * distance;
		deltaPos.z = -Mathf.Cos(cameraAngle * Mathf.Deg2Rad) * distance;


		// Mou camera

		transform.position = middle + deltaPos;

		transform.rotation = Quaternion.AngleAxis (-cameraAngle, Vector3.left);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;

		Gizmos.DrawWireCube (middlePos, new Vector3 (delta.x, 0.1f, delta.y));

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawFrustum (transform.position, Camera.main.fieldOfView, 50f, 0.3f, ratio);
		Gizmos.matrix = Matrix4x4.identity;
	}
}
