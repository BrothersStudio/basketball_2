using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{ 
    public static int GetDistance(Vector2 location_1, Vector2 location_2)
    {
        return (int)(Mathf.Abs(location_1.x - location_2.x) + Mathf.Abs(location_1.y - location_2.y));
    }

    public static List<Player> ReturnAdjacentOpponents(Player input_player)
    {
        List<Player> adjacent_players = new List<Player>();
        foreach (Tile tile in input_player.current_tile.adjacent_tiles)
        {
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
}
