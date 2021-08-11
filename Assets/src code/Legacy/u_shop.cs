using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoudation;

[System.Serializable]
public class o_item
{
    public o_item(string name, ITEM_TYPE type, int points)
    {
        this.name = name;
        this.points = points;
        TYPE = type;
    }
    public o_item(string name, ITEM_TYPE type)
    {
        this.name = name;
        TYPE = type;
    }
    public string name;
    public enum ITEM_TYPE
    {
        CONSUMABLE,
        WEAPON,
        KEY_ITEM
    }
    public ITEM_TYPE TYPE;
    public int points;  //This could be how much health it heals or how much damage it deals
    public int quantity;
};

[System.Serializable]
public class o_weapon : o_item
{
    public o_weapon(float attackPow, string name, WEAPON_TYPE weap, float ammoConsumption) : base(name, ITEM_TYPE.WEAPON,1)
    {
        this.attackPow = attackPow;
        weapon_type = weap;
        this.ammoConsumption = ammoConsumption;
    }
    public enum WEAPON_TYPE
    {
        WEAPON_MELEE,
        WEAPON_RANGED,
        WEAPON_RANGED_NO_STAND
    }
    public WEAPON_TYPE weapon_type;
    public float attackPow;
    public float ammoConsumption;
    public bool ignoreUserFaction;
};

[System.Serializable]
public struct o_shopItem
{
    public o_shopItem(o_item item, int price)
    {
        this.item = item;
        this.price = price;
    }
    public o_item item;
    public int price;
}


public class u_shop : s_utility {
    //Items

    public List<o_shopItem> items = new List<o_shopItem>();

    s_gui Gui;
    o_plcharacter chara;
    Text Txt;

    private new void Start()
    {
        base.Start();
        Txt = GameObject.Find("ShopText").GetComponent<Text>();
        Gui = GameObject.Find("General").GetComponent<s_gui>();
        chara = GameObject.Find("Player").GetComponent<o_plcharacter>();
        
        /*
        items.Add( new o_shopItem(new o_item("Kaj's magazine", o_item.ITEM_TYPE.KEY_ITEM), 5));
        items.Add(new o_shopItem(new o_item("Hamlet's costume", o_item.ITEM_TYPE.KEY_ITEM), 10));
        items.Add(new o_shopItem(new o_item("Nina's microphone", o_item.ITEM_TYPE.KEY_ITEM), 15));
        items.Add(new o_shopItem(new o_item("Loki's cushion", o_item.ITEM_TYPE.KEY_ITEM), 15));
        items.Add(new o_shopItem(new o_item("Okami's stew", o_item.ITEM_TYPE.CONSUMABLE, 2), 15));
        items.Add(new o_shopItem(new o_item("Sinro's cookie", o_item.ITEM_TYPE.CONSUMABLE, 1), 5));
        items.Add(new o_shopItem(new o_item("Carl's diary", o_item.ITEM_TYPE.KEY_ITEM), 20));
        items.Add(new o_shopItem(new o_item("Nak's cape", o_item.ITEM_TYPE.KEY_ITEM), 3));
        */

    }

    public enum SHOPSTATES
    {
        INITIALIZE,
        BUYING,
        EXIT
    }
    public SHOPSTATES SHOPSTATE = SHOPSTATES.INITIALIZE;

    public int menuchoice = 0;
    public int a = 0;

    public void Buy()
    {
        chara.weapons.Add((o_weapon)items[1].item);
    }

    public void AddItem(string itemname, int price, int type)
    {
        items.Add(new o_shopItem(new o_item(itemname, (o_item.ITEM_TYPE)type), price));
    }

    public new void Update()
    {
        switch (SHOPSTATE)
        {
            case SHOPSTATES.BUYING:

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    menuchoice += 1;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    menuchoice -= 1;
                }
                menuchoice = Mathf.Clamp(menuchoice, 0, items.Count - 1);


                Txt.text = "";
                for (int i = 0; i < items.Count; i++)
                {
                    o_shopItem it = items[i];
                    if (it.price > s_globals.Money)
                        Txt.text += "<color=red>";
                    if (i == menuchoice)
                        Txt.text += "-> ";
                    Txt.text += "Item: " + it.item.name + " Price: " + it.price;
                    if (it.price > s_globals.Money)
                        Txt.text += "</color>";
                    Txt.text += "\n";
                }
                Txt.text += "\n";
                Txt.text += "Press Z to purchase" + "\n";
                Txt.text += "Press X to quit";

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (items[menuchoice].price <= s_globals.Money)
                    {
                       // s_globals.AddItem(items[menuchoice].item);
                        s_globals.Money -= items[menuchoice].price;
                    }
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    Txt.text = "";
                    chara.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
                }
                break;
        }
    }
    /*

    public override IEnumerator EventTrigger()
    {
        //print(a);
        switch (SHOPSTATE)
        {
            case SHOPSTATES.INITIALIZE:
                a = 0;

                print(a);
                SHOPSTATE = SHOPSTATES.BUYING;
                break;

            case SHOPSTATES.BUYING:
                a = Gui.PickFromList(6);
                if (Input.GetKey(KeyCode.Space))
                {
                    Buy();
                }
                if (Input.GetKey(KeyCode.E))
                {
                    SHOPSTATE = SHOPSTATES.EXIT;
                }
                break;

            case SHOPSTATES.EXIT:
                eventState = 0;
                Gui.ResetData();
                yield return base.EventTrigger();
                SHOPSTATE = SHOPSTATES.INITIALIZE;
                eventState = 1;
                break;

        }
        yield return null;
    }
    */
}
