using UnityEngine;
public class Bullet : MonoBehaviour {
	void Start () { Destroy(gameObject, 3); }
	void Update () { transform.position += transform.forward * 35 * Time.deltaTime; }
    void OnTriggerEnter(Collider other) {

        var pl = other.GetComponent<NewPlayer>();

        if(!pl.hasAuthority)
        {
            GameManager.instancia.TheAutority.CmdEmpuje(pl.index, this.gameObject.transform.forward); 
        }
        
        Destroy(gameObject);
    }
}
