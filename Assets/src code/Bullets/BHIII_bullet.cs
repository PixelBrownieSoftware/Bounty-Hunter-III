using MagnumFoudation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHIII_bullet : MagnumFoudation.o_bullet, IPoolerObj
{
    
    public float damageImpact_timer = 0;
    float initTimer;

    public bool avoidOnJump = false;
    public bool avoidOnDash = false;

    public bool isTimer = false;
    public bool isValidHit = true;

    public bool isDashingBullet = true;
    public delegate void OnBulletHitFunc();
    public OnBulletHitFunc func;

    new void Start()
    {
        initTimer = timer;
        base.Start();
    }

    void IPoolerObj.SpawnStart()
    {
        initTimer = timer;
    }

    new void Update()
    {
        base.Update();
        if (isTimer) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                DespawnObject();
            }
        }
        /*
        if (isbullet)
        {
            transform.Translate(direction * 7 * 60 * Time.deltaTime);

            CheckCharacterIntersect<BHIII_character>();
        }
        else
        {
            CheckCharacterIntersect<BHIII_character>();
        }
        */
    }

    public void OneTimeHit() {
        //print("YaH!");
        CheckCharacterIntersect<BHIII_character>();
    }
    public void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.layer == 9)
        {
            if (isbullet)
            {
                DespawnObject();
            }
        }

        BHIII_character b = col.GetComponent<BHIII_character>();
        if (b != null)
        {
            if (b.faction == parent.faction)
                return;

            if (b.CHARACTER_STATE != o_character.CHARACTER_STATES.STATE_DASHING && !b.isInvicible)
            {
                if (avoidOnJump)
                {
                    if (grounded)
                    {
                        if (func != null)
                            func.Invoke();
                        b.OnHit(this);
                    }
                }
                else
                {
                    if (func != null)
                        func.Invoke();
                    b.OnHit(this);
                }
            }
        }
        /*
        print("OK!");
        BHIII_bullet b = GetBullet<BHIII_bullet>(col.GetComponent<BoxCollider2D>());
        if (b != null)
        {
            if (CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING)
            {
                if (b.avoidOnJump)
                {
                    if (grounded)
                        OnHit(b);
                }
                else
                    OnHit(b);
            }
        }
        */
    }

    public override void CheckCharacterIntersect<T>()
    {
        base.CheckCharacterIntersect<T>();
    }

    private void OnDisable()
    {
        isValidHit = true;
    }

    public override void OnImpact()
    {
        base.OnImpact();
        if (parent != null)
        {
            if (parent.name == "Player")
                if (name == "attack_object")
                { 
                    parent.GetComponent<o_plcharacter>().ammoPoints += 0.2f;
                }
        }
        if(isbullet)
            DespawnObject();
    }
}
