using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameCamera cameraPrefab;
    public VisualWater waterPrefab;
    public PlayerScript playerPrefab;
    public JelloScript jelloPrefab;
    public GameObject wallPrefab;
    public Scenery sceneryPrefab;
    public List<Sprite> badges;

    public float wallHeight = 3f;

    GameCamera gameCamera;
    VisualWater visualWater;
    WaterCPU waterCalculator;

    int numPlayers = 4;
    int maxMedusas = 180;
    int jellos = 1;
    PlayerScript[] players;
    List<GameObject> cameraObjects;
    int[] playerDeaths;
    JelloScript jello;

    public Vector3[,] waterDirAndHeight;

    float simulationFPS = 10f;
    float lastSimulationTime = 0;
    public UnityEngine.UI.Text simulationFPSText;

    int maxLives = 3;
    float timer;
    public bool playing;
    public List<PlayerScript> playersEliminated;
    Vector3 playerOffset = new Vector3(5f,0f,3f);

    // Use this for initialization
    void Awake() {
        lastSimulationTime = Time.unscaledTime;

        playersEliminated = new List<PlayerScript>();

        visualWater = ((GameObject)Instantiate(waterPrefab.gameObject)).GetComponent<VisualWater>();
        waterCalculator = visualWater.waterCalculator;
        visualWater.transform.SetParent(transform);
        visualWater.Init(this);

        //GenerateWalls();

        startGame();

        Vector2 poolSize = new Vector2(visualWater.width * visualWater.waterTileSize.x, visualWater.height * visualWater.waterTileSize.z);
        gameCamera.Init(cameraObjects, poolSize);
        gameCamera.transform.SetParent(transform);
        gameCamera.transform.position = transform.position + new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height / 2 * visualWater.waterTileSize.z);

        Scenery scenery = ((GameObject)Instantiate(sceneryPrefab.gameObject)).GetComponent<Scenery>();
        scenery.Init(poolSize);
    }

    private void startGame()
    {
        playing = true;

        jello = ((GameObject)Instantiate(jelloPrefab.gameObject)).GetComponent<JelloScript>();
        jello.transform.SetParent(transform);
        jello.transform.position = new Vector3(visualWater.width / 2 * visualWater.waterTileSize.x, 0, visualWater.height * .5f * visualWater.waterTileSize.z);
        jello.GetComponent<ImpulseSystem>().Init(visualWater.waterIntensityHeight, this, visualWater.width, visualWater.height);
        jello.gc = this;

        players = new PlayerScript[numPlayers];
        playerDeaths = new int[numPlayers];
        for (int i = 0; i < numPlayers; ++i)
        {
            players[i] = ((GameObject)Instantiate(playerPrefab.gameObject)).GetComponent<PlayerScript>();
            players[i].controller = this;
            players[i].tileSize = visualWater.waterTileSize.x;
            players[i].transform.SetParent(transform);
            players[i].transform.position = jello.transform.position + new Vector3(((i%2 != 0) ? playerOffset.x : -playerOffset.x),0, ((i < 2) ? playerOffset.z : -playerOffset.z));
            players[i].GetComponent<ImpulseSystem>().Init(visualWater.waterIntensityHeight, this, visualWater.width, visualWater.height);
            players[i].playerId = i + 1;
            players[i].gameObject.name = "player" + (i + 1).ToString();

            players[i].billboardRenderer.sprite = badges[i];

            playerDeaths[i] = 0;
        }

        gameCamera = ((GameObject)Instantiate(cameraPrefab.gameObject)).GetComponent<GameCamera>();

        cameraObjects = new List<GameObject>();
        for (int i = 0; i < players.Length; ++i)
        {
            cameraObjects.Add(players[i].gameObject);
        }
        cameraObjects.Add(jello.gameObject);
    }

    void endGame()
    {
        /*for (int i = 0; i < playersEliminated.Count; ++i)
        {
            Debug.Log(playersEliminated[i].playerId);
        }*/
        playing = false;
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
		if (waterCalculator.IsDirty () && playing) {
			float currentTime = Time.unscaledTime;
			float step = currentTime - lastSimulationTime;
			simulationFPS = 0.9f * simulationFPS + 0.1f * (1f / step);
			simulationFPSText.text = Mathf.FloorToInt (simulationFPS).ToString();

			waterCalculator.RetrieveIntensities (ref waterDirAndHeight);

			lastSimulationTime = currentTime;
			visualWater.UpdateMesh ();
		}
        if (!playing && Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(1);
        } 
    }

	public void AddWave(Vector3 pos, Vector2 dir) {

		Vector2 waterPos = WorldPosToWaterPos (pos);

		waterCalculator.AddWave (waterPos, dir);
	}

	public Vector2 WorldPosToWaterPos(Vector3 wPos) {
		return new Vector2 (wPos.x / visualWater.waterTileSize.x, wPos.z / visualWater.waterTileSize.z);
	}

    public Vector2 WaterPosToWorldPos(Vector2 wPos)
    {
        return new Vector2(wPos.x * visualWater.waterTileSize.x, wPos.y * visualWater.waterTileSize.z);
    }

    public void waitRespawn(int id)
	{
		players [id - 1].RemoveLive (playerDeaths [id - 1]);
        playerDeaths[id-1]++;
        if (playerDeaths[id-1] >= maxLives)
        {
            players[id - 1].gameOver = true;
            playersEliminated.Add(players[id - 1]);
            if (playersEliminated.Count >= numPlayers - 1) endGame();
        }
    }

    public void splitJello(GameObject jellOld)
    {
        if (jellos < maxMedusas && playing)
        {
            jellos++;
            jello = ((GameObject)Instantiate(jelloPrefab.gameObject)).GetComponent<JelloScript>();
            jello.transform.SetParent(transform);
            jello.transform.position = jellOld.transform.position;
            jello.GetComponent<ImpulseSystem>().Init(visualWater.waterIntensityHeight, this, visualWater.width, visualWater.height);
            jello.gc = this;

            Vector3 rd = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            Rigidbody rb = jellOld.GetComponent<Rigidbody>();
            rb.AddForce(rd * 100);
            rb = jello.GetComponent<Rigidbody>();
            rb.AddForce(-rd * 100);

            cameraObjects.Add(jello.gameObject);
        }
    }

    public bool stillMoreJellos()
    {
        return jellos < maxMedusas;
    }
}
