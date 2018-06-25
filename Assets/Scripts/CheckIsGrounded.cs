using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsGrounded : MonoBehaviour {

    public bool isGrounded;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
