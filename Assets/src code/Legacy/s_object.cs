using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

/*
public class s_object : MagnumFoudation.s_object
{
    public string ID; //To return back to the object pooler
    public GameObject shadow;
    public float Z_offset;  //To show the character jumping
    public int Z_floor;
    public BoxCollider2D collision;
    public Vector3 positioninworld;
    public Vector2 velocity;
    public s_nodegraph nodegraph;
    public float speed, terminalspd;
    public float gravity;
    public float height;
    protected s_object parentTrans;
    protected SpriteRenderer characterRenderer;

    public void Start ()
    {
        characterRenderer = GetComponent<SpriteRenderer>();
        collision = GetComponent<BoxCollider2D>();
    }
	
    public void DespawnObject()
    {
        ll_BHIII.LevEd.DespawnObject(this);
    }

    private void OnDrawGizmos()
    {
        if(collision != null)
        Gizmos.DrawWireCube(positioninworld, collision.size);
    }

    public s_node CheckNode(float x, float y)
    {
        return CheckNode(new Vector2(x, y));
    }
    public s_node CheckNode(Vector2 v)
    {
        if (nodegraph == null)
            return null;
        s_node no = nodegraph.PosToNode(v);
        if (no == null)
            return null;
        else
            return no;
    }
    protected void Update ()
    {
        if (Mathf.Abs(velocity.y) > terminalspd)
            velocity.y = terminalspd * Mathf.Sign(velocity.y);

        if (Mathf.Abs(velocity.x) > terminalspd)
            velocity.x = terminalspd * Mathf.Sign(velocity.x);

        velocity.x *= 0.89f;
        velocity.y *= 0.89f;
        if (Mathf.Abs(velocity.x) < 0.1f)
            velocity.x = 0;

        if (Mathf.Abs(velocity.y) < 0.1f)
            velocity.y = 0;

        if (shadow != null)
            shadow.transform.position = new Vector2(positioninworld.x, positioninworld.y);
    }

    internal bool IfTouching(BoxCollider2D collisn)
    {
        if (collisn == null)
            return false;

        return Physics2D.OverlapBox(positioninworld, collisn.size, 0);
    }

    public Collider2D GetCharacter(BoxCollider2D collisn) { return Physics2D.OverlapBox(positioninworld, collisn.size, 0); }
    public Collider2D GetCharacter(BoxCollider2D collisn, string name)
    {
        Collider2D col = Physics2D.OverlapBox(positioninworld, collisn.size, 0);
        if (col == null)
            return null;

        if (col.name == name)
            return col;
        else return null;
    }
    public Collider2D GetCharacter(BoxCollider2D collisn, int lay)
    {
        return Physics2D.OverlapBox(positioninworld, collisn.size, 0, lay);
    }
    public Collider2D GetCharacter(BoxCollider2D collisn, int lay, string nameofobj)
    {
        Collider2D col = Physics2D.OverlapBox(positioninworld, collisn.size, 0, lay);

        if (col == null)
            return null;
        //print(col.name);
        if (col.name == nameofobj)
            return col;
        else return null;
    }

    public Collider2D GetAllCharacters(BoxCollider2D collisn) { return Physics2D.OverlapBox(positioninworld, collisn.size, 0); }

    internal bool IfTouching(BoxCollider2D collisn, string nameofobj)
    {
        Collider2D col = Physics2D.OverlapBox(positioninworld, collisn.size, 0);
        if (collisn == null)
            return false;

        if (col == null)
            return false;
        
        if (col.gameObject == gameObject)
            return false;
        if (col)
            return col.name == nameofobj;

        return false;
    }
    internal Collider2D IfTouchingGetCol(BoxCollider2D collisn, object characterdata)
    {
        if (collisn == null)
            return null;

        Collider2D[] chara = Physics2D.OverlapBoxAll(positioninworld, collisn.size, 0);

        if (chara == null)
            return null;
        for (int i = 0; i < chara.Length; i++)
        {
            Collider2D co = chara[i];
            if (co.gameObject == gameObject)
                continue;

            //print(chara.gameObject.GetComponent<s_object>().GetType());
            s_object obj = co.gameObject.GetComponent<s_object>();
            if (obj == null)
                continue;

            if (obj.GetType() == characterdata)
            {
               // print(co.name);
                return co;
            }
        }
        return null;
    }
    internal Collider2D IfTouchingGetCol(BoxCollider2D collisn, object characterdata, string character)
    {
        if (collisn == null)
            return null;

        Collider2D[] chara = Physics2D.OverlapBoxAll(positioninworld, collisn.size, 0);

        if (chara == null)
            return null;
        for (int i = 0; i < chara.Length; i++)
        {
            Collider2D co = chara[i];
            if (co.gameObject == gameObject)
                continue;

            //print(chara.gameObject.GetComponent<s_object>().GetType());
            s_object obj = chara[i].gameObject.GetComponent<s_object>();
            if (obj == null)
                continue;
            if (name == "attack_object" && transform.parent == GameObject.Find("Player").transform)
                print(co.name);
            if (obj.GetType() == characterdata)
            {
                if (obj.name == character)
                {
                   // 
                    return co;
                }
            }
        }
        return null;
    }

    internal Collider2D IfTouchingGetCol(BoxCollider2D collisn, float size_multip, int layer)
    {
        if (collisn == null)
            return null;

        return Physics2D.OverlapBox(positioninworld, collisn.size * size_multip, 0, layer);
    }

    internal bool IfTouching(BoxCollider2D collisn, object characterdata)
    {
        if (collisn == null)
            return false;

        Collider2D chara = Physics2D.OverlapBox(positioninworld, collisn.size, 0);

        if (chara == null)
            return false;
        if (chara.gameObject == gameObject)
            return false;

        //print(chara.gameObject.GetComponent<s_object>().GetType());
        s_object obj = chara.gameObject.GetComponent<s_object>();
        if (obj == null)
            return false;

        if (obj.GetType() == characterdata)
            return true;

        return false;
    }

    internal bool IfTouching(BoxCollider2D collisn, int layer)
    {
        if (collisn == null)
            return false;

        return Physics2D.OverlapBox(positioninworld, collisn.size, 0, layer);
    }
    internal bool IfTouching(BoxCollider2D collisn, float size_multip, int layer)
    {
        if (collisn == null)
            return false;

        return Physics2D.OverlapBox(positioninworld, collisn.size * size_multip, 0, layer);
    }

    internal bool IfTouching(BoxCollider2D collisn, int layer, string nameofobj)
    {
        if (collisn == null)
            return false;
        Collider2D cap = Physics2D.OverlapBox(positioninworld, collisn.size, 0, layer);

        if (cap == null)
            return false;

        if (cap == this)
            return false;

        if (cap.name == name)
            return cap;

        return false;
    }

}
    */
