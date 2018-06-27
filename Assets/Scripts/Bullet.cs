using UnityEngine;
public class Bullet : MonoBehaviour {
	void Start () { Destroy(gameObject, 3); }
	void Update () { transform.position += transform.forward * 35 * Time.deltaTime; }
    void OnTriggerEnter(Collider other) {

        var pl = other.GetComponent<NewPlayer>();

        if(!pl.HasAutority)
        {
            new PacketBase(PacketIDs.Cmd_BulletCollision)
                .Add(pl.ID.ToString())
                .Add(this.gameObject.transform.forward)
                .Send();
        }
        
        Destroy(gameObject);
    }
}
