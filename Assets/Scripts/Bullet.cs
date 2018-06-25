using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


	void Start () {
        Destroy(gameObject, 3);
	}
	
	void Update () {
        transform.position += transform.forward * 35 * Time.deltaTime;
	}

    void OnTriggerEnter(Collider other)
    {
        var pl = other.GetComponent<Player>();

        if(!pl.hasAuthority)
        {
            GameManager.instancia.TheAutority.CmdEmpuje(pl.index, this.gameObject.transform.forward); 
        }
        
        Destroy(gameObject);
    }
}
