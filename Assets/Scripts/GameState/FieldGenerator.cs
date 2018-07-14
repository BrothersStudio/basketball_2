using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public GameObject tile_prefab;
    public GameObject hoop_prefab;
    public GameObject player_prefab;

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


    void Awake ()
    {
        // TODO: Some kind of coin flip or layup to determine initial possession?
        GenerateField(0);
	}

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

        int set_rows;
        int set_columns;

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

            Camera.main.transform.position = new Vector3(-8.41f, 0.33f, -10f);
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

            Camera.main.transform.position = new Vector3(-8.41f, 1.21f, -10f);
        }

        Possession.team = offensive_team;
        bool gave_ball = false;

        for (int i = 0; i < set_rows; i++)
        {
            for (int j = 0; j < set_columns; j++)
            {
                GameObject new_tile = Instantiate(tile_prefab, transform);
                new_tile.GetComponent<SpriteRenderer>().sprite = tile_sprites[Random.Range(0, tile_sprites.Length)];

                new_tile.transform.SetPositionAndRotation(new Vector3(j * -1.06f + i * -1.04f, j * -0.36f + i * 0.36f, j * -0.01f + i * 1f), Quaternion.identity);
                new_tile.name = "Tile " + i.ToString() + "," + j.ToString();
                new_tile.GetComponent<Tile>().position = new Vector2(i, j);

                all_objects.Add(new_tile);

                if (i == hoop_row && j == hoop_column)
                {
                    GameObject new_hoop = Instantiate(hoop_prefab, new_tile.transform);
                    new_hoop.GetComponent<Hoop>().current_tile = new_tile.GetComponent<Tile>();
                    all_objects.Add(new_hoop);
                }

                for (int n = 0; n < offensive_player_rows.Count; n++)
                {
                    if (i == offensive_player_rows[n] && j == offensive_player_columns[n])
                    {
                        GameObject new_player = Instantiate(player_prefab, new_tile.transform);
                        new_player.GetComponent<Player>().current_tile = new_tile.GetComponent<Tile>();
                        new_player.GetComponent<Player>().team = offensive_team;
                        all_objects.Add(new_player);
                        if (defensive_team == Team.A)
                        {
                            new_player.GetComponent<SpriteRenderer>().color = Color.red;
                        }

                        if (!gave_ball)
                        {
                            gave_ball = true;

                            // Add the ball so we can cleanly destroy it later
                            all_objects.Add(new_player.transform.GetChild(0).gameObject); 
                        }
                        else
                        {
                            Destroy(new_player.transform.GetChild(0).gameObject);
                        }
                    }
                }

                for (int n = 0; n < defensive_player_rows.Count; n++)
                {
                    if (i == defensive_player_rows[n] && j == defensive_player_columns[n])
                    {
                        GameObject new_player = Instantiate(player_prefab, new_tile.transform);
                        new_player.GetComponent<Player>().current_tile = new_tile.GetComponent<Tile>();
                        new_player.GetComponent<Player>().team = defensive_team;
                        Destroy(new_player.transform.GetChild(0).gameObject);  // Remove ball

                        if (defensive_team == Team.B)
                        {
                            new_player.GetComponent<SpriteRenderer>().color = Color.red;
                        }

                        all_objects.Add(new_player);
                    }
                }
            }
        }        

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.SetAdjacency();
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
