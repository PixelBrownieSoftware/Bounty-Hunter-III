using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class npc_breakable : BHIII_character
{
    new void Start()
    {
        nodegraph = GameObject.Find("General").GetComponent<s_nodegraph>();
        AI = true;
        maxHealth = 1;
        health = maxHealth;
        
        base.Start();
        Initialize();
    }

    new void Update()
    {
        base.Update();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnHit(BHIII_bullet b)
    {
        if (b.parent.ID == "Milbert")
            DespawnObject();
    }
}
