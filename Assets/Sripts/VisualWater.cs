using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWater : MonoBehaviour {

	public WaterCPU waterCalculator;

	public int width = 32;
	public int height = 16;

	public Vector2 cubeSize;

	public float[,] waterHeight;

	// Use this for initialization
	void Start () {

		waterCalculator.Init (width, height);

		waterHeight = new float[width, height];

	}
	
	// Update is called once per frame
	void Update () {

		waterCalculator.Step ();

		waterCalculator.RetrieveIntensities (ref waterHeight);
	}
}
