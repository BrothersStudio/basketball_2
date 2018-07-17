using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{ 
    public static int GetDistance(Vector2 location_1, Vector2 location_2)
    {
        return (int)(Mathf.Abs(location_1.x - location_2.x) + Mathf.Abs(location_1.y - location_2.y));
    }

    public static int GetDistanceFromAToBForTeam(Tile tile_1, Tile tile_2, Team team)
    {
        HashSet<Tile> tiles_to_walk = new HashSet<Tile>();
        tiles_to_walk.Add(tile_1);

        int counter = 0;
        while (true)
        {
            counter++;

            Tile[] current_walk_tiles = new Tile[tiles_to_walk.Count];
            tiles_to_walk.CopyTo(current_walk_tiles);
            foreach (Tile tile in current_walk_tiles)
            {
                foreach (Tile adjacent_tile in tile.adjacent_tiles)
                {
                    if (adjacent_tile.HasPlayer())
                    {
                        if (adjacent_tile.GetPlayer().team != team)
                        {
                            continue;
                        }
                    }

                    if (adjacent_tile == tile_2)
                    {
                        return counter;
                    }
                    tiles_to_walk.Add(adjacent_tile);
                }
            }
        }
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

    public static void ResetPassChecks()
    {
        foreach (Player player in GameObject.FindObjectsOfType<Player>())
        {
            player.ai_pass_check = false;
        }
    }
}
