using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWater : MonoBehaviour {

	public WaterCPU waterCalculator;
	public Vector3 waterTileSize;
	public float waterIntensityHeight = 1f;
    public int width = 32;
    public int height = 16;
    GameController gc;
    Mesh waterMesh;

	// Use this for initialization
	void Start () {
		waterCalculator.Init (width, height);
		CreateMesh ();		
	}

    public void Init(GameController gameC)
    {
        gc = gameC;
    }

	// Update is called once per frame
	void Update () {
		Vector3[] vertices = waterMesh.vertices;
		int w = width + 2;
		int h = height + 2;
		for (int i = 1; i < w - 1; ++i) {
			for (int j = 1; j < h - 1; ++j) {
				Vector3 v = vertices [i + j * w];
				v.y = gc.waterDirAndHeight [i - 1, j - 1].z * waterIntensityHeight;
				vertices [i + j * w] = v;
			}
		}
		waterMesh.vertices = vertices;
		waterMesh.RecalculateNormals ();
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

		int[] indices = new int[(w - 1) * (h - 1) * 6 * 2];
		for (int i = 0; i < w - 1; ++i) {
			for (int j = 0; j < h - 1; ++j) {
				int baseVertex = i + j * w;
				int baseIndex = (i + j * (w - 1)) * 6 * 2;
				indices [baseIndex + 0] = baseVertex;
				indices [baseIndex + 1] = baseVertex + w;
				indices [baseIndex + 2] = baseVertex + 1;

				indices [baseIndex + 3] = baseVertex + 1;
				indices [baseIndex + 4] = baseVertex + w;
				indices [baseIndex + 5] = baseVertex + w + 1;

				indices [baseIndex + 6] = baseVertex;
				indices [baseIndex + 7] = baseVertex + w + 1;
				indices [baseIndex + 8] = baseVertex + 1;

				indices [baseIndex + 9] = baseVertex;
				indices [baseIndex + 10] = baseVertex + w;
				indices [baseIndex + 11] = baseVertex + w + 1;
			}
		}

		waterMesh.vertices = vertices;
		waterMesh.SetIndices (indices, MeshTopology.Triangles, 0);
		waterMesh.uv = uv;
		waterMesh.RecalculateNormals ();


		GetComponent<MeshFilter> ().mesh = waterMesh;
		GetComponent<MeshRenderer> ().sortingLayerName = "Default";
	}
}
