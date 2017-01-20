using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWater : MonoBehaviour {

	public GameObject waterCubePrefab;

	public WaterCPU waterCalculator;

	public int width = 32;
	public int height = 16;

	public Vector3 waterCubeSize;

	public float[,] waterHeight;

	public float waterIntensityHeight = 1f;

	public GameObject[,] waterCubes;

	// Use this for initialization
	void Start () {

		waterCalculator.Init (width, height);

		waterHeight = new float[width, height];

		waterCubes = new GameObject[width, height];
		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {
				Vector3 pos = new Vector3 (i * waterCubeSize.x, 0f, j * waterCubeSize.z);

				waterCubes [i, j] = GameObject.Instantiate (waterCubePrefab, pos, Quaternion.identity);
				waterCubes [i, j].transform.localScale = waterCubeSize;

				waterHeight [i, j] = 0f;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		waterCalculator.Step ();

		waterCalculator.RetrieveIntensities (ref waterHeight);

		Vector3 pos;
		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {
				pos = waterCubes [i, j].transform.position;

				pos.y = waterHeight [i, j] * waterIntensityHeight;
			}
		}
	}
}
