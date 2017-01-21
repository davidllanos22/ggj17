using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    string[,] inputs = new string[,]
    {
        {"Horizontal", "Vertical", "A", "X" },
        {"Horiz", "Vert", "Swim", "Charge" }
    };

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
    public float tileSize;

    public GameController controller;
    public int playerId = 1;
    int inputType = 1;

    public float iFrames = 1f;
    float blinkRate = .08f;
    float blinkTimer = 0;

    SpriteRenderer rend;
    bool stateI = true;

    float deadTimer = 3f;
    float sinkingSpeed = .7f;
    bool alive = true;

    Vector3 lookDir;

    ImpulseSystem imp;

    public Animator anim;
	public SpriteRenderer billboardRenderer;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<SpriteRenderer>();
        lookDir = -Vector3.forward;
        imp = GetComponent<ImpulseSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            Vector3 charDir = new Vector3(Input.GetAxis(inputs[inputType, 0] + playerId), 0, Input.GetAxis(inputs[inputType, 1] + playerId));
            if (charDir.magnitude > 0.2f) lookDir = new Vector3(Mathf.Round(charDir.normalized.x), 0, Mathf.Round(charDir.normalized.z));
            if (timer > 0) timer -= Time.deltaTime;
            bool swimming = false;
            bool charging = false;
            if (Input.GetAxis(inputs[inputType, 2] + playerId) > .1f || Input.GetKey(KeyCode.K))
            {
                if (rb.velocity.magnitude < maxSpeed) rb.AddForce(Time.deltaTime * lookDir * speed);
                if (timer <= 0)
                {
                    controller.AddWave(transform.position - lookDir * .1f * tileSize, -new Vector2(lookDir.x, lookDir.z) * waveHeight);
                    timer = stepWave;
                }

                swimming = true;
            }

            if (Input.GetButton(inputs[inputType, 3] + playerId))
            {
                potency = Mathf.Min(potency + Time.deltaTime * chargeSpeed, maxPotency);
                charging = true;
            }

            if (Input.GetButtonUp(inputs[inputType, 3] + playerId))
            {
                controller.AddWave(transform.position + lookDir * .1f * tileSize, new Vector2(lookDir.x, lookDir.z) * waveHeight * potency * attackMultiplyer);
                rb.AddForce(Time.deltaTime * -lookDir * 2 * speed * potency);
                potency = 0;
            }

            SetAnimations(lookDir.x, lookDir.z, swimming, charging);

            if (iFrames > 0)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= blinkRate)
                {
                    if (stateI) rend.color = Color.gray;
                    else rend.color = Color.white;

                    stateI = !stateI;
                    blinkTimer = 0;
                }

                iFrames -= Time.deltaTime;
                if (iFrames < 0)
                {
                    rend.color = Color.white;
                    iFrames = 0;
                }
            }
        }
        else
        {
            imp.enabled = false;
            transform.position -= sinkingSpeed * Time.deltaTime * Vector3.up;
            deadTimer -= Time.deltaTime;
            if (deadTimer <= 0)
            {
                //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                imp.enabled = true;
                alive = true;
                iFrames = 2f;
            }
        }
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

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Jello" && iFrames <= 0 && alive)
        {
            controller.waitRespawn(playerId);
            deadTimer = 3f;
            alive = false;
        }
    }
}
