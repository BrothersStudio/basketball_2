using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandWindow : MonoBehaviour
{
    Player selected_player;

    int shot_range;
    int shot_percent;
    GameObject shot_confirm_window;

    void Start()
    {
        shot_confirm_window = transform.parent.Find("Shot Confirm Window").gameObject;
    }

    public void SetButtons(Player player)
    {
        selected_player = player;

        bool has_ball = selected_player.HasBall();

        Button shoot_button = transform.Find("Shoot Button").GetComponent<Button>();
        if (has_ball)
        {
            shoot_button.interactable = true;
            shoot_button.onClick.RemoveAllListeners();
            shoot_button.onClick.AddListener(selected_player.CheckShoot);
        }
        else
        {
            shoot_button.interactable = false;
        }

        Button move_button = transform.Find("Move Button").GetComponent<Button>();
        move_button.onClick.RemoveAllListeners();
        move_button.onClick.AddListener(selected_player.CheckMove);

        Button pass_button = transform.Find("Pass Button").GetComponent<Button>();
        if (has_ball)
        {
            pass_button.interactable = true;
            pass_button.onClick.RemoveAllListeners();
            pass_button.onClick.AddListener(selected_player.CheckPass);
        }
        else
        {
            pass_button.interactable = false;
        }

        gameObject.SetActive(true);
    }

    public void SetShot(int attack, int defense, int distance)
    {
        shot_confirm_window.transform.Find("Attack").GetComponent<Text>().text = "Shoot: " + attack.ToString();
        shot_confirm_window.transform.Find("Defense").GetComponent<Text>().text = "Block: " + defense.ToString();
        shot_confirm_window.transform.Find("Distance").GetComponent<Text>().text = "Distance: " + distance.ToString();

        shot_percent = (int)Mathf.Clamp(Utils.GetShotChanceAtDistance(distance) + (attack - defense) * 10, 0, 100);
        shot_confirm_window.transform.Find("Percent").GetComponent<Text>().text = shot_percent.ToString() + "%";

        shot_confirm_window.transform.Find("Shoot Button").GetComponent<Button>().onClick.RemoveAllListeners();
        shot_confirm_window.transform.Find("Shoot Button").GetComponent<Button>().onClick.AddListener(selected_player.Shoot);
        shot_confirm_window.transform.Find("Shoot Button").GetComponent<Button>().onClick.AddListener(TryShot);

        shot_confirm_window.SetActive(true);
    }

    void TryShot()
    {
        if (Random.Range(0, 100) < shot_percent)
        {
            int points = 2;
            if (shot_range > 4)
            {
                points = 3;
            }

            switch (selected_player.team)
            {
                case Team.A:
                    transform.parent.Find("Score Panel").GetComponent<ScoreCounter>().TeamAScores(points);
                    break;
                case Team.B:
                    transform.parent.Find("Score Panel").GetComponent<ScoreCounter>().TeamBScores(points);
                    break;
            }
        }

        Cancel();
    }

    public void ConfirmCancel()
    {
        shot_confirm_window.SetActive(false);
    }

    public void Cancel()
    {
        selected_player = null;
        shot_confirm_window.SetActive(false);
        gameObject.SetActive(false);
    }
}
