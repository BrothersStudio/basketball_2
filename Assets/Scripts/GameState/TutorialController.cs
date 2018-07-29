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

    float last_text_time = 0;

    int tutorial_ind = -1;
    List<string> tutorial_strings = new List<string>();

    Player tutorial_talker;
    Player player_character;

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
        tutorial_strings.Add("Welcome to Basketball 2!\nAre you ready to jam?");
        tutorial_strings.Add("The goal is to get the ball\nto the hoop.");
        tutorial_strings.Add("Once you do so,\nyou score!");
        tutorial_strings.Add("However, if the defense\nholds you off");
        tutorial_strings.Add("Until the shot clock\ngoes off...");
        tutorial_strings.Add("They get the ball!");
        tutorial_strings.Add("After 4 quarters,\na winner is called!");
        tutorial_strings.Add("I'll show you\nthe first action!");

        // Action
        tutorial_strings.Add("Doing move and push...");

        tutorial_strings.Add("When you aren't\nholding the ball");
        tutorial_strings.Add("You can move\n2 spaces");
        tutorial_strings.Add("And you can push people!");
        tutorial_strings.Add("Be careful! You can\nbe pushed off the edge");
        tutorial_strings.Add("Or into stage hazards!");
        tutorial_strings.Add("If you're holding the ball\nwhen that happens...");
        tutorial_strings.Add("The opponent gets possession!");

        // Action
        tutorial_strings.Add("Ally falling");

        tutorial_strings.Add("Fortunately, you won't\nbe alone!");
        tutorial_strings.Add("You can pass the ball\nto allies within 3 squares");
        tutorial_strings.Add("Holding the ball\nallows you to move 3 squars");
        tutorial_strings.Add("Instead of 2 without");
        tutorial_strings.Add("But remember,\nyou can't push while holding the ball!");
        tutorial_strings.Add("Pass and score!");

        // Action
        tutorial_strings.Add("Player control");

        tutorial_strings.Add("Nice! You're ready\nfor the big leagues!");
        tutorial_strings.Add("Good luck!");
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

        FindObjectOfType<Ball>().ToggleDribble();

        player_character = FindObjectsOfType<Player>()[1];
        TutorialPlayer tutorial = player_character.gameObject.AddComponent<TutorialPlayer>();
        tutorial.can_move = false;
        tutorial.can_pass = true;

        tutorial_talker = FindObjectsOfType<Player>()[0];
        tutorial_text.transform.parent.GetComponent<FollowFocusPlayer>().SetFocusPlayer(tutorial_talker.gameObject);
        tutorial_text.transform.parent.gameObject.SetActive(true);
    }

    void AdvanceText()
    {
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
            case 8:
                performing_action = true;
                tutorial_text.transform.parent.gameObject.SetActive(false);
                StartCoroutine(MoveAndPush());
                return true;
            case 16:
                performing_action = true;
                tutorial_text.transform.parent.gameObject.SetActive(false);
                StartCoroutine(SpawnAlly());
                return true;
            case 23:
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
        if (Input.anyKey && Time.timeSinceLevelLoad > last_text_time + 0.5f && !performing_action)
        {
            last_text_time = Time.timeSinceLevelLoad;

            AdvanceText();
        }
    }
}
