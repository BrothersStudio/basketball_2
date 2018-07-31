using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool impassable;

    public bool has_cursor = false;

    public bool highlighted = false;
    public Vector2 position;
    Player standing_player = null;

    Player querying_player = null;

    public Tile previous_walk_tile = null;

    public List<Tile> adjacent_tiles = new List<Tile>();

    public Color current_color = Color.white;

    public void SetAdjacency()
    {
        Vector2 position_1 = new Vector2(position.x + 1, position.y);
        Tile pos_1_tile = Utils.FindTileAtLocation(position_1);
        adjacent_tiles.Add(pos_1_tile);

        Vector2 position_2 = new Vector2(position.x - 1, position.y);
        Tile pos_2_tile = Utils.FindTileAtLocation(position_2);
        adjacent_tiles.Add(pos_2_tile);

        Vector2 position_3 = new Vector2(position.x, position.y + 1);
        Tile pos_3_tile = Utils.FindTileAtLocation(position_3);
        adjacent_tiles.Add(pos_3_tile);

        Vector2 position_4 = new Vector2(position.x, position.y - 1);
        Tile pos_4_tile = Utils.FindTileAtLocation(position_4);
        adjacent_tiles.Add(pos_4_tile);

        if (Utils.IsAdjacentToHoop(this))
        {
            GetComponent<TileHighlightAnimation>().enabled = true;
        }
    }

    public void Hover()
    {
        current_color = Color.green;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void Highlight(Player querying_player)
    {
        highlighted = true;

        this.querying_player = querying_player;

        current_color = Color.green;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void Dehighlight()
    {
        highlighted = false;
        previous_walk_tile = null;
        querying_player = null;

        current_color = Color.white;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Confirm()
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
        else if (HasPlayer())
        {
            standing_player.Confirm();
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

    public bool OnEdge()
    {
        foreach (Tile adjacent_tile in adjacent_tiles)
        {
            if (adjacent_tile == null)
            {
                return true;
            }
            else if (adjacent_tile.impassable)
            {
                return true;
            }
        }

        return false;
    }

    public Tile VisualizePushingOtherFromHere(Tile other_tile)
    {
        Vector2 new_tile_coordinate = (other_tile.position - position) + other_tile.position;
        return Utils.FindTileAtLocation(new_tile_coordinate);
    }
}
