using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFocusPlayer : MonoBehaviour
{
    GameObject focus_player;

    public void SetFocusPlayer(GameObject focus_player)
    {
        this.focus_player = focus_player;
    }

	void Update ()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(focus_player.transform.position);
        transform.position = pos + new Vector3(550, 200, 0);
    }
}
