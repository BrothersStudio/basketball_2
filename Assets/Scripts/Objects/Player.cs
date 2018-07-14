using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool passing = false;
    public bool pushing = false;
    public bool moving = false;

    public bool took_attack = false;
    public bool took_move = false;

    // Stats
    int move = 2;

    // Team Info
    public Team team;

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

    public void CheckPush()
    {
        pushing = true;

        foreach (Player player in Utils.ReturnAdjacentOpponents(this))
        {
            player.current_tile.Highlight(this);
        }
    }

    public void Push(Player other_player)
    {
        pushing = false;
        Utils.DehighlightTiles();

        Vector2 new_tile_coordinate = (other_player.current_tile.position - current_tile.position) + other_player.current_tile.position;
        Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
        other_player.PushedTo(new_tile);

        took_attack = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void PushedTo(Tile new_tile)
    {
        if (new_tile != null)
        {
            MoveToTile(new_tile);
        }
    }

    public void CheckMove()
    {
        moving = true;

        foreach(Tile tile in field_tiles)
        {
            if (Utils.GetDistance(tile.position, current_tile.position) <= move + (HasBall() ? 1 : 0) &&
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

        MoveToTile(new_tile);

        took_move = true;
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    void MoveToTile(Tile new_tile)
    {
        transform.SetParent(new_tile.transform, false);

        current_tile.RemovePlayer();
        new_tile.SetPlayer(this);
        current_tile = new_tile;

        CheckIfScored();
    }

    void CheckIfScored()
    {
        Hoop hoop = FindObjectOfType<Hoop>();
        if (Utils.GetDistance(current_tile.position, hoop.current_tile.position) <= 1)
        {
            FindObjectOfType<ScoreCounter>();
            Invoke("DelayChange", 0.5f);
        }
    }

    public void SetInactive()
    {
        passing = false;
        pushing = false;
        moving = false;

        Utils.DehighlightTiles();
    }

    void DelayChange()
    {
        FindObjectOfType<PhaseController>().ChangeSides();
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
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;
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
