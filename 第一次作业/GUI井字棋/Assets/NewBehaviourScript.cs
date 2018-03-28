using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int count = 0;
    private int[,] map = new int[3, 3];

    private void Start()
    {
        reset();
    }

    void OnGUI()
    {
        int winner = Check();
        if (winner == 1)
        {
            GUI.Label(new Rect(300, 350, 90, 50), "X wins!");
        }
        else if (winner == 2)
        {
            GUI.Label(new Rect(300, 350, 90, 50), "O wins!");
        }
        else if (count == 9)
        {
            GUI.Label(new Rect(280, 350, 100, 50), "Dead Game! Please Restart!");
        }
        else
        {
            for (int i = 0; i < 3; i += 1)
            {
                for (int j = 0; j < 3; j += 1)
                {
                    if (map[i, j] == 0)
                    {
                        if (GUI.Button(new Rect(250 + i * 50, 250 + j * 50, 50, 50), ""))
                        {
                            if (count%2 == 0) map[i, j] = 1;
                            else map[i, j] = 2;
                            count = count + 1;
                        }
                    }
                    if (map[i, j] == 1)
                    {
                        GUI.Button(new Rect(250 + i * 50, 250 + j * 50, 50, 50), "X");
                    }
                    if (map[i, j] == 2)
                    {
                        GUI.Button(new Rect(250 + i * 50, 250 + j * 50, 50, 50), "O");
                    }
                }
            }
        }
        if (GUI.Button(new Rect(275, 400, 100, 50), "Restart"))
        {
            reset();
        }
    }

    private void reset()
    {
        for (int i = 0; i < 3; i += 1)
        {
            for (int j = 0; j < 3; j += 1)
            {
                map[i, j] = 0;
            }
        }
        count = 0;
    }

    private int Check()
    {
        //横向连线
        for (int i = 0; i < 3; ++i)
        {
            if (map[i, 0] != 0 && map[i, 0] == map[i, 1] && map[i, 1] == map[i, 2])
            {
                return map[i, 0];
            }
        }
        //纵向连线  
        for (int j = 0; j < 3; ++j)
        {
            if (map[0, j] != 0 && map[0, j] == map[1, j] && map[1, j] == map[2, j])
            {
                return map[0, j];
            }
        }
        //斜向连线  
        if (map[1, 1] != 0 &&map[0, 0] == map[1, 1] && map[1, 1] == map[2, 2] ||map[0, 2] == map[1, 1] && map[1, 1] == map[2, 0])
        {
            return map[1, 1];
        }
        return 0;
    }
}