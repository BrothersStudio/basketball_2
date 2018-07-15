using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    Phase current_phase;
    FieldGenerator field_generator;

    public bool AiOn;

    void Awake()
    {
        current_phase = Phase.TeamAAct;

        field_generator = FindObjectOfType<FieldGenerator>();
        field_generator.GenerateField(0);
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
            if (AiOn)
            {
                GetComponent<AIController>().StartAITurn();
            }
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
            field_generator.GenerateField(1);
        }
        else
        {
            field_generator.GenerateField(0);
        }
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}