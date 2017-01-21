﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseSystem : MonoBehaviour {

    public float waterImpulse = 5;
    Vector3 waterTileSize;
    float waterIntensityHeight;
    GameController gc;

    Rigidbody rig;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 wts, float wih, GameController gameC)
    {
        waterTileSize = wts;
        waterIntensityHeight = wih;
        gc = gameC;
    }

    // Update is called once per frame
    void Update () {

        int xTilePos = (int)transform.position.x;
        int yTilePos = (int)transform.position.z;

        Vector3 pos = transform.position;
        Vector3 tileValue = gc.waterDirAndHeight[xTilePos, yTilePos];
        pos.y = tileValue.z * waterIntensityHeight;
        transform.position = pos;

        rig.AddForce(new Vector3(tileValue.x, 0, tileValue.y)* waterImpulse, ForceMode.Impulse);
        //Vector3(width / 2 * waterTileSize.x, 0, height / 2 * waterTileSize.z);
    }
}