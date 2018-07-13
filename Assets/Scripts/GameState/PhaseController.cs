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

    public void ChangeSides()
    {
        FindObjectOfType<MovingUI>().StartMoving();
        if (Possession.team == Team.A)
        {
            FindObjectOfType<FieldGenerator>().GenerateField(1);
        }
        else
        {
            FindObjectOfType<FieldGenerator>().GenerateField(0);
        }
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}