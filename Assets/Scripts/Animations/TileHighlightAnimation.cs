using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlightAnimation : MonoBehaviour
{
    public Sprite tile_1;
    public Sprite tile_2;

    int current_tile = 0;

    int time_tile_change = 0;

    void FixedUpdate()
    {
        time_tile_change++;
        if (time_tile_change > 15)
        {
            time_tile_change = 0;
            SwitchSprite();
        }
    }

    void SwitchSprite()
    {
        if (current_tile == 0)
        {
            current_tile = 1;
            GetComponent<SpriteRenderer>().sprite = tile_2;
        }
        else
        {
            current_tile = 0;
            GetComponent<SpriteRenderer>().sprite = tile_1;
        }

        if (GetComponent<Tile>() != null)
        {
            GetComponent<SpriteRenderer>().color = GetComponent<Tile>().current_color;
        }
    }
}
