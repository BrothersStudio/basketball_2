using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    Phase current_phase;

    void Start()
    {
        current_phase = Phase.TeamAAct;
    }

    public void ChangePhase()
    {
        foreach (Player player in FindObjectsOfType<Player>())
        {
            player.RefreshTurn();
        }

        if (current_phase == Phase.TeamAAct)
        {
            current_phase = Phase.TeamBAct;
            FindObjectOfType<TurnText>().StartMoving("B");
        }
        else if (current_phase == Phase.TeamBAct)
        {
            current_phase = Phase.TeamAAct;
            FindObjectOfType<TurnText>().StartMoving("A");
        }

        FindObjectOfType<TimeCounter>().DecreaseTime();
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}