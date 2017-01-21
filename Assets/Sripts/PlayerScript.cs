using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    Rigidbody rb;
    float maxSpeed = 100f;
    float speed = 5000f;
    float waveHeight = 5f;
    float attackMultiplyer = 5f;
    float potency = 0f;
    float chargeSpeed = 10f;
    float maxPotency = 15f;

    float timer = 0f;
    float stepWave = .05f;

	public GameController controller;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 charDir = new Vector3(Mathf.Round(Input.GetAxis("Horizontal")), 0, Mathf.Round(Input.GetAxis("Vertical"))).normalized;

        if (timer > 0) timer -= Time.deltaTime;

        if (Input.GetAxis("Fire1") > 0.1f || Input.GetKey(KeyCode.K))
        {
            if (rb.velocity.magnitude < maxSpeed) rb.AddForce(Time.deltaTime * charDir * speed);
            if (timer <= 0)
            {
                controller.AddWave(transform.position - charDir*0.5f, -new Vector2(charDir.x, charDir.z) * waveHeight);
                timer = stepWave;
            }
        }

        

        if (Input.GetKey(KeyCode.Space))
        {
            potency = Mathf.Min(potency + Time.deltaTime * chargeSpeed, maxPotency);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            controller.AddWave(transform.position + charDir, new Vector2(charDir.x, charDir.z) * waveHeight * potency * attackMultiplyer);
            rb.AddForce(Time.deltaTime * -charDir * 2 * speed * potency);
            potency = 0;
        }
    }
}
