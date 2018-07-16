using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    List<Player> ai_players = new List<Player>();

    public void StartAITurn()
    {
        AITurn.active = true;

        if (ai_players.Count == 0)
        {
            foreach (Player player in FindObjectsOfType<Player>())
            {
                if (player.team == Team.B)
                {
                    ai_players.Add(player);
                }
            }
        }

        if (Possession.team == Team.A)
        {
            StartCoroutine("Defend");
        }
        else
        {
            StartCoroutine("Attack");
        }
    }

    IEnumerator Defend()
    {
        foreach (Player player in ai_players)
        {
            // Todo: Some kind of logic for if we start our turn next to an enemy?

            // Moving
            player.CheckMove();
            yield return new WaitForSeconds(1f);
            FindClosestHighlightedTileTo(player, FindClosestEnemyTo(player).current_tile).OnMouseDown();

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
        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(0.5f);
        AITurn.active = false;
    }

    IEnumerator Attack()
    {
        // First let's see if any player is in range of the goal
        List<Player> in_score_range = new List<Player>();
        foreach (Player player in ai_players)
        {
            if (CanReachNet(player))
            {
                Debug.Log(player.name + " can reach the net");
                in_score_range.Add(player);
            }
        }

        // Does that player have the ball? If so just score
        foreach (Player player in in_score_range)
        {
            if (player.HasBall())
            {
                player.CheckMove();
                yield return new WaitForSeconds(1f);
                FindClosestHighlightedTileTo(player, FindObjectOfType<Hoop>().current_tile).OnMouseDown();
                yield return new WaitForSeconds(0.5f);
                AITurn.active = false;
                yield break;
            }
        }

        // If those player don't have the ball, can we get the ball to those players?
        foreach (Player player in in_score_range)
        {
            player.ai_pass_check = true;
            yield return StartCoroutine(GiveHimTheBall(player));
            if (player.HasBall())
            {
                player.CheckMove();
                yield return new WaitForSeconds(1f);
                FindClosestHighlightedTileTo(player, FindObjectOfType<Hoop>().current_tile).OnMouseDown();
                yield return new WaitForSeconds(0.5f);
                AITurn.active = false;
                yield break;
            }
        }

        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(0.5f);
        AITurn.active = false;
    }

    IEnumerator GiveHimTheBall(Player target_player)
    {
        Debug.Log("Trying to get the ball to " + target_player.name);
        foreach (Player player in ai_players)
        {
            if (player == target_player || player.ai_pass_check || player.took_attack) continue;

            Debug.Log("Checking " + player.name);

            player.CheckMove(true);
            Tile closest_tile = FindClosestHighlightedTileTo(player, target_player.current_tile);
            if (Utils.GetDistance(closest_tile.position, target_player.current_tile.position) <= 3)
            {
                player.ai_pass_check = true;
                player.SetInactive();

                if (player.HasBall())
                {
                    yield return StartCoroutine(PassTo(player, target_player));
                    yield break;
                }
                else
                {
                    yield return StartCoroutine(GiveHimTheBall(player));
                    if (player.HasBall())
                    {
                        yield return StartCoroutine(PassTo(player, target_player));
                        yield break;
                    }
                }
            }
            player.SetInactive();
        }
        Utils.ResetPassChecks();
    }

    IEnumerator PassTo(Player origin, Player target)
    {
        origin.CheckMove();
        Tile closest_tile = FindClosestHighlightedTileTo(origin, target.current_tile);
        yield return new WaitForSeconds(1f);
        closest_tile.OnMouseDown();
        origin.CheckPass();
        yield return new WaitForSeconds(0.5f);
        origin.Pass(target);
        yield return new WaitForSeconds(0.5f);
        //Utils.ResetPassChecks();
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

    Tile FindClosestHighlightedTileTo(Player moving_player, Tile target_tile)
    {
        List<Tile> min_tiles = new List<Tile>();
        int min_dist = 100;

        foreach (Tile highlighted_tile in moving_player.highlighted_tiles)
        {
            int check_dist = Utils.GetDistance(target_tile.position, highlighted_tile.position);
            if (check_dist < min_dist)
            {
                min_tiles.Clear();
                min_tiles.Add(highlighted_tile);
                min_dist = check_dist;
            }
            else if (check_dist == min_dist)
            {
                min_tiles.Add(highlighted_tile);
            }
        }

        // Break ties by picking tile closest to net
        Tile selected_tile = null;
        if (min_tiles.Count == 1)
        {
            selected_tile = min_tiles[0];
        }
        else
        {
            Tile hoop_tile = FindObjectOfType<Hoop>().current_tile;

            min_dist = 100;
            foreach (Tile tile in min_tiles)
            {
                int check_dist = Utils.GetDistance(tile.position, hoop_tile.position);
                if (check_dist < min_dist)
                {
                    min_dist = check_dist;
                    selected_tile = tile;
                }
            }
        }

        return selected_tile;
    }

    bool CanReachNet(Player player)
    {
        player.CheckMove();
        Vector2 hoop_location = FindObjectOfType<Hoop>().current_tile.position;
        foreach (Tile tile in player.highlighted_tiles)
        {
            if (Utils.GetDistance(tile.position, hoop_location) <= 1)
            {
                player.SetInactive();
                return true;
            }
        }
        player.SetInactive();
        return false;
    }
}