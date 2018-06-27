using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instancia;

    [Header("Layers")]
    public LayerMask layer_Player;
    public LayerMask layer_Floor;
    public LayerMask layer_Enemy;
    public Transform casifinal;

    public NewPlayer TheAutority;

    public GameObject bullet;
    public GameObject pantalla_de_espera;

    public Anim_Mensaje anim_mensaje;

    public Text txt_timer;
    public GameObject Cuadro;

    public Image bullet1;
    public Image bullet2;
    public Image bullet3;

    public Transform[] spawnpoint;

    private void Awake() { instancia = this; }


    public void PantallaDeEspera()
    {

    }

}
