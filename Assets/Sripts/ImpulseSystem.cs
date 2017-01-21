using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSystem : MonoBehaviour {

    public float waterImpulse = 5;
    public float reboundStrength = 1.5f;
    bool alive = true;
    float waterIntensityHeight;
    GameController gc;
	int width, height;

    float idealYPos = 0;
    float borderSeparation = .75f;

    Rigidbody rig;

    public void SetAlive(bool al)
    {
        alive = al;
    }

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

        Vector2 maxWorldPos = gc.WaterPosToWorldPos(new Vector2(width-.5f, height-.5f));
        Vector2 minWorldPos = gc.WaterPosToWorldPos(Vector2.one*-.5f);

        Vector3 pos = transform.position;
        Vector3 vel = rig.velocity;
        if (pos.x-borderSeparation < minWorldPos.x || pos.x + borderSeparation > maxWorldPos.x)
        {
            vel.x *= -reboundStrength;
            pos.x = (pos.x - borderSeparation < minWorldPos.x) ? minWorldPos.x + borderSeparation : maxWorldPos.x - borderSeparation;
        }
        if (pos.z - borderSeparation < minWorldPos.y || pos.z + borderSeparation > maxWorldPos.y)
        {
            vel.z *= -reboundStrength;
            pos.z = (pos.z - borderSeparation < minWorldPos.y) ? minWorldPos.y + borderSeparation : maxWorldPos.y - borderSeparation;
        }
        
        Vector3 tileValue = gc.waterDirAndHeight[xTilePos, yTilePos];
        if (alive)
        {
            float idealYPos = tileValue.z * waterIntensityHeight;
            pos.y = Mathf.Lerp(pos.y, idealYPos, .4f);
        }
        transform.position = pos;
        rig.velocity = vel;
        rig.AddForce(new Vector3(tileValue.x, 0, tileValue.y)* waterImpulse, ForceMode.Force);
    }
}
