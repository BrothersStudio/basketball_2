using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    Tile current_tile;

    int cycle_ind;
    Player cycle_player = null;

    float current_buffer = 0;
    float end_turn_buffer = 0.4f;
    GameObject end_turn_window;

    List<GameObject> faded_players = new List<GameObject>();

    void Awake()
    {
        current_tile = transform.parent.GetComponent<Tile>();
        current_tile.has_cursor = true;

        GameObject.Find("Canvas").transform.Find("Command Window").GetComponent<CommandWindow>().highlight = this;

        end_turn_window = GameObject.Find("Canvas").transform.Find("End Turn Fade").gameObject;
        GameObject.Find("Canvas").transform.Find("End Turn Fade/End Turn Window").GetComponent<EndTurnWindow>().highlight = this;

        GameObject.Find("Game Controller").GetComponent<PhaseController>().highlight = gameObject;
    }

    void OnEnable()
    {
        current_buffer = Time.timeSinceLevelLoad;
    }

    public void Reset()
    {
        cycle_player = null;

        gameObject.SetActive(true);
    }

    public void SelectMove()
    {
        gameObject.SetActive(true);
    }

    public void SelectCycleTarget(Player player)
    {
        cycle_ind = 0;
        cycle_player = player;
        Cycle(true);

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (cycle_player == null)
        {
            if (Input.GetKeyDown("up"))
            {
                MoveToAdjacentTile(0);
            }
            else if (Input.GetKeyDown("down"))
            {
                MoveToAdjacentTile(1);
            }
            else if (Input.GetKeyDown("right"))
            {
                MoveToAdjacentTile(3);

            }
            else if ((Input.GetKeyDown("left")))
            {
                MoveToAdjacentTile(2);
            }
        }
        else
        {
            if (Input.GetKeyDown("right"))
            {
                Cycle(forward: true);

            }
            else if ((Input.GetKeyDown("left")))
            {
                Cycle(forward: false);
            }
        }


        if (Input.GetKeyDown("space") || Input.GetKeyDown("return"))
        {
            Confirm();
        }
        else if (Input.GetKey("escape") && Time.timeSinceLevelLoad > end_turn_buffer + current_buffer)
        {
            end_turn_window.SetActive(true);
        }
    }

    public void InMenu()
    {
        gameObject.SetActive(false);
    }

    void MoveToAdjacentTile(int ind)
    {
        if (current_tile.adjacent_tiles[ind] != null)
        {
            MoveToTile(current_tile.adjacent_tiles[ind]);
        }
    }

    void MoveToTile(Tile new_tile)
    {
        current_tile.has_cursor = false;
        new_tile.has_cursor = true;

        transform.SetParent(new_tile.transform, false);

        current_tile = new_tile;

        FadeAdjacentPlayersTo(current_tile);
    }

    void Cycle(bool forward)
    {
        cycle_ind++;
        if (cycle_ind == cycle_player.highlighted_tiles.Count)
        {
            cycle_ind = 0;
        }

        MoveToTile(cycle_player.highlighted_tiles[cycle_ind]);
    }

    void ResetFadedPlayers()
    {
        foreach (GameObject player in faded_players)
        {
            Unfade(player);
        }
        faded_players.Clear();
    }

    void Unfade(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        Color current_color = sprite.color;
        current_color.a = 1;
        sprite.color = current_color;
    }

    void FadeAdjacentPlayersTo(Tile tile)
    {
        ResetFadedPlayers();
        foreach (Tile adjacent_tile in tile.adjacent_tiles)
        {
            if (adjacent_tile != null)
            {
                Player player = adjacent_tile.GetPlayer();
                if (player != null)
                {
                    if (!player.moving)
                    {
                        SpriteRenderer sprite_renderer = player.gameObject.GetComponent<SpriteRenderer>();
                        Color current_color = sprite_renderer.color;
                        current_color.a = 0.5f;
                        sprite_renderer.color = current_color;
                        faded_players.Add(sprite_renderer.gameObject);
                    }
                }
            }
        }
    }

    void Confirm()
    {
        current_tile.Confirm();
    }
}
