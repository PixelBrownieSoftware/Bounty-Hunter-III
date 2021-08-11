using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class o_throwable : BHIII_character
{
    public bool pickable = true;

    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
        util_throwUnlock u = IfTouchingGetCol<util_throwUnlock>(collision);
        if (u) {
            if(!u.isOn)
                pickable = true;
        }
    }
}
