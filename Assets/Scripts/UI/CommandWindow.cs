using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandWindow : MonoBehaviour
{
    Player selected_player;

    public GameObject arrow;
    bool lock_arrow = false;

    public Button attack_button;
    public Button move_button;
    public Button cancel_button;

    [HideInInspector]
    public Highlight highlight;

    Button current_button;

    void Update()
    {
        if (!lock_arrow)
        {
            if (Input.GetKeyDown("up"))
            {
                HandleUp();
            }
            else if (Input.GetKeyDown("down"))
            {
                HandleDown();
            }
            else if (Input.GetKeyDown("space") || Input.GetKeyDown("return"))
            {
                HandleEnter();
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            Cancel();
        }
    }

    void HandleUp()
    {
        if (current_button == move_button)
        {
            current_button = attack_button;
        }
        else if (current_button == cancel_button)
        {
            current_button = move_button;
        }
        arrow.transform.SetParent(current_button.transform, false);
    }

    void HandleDown()
    {
        if (current_button == attack_button)
        {
            current_button = move_button;
        }
        else if (current_button == move_button)
        {
            current_button = cancel_button;
        }
        arrow.transform.SetParent(current_button.transform, false);
    }

    void HandleEnter()
    {
        if (current_button.interactable)
        {
            current_button.onClick.Invoke();
        }
    }

    public void SetButtons(Player player)
    {
        selected_player = player;
        highlight.InMenu();

        bool has_ball = selected_player.HasBall();

        current_button = attack_button;
        arrow.transform.SetParent(current_button.transform, false);

        // Attack
        if (has_ball && !selected_player.took_attack && selected_player.CheckPass())
        {
            selected_player.SetInactive();  // Remove highlighted pass tiles

            attack_button.interactable = true;
            attack_button.onClick.RemoveAllListeners();
            attack_button.onClick.AddListener(delegate {
                lock_arrow = true;
                selected_player.CheckPass();
                highlight.SelectCycleTarget(selected_player);
            });

            attack_button.gameObject.GetComponentInChildren<Text>().text = "Pass";
        }
        else if (!selected_player.took_attack && Utils.ReturnAdjacentOpponents(selected_player).Count > 0)
        {
            attack_button.interactable = true;
            attack_button.onClick.RemoveAllListeners();
            attack_button.onClick.AddListener(delegate {
                lock_arrow = true;
                selected_player.CheckPush();
                highlight.SelectCycleTarget(selected_player);
            });

            attack_button.gameObject.GetComponentInChildren<Text>().text = "Push";
        }
        else
        {
            attack_button.interactable = false;

            if (has_ball)
            {
                attack_button.gameObject.GetComponentInChildren<Text>().text = "Pass";
            }
            else
            {
                attack_button.gameObject.GetComponentInChildren<Text>().text = "Push";
            }
        }

        // Move
        if (!selected_player.took_move)
        {
            move_button.interactable = true;
            move_button.onClick.RemoveAllListeners();
            move_button.onClick.AddListener(delegate {
                lock_arrow = true;
                highlight.SelectMove();
                selected_player.CheckMove();
            });
        }
        else
        {
            move_button.interactable = false;
        }

        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        if (selected_player != null)
        {
            selected_player.SetInactive();
            selected_player = null;
        }

        gameObject.SetActive(false);

        lock_arrow = false;

        if (!AITurn.Activity)
        {
            highlight.Reset();
        }
    }
}
