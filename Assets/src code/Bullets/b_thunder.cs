using MagnumFoudation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class b_thunder : BHIII_bullet
{
    BoxCollider2D bx;

    new void Start()
    {
        base.Start();
        animHand = GetComponent<s_animhandler>();
    }

    new void Update()
    {
        SetAnimation("thunder",false);
        timer -= Time.deltaTime;
        if (timer <= 0)
            DespawnObject();
        base.Update();
    }


    public override void OnImpact()
    {
        base.OnImpact();
        DespawnObject();
    }
}
