﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Meta : MonoBehaviour {


    private void OnTriggerEnter(Collider other) {

        var player = other.gameObject.GetComponent<NewPlayer>();

        if (player != null)
        {
            new PacketBase(PacketIDs.Cmd_ReciveWinner).Add(player.ID.ToString()).Send();
        }
    }
}
