using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class b_downwardFireAttacks : BHIII_bullet, IPoolerObj
{
    public SpriteRenderer SPR;
    public s_animhandler anim;
    void IPoolerObj.SpawnStart()
    {
        _Z_offset = 325;
        gravity = 0;
        grounded = false;
        //print("SpawnStart");
        timer = 0.6f;
        collision = GetComponent<BoxCollider2D>();
        collision.enabled = false;
    }

    new void Start()
    {
        collision = GetComponent<BoxCollider2D>();
        collision.enabled = false;
        base.Start();
    }

    new void Update()
    {
        base.Update();
        SPR.gameObject.transform.position = transform.position + new Vector3(0,_Z_offset,0);
        if (grounded) {
            timer -= Time.deltaTime;
            if(timer <=0) 
                DespawnObject();
        }
        anim.SetAnimation("none", true);
    }

    IEnumerator BlowUP() {

        anim.SetAnimation("break", true);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(50, 50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-50, 50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(50, -50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-50, -50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, 25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, 25), Quaternion.identity);

        collision.enabled = true;
        yield return new WaitForSeconds(0.9f);
        DespawnObject();
    }

    public override void OnGround()
    {
        StartCoroutine(BlowUP());
    }

    public override void OnImpact()
    {
        base.OnImpact();
    }
}
