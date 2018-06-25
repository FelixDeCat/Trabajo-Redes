using System;
using UnityEngine;
using UnityEngine.UI;
public abstract class  Anim_GoBack : MonoBehaviour {
    [Header("Inheritance")]
    public float velocidad = 1;
    protected float timer;
    bool anim;
    bool go;
    [SerializeField] protected Text mensaje;
    public delegate void AQuienAviso();

    public bool evento_auxiliar;
    
    /// <summary> Se auto Vacía cuando termina la animacion </summary>
    public AQuienAviso _aQuienAviso = delegate { };
    public AQuienAviso _aQuienAviso_cuando_termine_todo = delegate { };
    public virtual void Animar() { anim = true; go = true; timer = 0; }
    public virtual void Animar(string msj) { anim = true; go = true; timer = 0; mensaje.text = msj; }
    public virtual void AnimarSalir() { anim = true; go = false; timer = 0; }
    protected virtual void Awake() { }
    protected virtual void Update() {
        if (anim) {
            if (timer < 1f) {
                timer = timer + velocidad * Time.deltaTime;
                if (go) OnAnimation_Go(); else OnAnimation_Back();
            }
            else {
                anim = false;
                timer = 0;
                _aQuienAviso();
                _aQuienAviso = delegate { };

                
                if (evento_auxiliar && !go)
                {
                    _aQuienAviso_cuando_termine_todo();
                    _aQuienAviso_cuando_termine_todo = delegate { };
                }
            }
        }
    }

    protected virtual void OnAnimation_Go() { }
    protected virtual void OnAnimation_Back() { }
}
