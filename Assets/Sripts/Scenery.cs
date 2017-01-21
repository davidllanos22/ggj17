using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenery : MonoBehaviour {

	public float worldHeight = 2 / 16f;

	public float backFloorWidth = 2f;

	public GameObject poolWall;
	public GameObject floor;
	public GameObject roomWall;
	public GameObject stairPrefab;
	public GameObject trampolinePrefab;
	public GameObject referee;

	// Use this for initialization
	public void Init (Vector2 poolSize) {

		// Pool walls
		GameObject leftWall = GameObject.Instantiate (poolWall, new Vector3(-0.25f, worldHeight,-0.25f), Quaternion.AngleAxis(-90, Vector3.up));
		leftWall.transform.parent = transform;
		leftWall.transform.localScale = new Vector3 (poolSize.y, 1f, 1f);
		leftWall.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (poolSize.y, 1f);

		GameObject frontWall = GameObject.Instantiate (poolWall, new Vector3(-0.25f, worldHeight,-0.25f + poolSize.y), Quaternion.identity);
		frontWall.transform.parent = transform;
		frontWall.transform.localScale = new Vector3 (poolSize.x, 1f, 1f);
		frontWall.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (poolSize.x, 1f);

		GameObject rightWall = GameObject.Instantiate (poolWall, new Vector3(-0.25f + poolSize.x, worldHeight, poolSize.y -0.25f), Quaternion.AngleAxis(90, Vector3.up));
		rightWall.transform.parent = transform;
		rightWall.transform.localScale = new Vector3 (poolSize.y, 1f, 1f);
		rightWall.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (poolSize.y, 1f);

		// Floors
		GameObject poolFloor = GameObject.Instantiate (floor, new Vector3(-0.25f, worldHeight - 3f, -0.25f), Quaternion.identity);
		poolFloor.transform.parent = transform;
		poolFloor.transform.localScale = new Vector3 (poolSize.x, 1f, poolSize.y);
		poolFloor.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (poolFloor.transform.localScale.x, poolFloor.transform.localScale.z)*2f;

		float floorWidth = 20f;
		GameObject leftFloor = GameObject.Instantiate (floor, new Vector3(-0.25f - floorWidth, worldHeight,-0.25f - floorWidth), Quaternion.identity);
		leftFloor.transform.parent = transform;
		leftFloor.transform.localScale = new Vector3 (floorWidth, 1f, floorWidth + poolSize.y + backFloorWidth);
		leftFloor.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (leftFloor.transform.localScale.x, leftFloor.transform.localScale.z);

		GameObject rightFloor = GameObject.Instantiate (floor, new Vector3(-0.25f + poolSize.x, worldHeight,-0.25f - floorWidth), Quaternion.identity);
		rightFloor.transform.parent = transform;
		rightFloor.transform.localScale = new Vector3 (floorWidth, 1f, floorWidth + poolSize.y + backFloorWidth);
		rightFloor.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (rightFloor.transform.localScale.x, rightFloor.transform.localScale.z);

		GameObject backFloor = GameObject.Instantiate (floor, new Vector3(-0.25f, worldHeight, -0.25f + poolSize.y), Quaternion.identity);
		backFloor.transform.parent = transform;
		backFloor.transform.localScale = new Vector3 (poolSize.x, 1f, backFloorWidth);
		backFloor.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (backFloor.transform.localScale.x, backFloor.transform.localScale.z);

		GameObject frontFloor = GameObject.Instantiate (floor, new Vector3(-0.25f, worldHeight, -0.25f - floorWidth), Quaternion.identity);
		frontFloor.transform.parent = transform;
		frontFloor.transform.localScale = new Vector3 (poolSize.x, 1f, floorWidth);
		frontFloor.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (frontFloor.transform.localScale.x, frontFloor.transform.localScale.z);

		// Back Wall
		GameObject wall = GameObject.Instantiate (roomWall, new Vector3(-0.25f - floorWidth, worldHeight,-0.25f + poolSize.y + backFloorWidth), Quaternion.identity);
		wall.transform.parent = transform;
		wall.transform.localScale = new Vector3 (poolSize.x + floorWidth * 2f, 1f, 1f);
		wall.GetComponentInChildren<MeshRenderer> ().material.mainTextureScale = new Vector2 (wall.transform.localScale.x / 2f, 1f);

		// Other
		GameObject stairLeft = GameObject.Instantiate (stairPrefab, new Vector3(-0.25f + 1, worldHeight, -0.25f + poolSize.y - 0.1f), Quaternion.identity);
		stairLeft.transform.parent = transform;

		GameObject stairRight = GameObject.Instantiate (stairPrefab, new Vector3(-0.25f + poolSize.x - 2, worldHeight, -0.25f + poolSize.y - 0.1f), Quaternion.identity);
		stairRight.transform.parent = transform;

		GameObject trampolineLeft = GameObject.Instantiate (trampolinePrefab, new Vector3(-0.25f - 1, worldHeight, -0.25f + Mathf.FloorToInt(poolSize.y / 2)), Quaternion.identity);
		trampolineLeft.transform.parent = transform;

		GameObject trampolineRight = GameObject.Instantiate (trampolinePrefab, new Vector3(-0.25f + poolSize.x + 1, worldHeight, -0.25f + Mathf.FloorToInt(poolSize.y / 2) + 1), Quaternion.AngleAxis(180f, Vector3.up));
		trampolineRight.transform.parent = transform;

		GameObject refereeLeft = GameObject.Instantiate (referee, new Vector3(-0.25f + 3.5f, worldHeight, -0.25f + poolSize.y + 0.5f), Quaternion.identity);
		refereeLeft.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
