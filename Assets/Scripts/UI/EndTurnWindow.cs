using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnWindow : MonoBehaviour
{
    public Transform arrow;

    Button current_button;
    public Button confirm_button;
    public Button cancel_button;

    [HideInInspector]
    public Highlight highlight;

    float quit_game_timer = Mathf.Infinity;

    void OnEnable()
    {
        highlight.InMenu();

        current_button = confirm_button;
        arrow.SetParent(current_button.transform, false);

        quit_game_timer = Time.timeSinceLevelLoad;
    }

    void Update()
    {
        if (Input.GetKeyDown("up"))
        {
            Switch();
        }
        else if (Input.GetKeyDown("down"))
        {
            Switch();
        }
        else if (Input.GetKeyDown("space") || Input.GetKeyDown("return"))
        {
            Camera.main.GetComponent<AudioSource>().Play();
            HandleEnter();
        }
        else if (Input.GetKeyDown("escape"))
        {
            Cancel();
        }
        else if (Input.GetKey("escape"))
        {
            if (Time.timeSinceLevelLoad > quit_game_timer + 1.75f)
            {
                Debug.Log("QUIT");
                Application.Quit();
            }
        }
        else
        {
            quit_game_timer = Time.timeSinceLevelLoad;
        }
    }

    void Switch()
    {
        Camera.main.GetComponent<ClickSounds>().Move();
        if (current_button == confirm_button)
        {
            current_button = cancel_button;
        }
        else if (current_button == cancel_button)
        {
            current_button = confirm_button;
        }
        arrow.SetParent(current_button.transform, false);
    }

    void HandleEnter()
    {
        Camera.main.GetComponent<ClickSounds>().Select();
        current_button.onClick.Invoke();
    }

    public void Cancel()
    {
        transform.parent.gameObject.SetActive(false);
        if (!AITurn.Activity)
        {
            highlight.Reset();
        }
    }
}
