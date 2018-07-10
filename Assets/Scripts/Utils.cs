using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{ 
    public static int GetDistance(Vector2 location_1, Vector2 location_2)
    {
        return (int)(Mathf.Abs(location_1.x - location_2.x) + Mathf.Abs(location_1.y - location_2.y));
    }

    public static Tile FindTileAtLocation(Vector2 location)
    {
        Tile[] all_tiles = GameObject.FindObjectsOfType<Tile>();
        foreach (Tile tile in all_tiles)
        {
            if (tile.current_location == location)
            {
                return tile;
            }
        }
        return null;
    }

    public static float GetShotChanceAtDistance(int distance)
    {
        if (distance <= 1)
        {
            return 65f;
        }
        else if (distance <= 2)
        {
            return 50f;
        }
        else if (distance <= 3)
        {
            return 45f;
        }
        else if (distance <= 4)
        {
            return 40f;
        }
        else if (distance <= 5)
        {
            return 35f;
        }
        else if (distance <= 6)
        {
            return 30f;
        }
        else if (distance <= 7)
        {
            return 25f;
        }
        else if (distance <= 8)
        {
            return 20f;
        }
        else if (distance <= 9)
        {
            return 15f;
        }
        else if (distance <= 10)
        {
            return 10f;
        }
        else
        {
            return 5f;
        }
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
}
