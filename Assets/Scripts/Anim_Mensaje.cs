using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Anim_Mensaje : Anim_GoBack {

    public RectTransform myRt;
    public Vector2 v_inside;
    public Vector2 v_outside;

    public Graphic[] grs;
    Color[] originals;

    Color transp = new Color(1, 1, 1, 0);

    public override void Animar()
    {
        base.Animar();
        _aQuienAviso += TerminoAnimacion;
    }

    void TerminoAnimacion()
    {
        AnimarSalir();
    }

    protected override void Awake()
    {
        base.Awake();
        originals = new Color[grs.Length];
        for (int i = 0; i < grs.Length; i++) originals[i] = grs[i].color;
    }

    protected override void OnAnimation_Go() {
        myRt.anchoredPosition = Vector2.Lerp(v_outside, v_inside, timer);
        for (int i = 0; i < grs.Length; i++) grs[i].color = Color.Lerp(transp, originals[i], timer);
    }
    protected override void OnAnimation_Back() {
        myRt.anchoredPosition = Vector2.Lerp(v_inside, v_outside, timer);
        for (int i = 0; i < grs.Length; i++) grs[i].color = Color.Lerp(originals[i], transp, timer);
    }
}
