using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    Rigidbody rb;
    float maxSpeed = 100f;
    float speed = 8000f;
    float waveHeight = 5f;
    WaterCPU wCPU;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        wCPU = GameObject.FindGameObjectWithTag("Water").GetComponent<WaterCPU>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 charDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetAxis("Fire1") > 0.1f || Input.GetKey(KeyCode.K))
        {
            if (rb.velocity.magnitude < maxSpeed) rb.AddForce(Time.deltaTime * charDir.normalized * speed);
            wCPU.AddWave(transform.position, -new Vector2(charDir.x, charDir.z) * waveHeight);
        }


	}
}
