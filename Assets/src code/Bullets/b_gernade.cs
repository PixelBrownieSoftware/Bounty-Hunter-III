using MagnumFoudation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class b_gernade : BHIII_bullet, IPoolerObj
{
    float bombTimer = 5f;
    public s_animhandler anim;

    void IPoolerObj.SpawnStart() {
        _Z_offset = 165;
        bombTimer = 5f;
        gravity = 0;
        collision = GetComponent<BoxCollider2D>();
        collision.enabled = false;
        StartCoroutine(BombFunction());
    }

    IEnumerator BombFunction() {

        anim.SetAnimation("fireball", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetAnimation("fireball", true, 0.5f);
        yield return new WaitForSeconds(0.3f);
        anim.SetAnimation("fireball", true, 0.1f);
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 3; i++)
        {
            s_mapManager.LevEd.SpawnObject<o_particle>(
                "Explosion",
                transform.position + new Vector3(Random.Range(-120, 120), Random.Range(-120, 120)),
                Quaternion.identity);
        }
        StartCoroutine(Detonate());
    }

    IEnumerator Detonate() {

        collision.enabled = true;
        yield return new WaitForSeconds(0.5f);
        DespawnObject();
    }


    public void PrematureDestroy() {

        StopCoroutine(BombFunction());
        StartCoroutine(Detonate());
    }



    new void Start()
    {
        _Z_offset = 165;
        gravity = 0;
        collision = GetComponent<BoxCollider2D>();
        collision.enabled = false;
        base.Start();
    }

    new void Update()
    {
        rendererObj.gameObject.transform.position = transform.position + new Vector3(0, _Z_offset, 0);
        if (grounded)
        {
            if (bombTimer > 0)
                bombTimer -= Time.deltaTime;
        }
        base.Update();
        
        /*
        if (bombTimer > 3) {
            //Clickanim1
        }
        else if (bombTimer > 2) {

        }
        else if (bombTimer > 1) {

        }
        else if (bombTimer < 1) {
            collision.enabled = true;
            CheckCharacterIntersect<o_character>();
        }
        else if (bombTimer < 0) {
            DespawnObject();
        }
        */
    }
    public override void OnImpact()
    {
        base.OnImpact();
        DespawnObject();
    }
}
