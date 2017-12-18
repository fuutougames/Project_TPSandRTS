using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBase
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHightlightColor;
    public Color originalDotColor;

    protected override void OnStart()
    {
        base.OnStart();
        Cursor.visible = false;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHightlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
