using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

	public DataBetweenScenes data;

	int readyPlayers;
	float readyTime = 0;
	int readyCount = 0;
	public Animator[] readyAnim;

	public float speed = 1f;
	public float displacement = 10f;
	public float height = 1f;
	public float period = 2f;

	public Sprite[] count;
	public UnityEngine.UI.Image countdown;

	float time = 0f;
	float baseY = 0f;

	// Use this for initialization
	void Start () {
		time = 0f;
		baseY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		Vector3 pos;
		pos.z = transform.position.z;

		pos.x = (time * speed) % (displacement * 4f);
		if (pos.x > displacement * 2f) {
			pos.x = 4f * displacement - pos.x;
		}

		pos.x -= displacement;

		pos.y = baseY + Mathf.Sin (time * Mathf.PI * 2f / period) * height;

		transform.position = pos;

		#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
		string axis = "MSwim";
		#else
		string axis = "Swim";
        #endif
        string Kaxis = "KSwim";
        readyPlayers = 0;

        for (int i = 0; i < 4; ++i)
        {
            if (!data.playerPlaying[i]) continue;
            if(data.playerInputTypes[i] == 1)
                data.playerPlaying[i] = (Input.GetAxis(axis + data.playerControllerIds[i].ToString()) > 0.2f);
            else
                data.playerPlaying[i] = (Input.GetAxis(Kaxis + data.playerControllerIds[i].ToString()) > 0.2f);
        }

        for (int i = 0; i < 4; ++i)
        {
            if (!data.playerPlaying[i]) readyAnim[i].SetBool("pressed", false);
            else ++readyPlayers;
        }

        for (int i = 0; i < 4 && readyPlayers < 4; ++i) {
			if (Input.GetAxis (axis + (i + 1).ToString ()) > 0.2f) {
                int position = -1;
                for (int j = 3; j >= 0; --j)
                {
                    if(data.playerPlaying[j] && data.playerControllerIds[j] == (i+1) && data.playerInputTypes[j] == 1)
                    {
                        position = -1;
                        break;
                    }
                    if (!data.playerPlaying[j]) position = j;
                }
                if (position != -1)
                {
                    ++readyPlayers;
                    readyAnim[position].SetBool("pressed", true);
                    data.playerPlaying[position] = true;
                    data.playerControllerIds[position] = i+1;
                    data.playerInputTypes[position] = 1;
                }
            }
		}

        for (int i = 0; i < 2 && readyPlayers < 4; ++i)
        {
            if (Input.GetAxis(Kaxis + (i + 1).ToString()) > 0.2f)
            {
                int position = -1;
                for (int j = 3; j >= 0; --j)
                {
                    if (data.playerPlaying[j] && data.playerControllerIds[j] == (i + 1) && data.playerInputTypes[j] == 2)
                    {
                        position = -1;
                        break;
                    }
                    if (!data.playerPlaying[j]) position = j;
                }
                if (position != -1)
                {
                    ++readyPlayers;
                    readyAnim[position].SetBool("pressed", true);
                    data.playerPlaying[position] = true;
                    data.playerControllerIds[position] = i + 1;
                    data.playerInputTypes[position] = 2;
                }
            }
        }

        if (readyPlayers != readyCount) {
			readyTime = 0f;
		}

		if (readyPlayers >= 2) {
			readyTime += Time.deltaTime;
		} else {
			readyTime = 0;
		}

		if (readyTime == 0) {
			countdown.enabled = false;
		} else {
			countdown.enabled = true;
			int c = Mathf.Clamp(Mathf.FloorToInt (3f - readyTime), 0, count.Length);
			countdown.sprite = count [c];
		}

		readyCount = readyPlayers;
			

		if (readyTime >= 3f /*|| Input.GetKeyDown (KeyCode.Space)*/) {
			UnityEngine.SceneManagement.SceneManager.LoadScene (1);
			this.enabled = false;
		}


	}
}
