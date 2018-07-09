using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{ 
    public static int GetDistance(Vector2 location_1, Vector2 location_2)
    {
        return (int)(Mathf.Abs(location_1.x - location_2.x) + Mathf.Abs(location_1.y - location_2.y));
    }

    public static Player FindPlayerOnTile(Tile tile)
    {
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.current_tile == tile)
            {
                return player;
            }
        }
        Debug.LogError("Couldn't find player for this tile, why are you asking?");
        return null;
    }
}
