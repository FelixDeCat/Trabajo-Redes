using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject model;

    public static PlayerSpawner instancia;
    private void Awake() { instancia = this; }

    public NewPlayer SpawnPlayer(int id)
    {
        GameObject go = Instantiate(model);
        go.transform.localPosition = new Vector3(0, 0, 0);

        NewPlayer player = go.GetComponent<NewPlayer>();
        player.ID = id;

        return player;
    }

    public void FindAndDestroyGameObjects() {
        var todestroy = FindObjectsOfType<NewPlayer>();
        for (int i = 0; i < todestroy.Length; i++) Destroy(todestroy[i].gameObject);
    }

    public void SpawnPlayer(int id, int pos)
    {
        GameObject go = Instantiate(model);
        var spawn = GameManager.instancia.spawnpoint[pos];
        go.transform.localPosition = spawn.position;
        NewPlayer player = go.GetComponent<NewPlayer>();
        player.ID = id;
    }

    public void InitGame()
    {
        Console.WriteLine("Intentando iniciar juego");
        if (MultiplayerManager.instance.players.Count > 1 && MultiplayerManager.instance.players.Count < 5)
        {
            Console.WriteLine("Iniciando Juego....");

            var players = MultiplayerManager.instance.players;

            string col = "";

            for (int i = 0; i < players.Count; i++)
            {
                var spawn = GameManager.instancia.spawnpoint[i];
                players[i].transform.position = spawn.position;

                if (i == players.Count - 1)
                {
                    col += players[i].ID + "," + i;
                }
                else
                {
                    col += players[i].ID + "," + i + "-";
                }
            }

            Console.WriteLine("la col es: " + col);

            new PacketBase(PacketIDs.Instantiate_Players).Add(col).Send();
        }
    }
}
