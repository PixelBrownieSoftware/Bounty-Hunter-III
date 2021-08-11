using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

public class s_camera : MonoBehaviour {

    public static BHIII_character player;
    Vector2 offset;
    float offset_multiplier;
    Vector2 targetPos;
    Vector2 startPos;
    float speedProg = 0;

    public bool focus = true;
    public bool lerping = false;

    void Start () {
        SetPlayer(GameObject.Find("Player"));
	}

    public static void SetPlayer(BHIII_character cha)
    {
        player = cha;
    }

    public void CameraLerpInit(Vector2 _startpos, Vector2 _targetpos)
    {
        startPos = _startpos + (Vector2)new Vector3(0, 0, -15);
        targetPos = _targetpos + (Vector2)new Vector3(0,0,-15);
        speedProg = 0;
        lerping = true;
    }

    public bool CameraLerp()
    {
        if ((Vector2)transform.position == targetPos)
        {
            lerping = false;
            return true;
        }
        return false;
    }

    public static void SetPlayer(GameObject cha)
    {
        player = cha.GetComponent<BHIII_character>();
    }

    void FixedUpdate ()
    {
        if (lerping)
        {
            transform.position = Vector3.Lerp(transform.position , targetPos, speedProg) + new Vector3(0, 0, -15);
            speedProg += 0.15f * Time.deltaTime;
        }
        else
        {
            if (focus)
            {
                Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                offset = (mouse - player.transform.position).normalized;

                if (Vector2.Distance(mouse, player.transform.position) > 9)
                {
                    offset_multiplier = Vector3.Distance(player.transform.position, mouse) * 0.35f;
                }
                

                offset_multiplier = Mathf.Clamp(offset_multiplier, 0, 50f);
                offset *= offset_multiplier;
                Vector3 vec = Vector3.Lerp(player.transform.position + (Vector3)offset, transform.position, 0.6f);
                Vector3 newVec = new Vector3(vec.x, vec.y, -10);
                transform.position = newVec;
            }
        }


        //print(Vector2.Distance(mouse, player.transform.position));
        //print("offset mult = " + offset_multiplier);
        //print("offset = " + 
        //player.positioninworld + 
        //(Vector3)offset);

    }
}

    */