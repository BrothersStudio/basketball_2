using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    bool highlighted = false;
    public Vector2 current_location;
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

    private void OnMouseDown()
    {
        if (highlighted)
        {
            querying_player.Move(this);
        }
    }
}
