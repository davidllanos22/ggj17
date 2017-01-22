﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JelloScript : MonoBehaviour {

    Rigidbody rig;
    SpriteRenderer rend;
    public GameController gc;

    public Animator anim;

    float timerMitosis;

    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<SpriteRenderer>();

        timerMitosis = Random.Range(10, 20);
    }

    // Update is called once per frame
    void Update () {
        SetAnimations(rig.velocity.x, rig.velocity.z);

        timerMitosis -= Time.deltaTime;
        if (timerMitosis <= 0)
        {
            gc.splitJello(gameObject);
            timerMitosis = Random.Range(10, 20);
            rend.color = Color.white;
        }
        else if (timerMitosis < 5)
        {
            rend.color = new Color(1, rend.color.g - .2f * Time.deltaTime, rend.color.b - .2f * Time.deltaTime);
        }
    }

    void SetAnimations(float xAxis, float yAxis)
    {
        bool up = false;
        bool down = false;
        bool right = false;
        bool left = false;

        up = (yAxis >= 0.5f);
        down = (yAxis <= -0.5f);
        left = (xAxis <= -0.5f);
        right = (xAxis >= 0.5f);

        anim.SetBool("Up", up);
        anim.SetBool("Down", down);
        anim.SetBool("Left", left);
        anim.SetBool("Right", right);
    }
}
