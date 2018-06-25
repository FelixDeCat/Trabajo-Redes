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
        var player = other.gameObject.GetComponent<Player>();

        if (player != null) {

            if (player.hasAuthority)
            {
                player.CmdRealizarAccion(player.index, (int)buff + 1, veltoModify);
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
