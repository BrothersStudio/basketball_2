using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


[Serializable]
public class Stats
{
    public int playtime;

    public int num_turns = 0;
    public int num_passes = 0;
    public int num_dunks = 0;
    public int num_pushe = 0;

    public int num_cliffhangers = 0;
    public int num_cliffhanged = 0;

    public int num_wins = 0;
    public int num_losses = 0;

    public int avg_turns_game = 0;
}

public class Progression : MonoBehaviour
{
    public static int level = 3;

    public static bool game_1_victory = false;
    public static bool game_2_victory = false;
    public static bool game_3_victory = false;

    public static DateTime session_start = DateTime.Now;
    public static Stats session_stats = new Stats();
    public static string user_id = Utils.GetCreateUid();

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public static void GameResults(bool win)
    {
        switch (level)
        {
            case 1:
                game_1_victory = win;
                break;
            case 2:
                game_2_victory = win;
                break;
            case 3:
                game_3_victory = win;
                break;
        }
    }
}