using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameCamera cameraPrefab;
	public VisualWater waterPrefab;

	GameCamera gameCamera;
	VisualWater visualWater;

	// Use this for initialization
	void Awake () {
		
		visualWater = ((GameObject) Instantiate (waterPrefab.gameObject)).GetComponent<VisualWater> ();
		visualWater.transform.SetParent(transform);

		gameCamera = ((GameObject) Instantiate (cameraPrefab.gameObject)).GetComponent<GameCamera> ();
		gameCamera.transform.SetParent (transform);
		gameCamera.transform.position = transform.position + new Vector3 (visualWater.width/2 * visualWater.waterTileSize.x, 0, visualWater.height/2 * visualWater.waterTileSize.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
