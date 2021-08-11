using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class o_itemObject : o_itemObj {

    public enum ITEM_TYPE {
        RECOVER,
        MAX_HEALTH,
        MAX_AMMO,
        WEAPON
    }
    public ITEM_TYPE itType;
    public string weaponName;
    public override void CollectItem(o_character ob)
    {
        BHIII_character p = ob.GetComponent<BHIII_character>();
        if (p != null)
        {
            if (!p.AI) {

                switch (itType)
                {
                    case ITEM_TYPE.MAX_AMMO:
                        o_plcharacter pl = ob.GetComponent<o_plcharacter>();
                        if (pl != null) {
                            pl.maxAmmoPoints++;
                            base.CollectItem(ob);
                        }
                        else
                            return;
                        break;

                    case ITEM_TYPE.MAX_HEALTH:
                        p.maxHealth++;
                        s_soundmanager.sound.PlaySound("health_increase");
                        base.CollectItem(ob);
                        break;

                    case ITEM_TYPE.RECOVER:
                        if (p.health < p.maxHealth) {
                            s_soundmanager.sound.PlaySound("heal");
                            p.health++;
                            base.CollectItem(ob);
                        }
                        else
                            return;
                        break;
                    case ITEM_TYPE.WEAPON:
                        /*
                        switch (weaponName)
                        {
                            case "sgun":
                                ///I may make it so that shotguns work better up-close than far away, it has slow firing speed
                                p.weapons.Add(new o_weapon(7.5f, "Shotgun", o_weapon.WEAPON_TYPE.WEAPON_RANGED_NO_STAND, 5.8f));
                                break;
                            case "lazr":
                                ///This weapon's power is low, but it penetrates through and has great range 
                                p.weapons.Add(new o_weapon(2.5f, "Laser gun", o_weapon.WEAPON_TYPE.WEAPON_RANGED_NO_STAND, 4.5f));
                                break;
                            case "mchgun":
                                ///This weapon's power is very low, but it penetrates through and has great range 
                                p.weapons.Add(new o_weapon(0.5f, "Machine gun", o_weapon.WEAPON_TYPE.WEAPON_RANGED_NO_STAND, 0.3f));
                                break;

                        }
                        */
                        break;
                }
            }
        }
    }

}  

/*
public class o_itemObj : s_object {

    public int amount;

    public enum ITEM_TYPE
    {
        MONEY,
        HEALTH,
        MAX_HEALTH,
        AMMO,
        COLLECTIBLE, 
        WEAPON
    }
    public ITEM_TYPE it;
    public o_item ItemContain;
    public o_weapon WeaponContain;

    new void Start ()
    {
        base.Start();
    }
	
	new void Update ()
    {
        positioninworld = transform.position;

        Collider2D col = IfTouchingGetCol(collision, "Player");

        if (col != null)
        {
            o_plcharacter p = col.GetComponent<o_plcharacter>();

            if (p != null)
            {
                switch (it)
                {
                    case ITEM_TYPE.MONEY:
                        s_globals.Money += amount;
                        break;

                    case ITEM_TYPE.MAX_HEALTH:
                        p.maxHealth++;
                        break;

                    case ITEM_TYPE.COLLECTIBLE:
                        s_globals.AddItem(ItemContain);
                        break;

                    case ITEM_TYPE.WEAPON:
                        p.weapons.Add(WeaponContain);
                        break;
                }
                gameObject.SetActive(false);
            }
            //DespawnObject();
        }
	}
}

*/