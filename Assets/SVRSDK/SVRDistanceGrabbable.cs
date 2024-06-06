//using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVRDistanceGrabbable : SVRGrabbable
{


    public bool InRange
    {
        get { return m_inRange; }
        set
        {
            m_inRange = value;
        }
    }

    bool m_inRange;

    public bool Targeted
    {
        get { return m_targeted; }
        set
        {
            m_targeted = value;
        }
    }

    bool m_targeted;

    protected override void Start()
    {
        base.Start();
    }
}
