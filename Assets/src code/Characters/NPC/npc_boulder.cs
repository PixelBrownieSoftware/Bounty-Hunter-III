using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class npc_boulder : BHIII_character
{
    float boulderSpd = 120;
    public AudioClip destroy;

    new void Start()
    {
        health = maxHealth;

        terminalspd = 45f;
        base.Start();
        Initialize();
    }
    
    public override void OnHit(BHIII_bullet b)
    {
        //base.OnHit(b);
        if (b.parent.ID == "Milbert")
        {
            s_soundmanager.sound.PlaySound("destroy");
            Destroy(gameObject);
        }
    }

    new void FixedUpdate()
    {
        if (Mathf.Abs(rbody2d.velocity.x) > boulderSpd || 
            Mathf.Abs(rbody2d.velocity.y) > boulderSpd)
            EnableAttack();
        else
            DisableAttack();

        base.FixedUpdate();
    }

    new void Update()
    {
        base.Update();
    }
}
