using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class util_button : s_utility
{
    public bool isMilbertButton;
    public bool isOn;
    public Sprite[] sprites;
    public string labelCall;

    public new void Update()
    {
        base.Update();
        if (isMilbertButton)
        {
            pl_milbert mb = IfTouchingGetCol<pl_milbert>(collision);
            if (mb != null)
                isOn = true;
            else
                isOn = false;

            if (isOn)
                rendererObj.sprite = sprites[3];
            else
                rendererObj.sprite = sprites[2];
        }
        else
        {
            BHIII_character c = IfTouchingGetCol<BHIII_character>(collision);
            if (c != null)
                isOn = true;

            //else isOn = false;

            if (isOn)
                rendererObj.sprite = sprites[1];
            else
                rendererObj.sprite = sprites[0];
        }
    }
}
