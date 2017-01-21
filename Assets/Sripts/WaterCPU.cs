using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCPU : MonoBehaviour {

    int width, height;
    float poolmin = -10;
    float poolmax = 50;
    float[,,] pool1;
    float[,,] pool2;

    float[,] waterMatrix;
    bool usePool1 = true;
    bool running = true;

    List<WaveInfo> wavesToAdd;


    private System.Threading.Thread waveProc;

    class WaveInfo {
        public int i, j, k, power;
        public WaveInfo(int ii,int jj,int kk,int pp)
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

    // Use this for initialization
    public void Init (int w, int h) {
        width = w; height = h;
        pool1 = new float[w,h,9];
        pool2 = new float[w, h, 9];
        waterMatrix = new float[w, h];
        wavesToAdd = new List<WaveInfo>();

        for (int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                waterMatrix[i,j] = 0;
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
        while (running)
        {
            lock (wavesToAdd) {
                foreach (WaveInfo wi in wavesToAdd)
                {
                    AddWaveToPool(wi);
                }
                wavesToAdd.Clear();
            }
            Step();
        }
    }
	
	// Update is called once per frame
    void Step () {
        float[,,] oldPool = (usePool1) ? pool1 : pool2;
        float[,,] newPool = (usePool1) ? pool2 : pool1;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 0; k < 9; ++k)
                {
                    if(k == 0)
                    {
                        if (oldPool[i, j, k] < 0) newPool[i, j, k] = 0;
                        else newPool[i, j, k] = oldPool[i, j, k] * -.5f;
                        continue;
                    } else newPool[i, j, k] = 0;
                }
            }
        }

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 1; k < 9; ++k)
                {

                    
                    for (int t = 0; t < 5; ++t)
                    {
                        int tt = dirspawns[k,t];
                        int x = i+ dirvalues[tt, 0];
                        int y = j+ dirvalues[tt, 1];

                        if (x >= width || x < 0 || y >= height || y < 0) continue; //rebotes

                        float reduction = Mathf.Pow(.25f,Mathf.Abs(2 - t));
                        newPool[x, y, k] += oldPool[i, j, k] * .6f*reduction;
                        newPool[x, y, 0] = Mathf.Clamp(newPool[x, y, 0] + oldPool[x, y, k], poolmin, poolmax);
                    }
                }
            }
        }

        lock (waterMatrix) {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    waterMatrix[i, j] = Mathf.Sin((newPool[i, j, 0] / poolmax) * Mathf.PI * .5f);
                }
            }
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

        WaveInfo wi = new WaveInfo((int)position.x, (int)position.y, (int)angle, (int)wave.magnitude);
        lock (wavesToAdd)
        {
            wavesToAdd.Add(wi);
        }
    }

	// Matrix is w x h
	public void RetrieveIntensities (ref float[,] matrix) {
        lock(waterMatrix)
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    matrix[i, j] = waterMatrix[i,j];
                }
            }
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
}
