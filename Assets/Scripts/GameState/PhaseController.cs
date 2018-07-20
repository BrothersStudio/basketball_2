using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    Phase current_phase;
    FieldGenerator field_generator;

    public bool AiOn;

    public GameObject highlight;

    void Awake()
    {
        current_phase = Phase.TeamAAct;

        field_generator = FindObjectOfType<FieldGenerator>();
        field_generator.GenerateField(0);
        StartCoroutine(StartGame());
        Utils.DehighlightTiles();
    }

    IEnumerator StartGame()
    {
        if (current_phase == Phase.TeamBAct && AiOn)
        {
            AITurn.Activity = true;
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
        }

        FindObjectOfType<TimeCounter>().DecreaseTime();
    }

    public void ChangeSides()
    {
        FindObjectOfType<MovingUI>().StartMoving();
        if (Possession.team == Team.A)
        {
            field_generator.GenerateField(1);
            Possession.team = Team.B;
        }
        else
        {
            field_generator.GenerateField(0);
            Possession.team = Team.A;
        }
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}