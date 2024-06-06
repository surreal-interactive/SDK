using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
class TestSuite : InputTestFixture
{
    [Test]
    public void CanPressButtonOnGamepad()
    {
        var dm_input_device = InputSystem.AddDevice<DMInputDevice>();
    }
}
#endif
