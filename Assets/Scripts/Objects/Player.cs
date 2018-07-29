using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool passing = false;
    public bool pushing = false;
    public bool moving = false;

    public bool animating = false;

    public bool took_attack = false;
    public bool took_move = false;

    public bool ai_pass_check = false;

    // Tutorial
    public bool can_select = true;

    // SFX
    public AudioClip push_sound;
    public AudioClip out_of_bounds_sound;
    public AudioClip pass_1_sound;
    public AudioClip pass_2_sound;
    public AudioClip pass_3_sound;
    public List<AudioClip> sneaker_sounds;

    // Sprites
    public SpriteFacing facing;
    public List<Sprite> player_sprites = new List<Sprite>();
    public List<Sprite> dunk_sprites = new List<Sprite>();

    // Particles
    public GameObject sweat_particle_prefab;

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

    public void SetSprites(Sprite[] input_sprites, Sprite[] input_dunk_sprites)
    {
        player_sprites.Add(input_sprites[0]);
        player_sprites.Add(input_sprites[1]);

        dunk_sprites.Add(input_dunk_sprites[0]);
        dunk_sprites.Add(input_dunk_sprites[1]);
        dunk_sprites.Add(input_dunk_sprites[2]);
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

    public void HoverPass()
    {
        // Got a little lazy sorry
        foreach (Tile tile in current_tile.adjacent_tiles)
        {
            if (tile != null)
            {
                tile.Hover();
                foreach (Tile next_tile in tile.adjacent_tiles)
                {
                    if (next_tile != null)
                    {
                        next_tile.Hover();
                        foreach (Tile next_next_tile in next_tile.adjacent_tiles)
                        {
                            if (next_next_tile != null)
                            {
                                next_next_tile.Hover();
                            }
                        }
                    }
                }
            }
        }
    }

    public bool CheckPass()
    {
        if (!took_attack && !passing && HasBall() && !animating)
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
        if (!took_attack && !animating)
        {
            passing = false;

            pass_player.BroadcastMessage("TutorialGotBall", SendMessageOptions.DontRequireReceiver);

            GetComponentInChildren<Ball>().Pass(pass_player);
            GetComponentInChildren<Ball>().transform.SetParent(pass_player.transform, true);
            pass_player.CheckIfScored();

            FindObjectOfType<CameraShake>().Shake(0.2f);
            PlayPassAudio();

            took_attack = true;
            EndAction();
        }
    }

    void PlayPassAudio()
    {
        switch (Possession.passes_this_turn)
        {
            case 0:
                GetComponent<AudioSource>().clip = pass_1_sound;
                break;
            case 1:
                GetComponent<AudioSource>().clip = pass_2_sound;
                break;
            default:
                GetComponent<AudioSource>().clip = pass_3_sound;
                break;
        }
        GetComponent<AudioSource>().Play();
        Possession.passes_this_turn++;
    }

    public void HoverPush()
    {
        foreach (Tile tile in current_tile.adjacent_tiles)
        {
            if (tile != null)
            {
                tile.Hover();
            }
        }
    }

    public bool CheckPush()
    {
        if (!took_attack && !pushing && !HasBall() && !animating)
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
                else
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
        Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
        return new_tile;
    }

    public Tile VisualizePushing(Tile other_tile)
    {
        Vector2 new_tile_coordinate = (other_tile.position - current_tile.position) + other_tile.position;
        Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
        return new_tile;
    }

    public Tile VisualizePushing(Tile potential_tile, Tile other_tile)
    {
        Vector2 new_tile_coordinate = (other_tile.position - potential_tile.position) + other_tile.position;
        Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
        return new_tile;
    }

    public void Push(Player other_player)
    {
        if (!took_attack && !animating)
        {
            pushing = false;

            Vector3 average_pos = (transform.position + other_player.transform.position) / 2f;
            Instantiate(sweat_particle_prefab, average_pos, Quaternion.identity);

            GetComponent<AudioSource>().clip = push_sound;
            GetComponent<AudioSource>().Play();

            FindObjectOfType<CameraShake>().Shake(0.3f);

            Vector2 new_tile_coordinate = (other_player.current_tile.position - current_tile.position) + other_player.current_tile.position;
            Tile new_tile = Utils.FindTileAtLocation(new_tile_coordinate);
            DetermineSpriteFacing(current_tile, other_player.current_tile);
            if (new_tile == null || new_tile.impassable)
            {
                if (new_tile == null)
                {
                    other_player.PushedToFall(this);
                }
                else
                {
                    other_player.PushedToFall(this, is_lava: true);
                }
                
                if (other_player.HasBall())
                {
                    return;
                }
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
            StartCoroutine(MoveToTile(new_tile, pushed: true));
        }
    }

    public void PushedToFall(Player pushing_player, bool is_lava = false)
    {
        if (is_lava)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.AddForce((transform.position - pushing_player.gameObject.transform.position) * 500);
            Invoke("GoUp", 0.1f);
        }
        else
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.AddForce((transform.position - pushing_player.gameObject.transform.position) * 1000);
        }

        GetComponent<AudioSource>().clip = out_of_bounds_sound;
        GetComponent<AudioSource>().Play();

        if (HasBall())
        {
            Invoke("DelayChange", 1f);
        }
        else
        {
            Invoke("RemoveFromStage", 1f);
        }
    }

    void GoUp()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 2000));
    }

    void RemoveFromStage()
    {
        current_tile.RemovePlayer();
        Destroy(gameObject);
    }

    public void HoverMove()
    {
        HashSet<Tile> tiles_to_walk = new HashSet<Tile>();
        tiles_to_walk.Add(current_tile);

        int distance_to_walk = move + (HasBall()? 1 : 0);
        for (int i = 0; i < distance_to_walk; i++)
        {
            Tile[] current_walk_tiles = new Tile[tiles_to_walk.Count];
            tiles_to_walk.CopyTo(current_walk_tiles);
            foreach (Tile tile in current_walk_tiles)
            {
                foreach (Tile adjacent_tile in tile.adjacent_tiles)
                {
                    // Does tile exist and is walkable
                    if (adjacent_tile == null) continue;
                    if (adjacent_tile.impassable) continue;

                    if (adjacent_tile.HasPlayer())
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

                tile.Hover();
            }
        }
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
                        // Does tile exist and is walkable
                        if (adjacent_tile == null) continue;
                        if (adjacent_tile.impassable) continue;

                        if (adjacent_tile.HasPlayer() && !ignore_other_players)
                        {
                            if (adjacent_tile.GetPlayer().team != this.team || adjacent_tile.impassable)
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
        }
    }

    public void Move(Tile new_tile)
    {
        // Tutorial
        if (GetComponent<TutorialPlayer>() != null)
        {
            if (!Utils.IsAdjacentToHoop(new_tile))
            {
                return;
            }
        }

        if (!took_move)
        {
            moving = false;

            StartCoroutine(MoveToTile(new_tile));
        }
    }

    IEnumerator MoveToTile(Tile new_tile, bool pushed = false)
    {
        animating = true;

        List<Tile> tile_route = new List<Tile>();
        if (!pushed)
        {
            Tile current_walking_tile = new_tile;
            while (current_walking_tile != current_tile)
            {
                tile_route.Add(current_walking_tile);
                current_walking_tile = current_walking_tile.previous_walk_tile;
            }
            tile_route.Reverse();
        }
        else
        {
            tile_route.Add(new_tile);
        }

        bool squeaked = false;
        int squeak_chance = 40;

        Tile previous_tile = current_tile;
        foreach (Tile next_tile in tile_route)
        {
            transform.SetParent(next_tile.transform, false);

            // Sometimes sneaker squeak
            if (Random.Range(0, squeak_chance) < 50 && !squeaked)
            {
                squeaked = true;

                GetComponent<AudioSource>().pitch = Random.Range(0.95f, 1.05f);
                GetComponent<AudioSource>().clip = sneaker_sounds[Random.Range(0, sneaker_sounds.Count)];
                GetComponent<AudioSource>().Play();
            }
            else
            {
                squeak_chance += 40;
            }

            if (!pushed)
            {
                DetermineSpriteFacing(previous_tile, next_tile);
            }
            yield return new WaitForSeconds(0.2f);
            previous_tile = next_tile;
        }

        current_tile.RemovePlayer();
        new_tile.SetPlayer(this);
        current_tile = new_tile;

        if (HasBall())
        {
            bool scored = CheckIfScored();
            if (scored)
            {
                yield break;
            }
        }

        animating = false;
        if (!pushed)
        {
            took_move = true;
            EndAction();
        }
    }

    public void DetermineSpriteFacing(Tile previous_tile, Tile next_tile)
    {
        Vector2 difference = next_tile.position - previous_tile.position;
        difference.Normalize();
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
            Debug.LogError("No matching facing direction");
        }

        if (HasBall())
        {
            GetComponentInChildren<Ball>().FixPositionFacing(this);
        }
    }

    bool CheckIfScored()
    {
        Hoop hoop = FindObjectOfType<Hoop>();
        if (Utils.GetDistance(current_tile.position, hoop.current_tile.position) <= 1)
        {
            if (GetComponent<TutorialPlayer>() == null)
            {
                FindObjectOfType<ScoreCounter>().PlayerScored(this);
            }
            FindObjectOfType<DunkBannerMove>().Dunk(this);

            canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
            GetComponentInChildren<Ball>().gameObject.SetActive(false);
            StopAllCoroutines();

            Utils.SetDunkAnimation();

            Invoke("DelayChange", 3.5f);
            return true;
        }
        else
        {
            return false;
        }
    }

    void EndAction()
    {
        // Tutorial
        if (Progression.level == 0)
        {
            return;
        }

        SetInactive();
        CheckTurn();
        if (took_attack && took_move || (took_move && !(CheckPass() || CheckPush())))
        {
            SetInactive();
            canvas.transform.Find("Command Window").GetComponent<CommandWindow>().Cancel();
        }
        else 
        {
            SetInactive();
            Confirm();
        }
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

        GetComponent<Bounce>().stop = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool IsTutorialRestricted(TutorialAction action)
    {
        if (GetComponent<TutorialPlayer>() != null)
        {
            if (action == TutorialAction.Move)
            {
                return !GetComponent<TutorialPlayer>().can_move;
            }
            else if (action == TutorialAction.Pass)
            {
                return !GetComponent<TutorialPlayer>().can_pass;
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

    public void Confirm()
    {
        if (!(took_move && took_attack) && (team != Team.B) && (!AITurn.Activity) && !animating && can_select)
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

public enum TutorialAction
{
    Move,
    Pass
}