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


public static class Progression
{
    public static int level = 1;
    public static DateTime session_start = DateTime.Now;
    public static Stats session_stats = new Stats();
    public static string user_id = Utils.GetCreateUid();
}