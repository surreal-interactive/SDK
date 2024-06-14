//using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVRDistanceGrabbable : SVRGrabbable
{
    Renderer renderer;
    MaterialPropertyBlock mpb;

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
        mpb = new MaterialPropertyBlock();
        RefreshHighlight();
        renderer.SetPropertyBlock(mpb);
    }

    void RefreshHighlight()
    {
        renderer.GetPropertyBlock(mpb);
        if (isGrabbed || !InRange)
        {
            mpb.SetFloat("_HighlightState", 0.0f);
        }
        else if (Targeted)
        {
            mpb.SetFloat("_HighlightState", 0.5f);
        }
        else
        {
            mpb.SetFloat("_HighlightState", 0.3f);
        }
        renderer.SetPropertyBlock(mpb);
    }
}
