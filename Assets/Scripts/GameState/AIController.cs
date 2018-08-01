using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool random;

    float ai_wait_time = 0.6f;

    List<Player> ai_players = new List<Player>();

    void Start()
    {
        if (random)
        {
            int seed = System.DateTime.Now.Second + System.DateTime.Now.Minute + System.DateTime.Now.Hour;
            Random.InitState(seed);
        }
        else
        {
            Random.InitState(100);
        }
    }

    public void StartAITurn()
    {
        ai_players.Clear();
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.team == Team.B)
            {
                ai_players.Add(player);
            }
        }

        if (Possession.team == Team.A)
        {
            SortAIPlayersForDefense();
            StartCoroutine("Defend");
        }
        else
        {
            SortAIPlayersForAttack();
            StartCoroutine("Attack");
        }
    }

    IEnumerator Defend()
    {
        yield return new WaitForSeconds(ai_wait_time);

        // Can anyone push the ball carrier off?
        Player ball_carrier = GetPlayerWithBall();
        if (IsNearEdge(ball_carrier))
        {
            foreach (Player player in ai_players)
            {
                player.CheckMove();
                List<Tile> check_tile = ReturnAnyAdjacentTo(player.highlighted_tiles, ball_carrier.current_tile);
                if (check_tile.Count != 0)
                {
                    foreach (Tile tile in check_tile)
                    {
                        if (GetPushableAdjacentEnemiesToTile(tile).Count == 0) continue;

                        Tile test_tile = tile.VisualizePushingOtherFromHere(ball_carrier.current_tile);
                        if (test_tile == null || test_tile.impassable)
                        {
                            yield return new WaitForSeconds(ai_wait_time);
                            player.Move(tile);
                            yield return new WaitForSeconds(ai_wait_time);
                            player.CheckPush();
                            yield return new WaitForSeconds(ai_wait_time);
                            player.Push(ball_carrier);
                            yield break;
                        }
                        else if (test_tile.OnEdge())
                        {
                            foreach (Player other_player in ai_players)
                            {
                                if (player == other_player) continue;

                                other_player.CheckMove(ignore_other_players: true);
                                List<Tile> other_check_tile = ReturnAnyAdjacentTo(other_player.highlighted_tiles, test_tile);
                                if (other_check_tile.Count != 0)
                                {
                                    foreach (Tile other_tile in other_check_tile)
                                    {
                                        if (GetPushableAdjacentEnemiesToTile(other_tile).Count == 0) continue;

                                        Tile other_test_tile = other_tile.VisualizePushingOtherFromHere(test_tile);
                                        if (other_test_tile == null || other_test_tile.impassable)
                                        {
                                            player.SetInactive();
                                            other_player.SetInactive();

                                            // First move/push
                                            player.CheckMove();
                                            yield return new WaitForSeconds(ai_wait_time);
                                            player.Move(tile);
                                            yield return new WaitForSeconds(ai_wait_time);
                                            player.CheckPush();
                                            yield return new WaitForSeconds(ai_wait_time);
                                            player.Push(ball_carrier);
                                            yield return new WaitForSeconds(ai_wait_time);

                                            // Second
                                            other_player.CheckMove();
                                            yield return new WaitForSeconds(ai_wait_time);
                                            other_player.Move(other_tile);
                                            yield return new WaitForSeconds(ai_wait_time);
                                            other_player.CheckPush();
                                            yield return new WaitForSeconds(ai_wait_time);
                                            other_player.Push(ball_carrier);
                                            yield break;
                                        }
                                    }
                                }
                                other_player.SetInactive();
                            }
                        }
                    }
                }
                player.SetInactive();
            }
        }

        // Do a normal defensive turn
        foreach (Player player in ai_players)
        {
            bool pushed = false;

            // If I can already push, do so and then move annoyingly
            if (player.CheckPush())
            {
                foreach (Tile tile in player.highlighted_tiles)
                {
                    Tile potential_tile = player.current_tile.VisualizePushingOtherFromHere(tile);
                    if (potential_tile == null) continue;

                    if (GetDistanceFromAToBForTeam(potential_tile, FindObjectOfType<Hoop>().current_tile, Team.A) >
                        GetDistanceFromAToBForTeam(tile, FindObjectOfType<Hoop>().current_tile, Team.A))
                    {
                        // It's better for us to push this guy
                        yield return new WaitForSeconds(ai_wait_time);
                        tile.Confirm();
                        yield return new WaitForSeconds(ai_wait_time);
                        pushed = true;
                        break;
                    }
                }
                player.SetInactive();
            }

            // Moving
            player.CheckMove();
            yield return new WaitForSeconds(ai_wait_time);
            Player hate_target = FindClosestEnemyToHoop();
            Tile inconvienient = FindMostInconvienientTileFor(hate_target, player);
            if (inconvienient != player.current_tile)
            {
                inconvienient.Confirm();
                yield return new WaitForSeconds(ai_wait_time);
            }
            else
            {
                player.SetInactive();
            }

            // If we still haven't pushed...
            if (!pushed)
            {
                player.CheckPush();
                foreach (Tile tile in player.highlighted_tiles)
                {
                    if (GetPushableAdjacentEnemiesToTile(tile).Count == 0) continue;

                    Tile potential_tile = player.current_tile.VisualizePushingOtherFromHere(tile);
                    if (potential_tile == null)
                    {
                        // It's better for us to push this guy
                        yield return new WaitForSeconds(ai_wait_time);
                        tile.Confirm();
                        yield return new WaitForSeconds(ai_wait_time);
                        pushed = true;
                        break;
                    }
                    else if (GetDistanceFromAToBForTeam(potential_tile, FindObjectOfType<Hoop>().current_tile, Team.A) >
                        GetDistanceFromAToBForTeam(tile, FindObjectOfType<Hoop>().current_tile, Team.A))
                    {
                        // It's better for us to push this guy
                        yield return new WaitForSeconds(ai_wait_time);
                        tile.Confirm();
                        yield return new WaitForSeconds(ai_wait_time);
                        pushed = true;
                        break;
                    }
                }
                player.SetInactive();
            }
        }

        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(ai_wait_time);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(ai_wait_time);

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
                yield return new WaitForSeconds(ai_wait_time);
                FindClosestInGroupOfTilesTo(player, FindObjectOfType<Hoop>().current_tile).Confirm();
                yield return new WaitForSeconds(ai_wait_time);
                yield break;
            }
        }

        // Can any aggressive pushes open a path?
        foreach (Player player in ai_players)
        {
            if (player.HasBall()) continue;

            Player close_ally = FindClosestAllyToHoopAsCrowFlies();

            List<Tile> check_tiles = ReturnAnyTilesAdjacentToEnemy(player);
            foreach (Tile tile in check_tiles)
            {
                List<Player> adjacent_enemy_players = GetPushableAdjacentEnemiesToTile(tile);
                foreach (Player enemy_player in adjacent_enemy_players)
                {
                    Tile moved_to_tile = player.VisualizePushing(tile, enemy_player.current_tile);
                    if (GetDistanceForTeamIfTileImpassible(close_ally.current_tile, FindObjectOfType<Hoop>().current_tile, moved_to_tile, Team.B, enemy_player.current_tile) <
                        GetDistanceFromAToBForTeam(close_ally.current_tile, FindObjectOfType<Hoop>().current_tile, Team.B))
                    {
                        player.CheckMove();
                        yield return new WaitForSeconds(ai_wait_time);
                        tile.Confirm();
                        yield return new WaitForSeconds(ai_wait_time);
                        player.CheckPush();
                        yield return new WaitForSeconds(ai_wait_time);
                        player.Push(enemy_player);
                        yield return new WaitForSeconds(ai_wait_time);
                        goto next_player;
                    }
                }
            }
            next_player: continue;
        }

        // Let's move towards the goal using the ball to boost movement
        foreach (Player player in ai_players)
        {
            if (player.IsDone()) continue;

            // Can we pass it to anyone who hasn't moved?
            if (player.HasBall())
            {
                player.CheckMove();
                yield return new WaitForSeconds(ai_wait_time);
                FindClosestInGroupOfTilesTo(player, FindObjectOfType<Hoop>().current_tile, GetSafeTiles(player)).Confirm();
                yield return new WaitForSeconds(ai_wait_time);

                if (player.CheckPass())
                {
                    foreach (Tile tile in player.highlighted_tiles)
                    {
                        if (!tile.GetPlayer().took_move)
                        {
                            yield return new WaitForSeconds(ai_wait_time);
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
                        Tile potential_tile = player.VisualizePushing(tile.GetPlayer());
                        if (potential_tile == null) continue;

                        if (GetDistanceFromAToBForTeam(potential_tile, GetPlayerWithBall().current_tile, Team.A) >
                            GetDistanceFromAToBForTeam(tile, GetPlayerWithBall().current_tile, Team.A))
                        {
                            // It's better for us to push this guy
                            yield return new WaitForSeconds(ai_wait_time);
                            tile.Confirm();
                            yield return new WaitForSeconds(ai_wait_time);
                            break;
                        }
                    }
                }
                player.SetInactive();

                player.CheckMove();
                yield return new WaitForSeconds(ai_wait_time);

                if (Random.Range(0, 100) < 50)
                {
                    // Move toward hoop
                    FindClosestInGroupOfTilesTo(player, FindObjectOfType<Hoop>().current_tile).Confirm();
                    yield return new WaitForSeconds(ai_wait_time);
                }
                else
                {
                    // Move and push an enemy
                    Player enemy = FindClosestEnemyTo(player);
                    FindClosestInGroupOfTilesTo(player, enemy.current_tile).Confirm();
                    yield return new WaitForSeconds(ai_wait_time);
                    if (player.CheckPush())
                    {
                        yield return new WaitForSeconds(ai_wait_time);
                        Player chosen_player = player.highlighted_tiles[Random.Range(0, player.highlighted_tiles.Count)].GetPlayer();
                        player.Push(chosen_player);
                        yield return new WaitForSeconds(ai_wait_time);
                    }
                }
            }
            player.SetInactive();
        }

        GetComponent<PhaseController>().ChangePhase();
        yield return new WaitForSeconds(ai_wait_time);
    }

    IEnumerator GiveHimTheBall(Player target_player)
    {
        if (target_player.HasBall()) yield break;

        foreach (Player player in ai_players)
        {
            if (player == target_player || player.ai_pass_check || player.took_attack) continue;

            player.CheckMove(true);
            Tile closest_tile = FindClosestInGroupOfTilesTo(player, target_player.current_tile);
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
        ResetPassChecks();
    }

    IEnumerator PassTo(Player origin, Player target)
    {
        origin.CheckMove();
        Tile closest_tile = FindClosestInGroupOfTilesTo(origin, target.current_tile);
        yield return new WaitForSeconds(ai_wait_time);
        closest_tile.Confirm();
        origin.CheckPass();
        yield return new WaitForSeconds(ai_wait_time);
        origin.Pass(target);
        yield return new WaitForSeconds(ai_wait_time);
    }

    Player FindClosestEnemyTo(Player searching_player)
    {
        List<Player> min_players = new List<Player>();
        int min_dist = 100;

        // Let's find the nearest enemy then go to the highlighted tile nearest to him
        foreach (Player enemy_player in FindObjectsOfType<Player>())
        {
            if (enemy_player.team == Team.A)
            {
                int check_dist = Utils.GetDistance(enemy_player.current_tile.position, searching_player.current_tile.position);
                if (check_dist <= min_dist)
                {
                    min_players.Add(enemy_player);
                    min_dist = check_dist;
                }
            }
        }

        // Break ties with player holding ball
        foreach (Player player in min_players)
        {
            if (player.HasBall())
            {
                return player;
            }
        }

        // Else, random tiebreaker is fine
        return min_players[Random.Range(0, min_players.Count)];
    }

    Tile FindClosestInGroupOfTilesTo(Player moving_player, Tile target_tile, List<Tile> input_tiles = null)
    {
        List<Tile> min_tiles = new List<Tile>();
        int min_dist = 100;

        List<Tile> tiles_to_check;
        if (input_tiles == null)
        {
            tiles_to_check = moving_player.highlighted_tiles;
        }
        else
        {
            tiles_to_check = input_tiles;
        }

        foreach (Tile highlighted_tile in tiles_to_check)
        {
            int check_dist = GetDistanceFromAToBForTeam(target_tile, highlighted_tile, moving_player.team);
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
                int check_dist = GetDistanceFromAToBForTeam(tile, hoop_tile, Team.B);
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

    void SortAIPlayersForAttack()
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

    void SortAIPlayersForDefense()
    {
        Player danger_player = FindClosestEnemyToHoop();

        // Sort
        Player temp = null;
        for (int write = 0; write < ai_players.Count; write++)
        {
            for (int sort = 0; sort < ai_players.Count - 1; sort++)
            {
                if (Utils.GetDistance(ai_players[sort].current_tile.position, danger_player.current_tile.position) >
                    Utils.GetDistance(ai_players[sort + 1].current_tile.position, danger_player.current_tile.position))
                {
                    temp = ai_players[sort + 1];
                    ai_players[sort + 1] = ai_players[sort];
                    ai_players[sort] = temp;
                }
            }
        }
    }

    Player GetPlayerWithBall()
    {
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.HasBall())
            {
                return player;
            }
        }
        return null;
    }

    Tile FindMostInconvienientTileFor(Player hate_player, Player check_player)
    {
        // Include current tile
        List<Tile> check_tiles = new List<Tile>();
        check_tiles.AddRange(check_player.highlighted_tiles);
        check_tiles.Add(check_player.current_tile);

        Tile hoop_tile = FindObjectOfType<Hoop>().current_tile;

        int max_dist = 0;
        List<Tile> best_tiles = new List<Tile>();
        foreach (Tile highlighted_tile in check_tiles)
        {
            int check_dist = GetDistanceForTeamIfTileImpassible(hate_player.current_tile, hoop_tile, highlighted_tile, Team.A, check_player.current_tile);
            if (check_dist >= max_dist)
            {
                max_dist = check_dist;
                best_tiles.Add(highlighted_tile);
            }
        }
        return FindClosestInGroupOfTilesTo(check_player, hate_player.current_tile, best_tiles);
    }

    int GetDistanceFromAToBForTeam(Tile tile_1, Tile tile_2, Team team)
    {
        HashSet<Tile> tiles_to_walk = new HashSet<Tile>();
        tiles_to_walk.Add(tile_1);

        int counter = 0;
        while (counter < 10)  // Juuust in case there's some ugly infinite nonsense, 10 is p far
        {
            counter++;

            Tile[] current_walk_tiles = new Tile[tiles_to_walk.Count];
            tiles_to_walk.CopyTo(current_walk_tiles);
            foreach (Tile tile in current_walk_tiles)
            {
                foreach (Tile adjacent_tile in tile.adjacent_tiles)
                {
                    if (adjacent_tile == null) continue;

                    if (adjacent_tile == tile_2)
                    {
                        return counter;
                    }

                    if (adjacent_tile.HasPlayer())
                    {
                        if (adjacent_tile.GetPlayer().team != team)
                        {
                            continue;
                        }
                    }

                    tiles_to_walk.Add(adjacent_tile);
                }
            }
        }
        return counter;
    }

    int GetDistanceForTeamIfTileImpassible(Tile tile_1, Tile tile_2, Tile impassible_tile, Team team, Tile in_motion_tile)
    {
        HashSet<Tile> tiles_to_walk = new HashSet<Tile>();
        tiles_to_walk.Add(tile_1);

        int counter = 0;
        while (counter < 10)  // Juuust in case there's some ugly infinite nonsense, 20 is p far
        {
            counter++;

            Tile[] current_walk_tiles = new Tile[tiles_to_walk.Count];
            tiles_to_walk.CopyTo(current_walk_tiles);
            foreach (Tile tile in current_walk_tiles)
            {
                foreach (Tile adjacent_tile in tile.adjacent_tiles)
                {
                    if (adjacent_tile == null) continue;

                    if (adjacent_tile == tile_2)
                    {
                        return counter;
                    }

                    if (adjacent_tile == impassible_tile)
                    {
                        continue;
                    }
                    else if (adjacent_tile.HasPlayer())
                    {
                        if (adjacent_tile.GetPlayer().team != team && adjacent_tile != in_motion_tile)
                        {
                            continue;
                        }
                    }

                    tiles_to_walk.Add(adjacent_tile);
                }
            }
        }
        return counter;
    }

    bool IsNearEdge(Player player)
    {
        if (player.current_tile.OnEdge())
        {
            return true;
        }

        foreach (Tile adjacent_tile in player.current_tile.adjacent_tiles)
        {
            if (adjacent_tile == null)
            {
                return true;
            }
            else if (adjacent_tile.OnEdge())
            {
                return true;
            }
        }
        return false;
    }

    List<Tile> ReturnAnyAdjacentTo(List<Tile> highlighted_tiles, Tile target_tile)
    {
        List<Tile> output_tiles = new List<Tile>();
        foreach (Tile tile in highlighted_tiles)
        {
            foreach (Tile adjacent_tile in target_tile.adjacent_tiles)
            {
                if (tile == adjacent_tile)
                {
                    output_tiles.Add(tile);
                }
            }
        }
        return output_tiles;
    }

    List<Tile> ReturnAnyTilesAdjacentToEnemy(Player player)
    {
        List<Tile> output_tiles = new List<Tile>();

        player.CheckMove();
        foreach (Tile tile in player.highlighted_tiles)
        {
            foreach (Tile adjacent_tile in tile.adjacent_tiles)
            {
                if (adjacent_tile == null) continue;

                if (adjacent_tile.HasPlayer())
                {
                    if (adjacent_tile.GetPlayer().team != player.team)
                    {
                        output_tiles.Add(tile);
                        break;
                    }
                }
            }
        }
        player.SetInactive();

        return output_tiles;
    }

    List<Player> GetPushableAdjacentEnemiesToTile(Tile input_tile)
    {
        List<Player> output_players = new List<Player>();
        foreach (Tile tile in input_tile.adjacent_tiles)
        {
            if (tile == null) continue;

            if (tile.HasPlayer())
            {
                if (tile.GetPlayer().team == Team.A)
                {
                    // Check if we can push this guy legally
                    if (Utils.FindTileAtLocation((tile.position - input_tile.position) + tile.position) == null)
                    {
                        output_players.Add(tile.GetPlayer());
                        continue;
                    }
                    else if (!Utils.FindTileAtLocation((tile.position - input_tile.position) + tile.position).HasPlayer())
                    {
                        output_players.Add(tile.GetPlayer());
                        continue;
                    }
                }
            }
        }
        return output_players;
    }

    Player FindClosestEnemyToHoop()
    {
        Tile hoop_tile = FindObjectOfType<Hoop>().current_tile;

        int min_dist = 100;
        Player min_player = null;
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.team == Team.A)
            {
                int check_dist = GetDistanceFromAToBForTeam(player.current_tile, hoop_tile, Team.A);
                if (check_dist < min_dist)
                {
                    min_dist = check_dist;
                    min_player = player;
                }
                else if (check_dist == min_dist && player.HasBall())
                {
                    min_dist = check_dist;
                    min_player = player;
                }
            }
        }
        return min_player;
    }

    Player FindClosestAllyToHoopAsCrowFlies()
    {
        Tile hoop_tile = FindObjectOfType<Hoop>().current_tile;

        int min_dist = 100;
        Player min_player = null;
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.team == Team.B)
            {
                int check_dist = Utils.GetDistance(player.current_tile.position, hoop_tile.position);
                if (check_dist < min_dist)
                {
                    min_dist = check_dist;
                    min_player = player;
                }
                else if (check_dist == min_dist && player.HasBall())
                {
                    min_dist = check_dist;
                    min_player = player;
                }
            }
        }
        return min_player;
    }

    List<Tile> GetSafeTiles(Player player)
    {
        List<Tile> safe_tiles = new List<Tile>();
        foreach (Tile tile in player.highlighted_tiles)
        {
            if (!tile.OnEdge())
            {
                safe_tiles.Add(tile);
            }
        }

        if (safe_tiles.Count == 0)
        {
            return player.highlighted_tiles;
        }
        else
        {
            return safe_tiles;
        }
    }

    public static void ResetPassChecks()
    {
        foreach (Player player in FindObjectsOfType<Player>())
        {
            player.ai_pass_check = false;
        }
    }
}