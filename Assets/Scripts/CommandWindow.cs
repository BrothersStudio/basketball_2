using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandWindow : MonoBehaviour
{
    Player active_player;

    public void SetButtons(Player player)
    {
        active_player = player;
        bool has_ball = player.HasBall();

        Button shoot_button = transform.Find("Shoot Button").GetComponent<Button>();
        if (has_ball)
        {
            shoot_button.onClick.RemoveAllListeners();
            shoot_button.onClick.AddListener(player.Shoot);
        }
        else
        {
            shoot_button.interactable = false;
        }

        Button move_button = transform.Find("Move Button").GetComponent<Button>();
        move_button.onClick.RemoveAllListeners();
        move_button.onClick.AddListener(player.CheckMove);

        Button pass_button = transform.Find("Pass Button").GetComponent<Button>();
        if (has_ball)
        {
            pass_button.onClick.RemoveAllListeners();
            pass_button.onClick.AddListener(player.CheckPass);
        }
        else
        {
            pass_button.interactable = false;
        }

        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
