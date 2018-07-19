using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    Tile current_tile;

    void Start()
    {
        current_tile = transform.parent.GetComponent<Tile>();
        current_tile.has_cursor = true;

        GameObject.Find("Canvas").transform.Find("Command Window").GetComponent<CommandWindow>().highlight = gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown("up"))
        {
            MoveToTile(0);
        }
        else if (Input.GetKeyDown("down"))
        {
            MoveToTile(1);
        }
        else if (Input.GetKeyDown("right"))
        {
            MoveToTile(3);

        }
        else if ((Input.GetKeyDown("left")))
        {
            MoveToTile(2);
        }
        else if (Input.GetKeyDown("space"))
        {
            Confirm();
        }
        else if (Input.GetKeyDown("escape"))
        {
            FindObjectOfType<CommandWindow>().Cancel();
        }
    }

    public void InMenu()
    {
        gameObject.SetActive(false);
    }

    void MoveToTile(int ind)
    {
        if (current_tile.adjacent_tiles[ind] != null)
        {
            current_tile.has_cursor = false;
            current_tile.adjacent_tiles[ind].has_cursor = true;

            transform.SetParent(current_tile.adjacent_tiles[ind].transform, false);

            current_tile = current_tile.adjacent_tiles[ind];
        }
    }

    void Confirm()
    {
        current_tile.Confirm();
    }
}
