using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool passing = false;
    public bool moving = false;

    public bool took_attack = false;
    public bool took_move = false;

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
        //GetComponentInChildren<Ball>().Shoot();
        ShootRebound();
        //transform.Find("Ball").SetParent(pass_player.transform, true);

        took_attack = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void ShootRebound()
    {
        Hoop hoop = FindObjectOfType<Hoop>();

        Tile rebound_tile = null;
        while (rebound_tile == null)
        {
            int x = (int)hoop.current_tile.current_location.x + Random.Range(-2, 2);
            int y = (int)hoop.current_tile.current_location.y + Random.Range(-2, 2);
            if (x == 0 && y == 0) continue;

            Vector2 try_location = new Vector2(x, y);
            rebound_tile = Utils.FindTileAtLocation(try_location);
        }
        GetComponentInChildren<Ball>().ShootRebound(rebound_tile);
        transform.Find("Ball").SetParent(rebound_tile.transform, true);

        took_attack = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void CheckPass()
    {
        passing = true;

        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player != this && player.team == this.team)
            {
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

        took_attack = true;
        CheckTurn();
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

        if (new_tile.has_ball)
        {
            Ball ball = FindObjectOfType<Ball>();

            ball.transform.SetParent(transform);
            FindObjectOfType<Ball>().SetCaught();
        }

        took_move = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    void CheckTurn()
    {
        if (took_attack && took_move)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;

            if (Utils.TeamIsDoneTurn(team))
            {
                FindObjectOfType<PhaseController>().ChangePhase();
            }
        }
    }

    public bool IsDone()
    {
        if (took_attack && took_move)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RefreshTurn()
    {
        took_attack = false;
        took_move = false;

        if (team == Team.A)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void OnMouseDown()
    {
        if (current_tile.highlighted)
        {
            current_tile.OnMouseDown();
        }
        else if (!(took_move && took_attack))
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
