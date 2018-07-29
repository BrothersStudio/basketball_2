using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    public bool can_move = false;
    public bool can_pass = false;

    public void TutorialGotBall()
    {
        GetComponent<Player>().can_select = true;
        FindObjectOfType<CommandWindow>().Cancel();
    }
}
