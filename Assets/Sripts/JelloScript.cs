using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JelloScript : MonoBehaviour {

    Rigidbody rig;
    public Animator anim;

    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        SetAnimations(rig.velocity.x, rig.velocity.z);
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
