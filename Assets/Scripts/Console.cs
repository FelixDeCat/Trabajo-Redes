using UnityEngine.UI;
using UnityEngine;
public class Console : MonoBehaviour {
    static Text console;
    public static Console instancia;
    private void Awake() {
        instancia = this;
        console = this.gameObject.GetComponent<Text>();
    }
    public static void WriteLine(string s)
    {
        //console.text += "\n" + s;
    }
}
