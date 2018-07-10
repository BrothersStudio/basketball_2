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

    public int hoop_row;
    public int hoop_column;

    bool gave_ball = false;
    public List<int> team_A_player_rows;
    public List<int> team_A_player_columns;

    public List<int> team_B_player_rows;
    public List<int> team_B_player_columns;

    void Awake ()
    {
		for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject new_tile = Instantiate(tile_prefab, transform);
                new_tile.GetComponent<SpriteRenderer>().sprite = tile_sprites[Random.Range(0, tile_sprites.Length)];

                new_tile.transform.SetPositionAndRotation(new Vector3(j * -1.06f + i * -1.04f, j * -0.36f + i * 0.36f, j * -0.01f + i * 1f), Quaternion.identity);
                new_tile.name = "Tile " + i.ToString() + "," + j.ToString();
                new_tile.GetComponent<Tile>().current_location = new Vector2(i, j);

                if (i == hoop_row && j == hoop_column)
                {
                    GameObject new_hoop = Instantiate(hoop_prefab, new_tile.transform);
                    new_hoop.GetComponent<Hoop>().current_tile = new_tile.GetComponent<Tile>();
                }

                for (int n = 0; n < team_A_player_rows.Count; n++)
                {
                    if (i == team_A_player_rows[n] && j == team_A_player_columns[n])
                    {
                        GameObject new_player = Instantiate(player_prefab, new_tile.transform);
                        new_player.GetComponent<Player>().current_tile = new_tile.GetComponent<Tile>();
                        new_player.GetComponent<Player>().team = Team.A;

                        if (!gave_ball)
                        {
                            gave_ball = true;
                        }
                        else
                        {
                            Destroy(new_player.transform.GetChild(0).gameObject);
                        }
                    }
                }

                for (int n = 0; n < team_B_player_rows.Count; n++)
                {
                    if (i == team_B_player_rows[n] && j == team_B_player_columns[n])
                    {
                        GameObject new_player = Instantiate(player_prefab, new_tile.transform);
                        new_player.GetComponent<Player>().current_tile = new_tile.GetComponent<Tile>();
                        new_player.GetComponent<Player>().team = Team.B;
                        new_player.GetComponent<SpriteRenderer>().color = Color.red;

                        Destroy(new_player.transform.GetChild(0).gameObject);
                    }
                }
            }
        }
	}
}
