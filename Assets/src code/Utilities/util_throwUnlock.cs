using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class util_throwUnlock : s_utility
{
    public bool isOn;
    public util_door door;

    // Update is called once per frame
    new void Update()
    {
        if (isOn) {
            door.isUnlocked = true;
        }
    }
}
