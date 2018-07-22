using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public GameObject ball_prefab;
    public GameObject tile_prefab;
    public GameObject hoop_prefab;
    public GameObject player_prefab;
    public GameObject highlight_prefab;

    public int rows;
    public int columns;
    public Sprite[] tile_sprites;
    List<GameObject> all_objects = new List<GameObject>();

    // Team A Offense
    public List<int> offensive_A_player_rows;
    public List<int> offensive_A_player_columns;

    public List<int> defensive_A_player_rows;
    public List<int> defensive_A_player_columns;

    int A_field_hoop_row = 6;
    int A_field_hoop_column = 5;

    // Team B Offense
    public List<int> offensive_B_player_rows;
    public List<int> offensive_B_player_columns;

    public List<int> defensive_B_player_rows;
    public List<int> defensive_B_player_columns;

    int B_field_hoop_row = 5;
    int B_field_hoop_column = 0;

    public int set_rows;
    public int set_columns;

    public void GenerateField(int setup)
    {
        RemoveOldObjects();

        Team offensive_team;
        Team defensive_team;

        List<int> offensive_player_rows;
        List<int> offensive_player_columns;
        List<int> defensive_player_rows;
        List<int> defensive_player_columns;

        int hoop_row;
        int hoop_column;

        int gave_ball_ind;

        if (setup == 0)
        {
            offensive_player_rows = offensive_A_player_rows;
            offensive_player_columns = offensive_A_player_columns;

            defensive_player_rows = defensive_A_player_rows;
            defensive_player_columns = defensive_A_player_columns;

            hoop_row = A_field_hoop_row;
            hoop_column = A_field_hoop_column;

            set_rows = rows;
            set_columns = columns;

            offensive_team = Team.A;
            defensive_team = Team.B;

            gave_ball_ind = 1;

            Camera.main.transform.position = new Vector3(-8.41f, 0.75f, -10f);
        }
        else
        {
            offensive_player_rows = offensive_B_player_rows;
            offensive_player_columns = offensive_B_player_columns;

            defensive_player_rows = defensive_B_player_rows;
            defensive_player_columns = defensive_B_player_columns;

            hoop_row = B_field_hoop_row;
            hoop_column = B_field_hoop_column;

            set_rows = columns;
            set_columns = rows;

            offensive_team = Team.B;
            defensive_team = Team.A;

            gave_ball_ind = 0;

            Camera.main.transform.position = new Vector3(-8.41f, 2.2f, -10f);
        }

        Possession.team = offensive_team;
        bool gave_ball = false;

        for (int i = 0; i < set_rows; i++)
        {
            for (int j = 0; j < set_columns; j++)
            {
                GameObject new_tile = Instantiate(tile_prefab, transform);
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
                    new_hoop.GetComponent<SpriteRenderer>().flipX = setup == 0 ? true : false;
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
                        new_player.GetComponent<Player>().team = offensive_team;
                        all_objects.Add(new_player);
                        if (defensive_team == Team.A)
                        {
                            new_player.GetComponent<SpriteRenderer>().color = Color.blue;
                        }

                        if (!gave_ball && n == gave_ball_ind)
                        {
                            gave_ball = true;

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
                        new_player.GetComponent<Player>().team = defensive_team;

                        if (defensive_team == Team.B)
                        {
                            new_player.GetComponent<SpriteRenderer>().color = Color.blue;
                        }

                        all_objects.Add(new_player);
                    }
                }

                // For the fall
                new_tile.transform.Translate(new Vector3(0, 10, 0), Space.World);
            }
        }        

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.SetAdjacency();
        }

        Tile hoop_tile = FindObjectOfType<Hoop>().current_tile;
        Tile ball_tile = FindObjectOfType<Ball>().transform.parent.GetComponent<Player>().current_tile;
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.team == Possession.team)
            {
                // Offensive facing
                player.DetermineSpriteFacing(player.current_tile, hoop_tile);
            }
            else
            {
                // Defensive facing
                player.DetermineSpriteFacing(player.current_tile, ball_tile);
            }
        }
    }

    void RemoveOldObjects()
    {
        foreach (GameObject obj in all_objects)
        {
            Destroy(obj);
        }
        all_objects.Clear();
    }
}
