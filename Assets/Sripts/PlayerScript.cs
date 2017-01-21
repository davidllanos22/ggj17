using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    Rigidbody rb;
    float maxSpeed = 100f;
    float speed = 8000f;
    float waveHeight = 1f;
    float potency = 0f;
    float chargeSpeed = 5f;
    float maxPotency = 15f;

	public GameController controller;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 charDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if (Input.GetAxis("Fire1") > 0.1f || Input.GetKey(KeyCode.K))
        {
            if (rb.velocity.magnitude < maxSpeed) rb.AddForce(Time.deltaTime * charDir * speed);
            controller.AddWave(transform.position - charDir, -new Vector2(charDir.x, charDir.z) * waveHeight);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            potency = Mathf.Min(potency + Time.deltaTime * chargeSpeed, maxPotency);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            controller.AddWave(transform.position + charDir, new Vector2(charDir.x, charDir.z) * waveHeight * potency);
            rb.AddForce(Time.deltaTime * -charDir * 2 * speed * potency);
            potency = 0;
        }
    }
}
