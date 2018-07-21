using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    Phase current_phase;
    FieldGenerator field_generator;

    public bool AiOn;

    [HideInInspector]
    public GameObject highlight;

    void Awake()
    {
        current_phase = Phase.TeamAAct;

        field_generator = FindObjectOfType<FieldGenerator>();
        field_generator.GenerateField(0);
        StartCoroutine(StartRound());
        Utils.DehighlightTiles();
    }

    IEnumerator StartRound()
    {
        if (current_phase == Phase.TeamBAct && AiOn)
        {
            AITurn.Activity = true;
            highlight.SetActive(false);
            yield return new WaitForSeconds(1f);
            GetComponent<AIController>().StartAITurn();
        }
        else
        {
            AITurn.Activity = false;
        }
    }

    public void ChangePhase()
    {
        AITurn.Activity = !AITurn.Activity;

        foreach (Player player in FindObjectsOfType<Player>())
        {
            player.SetInactive();
            player.RefreshTurn();
        }

        if (current_phase == Phase.TeamAAct)
        {
            current_phase = Phase.TeamBAct;
            FindObjectOfType<TurnText>().StartMoving("B");
            if (AiOn)
            {
                highlight.SetActive(false);
                GetComponent<AIController>().StartAITurn();
            }
        }
        else if (current_phase == Phase.TeamBAct)
        {
            GetComponent<AIController>().StopAllCoroutines();

            current_phase = Phase.TeamAAct;
            FindObjectOfType<TurnText>().StartMoving("A");

            highlight.SetActive(true);

            FindObjectOfType<ShotClock>().DecreaseTime();
        }

        FindObjectOfType<TimeCounter>().DecreaseTime();
    }

    public void ChangeSides()
    {
        // Stop normal "end turn" text, if applicable
        FindObjectOfType<TurnText>().StopAndReset();

        // Move "switch sides" text
        FindObjectOfType<MovingUI>().StartMoving();
        if (Possession.team == Team.A)
        {
            current_phase = Phase.TeamBAct;
            Possession.team = Team.B;
            field_generator.GenerateField(1);
            StartCoroutine(StartRound());
        }
        else
        {
            current_phase = Phase.TeamAAct;
            Possession.team = Team.A;
            field_generator.GenerateField(0);
            StartCoroutine(StartRound());
        }
        FindObjectOfType<ShotClock>().Restart();
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}