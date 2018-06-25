using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Meta : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        var player = other.gameObject.GetComponent<Player>();

        player.CmdLlegoALaMeta(player.index + 1);
        Console.WriteLine("el player es: " + player.index);
    }
}
