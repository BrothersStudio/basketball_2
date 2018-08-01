using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhaseController : MonoBehaviour
{
    public Phase current_phase;
    FieldGenerator field_generator;

    public bool AiOn;

    [HideInInspector]
    public GameObject highlight;

    bool game_over = false;

    public GameObject overall_results_panel;
    public GameObject end_turn_text;

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
            if (Progression.level != 0)
            {
                end_turn_text.SetActive(false);
            }

            AITurn.Activity = true;
            highlight.SetActive(false);
            yield return new WaitForSeconds(1f);
            GetComponent<AIController>().StartAITurn();
        }
        else
        {
            if (Progression.level != 0)
            {
                end_turn_text.SetActive(true);
            }

            AITurn.Activity = false;
        }
    }

    public void ChangePhase()
    {
        if (!game_over)
        {
            if (current_phase == Phase.TeamAAct && Possession.team == Team.A)
            {
                FindObjectOfType<ShotClock>().DecreaseTime();
            }
            else if (current_phase == Phase.TeamBAct && Possession.team == Team.B)
            {
                FindObjectOfType<ShotClock>().DecreaseTime();
            }

            Possession.passes_this_turn = 0;
            GetComponent<AIController>().StopAllCoroutines();
            AITurn.Activity = !AITurn.Activity;

            foreach (Player player in FindObjectsOfType<Player>())
            {
                player.SetInactive();
                player.RefreshTurn();
            }

            if (Progression.level != 0)
            {
                end_turn_text.GetComponent<EndTurnFlash>().CountPlayers();
            }

            if (current_phase == Phase.TeamAAct)
            {
                current_phase = Phase.TeamBAct;
                FindObjectOfType<TurnText>().StartMoving("B");
                if (AiOn)
                {
                    highlight.SetActive(false);
                    end_turn_text.SetActive(false);

                    GetComponent<AIController>().StartAITurn();
                }
            }
            else if (current_phase == Phase.TeamBAct)
            {
                current_phase = Phase.TeamAAct;
                FindObjectOfType<TurnText>().StartMoving("A");

                highlight.GetComponent<Highlight>().Reset();
                end_turn_text.SetActive(true);
            }

            
        }
    }

    public void ChangeSides(bool shot_clock_out = false)
    {
        // Tutorial
        if (Progression.level == 0)
        {
            FindObjectOfType<TutorialController>().Scored();
            return;
        }

        if (!game_over)
        {
            Possession.passes_this_turn = 0;
            GetComponent<AIController>().StopAllCoroutines();

            // Stop normal "end turn" text, if applicable
            FindObjectOfType<TurnText>().StopAndReset();

            // Move "switch sides" text
            FindObjectOfType<MovingUI>().StartMoving(shot_clock_out);
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
            FindObjectOfType<TimeCounter>().DecreaseTime();
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

        bool win = FindObjectOfType<ScoreCounter>().DidPlayerWin();
        FindObjectOfType<MovingUI>().GameOver(win);
        Progression.GameResults(win);

        GetComponent<AudioSource>().Play();
        Camera.main.GetComponentInChildren<SongSelector>().PlayVictoryTheme();

        Progression.level++;
        if (Progression.level == 4)
        {
            Invoke("DisplayOverallResults", 3);
        }
        else
        {
            Invoke("ChangeScene", 5);
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Game");
    }

    void DisplayOverallResults()
    {
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            ball.ToggleDribble();
        }
        overall_results_panel.SetActive(true);
    }
}

public enum Phase
{
    TeamAAct,
    TeamBAct
}