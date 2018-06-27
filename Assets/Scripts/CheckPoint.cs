using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckPoint : MonoBehaviour {


    private void OnTriggerEnter(Collider other) {

        var player = other.gameObject.GetComponent<NewPlayer>();

        if (player != null)
        {
            player.mySpawnPosition = this.transform.position;
            player.mySpawnRotation = this.transform.rotation;
        }
    }
}
