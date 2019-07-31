using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEventReporter : MonoBehaviour
{
    // Update is called once per frame
    public void ThrowStart()
    {
        EventManager.TriggerEvent<ThrowStartEvent>();
    }

    public void ThrowRelease()
    {
        EventManager.TriggerEvent<ThrowReleaseEvent>();
    }
}
