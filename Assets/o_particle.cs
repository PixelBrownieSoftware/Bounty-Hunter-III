using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class o_particle : s_object
{
    public ParticleSystem part;
    ParticleSystemRenderer partRend;
    Sprite spr;
    public BHIII_character parent;

    public void SetSprite(SpriteRenderer renderThing) {
        spr = renderThing.sprite;
    }

    public void StartThing(float rotation)
    {
        if (rotation == 180)
            partRend.flip = new Vector3(1, 1, 1);
        else
            partRend.flip = new Vector3(0, 0, 0);
        if (part.isStopped)
            part.Play();
    }

    private new void Start()
    {
        base.Start();
        part = GetComponent<ParticleSystem>();
        partRend = GetComponent<ParticleSystemRenderer>();
    }

    private new void Update()
    {
        if(parent != null)
            transform.position = parent.transform.position;
        part.textureSheetAnimation.SetSprite(0, spr);
    }

    public void Desp()
    {
        if(part.isPlaying)
            part.Stop();
    }
}
