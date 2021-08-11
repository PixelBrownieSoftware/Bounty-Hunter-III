using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class util_door : s_utility
{

    public List<util_button> buttons;
    bool [] buttonsPressed;
    public Sprite[] sprites;

    public bool isUnlocked;

    private new void Start()
    {
        base.Start();
        InitializeButtons();
    }

    public void InitializeButtons()
    {
        buttonsPressed = new bool[buttons.Count];
    }

    public void FindSprite(string str) {
        foreach (Sprite spr in sprites)
        {
            if (spr.name == str)
            {
                rendererObj.sprite = spr;
                return;
            }
        }
    }

    new void Update()
    {
        base.Update();
        if(buttons != null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                util_button b = buttons[i];
                buttonsPressed[i] = b.isOn;
            }
            int numPressed = 0;
            for (int i = 0; i < buttonsPressed.Length; i++)
            {
                if (buttonsPressed[i])
                {
                    numPressed++;
                }
            }
            if (numPressed == buttonsPressed.Length)
            {
                isUnlocked = true;
            }
        }

        if (isUnlocked)
            Destroy(gameObject);
    }
}
