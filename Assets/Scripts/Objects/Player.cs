using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool passing = false;
    public bool moving = false;

    // Stats
    public int shoot_skill;
    public int block_skill;
    public int move_skill;

    // Team Info
    public Team team;
    public Player defender;

    public Tile current_tile;
    List<Tile> field_tiles = new List<Tile>();
    GameObject canvas;

	void Start ()
    {
        canvas = GameObject.Find("Canvas");

        current_tile.SetPlayer(this);

        Tile[] tile_array = FindObjectsOfType<Tile>();
        foreach (Tile tile in tile_array)
        {
            field_tiles.Add(tile);
        }

        team = Team.A;
	}

    public bool HasBall()
    {
        if (transform.childCount > 0) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckShoot()
    {
        Hoop hoop = FindObjectOfType<Hoop>();

        int enemy_block = 0;
        if (defender != null)
        {
            enemy_block = defender.block_skill;
        }

        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().SetShot(shoot_skill, enemy_block, Utils.GetDistance(current_tile.current_location, hoop.current_tile.current_location));
    }

    public void Shoot()
    {
        GetComponentInChildren<Ball>().Shoot();
        //transform.Find("Ball").SetParent(pass_player.transform, true);
    }

    public void CheckPass()
    {
        passing = true;

        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player != this)
            {
                // TODO: if on team
                player.current_tile.Highlight(this);
            }
        }
    }

    public void Pass(Player pass_player)
    {
        passing = false;
        DehighlightTiles();

        GetComponentInChildren<Ball>().Pass(pass_player);
        transform.Find("Ball").SetParent(pass_player.transform, true);

        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void CheckMove()
    {
        moving = true;

        foreach(Tile tile in field_tiles)
        {
            if (Utils.GetDistance(tile.current_location, current_tile.current_location) <= move_skill &&
                !tile.HasPlayer())
            {
                tile.Highlight(this);
            }
        }
    }

    public void Move(Tile new_tile)
    {
        moving = false;
        DehighlightTiles();

        transform.SetParent(new_tile.transform, false);

        current_tile.RemovePlayer();
        new_tile.SetPlayer(this);
        current_tile = new_tile;

        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    void OnMouseDown()
    {
        if (current_tile.highlighted)
        {
            current_tile.OnMouseDown();
        }
        else
        { 
            canvas.transform.Find("Command Window").GetComponent<CommandWindow>().SetButtons(this);
        }
    }

    void DehighlightTiles()
    {
        foreach (Tile tile in field_tiles)
        {
            tile.Dehighlight();
        }
    }
}

public enum Team
{
    A,
    B
}
