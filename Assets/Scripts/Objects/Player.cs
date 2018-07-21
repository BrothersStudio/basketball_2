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

    public bool ai_pass_check = false;

    // Sprites
    public SpriteFacing facing;
    public List<Sprite> player_sprites;

    // Stats
    int move = 2;

    // Team Info
    public Team team;

    public Tile current_tile;
    public List<Tile> highlighted_tiles = new List<Tile>();

    GameObject canvas;

    void Start ()
    {
        canvas = GameObject.Find("Canvas");

        current_tile.SetPlayer(this);
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

    public bool CheckPass()
    {
        if ((!took_attack && !passing) && HasBall())
        {
            passing = true;

            Player[] players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                if (player != this && player.team == this.team &&
                    Utils.GetDistance(player.current_tile.position, current_tile.position) <= 3)
                {
                    player.current_tile.Highlight(this);
                    highlighted_tiles.Add(player.current_tile);
                }
            }

            if (highlighted_tiles.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void Pass(Player pass_player)
    {
        if (!took_attack)
        {
            passing = false;

            GetComponentInChildren<Ball>().Pass(pass_player);
            transform.Find("Ball").SetParent(pass_player.transform, true);

            took_attack = true;
            EndAction();
        }
    }

    public bool CheckPush()
    {
        if (!took_attack && !pushing && !HasBall())
        {
            pushing = true;

            foreach (Player player in Utils.ReturnAdjacentOpponents(this))
            {
                // Don't light up tiles when there's a person on the push destination tile, making the push impossible
                Vector2 new_tile_coordinate = (player.current_tile.position - current_tile.position) + player.current_tile.position;
                Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
                if (new_tile != null)
                {
                    if (!new_tile.HasPlayer())
                    {
                        player.current_tile.Highlight(this);
                        highlighted_tiles.Add(player.current_tile);
                    }
                }
                else if (new_tile == null && player.HasBall())
                {
                    // On edge
                    player.current_tile.Highlight(this);
                    highlighted_tiles.Add(player.current_tile);
                }
            }

            if (highlighted_tiles.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool OnEdge()
    {
        return current_tile.OnEdge();
    }

    public Tile VisualizePushing(Player other_player)
    {
        Vector2 new_tile_coordinate = (other_player.current_tile.position - current_tile.position) + other_player.current_tile.position;
        return Utils.FindTileAtLocation(new_tile_coordinate);
    }

    public Tile VisualizePushing(Tile other_tile)
    {
        Vector2 new_tile_coordinate = (other_tile.position - current_tile.position) + other_tile.position;
        return Utils.FindTileAtLocation(new_tile_coordinate);
    }

    public void Push(Player other_player)
    {
        if (!took_attack)
        {
            pushing = false;

            Vector2 new_tile_coordinate = (other_player.current_tile.position - current_tile.position) + other_player.current_tile.position;
            Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
            if (new_tile == null)
            {
                other_player.PushedToFall(this);
                return;
            }
            else
            {
                other_player.PushedTo(new_tile);
            }

            took_attack = true;
            EndAction();
        }
    }

    public void PushedTo(Tile new_tile)
    {
        if (new_tile != null)
        {
            StartCoroutine(MoveToTile(new_tile));
        }
    }

    public void PushedToFall(Player pushing_player)
    {
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.AddForce((transform.position - pushing_player.gameObject.transform.position) * 1000);
        Invoke("DelayChange", 1f);
    }

    public void CheckMove(bool checking = false, bool ignore_other_players = false)
    {
        if (!took_move && !moving)
        {
            moving = true;

            HashSet<Tile> tiles_to_walk = new HashSet<Tile>();
            tiles_to_walk.Add(current_tile);

            int distance_to_walk = move + (HasBall() || checking ? 1 : 0);
            for (int i = 0; i < distance_to_walk; i++)
            {
                Tile[] current_walk_tiles = new Tile[tiles_to_walk.Count];
                tiles_to_walk.CopyTo(current_walk_tiles);
                foreach (Tile tile in current_walk_tiles)
                {
                    foreach (Tile adjacent_tile in tile.adjacent_tiles)
                    {
                        if (adjacent_tile == null) continue;

                        if (adjacent_tile.HasPlayer() && !ignore_other_players)
                        {
                            if (adjacent_tile.GetPlayer().team != this.team)
                            {
                                continue;
                            }
                        }
                        
                        // If it's not already registered, figure out the tile that came before for walking purposes
                        if (!tiles_to_walk.Contains(adjacent_tile))
                        {
                            adjacent_tile.previous_walk_tile = tile;
                        }
                        tiles_to_walk.Add(adjacent_tile);
                    }
                }
            }

            foreach (Tile tile in tiles_to_walk)
            {
                if (!tile.HasPlayer())
                {
                    // If on defense, can't stand next to hoop
                    if (this.team != Possession.team && Utils.IsAdjacentToHoop(tile)) continue;
                    
                    tile.Highlight(this);
                    highlighted_tiles.Add(tile);
                }
                else if (ignore_other_players)
                {
                    tile.Highlight(this);
                    highlighted_tiles.Add(tile);
                }
            }

            // This makes AI stuff a little easier later. Adding current tile.
            current_tile.Highlight(this);
            highlighted_tiles.Add(current_tile);
        }
    }

    public void Move(Tile new_tile)
    {
        if (!took_move)
        {
            moving = false;

            StartCoroutine(MoveToTile(new_tile));

            took_move = true;
            EndAction();
        }
    }

    IEnumerator MoveToTile(Tile new_tile)
    {
        Tile current_walking_tile = new_tile;
        List<Tile> tile_route = new List<Tile>();
        while (current_walking_tile != current_tile)
        {
            tile_route.Add(current_walking_tile);
            current_walking_tile = current_walking_tile.previous_walk_tile;
        }
        tile_route.Reverse();

        Tile previous_tile = current_tile;
        foreach (Tile next_tile in tile_route)
        {
            transform.SetParent(next_tile.transform, false);
            DetermineSpriteFacing(previous_tile, next_tile);
            yield return new WaitForSeconds(0.2f);
            previous_tile = next_tile;
        }

        current_tile.RemovePlayer();
        new_tile.SetPlayer(this);
        current_tile = new_tile;

        CheckIfScored();
    }

    public void DetermineSpriteFacing(Tile previous_tile, Tile next_tile)
    {
        Vector2 difference = next_tile.position - previous_tile.position;
        difference.Normalize();
        Debug.Log(gameObject.name);
        Debug.Log(difference);

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            difference = new Vector2(Mathf.RoundToInt(difference.x), 0);
        }
        else
        {
            difference = new Vector2(0, Mathf.RoundToInt(difference.y));
        }

        if (difference == new Vector2(1, 0))
        {
            GetComponent<SpriteRenderer>().sprite = player_sprites[1];
            GetComponent<SpriteRenderer>().flipX = true;
            facing = SpriteFacing.NW;
        }
        else if (difference == new Vector2(0, 1))
        {
            GetComponent<SpriteRenderer>().sprite = player_sprites[0];
            GetComponent<SpriteRenderer>().flipX = true;
            facing = SpriteFacing.SW;
        }
        else if (difference == new Vector2(-1, 0))
        {
            GetComponent<SpriteRenderer>().sprite = player_sprites[0];
            GetComponent<SpriteRenderer>().flipX = false;
            facing = SpriteFacing.SE;
        }
        else if (difference == new Vector2(0, -1))
        {
            GetComponent<SpriteRenderer>().sprite = player_sprites[1];
            GetComponent<SpriteRenderer>().flipX = false;
            facing = SpriteFacing.NE;
        }
        else
        {
            Debug.LogWarning("No matching facing direction");
        }

        if (HasBall())
        {
            GetComponentInChildren<Ball>().FixPositionFacing(this);
        }
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

    void EndAction()
    {
        SetInactive();
        CheckTurn();
        canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
    }

    public void SetInactive()
    {
        passing = false;
        pushing = false;
        moving = false;

        highlighted_tiles.Clear();
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
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    public void Confirm()
    {
        if (!(took_move && took_attack) && (team != Team.B) && (!AITurn.Activity))
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

public enum SpriteFacing
{
    NW,
    SW,
    NE,
    SE
}