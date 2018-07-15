﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public void StartAITurn()
    {
        StartCoroutine("PerformTurn");
    }

    IEnumerator PerformTurn()
    {
        /*bool defense = false;
        if (Possession.team == Team.A)
        {
            defense = true;
        }*/

        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.team == Team.B)
            {
                // Moving
                player.CheckMove();
                yield return new WaitForSeconds(1f);
                FindClosestHighlightedTileTo(player, FindClosestEnemyTo(player)).OnMouseDown();

                yield return new WaitForSeconds(0.5f);

                // Pushing
                player.CheckPush();
                yield return new WaitForSeconds(1f);
                bool pushed = false;

                // Push ball carrier if you have the option
                foreach (Tile tile in player.highlighted_tiles)
                {
                    if (tile.HasPlayer())
                    {
                        if (tile.GetPlayer().HasBall())
                        {
                            tile.OnMouseDown();
                            pushed = true;
                            break;
                        }
                    }
                }

                // If no ball carrier, push a random guy I guess
                if (!pushed && player.highlighted_tiles.Count != 0)
                {
                    player.highlighted_tiles[Random.Range(0, player.highlighted_tiles.Count)].OnMouseDown();
                    pushed = true;
                }
            }
        }
        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(0.5f);
    }

    Player FindClosestEnemyTo(Player searching_player)
    {
        Player min_player = null;
        int min_dist = 100;

        // Let's find the nearest enemy then go to the highlighted tile nearest to him
        foreach (Player enemy_player in FindObjectsOfType<Player>())
        {
            if (enemy_player.team == Team.A)
            {
                int check_dist = Utils.GetDistance(enemy_player.current_tile.position, searching_player.current_tile.position);
                if (check_dist < min_dist)
                {
                    min_player = enemy_player;
                    min_dist = check_dist;
                }
            }
        }

        return min_player;
    }

    Tile FindClosestHighlightedTileTo(Player moving_player, Player target_player)
    {
        Tile min_tile = null;
        int min_dist = 100;

        foreach (Tile highlighted_tile in moving_player.highlighted_tiles)
        {
            int check_dist = Utils.GetDistance(target_player.current_tile.position, highlighted_tile.position);
            if (check_dist < min_dist)
            {
                min_tile = highlighted_tile;
                min_dist = check_dist;
            }
        }

        return min_tile;
    }
}