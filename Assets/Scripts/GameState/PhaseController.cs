using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public Phase current_phase;
    FieldGenerator field_generator;

    public bool AiOn;

    [HideInInspector]
    public GameObject highlight;

    bool game_over = false;

    void Awake()
    {
        Possession.team = Team.A;

        if (Possession.team == Team.A)
        {
            current_phase = Phase.TeamAAct;
        }
        else
        {
            current_phase = Phase.TeamBAct;
        }

        field_generator = FindObjectOfType<FieldGenerator>();
        field_generator.GenerateField();

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
        if (!game_over)
        {
            Possession.passes_this_turn = 0;
            GetComponent<AIController>().StopAllCoroutines();
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
                current_phase = Phase.TeamAAct;
                FindObjectOfType<TurnText>().StartMoving("A");

                highlight.SetActive(true);

                FindObjectOfType<ShotClock>().DecreaseTime();
            }

            FindObjectOfType<TimeCounter>().DecreaseTime();
        }
    }

    public void ChangeSides()
    {
        if (!game_over)
        {
            Possession.passes_this_turn = 0;
            GetComponent<AIController>().StopAllCoroutines();

            // Stop normal "end turn" text, if applicable
            FindObjectOfType<TurnText>().StopAndReset();

            // Move "switch sides" text
            FindObjectOfType<MovingUI>().StartMoving();
            if (Possession.team == Team.A)
            {
                current_phase = Phase.TeamBAct;
                Possession.team = Team.B;
                field_generator.GenerateField();
                StartCoroutine(StartRound());
            }
            else
            {
                current_phase = Phase.TeamAAct;
                Possession.team = Team.A;
                field_generator.GenerateField();
                StartCoroutine(StartRound());
            }
            FindObjectOfType<ShotClock>().Restart();
        }
    }

    public void GameOver()
    {
        game_over = true;

        StopAllCoroutines();

        Destroy(FindObjectOfType<TurnText>().gameObject);
        Highlight highlight = FindObjectOfType<Highlight>();
        if (highlight != null)
        {
            Destroy(highlight.gameObject);
        }
        Transform canvas = GameObject.Find("Canvas").transform;
        Destroy(canvas.Find("End Turn Fade").gameObject);
        Destroy(canvas.Find("Command Window").gameObject);

        GetComponent<AIController>().StopAllCoroutines();
        
        Utils.DehighlightTiles();
        Utils.DeactivatePlayers();

        FindObjectOfType<MovingUI>().GameOver();
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}