using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWater : MonoBehaviour {

	public WaterCPU waterCalculator;

	public int width = 32;
	public int height = 16;

	public Vector3 waterTileSize;

	public float[,] waterHeight;

	public float waterIntensityHeight = 1f;

	Mesh waterMesh;

	// Use this for initialization
	void Start () {

		waterCalculator.Init (width, height);

		waterHeight = new float[width, height];

		CreateMesh ();

		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {

				waterHeight [i, j] = 0f;
			}
		}
	}

	// Update is called once per frame
	void Update () {

		waterCalculator.Step ();

		waterCalculator.RetrieveIntensities (ref waterHeight);


		Vector3[] vertices = waterMesh.vertices;
		int w = width + 2;
		int h = height + 2;
		for (int i = 1; i < w - 1; ++i) {
			for (int j = 1; j < h - 1; ++j) {
				Vector3 v = vertices [i + j * w];
				v.y = waterHeight [i, j] * waterIntensityHeight;
				vertices [i + j * w] = v;
			}
		}
		waterMesh.vertices = vertices;
	}

	void CreateMesh() {
		waterMesh = new Mesh ();
		waterMesh.name = "Water mesh";

		waterMesh.MarkDynamic ();

		int w = width + 2;
		int h = height + 2;

		Vector3[] vertices = new Vector3[w * h];
		Vector2[] uv = new Vector2[w * h];
		for (int i = 0; i < w; ++i) {
			for (int j = 0; j < h; ++j) {
				vertices [i + j * w] = new Vector3 ((i - 1)* waterTileSize.x, 0, (j - 1) * waterTileSize.z);
				uv [i + j * w] = new Vector2 ((float)i, (float)j);
			}
		}

		int[] indices = new int[(w - 1) * (h - 1) * 6];
		for (int i = 0; i < w - 1; ++i) {
			for (int j = 0; j < h - 1; ++j) {
				int baseVertex = i + j * w;
				int baseIndex = (i + j * (w - 1)) * 6;
				indices [baseIndex + 0] = baseVertex;
				indices [baseIndex + 1] = baseVertex + w;
				indices [baseIndex + 2] = baseVertex + 1;

				indices [baseIndex + 3] = baseVertex + 1;
				indices [baseIndex + 4] = baseVertex + w;
				indices [baseIndex + 5] = baseVertex + w + 1;
			}
		}

		waterMesh.vertices = vertices;
		waterMesh.SetIndices (indices, MeshTopology.Triangles, 0);
		waterMesh.uv = uv;
		waterMesh.RecalculateNormals ();


		GetComponent<MeshFilter> ().mesh = waterMesh;
	}
}
