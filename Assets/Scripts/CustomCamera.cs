using UnityEngine;
using System.Collections;

public class CustomCamera : MonoBehaviour
{

    public GameObject target;
    public float distance;
    public float height;
    public bool follow;
    private Player _player;


    void LateUpdate()
    {
        if (target == null)
        {
            _player = FindObjectOfType<Player>();
            if (_player) target = _player.gameObject;
            return;
        }

        if (follow)
        {
            transform.position = target.transform.position;
            transform.position -= target.transform.forward * distance;
            transform.position += Vector3.up * height;
        }

        transform.LookAt(target.transform);
    }
}
