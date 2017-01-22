using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    string[,] inputs = new string[,]
    {
        {"Horizontal", "Vertical", "A", "X", "B" },
        {"Horiz", "Vert", "Swim", "Charge", "Scream" }
    };

    Rigidbody rb;
    float maxSpeed = 100f;
    float speed = 5000f;
    float waveHeight = 10f;
    float attackMultiplyer = 5f;
    float potency = 0f;
    float chargeSpeed = 10f;
    float maxPotency = 15f;

    float timer = 0f;
    float stepWave = .05f;
    public float tileSize;

    public GameController controller;
    public int playerId = 1;
    public int inputType = 1;

    public float iFrames = 1f;
    float blinkRate = .08f;
    float blinkTimer = 0;

    SpriteRenderer rend;
    bool stateI = true;

    float deadTimer = 3f;
    float sinkingSpeed = .6f;
    bool alive = true;
    public bool gameOver = false;

    Vector3 lookDir;

    ImpulseSystem imp;

    public Animator anim;
	public SpriteRenderer billboardRenderer;


    public AudioSource oneShotAudio;
    public AudioSource cutAudio;
    public AudioClip[] attackInterv;
    public AudioClip[] swimRand;
    public AudioClip[] screams;
    public AudioClip drownAudio;

	public Animator[] lives;
	public GameObject livesHolder;

    float audioSwimTimer = 0f;
    float audioSwimStep = .85f;


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<SpriteRenderer>();
        lookDir = -Vector3.forward;
        imp = GetComponent<ImpulseSystem>();
		livesHolder.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
        if (alive && controller.playing)
        {
            Vector3 charDir = new Vector3(Input.GetAxis(inputs[inputType, 0] + playerId), 0, Input.GetAxis(inputs[inputType, 1] + playerId));
            if (charDir.magnitude > 0.2f) lookDir = new Vector3(charDir.normalized.x, 0, charDir.normalized.z);
            if (timer > 0) timer -= Time.deltaTime;
            if (audioSwimTimer > 0) audioSwimTimer -= Time.deltaTime;

            bool swimming = false;
            bool charging = false;
            if (Input.GetAxis(inputs[inputType, 2] + playerId) > .1f)
            {
                if (rb.velocity.magnitude < maxSpeed) rb.AddForce(Time.deltaTime * lookDir * speed);
                if (timer <= 0)
                {
                    controller.AddWave(transform.position - lookDir * (.3f + ((lookDir.z > 0) ? 0 : 1f)) * tileSize, -new Vector2(lookDir.x, lookDir.z) * waveHeight);
                    timer = stepWave;
                }

                if(audioSwimTimer <= 0)
                {
                    //SwimAudio
                    int index = Random.Range(0, swimRand.Length);
                    oneShotAudio.PlayOneShot(swimRand[index]);
                    audioSwimTimer = audioSwimStep;
                }

                swimming = true;
            }

            if (Input.GetButton(inputs[inputType, 3] + playerId))
            {
                potency = Mathf.Min(potency + Time.deltaTime * chargeSpeed, maxPotency);
                charging = true;
                if (potency < maxPotency) rend.color = new Color(rend.color.r, rend.color.g - .3f *Time.deltaTime * maxPotency / chargeSpeed, rend.color.b - Time.deltaTime * maxPotency / chargeSpeed);
            }

            if (Input.GetButtonUp(inputs[inputType, 3] + playerId))
            {
                controller.AddWave(transform.position + lookDir * (.3f + ((lookDir.z < 0) ? 0 : .3f)) * tileSize, new Vector2(lookDir.x, lookDir.z) * waveHeight * potency * attackMultiplyer);
                rb.AddForce(Time.deltaTime * -lookDir * 4f * speed * potency);
                
                //AttackAudio
                int index = Mathf.FloorToInt((potency / maxPotency) * (attackInterv.Length-.1f));
                oneShotAudio.PlayOneShot(attackInterv[index]);

                potency = 0;

                rend.color = Color.white;
            }

            //Scream audio
            if(Input.GetButtonDown(inputs[inputType,4] + playerId))
            {
                int index = Random.Range(0,screams.Length);
                cutAudio.Stop();
                cutAudio.clip = screams[index];
                cutAudio.Play();
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
            if (!gameOver)
            {
                transform.position -= sinkingSpeed * Time.deltaTime * Vector3.up;
                deadTimer -= Time.deltaTime;
                if (deadTimer <= 0)
                {
                    //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                    imp.SetAlive(true);
                    alive = true;
                    iFrames = 2f;
                    anim.SetBool("Alive", alive);

					livesHolder.SetActive (false);
                }
            }
            else transform.position -= sinkingSpeed * Time.deltaTime * Vector3.up;
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
            anim.SetBool("Alive", alive);
            lookDir = -Vector3.forward;
            imp.SetAlive(false);

            //Drown Audio
            oneShotAudio.PlayOneShot(drownAudio);

        }
    }

	public void RemoveLive(int deaths) {
		
		livesHolder.SetActive (true);
		lives [deaths].SetTrigger ("Die");
	}
}
