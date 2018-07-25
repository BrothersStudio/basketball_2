using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Utils
{ 
    public static int GetDistance(Vector2 location_1, Vector2 location_2)
    {
        return (int)(Mathf.Abs(location_1.x - location_2.x) + Mathf.Abs(location_1.y - location_2.y));
    }

    public static bool IsAdjacentToHoop(Tile tile)
    {
        if (GetDistance(GameObject.FindObjectOfType<Hoop>().current_tile.position, tile.position) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static List<Player> ReturnAdjacentOpponents(Player input_player)
    {
        List<Player> adjacent_players = new List<Player>();
        foreach (Tile tile in input_player.current_tile.adjacent_tiles)
        {
            if (tile == null) continue;

            Player player = tile.GetPlayer();
            if (player != null)
            {
                if (player.team != input_player.team)
                {
                    adjacent_players.Add(player);
                }
            }
        }
        return adjacent_players;
    }

    public static Tile FindTileAtLocation(Vector2 location)
    {
        Tile[] all_tiles = GameObject.FindObjectsOfType<Tile>();
        foreach (Tile tile in all_tiles)
        {
            if (tile.position == location)
            {
                return tile;
            }
        }
        return null;
    }

    public static bool TeamIsDoneTurn(Team team)
    {
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.team == team && !player.IsDone())
            {
                return false;
            }
        }
        return true;
    }

    public static void DehighlightTiles()
    {
        foreach (Tile tile in GameObject.FindObjectsOfType<Tile>())
        {
            tile.Dehighlight();
        }
    }

    public static void DeactivatePlayers()
    {
        foreach (Player player in GameObject.FindObjectsOfType<Player>())
        {
            player.StopAllCoroutines();

            player.took_attack = true;
            player.took_move = true;
        }
    }

    public static string GetCreateUid()
    {
        string uid = "";

#if UNITY_STANDALONE_WIN
        string id_directory = string.Format("{0}\\AppData\\Roaming\\BrothersStudio\\", Environment.GetEnvironmentVariable("HOMEPATH"));
#endif
#if UNITY_STANDALONE_OSX
        string id_directory = string.Format("{0}/Library/com.brothersstudio.tps/", Environment.GetEnvironmentVariable("HOME"));
#endif
        string id_file = string.Format("{0}tps_id.blob", id_directory);
        if (!File.Exists(id_file))
        {
            Directory.CreateDirectory(id_directory);
            StreamWriter cache = new StreamWriter(id_file);
            System.Random rand = new System.Random();
            uid = rand.Next(1000000000).ToString("D10");
            cache.Write(uid);
            cache.Close();
        }
        else if (File.Exists(id_file))
        {
            var reader = new StreamReader(id_file);
            uid = reader.ReadToEnd();
        }

        Debug.Log(string.Format("Got uid: {0}", uid));
        return uid;
    }
}
