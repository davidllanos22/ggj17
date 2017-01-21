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
    int playerId = 1;

    Animator anim;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 charDir = new Vector3(Mathf.Round(Input.GetAxis("Horizontal"+playerId)), 0, Mathf.Round(Input.GetAxis("Vertical"+playerId))).normalized;
        if (timer > 0) timer -= Time.deltaTime;
        bool swimming = false;
        bool charging = false;
        if (Input.GetButton("A"+playerId) || Input.GetKey(KeyCode.K))
        {
            if (rb.velocity.magnitude < maxSpeed) rb.AddForce(Time.deltaTime * charDir * speed);
            if (timer <= 0)
            {
                controller.AddWave(transform.position - charDir*0.5f, -new Vector2(charDir.x, charDir.z) * waveHeight);
                timer = stepWave;
            }

            swimming = true;
        }        

        if (Input.GetButton("X"+playerId))
        {
            potency = Mathf.Min(potency + Time.deltaTime * chargeSpeed, maxPotency);
            charging = true;
        }

        if (Input.GetButtonUp("X" + playerId))
        {
            controller.AddWave(transform.position + charDir, new Vector2(charDir.x, charDir.z) * waveHeight * potency * attackMultiplyer);
            rb.AddForce(Time.deltaTime * -charDir * 2 * speed * potency);
            potency = 0;
        }

        SetAnimations(charDir.x, charDir.z, swimming, charging);

    }

    void SetAnimations(float xAxis, float yAxis, bool swimming, bool charging)
    {
        bool up = false;
        bool down = false;
        bool right = false;
        bool left = false;

        up = (yAxis >= 0.15f);
		down = (yAxis <= -0.15f);
		left = (xAxis <= -0.15f);
		right = (xAxis >= 0.15f);

        anim.SetBool("Up", up);
        anim.SetBool("Down", down);
        anim.SetBool("Left", left);
        anim.SetBool("Right", right);
        anim.SetBool("Swimming", swimming);
        anim.SetBool("Charging", charging);
    }
}
