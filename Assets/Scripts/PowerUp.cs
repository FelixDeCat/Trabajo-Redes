using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public enum Buffeo { Velocidad = 0, reduccion = 1, noDispara = 2, Jump = 3 }
    public Buffeo buff;

    Collider myCol;
    public GameObject gopart;

    public float veltoModify = 7;

    private void Awake()
    {
        myCol = gameObject.GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Console.WriteLine("Lo colisiono alguien: power Up =>" + buff.ToString());

        var player = other.gameObject.GetComponent<NewPlayer>();

        Console.WriteLine("Lo colisiono alguien: power Up =>" + buff.ToString());

        if (player != null) {

            Console.WriteLine("Player no es null");

            if (player.HasAutority)
            {
                Console.WriteLine("Player Tiene autoridad");

                // player.CmdRealizarAccion(player.ID, (int)buff + 1, veltoModify);
                player.RealizarAccion((int)buff + 1, veltoModify);
                myCol.enabled = false;
                gopart.SetActive(false);
                Invoke("Reactivar", 7.5f);
            }
        }
    }

    void Reactivar()
    {
        myCol.enabled = true;
        gopart.SetActive(true);
    }
}
