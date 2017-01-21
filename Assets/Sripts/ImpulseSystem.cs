using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSystem : MonoBehaviour {

    public float waterImpulse = 5;
    public float reboundStrength = 1.5f;
    float waterIntensityHeight;
    GameController gc;
	int width, height;

    float idealYPos = 0;

    Rigidbody rig;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

	public void Init(float wih, GameController gameC, int w, int h)
    {
        waterIntensityHeight = wih;
        gc = gameC;
		width = w;
		height = h;
    }

    // Update is called once per frame
    void FixedUpdate () {

		Vector2 waterPos = gc.WorldPosToWaterPos (transform.position);

		int xTilePos = Mathf.Clamp (Mathf.RoundToInt (waterPos.x), 0, width - 1);//;(int)(transform.position.x * waterTileSize.x);
		int yTilePos = Mathf.Clamp (Mathf.RoundToInt (waterPos.y), 0, height - 1);//Mathf.RoundToInt (waterPos.x);(int)(transform.position.z * waterTileSize.z);

        Vector2 worldPos = gc.WaterPosToWorldPos(new Vector2(xTilePos, yTilePos));

        Vector3 pos = transform.position;
        Vector3 vel = rig.velocity;
        if (waterPos.x < 0 || waterPos.x > width - 1)
        {
            vel.x *= -reboundStrength;
            pos.x = worldPos.x;
        }
        if (waterPos.y < 0 || waterPos.y > height - 1)
        {
            vel.z *= -reboundStrength;
            pos.z = worldPos.y;
        }
        
        Vector3 tileValue = gc.waterDirAndHeight[xTilePos, yTilePos];
        float idealYPos = tileValue.z * waterIntensityHeight;
        pos.y = Mathf.Lerp(pos.y, idealYPos, .4f);
        transform.position = pos;
        rig.velocity = vel;
        rig.AddForce(new Vector3(tileValue.x, 0, tileValue.y)* waterImpulse, ForceMode.Force);
    }
}
