using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class b_homingMissile : BHIII_bullet
{
    public BHIII_character target;
    public SpriteRenderer SPR;
    new void Start()
    {
        collision = GetComponent<BoxCollider2D>();
        collision.enabled = false;
        base.Start();
    }

    new void Update()
    {
        base.Update();
        direction = Vector3.RotateTowards(transform.position, target.transform.position, Mathf.PI * 0.5f, 1);
    }


    public override void OnImpact()
    {
        base.OnImpact();
    }
}
