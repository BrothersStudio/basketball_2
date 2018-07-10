using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool highlighted = false;
    public Vector2 current_location;
    Player standing_player;

    Player querying_player;

    public void Highlight(Player querying_player)
    {
        highlighted = true;

        this.querying_player = querying_player;

        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void Dehighlight()
    {
        highlighted = false;
        querying_player = null;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnMouseDown()
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
        }
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
}
