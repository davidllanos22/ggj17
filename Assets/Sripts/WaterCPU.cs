﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCPU : MonoBehaviour {

    int width, height;
    float poolmin = -10;
    float poolmax = 50;
    float[,,] pool; 

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

	// Use this for initialization
	public void Init (int w, int h) {
        width = w; height = h;
        pool = new float[w,h,9];

        for(int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                for (int k = 0; k < 9; ++k)
                {
                    pool[i, j, k] = 0;
                }
            }
        }
	}
	
	// Update is called once per frame
	public void Step () {
        float[,,] newPool = new float[width, height, 9]; ;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 0; k < 9; ++k)
                {
                    if(k == 0)
                    {
                        if (pool[i, j, k] < 0) newPool[i, j, k] = 0;
                        else newPool[i, j, k] = pool[i, j, k] * -.2f;
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

                    for (int t = k-1; t < k+2; ++t)
                    {
                        int tt = t;
                        if (tt == 0) tt = 8;
                        if (tt == 9) tt = 1;
                        int x = i+ dirvalues[tt, 0];
                        int y = j+ dirvalues[tt, 1];

                        if (x >= width || x < 0 || y >= height || y < 0) continue; //rebotes
                        
                        newPool[x, y, k] += pool[i,j,k]*((tt == k) ? .5f: .2f);
                        newPool[x, y, 0] = Mathf.Clamp(newPool[x, y, 0] + newPool[x, y, k], poolmin, poolmax);
                    }
                }
            }
        }
        pool = newPool;
    }

	public void AddWave (Vector2 position, Vector2 wave) {
        float angle = Vector2.Angle(new Vector2(0, 1), wave);
        if (wave.x < 0) angle = 360 - angle;
        angle = Mathf.Round(angle / 45 + 1);

        pool[(int)position.x, (int)position.y, (int)angle] += wave.magnitude;
	}

	// Matrix is w x h
	public void RetrieveIntensities (ref float[,] matrix) {
        for(int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                matrix[i, j] = pool[i, j, 0] / poolmax;
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
}
