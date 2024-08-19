//using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVRDistanceGrabbable : SVRGrabbable
{
    Renderer renderer;
    //MaterialPropertyBlock mpb;
    Material mat;

    public bool InRange
    {
        get { return m_inRange; }
        set
        {
            m_inRange = value;
            RefreshHighlight();
        }
    }

    bool m_inRange;

    public bool Targeted
    {
        get { return m_targeted; }
        set
        {
            m_targeted = value;
            RefreshHighlight();
        }
    }

    bool m_targeted;

    protected override void Start()
    {
        base.Start();
        renderer = gameObject.GetComponent<Renderer>();
        mat = renderer.material;
        //mpb = new MaterialPropertyBlock();
        //RefreshHighlight();
        //renderer.SetPropertyBlock(mpb);
    }

    void RefreshHighlight()
    {
        //renderer.GetPropertyBlock(mpb);
        if (isGrabbed || !InRange)
        {
            renderer.material.SetFloat("_HighlightState", 0.0f);
            //mpb.SetFloat("_HighlightState", 0.0f);
        }
        else if (Targeted)
        {
            renderer.material.SetFloat("_HighlightState", 0.5f);
            //mpb.SetFloat("_HighlightState", 0.5f);
        }
        else
        {
            renderer.material.SetFloat("_HighlightState", 0.3f);
            //mpb.SetFloat("_HighlightState", 0.3f);
        }
        //renderer.SetPropertyBlock(mpb);
    }
}
