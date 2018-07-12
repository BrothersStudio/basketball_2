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

    int juke_percent;
    GameObject juke_confirm_window;
    public GameObject juke_text_prefab;

    void Start()
    {
        shot_confirm_window = transform.parent.Find("Shot Confirm Window").gameObject;
        juke_confirm_window = transform.parent.Find("Juke Confirm Window").gameObject;
    }

    public void SetButtons(Player player)
    {
        selected_player = player;

        if (player.team == Possession.team)
        {
            bool has_ball = selected_player.HasBall();

            // Turn off defensive buttons
            transform.Find("Block Button").gameObject.SetActive(false);
            transform.Find("Steal Button").gameObject.SetActive(false);

            Button shoot_button = transform.Find("Shoot Button").GetComponent<Button>();
            shoot_button.gameObject.SetActive(true);
            if (has_ball && !selected_player.took_attack)
            {
                shoot_button.interactable = true;
                shoot_button.onClick.RemoveAllListeners();
                shoot_button.onClick.AddListener(selected_player.CheckShoot);
            }
            else
            {
                shoot_button.interactable = false;
            }

            Button pass_button = transform.Find("Pass Button").GetComponent<Button>();
            pass_button.gameObject.SetActive(true);
            if (has_ball && !selected_player.took_attack)
            {
                pass_button.interactable = true;
                pass_button.onClick.RemoveAllListeners();
                pass_button.onClick.AddListener(selected_player.CheckPass);
            }
            else
            {
                pass_button.interactable = false;
            }

            Button juke_button = transform.Find("Juke Button").GetComponent<Button>();
            juke_button.gameObject.SetActive(true);
            if (!selected_player.took_move && Utils.ReturnAdjacentOpponent(selected_player) != null)
            {
                juke_button.interactable = true;
                juke_button.onClick.RemoveAllListeners();
                juke_button.onClick.AddListener(selected_player.CheckJuke);
            }
            else
            {
                juke_button.interactable = false;
            }
        }
        else
        {
            // Turn off offensive buttons
            transform.Find("Shoot Button").gameObject.SetActive(false);
            transform.Find("Pass Button").gameObject.SetActive(false);
            transform.Find("Juke Button").gameObject.SetActive(false);

            Button block_button = transform.Find("Block Button").GetComponent<Button>();
            block_button.gameObject.SetActive(true);
            if (!selected_player.took_attack)
            {
                block_button.interactable = true;
                block_button.onClick.RemoveAllListeners();
                block_button.onClick.AddListener(selected_player.Block);
            }
            else
            {
                block_button.interactable = false;
            }

            Button steal_button = transform.Find("Steal Button").GetComponent<Button>();
            steal_button.gameObject.SetActive(true);
            if (!selected_player.took_attack)
            {
                steal_button.interactable = true;
                steal_button.onClick.RemoveAllListeners();
                steal_button.onClick.AddListener(selected_player.Steal);
            }
            else
            {
                steal_button.interactable = false;
            }
        }

        Button move_button = transform.Find("Move Button").GetComponent<Button>();
        if (!selected_player.took_move)
        {
            move_button.interactable = true;
            move_button.onClick.RemoveAllListeners();
            move_button.onClick.AddListener(selected_player.CheckMove);
        }
        else
        {
            move_button.interactable = false;
        }

        gameObject.SetActive(true);
    }

    public void SetShot(int attack, int defense, int distance)
    {
        shot_confirm_window.transform.Find("Attack").GetComponent<Text>().text = "Shoot: " + attack.ToString();
        shot_confirm_window.transform.Find("Defense").GetComponent<Text>().text = "Block: " + defense.ToString();
        shot_confirm_window.transform.Find("Distance").GetComponent<Text>().text = "Distance: " + distance.ToString();
        shot_range = distance;

        shot_percent = (int)Mathf.Clamp(Utils.GetShotChanceAtDistance(distance) + (attack - defense) * 10, 0, 100);
        shot_confirm_window.transform.Find("Percent").GetComponent<Text>().text = shot_percent.ToString() + "%";

        shot_confirm_window.transform.Find("Shoot Button").GetComponent<Button>().onClick.RemoveAllListeners();
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

            selected_player.ShootAndScore();
        }
        else
        {
            selected_player.ShootRebound();
        }

        Cancel();
    }

    public void SetJuke(int attack, int defense)
    {
        juke_confirm_window.transform.Find("Attack").GetComponent<Text>().text = "Control: " + attack.ToString();
        juke_confirm_window.transform.Find("Defense").GetComponent<Text>().text = "Stance: " + defense.ToString();

        juke_percent = Mathf.Clamp(50 + (attack - defense) * 10, 0, 100);
        juke_confirm_window.transform.Find("Percent").GetComponent<Text>().text = juke_percent.ToString() + "%";

        juke_confirm_window.transform.Find("Juke Button").GetComponent<Button>().onClick.RemoveAllListeners();
        juke_confirm_window.transform.Find("Juke Button").GetComponent<Button>().onClick.AddListener(TryJuke);

        juke_confirm_window.SetActive(true);
    }

    public void TryJuke()
    {
        if (Random.Range(0, 100) < juke_percent)
        {
            GameObject juke_text = Instantiate(juke_text_prefab, transform.parent);
            juke_text.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(selected_player.transform.position);
            selected_player.SuccessfulJuke();
            juke_confirm_window.SetActive(false);
        }
        else
        {
            selected_player.FailJuke();
        }
    }

    public void ConfirmCancel()
    {
        shot_confirm_window.SetActive(false);
    }

    public void Cancel()
    {
        selected_player = null;
        shot_confirm_window.SetActive(false);
        juke_confirm_window.SetActive(false);
        Utils.DehighlightTiles();
        gameObject.SetActive(false);
    }
}
