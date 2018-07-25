using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class Stats
{
    public string user_id = Utils.GetCreateUid();
    public int playtime;

    public int num_turns;
    public int num_passes;
    public int num_dunks;
    public int num_pushes;

    public int num_cliffhangers;
    public int num_cliffhanged;

    public int num_wins;
    public int num_losses;

    public int avg_turns_game;
}


public static class Progression
{
    public static int level = 1;
    public static DateTime session_start = DateTime.Now;
    public static Stats session_stats = new Stats();
}