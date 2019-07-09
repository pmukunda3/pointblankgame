using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEventReporter : MonoBehaviour
{
    // Update is called once per frame
    public void Throw()
    {
        EventManager.TriggerEvent<ThrowEvent>();
    }
}
