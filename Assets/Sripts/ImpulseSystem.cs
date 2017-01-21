using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSystem : MonoBehaviour {

    public float waterImpulse = 5;
    Vector3 waterTileSize;
    float waterIntensityHeight;
    GameController gc;
	int width, height;

    Rigidbody rig;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

	public void Init(Vector3 wts, float wih, GameController gameC, int w, int h)
    {
        waterTileSize = wts;
        waterIntensityHeight = wih;
        gc = gameC;
		width = w;
		height = h;
    }

    // Update is called once per frame
    void Update () {

		Vector2 waterPos = gc.WorldPosToWaterPos (transform.position);

		int xTilePos = Mathf.Clamp (Mathf.RoundToInt (waterPos.x), 0, width - 1);//;(int)(transform.position.x * waterTileSize.x);
		int yTilePos = Mathf.Clamp (Mathf.RoundToInt (waterPos.y), 0, height - 1);//Mathf.RoundToInt (waterPos.x);(int)(transform.position.z * waterTileSize.z);

        Vector3 pos = transform.position;
        Vector3 tileValue = gc.waterDirAndHeight[xTilePos, yTilePos];
        pos.y = tileValue.z * waterIntensityHeight;
        transform.position = pos;

        rig.AddForce(new Vector3(tileValue.x, 0, tileValue.y)* waterImpulse, ForceMode.Impulse);
    }
}
