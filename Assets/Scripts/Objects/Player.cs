using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool offense;

    public bool passing = false;
    public bool moving = false;

    public bool took_attack = false;
    public bool took_move = false;

    public bool used_juke = false;
    int temp_speed = 0;

    int temp_block = 0;
    int temp_steal = 0;

    // Stats
    int shoot_skill = 2;
    int control_skill = 2;
    int block_skill = 2;
    int steal_skill = 2;
    int move_skill = 2;

    // Team Info
    public Team team;
    public Player defender;

    public Tile current_tile;
    List<Tile> field_tiles = new List<Tile>();
    GameObject canvas;

    public int Block_Skill
    {
        get
        {
            return block_skill + temp_block;
        }
    }

    public int Steal_Skill
    {
        get
        {
            return steal_skill + temp_steal;
        }
    }

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
        int enemy_block = 0;
        if (defender != null)
        {
            enemy_block = defender.Block_Skill;
        }

        Hoop hoop = FindObjectOfType<Hoop>();
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
        Utils.DehighlightTiles();

        GetComponentInChildren<Ball>().Pass(pass_player);
        transform.Find("Ball").SetParent(pass_player.transform, true);

        took_attack = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void Block()
    {
        temp_block += 1;

        took_attack = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void Steal()
    {
        temp_steal += 1;

        took_attack = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void CheckMove()
    {
        moving = true;

        int current_move_skill = move_skill + temp_speed;
        if (Utils.ReturnAdjacentOpponent(this) != null)
        {
            current_move_skill--;
        }

        foreach(Tile tile in field_tiles)
        {
            if (Utils.GetDistance(tile.current_location, current_tile.current_location) <= current_move_skill &&
                !tile.HasPlayer())
            {
                tile.Highlight(this);
            }
        }
    }

    public void Move(Tile new_tile)
    {
        moving = false;
        Utils.DehighlightTiles();

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

    public void CheckJuke()
    {
        int enemy_steal = 0;
        if (defender != null)
        {
            enemy_steal = defender.Steal_Skill;
        }
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().SetJuke(control_skill, enemy_steal);
    }

    public void SuccessfulJuke()
    {
        used_juke = true;
        temp_speed = 2;
        CheckMove();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void FailJuke()
    {
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
        used_juke = false;

        temp_block = 0;
        temp_steal = 0;
        temp_speed = 0;

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
}

public enum Team
{
    A,
    B
}
