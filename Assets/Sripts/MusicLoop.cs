using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoop : MonoBehaviour {


    public AudioSource aud;
    private void Start()
    {
        aud.time = 40;
    }
    
	void Update () {
		if(!aud.isPlaying)
        {
            aud.time = 16.05f;
            aud.Play();
        }
	}
}
