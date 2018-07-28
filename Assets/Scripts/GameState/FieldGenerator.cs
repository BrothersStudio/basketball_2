using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public GameObject ball_prefab;
    public GameObject tile_prefab;
    public GameObject lava_tile_prefab;
    public GameObject hoop_prefab;
    public GameObject player_prefab;
    public GameObject highlight_prefab;

    public Sprite[] tile_sprites;
    List<GameObject> all_objects = new List<GameObject>();

    // Level setup variables
    List<int> offensive_player_rows;
    List<int> offensive_player_columns;

    List<int> defensive_player_rows;
    List<int> defensive_player_columns;

    List<int> lava_rows = new List<int>();
    List<int> lava_columns = new List<int>();

    int hoop_row;
    int hoop_column;

    int give_ball_ind;

    int set_rows, set_columns;

    public void SetupBasedOnLevel()
    {
        switch (Progression.level)
        {
            case 1:
                if (Possession.team == Team.A)
                {
                    offensive_player_rows    = new List<int>(new int[] { 1, 0, 1, 1, 1 });
                    offensive_player_columns = new List<int>(new int[] { 1, 5, 9, 3, 7 });

                    defensive_player_rows    = new List<int>(new int[] { 3, 2, 3, 3, 3 });
                    defensive_player_columns = new List<int>(new int[] { 1, 5, 3, 7, 9 });

                    lava_rows.Clear();
                    lava_rows.Clear();

                    hoop_row = 6;
                    hoop_column = 5;

                    set_rows = 7;
                    set_columns = 11;

                    Camera.main.GetComponent<CameraShake>().SetNewPosition(new Vector3(-8.41f, 0.75f, -10f));
                }
                else if (Possession.team == Team.B)
                {
                    offensive_player_rows    = new List<int>(new int[] { 5, 1, 9, 3, 7 });
                    offensive_player_columns = new List<int>(new int[] { 6, 5, 5, 5, 5 });

                    defensive_player_rows    = new List<int>(new int[] { 5, 3, 7, 1, 9 });
                    defensive_player_columns = new List<int>(new int[] { 4, 3, 3, 3, 3 });

                    lava_rows.Clear();
                    lava_rows.Clear();

                    hoop_row = 5;
                    hoop_column = 0;

                    set_rows = 11;
                    set_columns = 7;

                    Camera.main.GetComponent<CameraShake>().SetNewPosition(new Vector3(-8.41f, 2.2f, -10f));
                }
                break;
            case 2:
                if (Possession.team == Team.A)
                {
                    offensive_player_rows    = new List<int>(new int[] { 1, 0, 1, 1, 1 });
                    offensive_player_columns = new List<int>(new int[] { 1, 5, 9, 3, 7 });

                    defensive_player_rows    = new List<int>(new int[] { 3, 2, 3, 3, 3 });
                    defensive_player_columns = new List<int>(new int[] { 1, 5, 3, 7, 9 });

                    lava_rows    = new List<int>(new int[] { 2, 3, 3, 2, 4, 4, 6, 6 });
                    lava_columns = new List<int>(new int[] { 8, 6, 4, 2, 8, 2, 7, 3 });

                    hoop_row = 6;
                    hoop_column = 5;

                    set_rows = 7;
                    set_columns = 11;

                    Camera.main.GetComponent<CameraShake>().SetNewPosition(new Vector3(-8.41f, 0.75f, -10f));
                }
                else if (Possession.team == Team.B)
                {
                    offensive_player_rows = new List<int>(new int[] { 5, 1, 9, 3, 7 });
                    offensive_player_columns = new List<int>(new int[] { 6, 5, 5, 5, 5 });

                    defensive_player_rows = new List<int>(new int[] { 5, 3, 7, 1, 9 });
                    defensive_player_columns = new List<int>(new int[] { 4, 3, 3, 3, 3 });

                    lava_rows    = new List<int>(new int[] { 2, 4, 6, 8, 8, 2, 3, 7 });
                    lava_columns = new List<int>(new int[] { 4, 3, 3, 4, 2, 2, 0, 0 });

                    hoop_row = 5;
                    hoop_column = 0;

                    set_rows = 11;
                    set_columns = 7;

                    Camera.main.GetComponent<CameraShake>().SetNewPosition(new Vector3(-8.41f, 2.2f, -10f));
                }
                break;
            case 3:
                break;
            default:
                Debug.LogError("Trying to generate field for unknown level");
                break;
        }
    }

    public void GenerateField()
    {
        RemoveOldObjects();
        SetupBasedOnLevel();

        give_ball_ind = Random.Range(0, 5);

        for (int i = 0; i < set_rows; i++)
        {
            for (int j = 0; j < set_columns; j++)
            {
                GameObject new_tile;
                if (IsLavaTile(i, j) && Random.Range(0, 100) < 60)
                {
                    new_tile = Instantiate(lava_tile_prefab, transform);
                }
                else
                {
                    new_tile = Instantiate(tile_prefab, transform);
                }
                new_tile.GetComponent<SpriteRenderer>().sprite = tile_sprites[Random.Range(0, tile_sprites.Length)];

                // Set final position, and then move tile upwards so it can drop into place
                Vector3 tile_final_position = new Vector3(j * -1.06f + i * -1.04f, j * -0.36f + i * 0.36f, j * -0.01f + i * 1f);
                new_tile.transform.position = tile_final_position;
                new_tile.GetComponent<FallIntoPlace>().SetFinalPosition(tile_final_position);
                new_tile.GetComponent<FallIntoPlace>().FallSoon();

                new_tile.name = "Tile " + i.ToString() + "," + j.ToString();
                new_tile.GetComponent<Tile>().position = new Vector2(i, j);

                all_objects.Add(new_tile);

                if (i == hoop_row && j == hoop_column)
                {
                    GameObject new_hoop = Instantiate(hoop_prefab, new_tile.transform);

                    Vector3 hoop_final_position = new_hoop.transform.position;
                    new_hoop.transform.Translate(new Vector3(0, 10, 0), Space.World);
                    new_hoop.GetComponent<FallIntoPlace>().SetFinalPosition(hoop_final_position);

                    new_hoop.GetComponent<Hoop>().current_tile = new_tile.GetComponent<Tile>();
                    new_hoop.GetComponent<SpriteRenderer>().flipX = Possession.team == Team.A ? true : false;
                    all_objects.Add(new_hoop);
                }

                for (int n = 0; n < offensive_player_rows.Count; n++)
                {
                    if (i == offensive_player_rows[n] && j == offensive_player_columns[n])
                    {
                        GameObject new_player = Instantiate(player_prefab, new_tile.transform);

                        Vector3 player_final_position = new_player.transform.position;
                        new_player.GetComponent<FallIntoPlace>().SetFinalPosition(player_final_position);

                        new_player.name = "Offender " + n.ToString();
                        new_player.GetComponent<Player>().current_tile = new_tile.GetComponent<Tile>();
                        new_player.GetComponent<Player>().team = Possession.team;
                        all_objects.Add(new_player);

                        if (n == give_ball_ind)
                        {
                            GameObject new_ball = Instantiate(ball_prefab, new_player.transform);
                            all_objects.Add(new_ball);

                            // Start highlight on this tile
                            GameObject highlight = Instantiate(highlight_prefab, new_tile.transform);
                            all_objects.Add(highlight);
                        }

                        new_player.transform.Translate(new Vector3(0, 10, 0), Space.World);
                    }
                }

                for (int n = 0; n < defensive_player_rows.Count; n++)
                {
                    if (i == defensive_player_rows[n] && j == defensive_player_columns[n])
                    {
                        GameObject new_player = Instantiate(player_prefab, new_tile.transform);

                        Vector3 player_final_position = new_player.transform.position;
                        new_player.GetComponent<FallIntoPlace>().SetFinalPosition(player_final_position);
                        new_player.transform.Translate(new Vector3(0, 10, 0), Space.World);

                        new_player.name = "Defender " + n.ToString();
                        new_player.GetComponent<Player>().current_tile = new_tile.GetComponent<Tile>();
                        new_player.GetComponent<Player>().team = GetDefensiveTeam();
                        all_objects.Add(new_player);
                    }
                }

                // For the fall
                new_tile.transform.Translate(new Vector3(0, 10, 0), Space.World);
            }
        }        

        foreach (GameObject tile in all_objects)
        {
            if (tile.GetComponent<Tile>() != null)
            {
                tile.GetComponent<Tile>().SetAdjacency();
            }
        }

        Tile hoop_tile = null;
        foreach (GameObject hoop in all_objects)
        {
            if (hoop.GetComponent<Hoop>() != null)
            {
                hoop_tile = hoop.GetComponent<Hoop>().current_tile;
            }
        }

        Tile ball_tile = null;
        foreach (GameObject ball in all_objects)
        {
            if (ball.GetComponent<Ball>() != null)
            {
                ball_tile = ball.transform.parent.GetComponent<Player>().current_tile;
            }
        }

        foreach (GameObject player in all_objects)
        {
            if (player.GetComponent<Player>() != null)
            {
                if (player.GetComponent<Player>().team == Possession.team)
                {
                    // Offensive facing
                    player.GetComponent<Player>().DetermineSpriteFacing(player.GetComponent<Player>().current_tile, hoop_tile);
                }
                else
                {
                    // Defensive facing
                    player.GetComponent<Player>().DetermineSpriteFacing(player.GetComponent<Player>().current_tile, ball_tile);
                }
            }
        }
    }

    bool IsLavaTile(int i, int j)
    {
        for (int n = 0; n < lava_rows.Count; n++)
        {
            if (lava_rows[n] == i && lava_columns[n] == j)
            {
                return true;
            }
        }
        return false;
    }

    void RemoveOldObjects()
    {
        foreach (GameObject obj in all_objects)
        {
            Destroy(obj);
        }
        all_objects.Clear();
    }

    Team GetDefensiveTeam()
    {
        if (Possession.team == Team.A)
        {
            return Team.B;
        }
        else
        {
            return Team.A;
        }
    }
}