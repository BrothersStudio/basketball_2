using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool random;

    List<Player> ai_players = new List<Player>();

    void Start()
    {
        if (random)
        {
            int seed = System.DateTime.Now.Second + System.DateTime.Now.Minute + System.DateTime.Now.Hour;
            Debug.Log("Seed: " + seed.ToString());
            Random.InitState(seed);
        }
        else
        {
            Random.InitState(100);
        }
    }

    public void StartAITurn()
    {
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
            SortAIPlayers();
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
            FindClosestHighlightedTileTo(player, FindClosestEnemyTo(player).current_tile).Confirm();

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
                        tile.Confirm();
                        pushed = true;
                        break;
                    }
                }
            }

            // If no ball carrier, push a random guy I guess
            if (!pushed && player.highlighted_tiles.Count != 0)
            {
                player.highlighted_tiles[Random.Range(0, player.highlighted_tiles.Count)].Confirm();
                pushed = true;
            }
        }

        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator Attack()
    {
        // First let's see if any player is in range of the goal
        List<Player> in_score_range = new List<Player>();
        foreach (Player player in ai_players)
        {
            if (CanReachNet(player))
            {
                in_score_range.Add(player);
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
                FindClosestHighlightedTileTo(player, FindObjectOfType<Hoop>().current_tile).Confirm();
                yield return new WaitForSeconds(0.5f);
                yield break;
            }
        }

        // Okay, I can't win let's move towards the goal using the ball to boost movement
        foreach (Player player in ai_players)
        {
            // Can we pass it to anyone who hasn't moved?
            if (player.HasBall())
            {
                player.CheckMove();
                yield return new WaitForSeconds(1f);
                FindClosestHighlightedTileTo(player, FindObjectOfType<Hoop>().current_tile).Confirm();
                yield return new WaitForSeconds(0.5f);

                if (player.CheckPass())
                {
                    foreach (Tile tile in player.highlighted_tiles)
                    {
                        if (!tile.GetPlayer().took_move)
                        {
                            yield return new WaitForSeconds(0.5f);
                            tile.Confirm();
                            break;
                        }
                    }
                }
            }
            else
            {
                // Can we push anyone away from the ball carrier first? 
                if (player.CheckPush())
                {
                    foreach (Tile tile in player.highlighted_tiles)
                    {
                        Tile potential_tile = player.VisualizePush(tile.GetPlayer());
                        if (potential_tile == null) continue;

                        if (Utils.GetDistanceFromAToBForTeam(potential_tile, GetAIPlayerWithBall().current_tile, Team.A) >
                            Utils.GetDistanceFromAToBForTeam(tile, GetAIPlayerWithBall().current_tile, Team.A))
                        {
                            // It's better for us to push this guy
                            yield return new WaitForSeconds(0.5f);
                            tile.Confirm();
                            break;
                        }
                    }
                    player.SetInactive();
                }

                player.CheckMove();
                yield return new WaitForSeconds(1f);

                if (Random.Range(0, 100) < 50)
                {
                    // Move toward hoop
                    FindClosestHighlightedTileTo(player, FindObjectOfType<Hoop>().current_tile).Confirm();
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    // Move and push an enemy
                    Player enemy = FindClosestEnemyTo(player);
                    FindClosestHighlightedTileTo(player, enemy.current_tile).Confirm();
                    yield return new WaitForSeconds(0.5f);
                    if (player.CheckPush())
                    {
                        Player chosen_player = player.highlighted_tiles[Random.Range(0, player.highlighted_tiles.Count)].GetPlayer();
                        player.Push(chosen_player);
                    }
                }
            }
            player.SetInactive();
        }

        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator GiveHimTheBall(Player target_player)
    {
        if (target_player.HasBall()) yield break;

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
        closest_tile.Confirm();
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
            int check_dist = Utils.GetDistanceFromAToBForTeam(target_tile, highlighted_tile, moving_player.team);
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
                int check_dist = Utils.GetDistanceFromAToBForTeam(tile, hoop_tile, Team.B);
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

    void SortAIPlayers()
    {
        Player ball_carrier = null;
        foreach (Player player in ai_players)
        {
            if (player.HasBall())
            {
                ball_carrier = player;
                break;
            }
        }
        ai_players.Remove(ball_carrier);

        // Sort
        Player temp = null;
        for (int write = 0; write < ai_players.Count; write++)
        {
            for (int sort = 0; sort < ai_players.Count - 1; sort++)
            {
                if (Utils.GetDistance(ai_players[sort].current_tile.position, ball_carrier.current_tile.position) >
                    Utils.GetDistance(ai_players[sort + 1].current_tile.position, ball_carrier.current_tile.position))
                {
                    temp = ai_players[sort + 1];
                    ai_players[sort + 1] = ai_players[sort];
                    ai_players[sort] = temp;
                }
            }
        }

        ai_players.Insert(0, ball_carrier);
    }

    Player GetAIPlayerWithBall()
    {
        foreach (Player player in ai_players)
        {
            if (player.HasBall())
            {
                return player;
            }
        }
        return null;
    }
}