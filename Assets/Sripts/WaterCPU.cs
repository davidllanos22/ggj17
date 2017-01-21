using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCPU : MonoBehaviour {

    public float tilesPerSecond = 10;
    public float reboundReduction = .9f;
    static private long TICKSINSECONDS = 10000000;

    int width, height;
    float poolmin = -10;
    float poolmax = 50;
    float threshold = .005f;
    float[,,] pool1;
    float[,,] pool2;

    Vector3[,] waterMatrix;
    bool usePool1 = true;
    bool running = true;

	bool isDirty = false;

    List<WaveInfo> wavesToAdd;
    List<WaveInfo> reboundsToAdd;

    private System.Threading.Thread waveProc;

    class WaveInfo {
        public int i, j, k;
        public float power;
        public WaveInfo(int ii,int jj,int kk,float pp)
        {
            i = ii;
            j = jj;
            k = kk;
            power = pp;
        }
    }

    int[,] dirvalues = new int[,]
    {
        {0,0 },
        {0,1 },
        {1,1 },
        {1,0 },
        {1,-1 },
        {0,-1 },
        {-1,-1 },
        {-1,0 },
        {-1,1 }
    };

    int[,] dirspawns = new int[,]
    {
        {0,0,0,0,0},
        {7,8,1,2,3},
        {8,1,2,3,4},
        {1,2,3,4,5},
        {2,3,4,5,6},
        {3,4,5,6,7},
        {4,5,6,7,8},
        {5,6,7,8,1},
        {6,7,8,1,2}
    };

    int[,] dirrebound = new int[,] //x colision, y colision, xy colision
    {
        {0,0,0},
        {1,5,5},
        {8,4,6},
        {7,3,7},
        {6,2,8},
        {5,1,1},
        {4,8,2},
        {3,7,3},
        {2,6,4}
    };

    int[,] dirColisions = new int[,] //0: negative colision, 1: positive colision, 2: no colision
    {
        {2,2},
        {2,1},
        {1,1},
        {1,2},
        {1,0},
        {2,0},
        {0,0},
        {0,2},
        {0,1}
    };

    // Use this for initialization
    public void Init (int w, int h) {
        width = w; height = h;
        pool1 = new float[w,h,9];
        pool2 = new float[w, h, 9];
        waterMatrix = new Vector3[w, h];
        wavesToAdd = new List<WaveInfo>();
        reboundsToAdd = new List<WaveInfo>();

        for (int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                waterMatrix[i,j] = Vector3.zero;
                for (int k = 0; k < 9; ++k)
                {
                    pool1[i, j, k] = 0;
                }
            }
        }
        waveProc = new System.Threading.Thread(() => Waves());
        waveProc.Start();
	}

    void Waves()
    {
        long startTime;
        long endTime;
        while (running)
        {

            startTime = System.DateTime.Now.Ticks;
            lock (wavesToAdd) {
                foreach (WaveInfo wi in wavesToAdd)
                {
                    AddWaveToPool(wi);
                }
                wavesToAdd.Clear();
            }
            Step();
            endTime = System.DateTime.Now.Ticks;
            while (endTime - startTime < TICKSINSECONDS / tilesPerSecond) endTime = System.DateTime.Now.Ticks;
        }
    }
	
	// Update is called once per frame
    void Step () {
        float[,,] oldPool = (usePool1) ? pool1 : pool2;
        float[,,] newPool = (usePool1) ? pool2 : pool1;

        bool[] xCols = new bool[3];
        bool[] yCols = new bool[3];
        xCols[2] = false;
        yCols[2] = false;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                xCols[1] = i == (width - 1);
                xCols[0] = i == 0;
                yCols[1] = j == (height - 1);
                yCols[0] = j == 0;

                for (int k = 0; k < 9; ++k)
                {
                    if(k == 0)
                    {
                        if (oldPool[i, j, k] < 0) newPool[i, j, k] = 0;
                        else newPool[i, j, k] = oldPool[i, j, k] * -.5f;
                        continue;
                    } else newPool[i, j, k] = 0;

                    //Rebound
                    if (xCols[dirColisions[k, 0]] || yCols[dirColisions[k, 1]])
                    {
                        WaveInfo wi;
                        if (!yCols[dirColisions[k, 1]]) wi = new WaveInfo(i, j, dirrebound[k, 0], oldPool[i, j, k] * reboundReduction); //xColision only
                        else if (!xCols[dirColisions[k, 0]]) wi = new WaveInfo(i, j, dirrebound[k, 1], oldPool[i, j, k] * reboundReduction); //yColision only
                        else wi = new WaveInfo(i, j, dirrebound[k, 2], oldPool[i, j, k] * reboundReduction); //both colisions
                        oldPool[i, j, k] = 0;
                        reboundsToAdd.Add(wi);
                    }
                }
            }
        }

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 1; k < 9; ++k)
                {
                    //wave control
                    if (oldPool[i, j, k] < threshold) continue;

                    for (int t = 0; t < 5; ++t)
                    {
                        int tt = dirspawns[k,t];
                        int x = i+ dirvalues[tt, 0];
                        int y = j+ dirvalues[tt, 1];                        

                        if (x >= width || x < 0 || y >= height || y < 0) continue; //out of bounds

                        float reduction = Mathf.Pow(.25f,Mathf.Abs(2 - t));
                        newPool[x, y, k] += oldPool[i, j, k] * .6f*reduction;
                        newPool[x, y, 0] = Mathf.Clamp(newPool[x, y, 0] + oldPool[x, y, k], poolmin, poolmax);
                    }
                }
            }
        }

        foreach (WaveInfo wi in reboundsToAdd)
        {
            newPool[wi.i, wi.j, wi.k] += wi.power;
            newPool[wi.i, wi.j, 0] = Mathf.Clamp(newPool[wi.i, wi.j, 0] + oldPool[wi.i, wi.j, wi.k], poolmin, poolmax);
        }
        reboundsToAdd.Clear();

        lock (waterMatrix) {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {

                    Vector2 dirSum = Vector2.zero;
                    for (int k = 1; k < 9; ++k)
                    {
                        Vector2 dir = new Vector2(dirvalues[k, 0], dirvalues[k, 1]);
                        dirSum += dir.normalized*newPool[i, j, k];
                    }
                    waterMatrix[i, j] = new Vector3(dirSum.x,dirSum.y,Mathf.Sin((newPool[i, j, 0] / poolmax) * Mathf.PI * .5f));
                }
            }

			isDirty = true;
        }


        usePool1 = !usePool1;
    }

	void AddWaveToPool (WaveInfo wi) {
        
        if(usePool1) pool1[wi.i, wi.j, wi.k] += wi.power;
        else pool2[wi.i, wi.j, wi.k] += wi.power;
    }

    public void AddWave(Vector2 position, Vector2 wave)
    {
        float angle = Vector2.Angle(new Vector2(0, 1), wave);
        if (wave.x < 0) angle = 360 - angle;
        angle = Mathf.Floor(angle / 45 + 1);

        WaveInfo wi = new WaveInfo((int)position.x, (int)position.y, (int)angle, wave.magnitude);
        lock (wavesToAdd)
        {
            wavesToAdd.Add(wi);
        }
    }

	// Matrix is w x h
	public void RetrieveIntensities (ref Vector3[,] matrix) {
        lock(waterMatrix)
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    matrix[i, j] = waterMatrix[i,j];
                }
            }

			isDirty = false;
        }
	}
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Vector2 pos = new Vector2(Random.Range(0, width), Random.Range(0, height));
            Vector2 wave = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            AddWave(pos, wave.normalized*Random.Range(10f,30f));
        }
    }

    private void OnDestroy()
    {
        running = false;
    }

	public bool IsDirty() {
		return isDirty;
	}
}
