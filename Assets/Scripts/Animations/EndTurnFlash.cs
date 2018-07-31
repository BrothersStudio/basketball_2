using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnFlash : MonoBehaviour
{
    bool flashing = false;

    bool growing = true;

    Player[] players;

    float flash_speed = 0.1f;

    public void CountPlayers()
    {
        flashing = false;
        players = FindObjectsOfType<Player>();
        GetComponent<Outline>().effectDistance = new Vector2(2, -2);
    }

	void Update ()
    {
		if (!flashing)
        {
            if (AllMoved())
            {
                flashing = true;
            }
        }
        else
        {
            Flash();
        }
	}

    bool AllMoved()
    {
        foreach (Player player in players)
        {
            if (player.team == Team.A)
            {
                if (!player.took_move)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void Flash()
    {
        if (growing && GetComponent<Outline>().effectDistance.x < 5)
        {
            GetComponent<Outline>().effectDistance = new Vector2(GetComponent<Outline>().effectDistance.x + flash_speed, GetComponent<Outline>().effectDistance.y - flash_speed);
        }
        else if (growing)
        {
            growing = false;
        }
        else if (!growing && GetComponent<Outline>().effectDistance.x > 1)
        {
            GetComponent<Outline>().effectDistance = new Vector2(GetComponent<Outline>().effectDistance.x - flash_speed, GetComponent<Outline>().effectDistance.y + flash_speed);
        }
        else if (!growing)
        {
            growing = true;
        }
    }
}
