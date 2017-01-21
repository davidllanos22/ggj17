using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameCamera cameraPrefab;
	public VisualWater waterPrefab;
    public PlayerScript playerPrefab;
    public JelloScript jelloPrefab;

    GameCamera gameCamera;
	VisualWater visualWater;
    WaterCPU waterCalculator;
    PlayerScript[] players;
    JelloScript jello;

    public Vector3[,] waterDirAndHeight;


    // Use this for initialization
    void Awake () {
		
		visualWater = ((GameObject) Instantiate (waterPrefab.gameObject)).GetComponent<VisualWater> ();
        waterCalculator = visualWater.waterCalculator;
		visualWater.transform.SetParent(transform);
        visualWater.Init(this);

        players = new PlayerScript[1];
        for(int i = 0; i < 1; ++i)
        {
            players[i] = ((GameObject)Instantiate(playerPrefab.gameObject)).GetComponent<PlayerScript>();
            players[i].transform.SetParent(transform);
            players[i].transform.position =  new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
            players[i].GetComponent<ImpulseSystem>().Init(visualWater.waterTileSize, visualWater.waterIntensityHeight, this);
        }

        jello = ((GameObject)Instantiate(jelloPrefab.gameObject)).GetComponent<JelloScript>();
        jello.transform.SetParent(transform);
        jello.transform.position = new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height*.4f * visualWater.waterTileSize.z);
        jello.GetComponent<ImpulseSystem>().Init(visualWater.waterTileSize, visualWater.waterIntensityHeight, this);


        gameCamera = ((GameObject) Instantiate (cameraPrefab.gameObject)).GetComponent<GameCamera> ();
		gameCamera.transform.SetParent (transform);
		gameCamera.transform.position = transform.position + new Vector3 (visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
	}

    private void Start()
    {
        waterDirAndHeight = new Vector3[visualWater.width, visualWater.height];

        for (int i = 0; i < visualWater.width; ++i)
        {
            for (int j = 0; j < visualWater.height; ++j)
            {
                waterDirAndHeight[i, j] = Vector3.zero;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        waterCalculator.RetrieveIntensities(ref waterDirAndHeight);
    }
}
