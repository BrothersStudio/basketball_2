using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool has_ball;

    // Stats
    public int move_speed = 1;

    public Tile current_tile;
    List<Tile> field_tiles = new List<Tile>();
    GameObject canvas;

	void Start ()
    {
        canvas = GameObject.Find("Canvas");

        Tile[] tile_array = FindObjectsOfType<Tile>();
        foreach (Tile tile in tile_array)
        {
            field_tiles.Add(tile);
        }
	}

    public bool CanShoot()
    {
        if (has_ball)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Shoot()
    {
        GetComponentInChildren<Ball>().Shoot();
        has_ball = false;
    }

    public void Pass()
    {
        has_ball = false;
    }

    public void CheckMove()
    {
        foreach(Tile tile in field_tiles)
        {
            if (Utils.GetDistance(tile.current_location, current_tile.current_location) <= move_speed)
            {
                tile.Highlight(this);
            }
        }
    }

    public void Move(Tile new_tile)
    {
        foreach (Tile tile in field_tiles)
        {
            tile.Dehighlight();
        }

        transform.SetParent(new_tile.transform, false);
        current_tile = new_tile;
    }

    void OnMouseDown()
    {
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().SetButtons(this);
    }

    void Update ()
    {
		
	}
}
