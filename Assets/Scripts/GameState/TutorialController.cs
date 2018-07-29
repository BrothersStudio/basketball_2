using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    GameObject canvas;
    Text tutorial_text;

    bool performing_action = false;
    Highlight highlight;

    float last_text_time = Mathf.Infinity;

    int tutorial_ind = -1;
    List<string> tutorial_strings = new List<string>();

    Player tutorial_talker;
    Player player_character;

    float end_tutorial_timer = Mathf.Infinity;

    void Awake()
    {
        if (Progression.level != 0)
        {
            Destroy(this);
        }
    }

    void SetTutorialStrings()
    {
        // Start
        tutorial_strings.Add("Welcome to Basketball 2! Are you ready to jam?");
        tutorial_strings.Add("Your goal is to get the ball to the hoop.");
        tutorial_strings.Add("However, if the defense holds you off until the shot clock goes off, they get the ball!");
        tutorial_strings.Add("I'll show you the first action type!");

        // Action
        tutorial_strings.Add("Doing move and push...");

        tutorial_strings.Add("When you aren't holding the ball, you can move 2 spaces!");
        tutorial_strings.Add("And you can push people out of the way or even off the stage!");
        tutorial_strings.Add("If you're holding the ball when you fall, the opponent gets it!");

        // Action
        tutorial_strings.Add("Ally falling");

        tutorial_strings.Add("Fortunately, you won't be alone!");
        tutorial_strings.Add("You can pass the ball to teammates within 3 squares of you.");
        tutorial_strings.Add("Holding the ball\nallows you to move 3 squares, instead of the 2 without.");
        tutorial_strings.Add("Select the ball carrier, pass to your teammate, and score!");

        // Action
        tutorial_strings.Add("Player control");

        tutorial_strings.Add("Nice! You're ready for the big time! Good luck!");
    }

    public void StartTutorial()
    {
        SetTutorialStrings();

        canvas = GameObject.Find("Canvas");
        tutorial_text = canvas.transform.Find("Tutorial Box/Tutorial Text").GetComponent<Text>();
        AdvanceText();

        // Turn off stuff
        highlight = FindObjectOfType<Highlight>();
        highlight.InMenu();

        canvas.transform.Find("Score Panel").gameObject.SetActive(false);
        canvas.transform.Find("Team X Turn").gameObject.SetActive(false);
        canvas.transform.Find("End Turn").gameObject.SetActive(false);
        canvas.transform.Find("Shot Clock").gameObject.SetActive(false);

        canvas.transform.Find("End Tutorial").gameObject.SetActive(true);

        FindObjectOfType<Ball>().ToggleDribble();

        player_character = FindObjectsOfType<Player>()[1];
        TutorialPlayer tutorial = player_character.gameObject.AddComponent<TutorialPlayer>();
        tutorial.can_move = false;
        tutorial.can_pass = true;

        tutorial_talker = FindObjectsOfType<Player>()[0];
        StartCoroutine(DelayAppear());

        end_tutorial_timer = Time.timeSinceLevelLoad;
    }

    IEnumerator DelayAppear()
    {
        yield return new WaitForSeconds(2);
        last_text_time = Time.timeSinceLevelLoad;
        tutorial_text.transform.parent.gameObject.SetActive(true);
    }

    void AdvanceText()
    {
        Camera.main.GetComponent<ClickSounds>().Select();
        tutorial_ind++;
        if (tutorial_ind < tutorial_strings.Count)
        {
            if (!PerformActions(tutorial_ind))
            {
                tutorial_text.text = tutorial_strings[tutorial_ind];
            }
        }
        else 
        {
            Progression.level++;
            SceneManager.LoadScene("Game");
        }
    }

    bool PerformActions(int ind)
    {
        switch (ind)
        {
            case 4:
                performing_action = true;
                tutorial_text.transform.parent.gameObject.SetActive(false);
                StartCoroutine(MoveAndPush());
                return true;
            case 8:
                performing_action = true;
                tutorial_text.transform.parent.gameObject.SetActive(false);
                StartCoroutine(SpawnAlly());
                return true;
            case 13:
                performing_action = true;
                tutorial_text.transform.parent.gameObject.SetActive(false);
                PlayerControl();
                return true;
            default:
                return false;
        }
    }

    IEnumerator MoveAndPush()
    {
        tutorial_talker.CheckMove();
        yield return new WaitForSeconds(0.5f);
        Utils.FindTileAtLocation(new Vector2(3, 5)).Confirm();
        tutorial_talker.SetInactive();
        yield return new WaitForSeconds(0.5f);
        tutorial_talker.CheckPush();
        yield return new WaitForSeconds(0.5f);
        tutorial_talker.Push(tutorial_talker.highlighted_tiles[0].GetPlayer());
        tutorial_talker.SetInactive();
        yield return new WaitForSeconds(0.5f);

        performing_action = false;
        AdvanceText();
        tutorial_text.transform.parent.gameObject.SetActive(true);
    }

    IEnumerator SpawnAlly()
    {
        FindObjectOfType<FieldGenerator>().SpawnTutorialPlayer(new Vector2(3, 6));
        yield return new WaitForSeconds(1.5f);

        performing_action = false;
        AdvanceText();
        tutorial_text.transform.parent.gameObject.SetActive(true);
    }

    void PlayerControl()
    {
        performing_action = true;
        FindObjectOfType<Ball>().ToggleDribble();
        highlight.Reset();
    }

    public void Scored()
    {
        performing_action = false;
        highlight.InMenu();

        AdvanceText();
        tutorial_text.transform.parent.gameObject.SetActive(true);
    }

    void Update()
    {
        // Skipping tutorial
        if (Input.GetKey("escape"))
        {
            if (Time.timeSinceLevelLoad > end_tutorial_timer + 1.75f)
            {
                Progression.level++;
                SceneManager.LoadScene("Game");
            }
        }
        else
        {
            end_tutorial_timer = Time.timeSinceLevelLoad;
        }

        if (Input.anyKeyDown && Time.timeSinceLevelLoad > last_text_time + 0.5f && !performing_action)
        {
            last_text_time = Time.timeSinceLevelLoad;

            AdvanceText();
        }
    }
}
