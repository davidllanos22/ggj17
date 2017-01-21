using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameCamera cameraPrefab;
	public VisualWater waterPrefab;
    public PlayerScript playerPrefab;
    public JelloScript jelloPrefab;
    public GameObject wallPrefab;
    public List<Sprite> badges;

    GameCamera gameCamera;
	VisualWater visualWater;
    WaterCPU waterCalculator;

    int numPlayers = 4;
    PlayerScript[] players;
    int[] playerDeaths;
    float[] respanTimers;
    JelloScript jello;

    public Vector3[,] waterDirAndHeight;

	float simulationFPS = 10f;
	float lastSimulationTime = 0;
	public UnityEngine.UI.Text simulationFPSText;

    // Use this for initialization
    void Awake () {
		lastSimulationTime = Time.unscaledTime;
		
		visualWater = ((GameObject) Instantiate (waterPrefab.gameObject)).GetComponent<VisualWater> ();
        waterCalculator = visualWater.waterCalculator;
		visualWater.transform.SetParent(transform);
        visualWater.Init(this);

        GenerateWalls();

        players = new PlayerScript[numPlayers];
        playerDeaths = new int[numPlayers];
        respanTimers = new float[numPlayers];
        GameObject badge;
        for(int i = 0; i < numPlayers; ++i)
        {
            players[i] = ((GameObject)Instantiate(playerPrefab.gameObject)).GetComponent<PlayerScript>();
			players[i].controller = this;
            players[i].transform.SetParent(transform);
            players[i].transform.position =  new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x + 3*i, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
			players[i].GetComponent<ImpulseSystem>().Init(visualWater.waterTileSize, visualWater.waterIntensityHeight, this, visualWater.width, visualWater.height);
            players[i].playerId = i + 1;
            players[i].gameObject.name = "player" + (i+1).ToString();

            players[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = badges[i];

            playerDeaths[i] = 0;
            respanTimers[i] = 0;
        }

        jello = ((GameObject)Instantiate(jelloPrefab.gameObject)).GetComponent<JelloScript>();
        jello.transform.SetParent(transform);
        jello.transform.position = new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height*.4f * visualWater.waterTileSize.z);
		jello.GetComponent<ImpulseSystem>().Init(visualWater.waterTileSize, visualWater.waterIntensityHeight, this, visualWater.width, visualWater.height);

        gameCamera = ((GameObject) Instantiate (cameraPrefab.gameObject)).GetComponent<GameCamera> ();
		gameCamera.transform.SetParent (transform);
		gameCamera.transform.position = transform.position + new Vector3 (visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
	}

    void GenerateWalls()
    {
        GameObject wall = ((GameObject)Instantiate(wallPrefab.gameObject));
        wall.transform.SetParent(transform);
        wall.transform.position = new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, -.5f * visualWater.waterTileSize.z);
        wall.transform.localScale = new Vector3(visualWater.width, 10, visualWater.waterTileSize.z);

        wall = ((GameObject)Instantiate(wallPrefab.gameObject));
        wall.transform.SetParent(transform);
        wall.transform.position = new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, (visualWater.height + .5f) * visualWater.waterTileSize.z);
        wall.transform.localScale = new Vector3(visualWater.width, 10, visualWater.waterTileSize.z);

        wall = ((GameObject)Instantiate(wallPrefab.gameObject));
        wall.transform.SetParent(transform);
        wall.transform.position = new Vector3(-.5f * visualWater.waterTileSize.x, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
        wall.transform.localScale = new Vector3(visualWater.waterTileSize.x, 10, visualWater.height);

        wall = ((GameObject)Instantiate(wallPrefab.gameObject));
        wall.transform.SetParent(transform);
        wall.transform.position = new Vector3((visualWater.width+.5f) * visualWater.waterTileSize.x, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
        wall.transform.localScale = new Vector3(visualWater.waterTileSize.x, 10, visualWater.height);

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
		if (waterCalculator.IsDirty ()) {
			float currentTime = Time.unscaledTime;
			float step = currentTime - lastSimulationTime;
			simulationFPS = 0.9f * simulationFPS + 0.1f * (1f / step);
			simulationFPSText.text = Mathf.FloorToInt (simulationFPS).ToString();

			waterCalculator.RetrieveIntensities (ref waterDirAndHeight);

			lastSimulationTime = currentTime;
			visualWater.UpdateMesh ();
		}

        for (int i = 0; i < numPlayers; i++)
        {
            if (respanTimers[i] > 0)
            {
                respanTimers[i] -= Time.deltaTime;
                if (respanTimers[i] <= 0)
                {
                    players[i] = ((GameObject)Instantiate(playerPrefab.gameObject)).GetComponent<PlayerScript>();
                    players[i].controller = this;
                    players[i].transform.SetParent(transform);
                    players[i].transform.position = new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x + 3 * i, 0, visualWater.height / 2 * visualWater.waterTileSize.z);
                    players[i].GetComponent<ImpulseSystem>().Init(visualWater.waterTileSize, visualWater.waterIntensityHeight, this, visualWater.width, visualWater.height);
                    players[i].playerId = i + 1;
                    players[i].iFrames = 2;
                    respanTimers[i] = 0;
                }
            }
        }
    }

	public void AddWave(Vector3 pos, Vector2 dir) {

		Vector2 waterPos = WorldPosToWaterPos (pos);

		waterCalculator.AddWave (waterPos, dir);
	}

	public Vector2 WorldPosToWaterPos(Vector3 wPos) {
		return new Vector2 (wPos.x / visualWater.waterTileSize.x, wPos.z / visualWater.waterTileSize.z);
	}

    public void waitRespawn(int id)
    {
        playerDeaths[id-1]++;
        respanTimers[id-1] = 3;
    }
}
