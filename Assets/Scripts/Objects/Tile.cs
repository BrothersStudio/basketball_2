using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool highlighted = false;
    public Vector2 position;
    Player standing_player = null;

    Player querying_player;

    public List<Tile> adjacent_tiles = new List<Tile>();

    public void SetAdjacency()
    {
        Vector2 position_1 = new Vector2(position.x + 1, position.y);
        Tile pos_1_tile = Utils.FindTileAtLocation(position_1);
        if (pos_1_tile != null)
        {
            adjacent_tiles.Add(pos_1_tile);
        }

        Vector2 position_2 = new Vector2(position.x - 1, position.y);
        Tile pos_2_tile = Utils.FindTileAtLocation(position_2);
        if (pos_2_tile != null)
        {
            adjacent_tiles.Add(pos_2_tile);
        }

        Vector2 position_3 = new Vector2(position.x, position.y + 1);
        Tile pos_3_tile = Utils.FindTileAtLocation(position_3);
        if (pos_3_tile != null)
        {
            adjacent_tiles.Add(pos_3_tile);
        } 

        Vector2 position_4 = new Vector2(position.x, position.y - 1);
        Tile pos_4_tile = Utils.FindTileAtLocation(position_4);
        if (pos_4_tile != null)
        {
            adjacent_tiles.Add(pos_4_tile);
        }
    }

    public void Highlight(Player querying_player)
    {
        highlighted = true;

        this.querying_player = querying_player;

        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void Dehighlight()
    {
        highlighted = false;
        querying_player = null;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnMouseDown()
    {
        if (highlighted)
        {
            if (querying_player.moving)
            {
                querying_player.Move(this);
            }
            else if (querying_player.passing)
            {
                querying_player.Pass(standing_player);
            }
            else if (querying_player.pushing)
            {
                querying_player.Push(standing_player);
            }
        }
    }

    public Player GetPlayer()
    {
        return standing_player;
    }

    public void SetPlayer(Player player)
    {
        standing_player = player;
    }

    public void RemovePlayer()
    {
        standing_player = null;
    }

    public bool HasPlayer()
    {
        if (standing_player != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
