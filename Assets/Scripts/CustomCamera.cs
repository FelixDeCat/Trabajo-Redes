using UnityEngine;
using System.Collections;
using System.Linq;

public class CustomCamera : MonoBehaviour
{

    public GameObject target;
    public float distance;
    public float height;
    public bool follow;
    private NewPlayer _player;

    bool optimize;

    void LateUpdate()
    {
        if (target == null)
        {
            var col = FindObjectsOfType<NewPlayer>().Where(x => x.ID == MultiplayerManager.connectionID).ToList();
            if (col.Count == 0) return;
            _player = col.First();
            if (_player) target = _player.gameObject;
            optimize = true;
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
