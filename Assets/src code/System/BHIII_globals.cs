using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoudation;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class BHIII_globals : s_globals
{
    public List<BHIII_character> bossCharacters;
    public BHIII_character targetCharacter;
    public pl_milbert player2;
    public o_plcharacter playerPeast;

    public Image hurtFX;

    public static BHIII_globals gl;

    public bool bossDisplayOn = false;
    public List<o_character> bossChar;

    public Color peastColour;
    public Color milbertColour;

    bool camOnPeast = true;

    bool gameOverOn = false;

    public Camera cam;

    public Text MilbertGUI;
    public Text OyxgenMetre;

    public Texture bossHPBar;
    public Texture bossHPBarFull;
    public Texture ammobar;

    public static List<o_item> inventory = new List<o_item>();
    public static HashSet<o_weapon> weapons = new HashSet<o_weapon>();
    public static HashSet<o_item> inventory_unique
    { 
        get
        {
            HashSet<o_item> it = new HashSet<o_item>();
            foreach (o_item i in inventory)
            {
                it.Add(i);
            }
            return it;
        }
    }

    public static void AddItem(o_item it)
    {
            inventory.Add(it);
    }
    public static void RemoveItem(o_item it)
    {
        List<o_item> l = inventory.FindAll(x => x.name == it.name && x.TYPE == it.TYPE);
        foreach (o_item i in l)
        {
            inventory.Remove(i);
        }
    }
    public static void RemoveOneItem(o_item it)
    {
        o_item l = inventory.Find(x => x.name == it.name && x.TYPE == it.TYPE);
        inventory.Remove(l);
    }
    public static bool CheckItem(o_item it)
    {
        o_item i = inventory.Find(x => x.name == it.name && x.TYPE == it.TYPE);
        if (i == null)
            return false;
        else
            return true;
    }

    private new void Awake()
    {
        if (gl == null)
            gl = this;
        if (isMainGame)
        {
            if (GameObject.Find("Player") != null)
            {
                player = GameObject.Find("Player").GetComponent<o_plcharacter>();
                playerPeast = player.GetComponent<o_plcharacter>();
            }
            cam = GameObject.Find("CameraGame").GetComponent<Camera>();
        }
        base.Awake();
    }

    public void ClearBossCharacters() { bossCharacters.Clear(); }

    public void AddBossCharacter(string n) {
        bossCharacters.Add(GameObject.Find(n).GetComponent<BHIII_character>());
    }

    public void GameEnd()
    {
        gameOverOn = true;
        SetGlobalFlag("M_ACTIVE", 0);
        GetComponent<s_triggerhandler>().doingEvents = false;
        bossChar.Clear();
        player.gameObject.SetActive(false);
        gameObject.SetActive(false);
        SceneManager.LoadScene("ending");

        gameOverOn = false;
    }

    public void DefeatFX()
    {
        Time.timeScale = 0.001f;
        Time.fixedDeltaTime = Time.timeScale * .1f;
        StartCoroutine(DefeatEnemyFX());
    }

    public void DMGFX()
    {
        //print(Time.timeScale);
        Time.timeScale = 0.1f;
        //print("dt: " + Time.fixedDeltaTime);
        Time.fixedDeltaTime = Time.timeScale * .1f;
        StartCoroutine(DamageEffectPlayer());
    }

    public IEnumerator DamageEffectPlayer()
    {
        float hitfadetimer = 0.02f;
        float timer = 0.04f;
        hurtFX.enabled = true;
        while (timer > 0)
        {
            hitfadetimer -= Time.unscaledDeltaTime;
            timer -= Time.unscaledDeltaTime;
            if (hitfadetimer < 0) {
                hurtFX.enabled = false;
            }
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    public IEnumerator DefeatEnemyFX()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    public void GameOver(bool isPeast)
    {
        print("YOU ARE DEAD YOU DUMB FUCK");
        StartCoroutine(GameOverTime(isPeast));
    }

    public IEnumerator GameOverTime(bool isPeast) {

        gameOverOn = true;
        SetGlobalFlag("M_ACTIVE", 0);
        //SetGlobalFlag("W", 0);
        //yield return new WaitForSeconds(0.3f);
        //s_triggerhandler.trig = null;
        //s_mapManager.LevEd = null;
        //Destroy(s_camera.cam.gameObject);
        //s_camera.cam = null;
        Time.timeScale = 0;

        float t2 = 0;
        if (isPeast)
        {
            player.rendererObj.color = Color.white;
            s_soundmanager.sound.PlaySound("peast_defeat");
            player.SetAnimation("defeat", false);
            player.rendererObj.color = Color.white;
        }
        else
        {
            player2.rendererObj.color = Color.white;
            s_soundmanager.sound.PlaySound("milbert_defeat");
            player2.SetAnimation("defeat", false);
        }
        hurtFX.enabled = false;
        while (s_trig.trig.fade.color != Color.black)
        {
            t2 += Time.unscaledDeltaTime / 1.5f;
            s_trig.trig.fade.color = Color.Lerp(Color.clear, Color.black, t2);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        GetComponent<s_triggerhandler>().doingEvents = false;
        bossChar.Clear();
        player.transform.position = Vector2.zero;
        player.gameObject.SetActive(false);
        gameObject.SetActive(false);
        SceneManager.LoadScene("Title");

        isMainGame = false;
        s_camera.cam.mapSize = Vector2.zero;
        s_camera.cam.transform.position = new Vector3(0, 0, -10);
        Time.timeScale = 1;
        player.SetAnimation("idle_d", false);
        player.rendererObj.color = Color.white;

        gameOverOn = false;
    }

    new private void Update()
    {
        if (isMainGame)
        {
            if (MilbertGUI != null)
                MilbertGUI.text = "";
            if (!gameOverOn)
            {
                if (player != null)
                {
                    if (player.health <= 0)
                        GameOver(true);
                }
                if (player2 != null)
                {
                    if (player2.health <= 0)
                        GameOver(false);
                }
            }
            if (OyxgenMetre != null)
            {
                if (playerPeast.oyxegenAmount >= 100)
                    OyxgenMetre.text = "";
                else
                    OyxgenMetre.text = "Oxygen: " + playerPeast.oyxegenAmount;
            }
            //If player dies
            switch (GetGlobalFlag("M_ACTIVE")) 
            {
                case 0:

                    if (player2 != null)
                    {
                        if (player2.MB_STATE != pl_milbert.MILBERT_STATE.NONE)
                            player2.MB_STATE = pl_milbert.MILBERT_STATE.NONE;
                    }
                    MilbertGUI.text = "";
                    break;

                case 1:

                    if (player2 == null)
                        if(GameObject.Find("Milbert"))
                            player2 = GameObject.Find("Milbert").GetComponent<pl_milbert>();
                    if (player2 != null)
                    {
                        /*
                        if (!player2.control)
                        {
                            player2.MB_STATE = pl_milbert.MILBERT_STATE.AUTOMATIC;
                            player2.control = true;
                        }
                        */
                        if (player2.MB_STATE == pl_milbert.MILBERT_STATE.NONE)
                            player2.MB_STATE = pl_milbert.MILBERT_STATE.AUTOMATIC;

                        /*
                        if (MilbertGUI)
                        {
                            player2.IS_KINEMATIC = player.IS_KINEMATIC;
                            if (player2.CHARACTER_STATE != o_character.CHARACTER_STATES.STATE_DEFEAT)
                            {
                                if (Input.GetKeyDown(KeyCode.F))
                                {
                                    player2.MB_STATE++;
                                    if (player2.MB_STATE == (pl_milbert.MILBERT_STATE)2)
                                        player2.MB_STATE = pl_milbert.MILBERT_STATE.MANUAL;
                                }

                                switch (player2.MB_STATE)
                                {
                                    case pl_milbert.MILBERT_STATE.AUTOMATIC:
                                        player.control = true;
                                        MilbertGUI.text = "I'm following you. (Press F to change)" + "\n";
                                        break;

                                    case pl_milbert.MILBERT_STATE.MANUAL:
                                        player.control = false;
                                        MilbertGUI.text = "I'm on my own now (WASD to control + Press F to change)." + "\n";

                                        break;
                                }
                            }

                        }
                        */
                    }
                    break;

                case 2:
                    if (player2 == null)
                        player2 = GameObject.Find("Milbert").GetComponent<pl_milbert>();
                    else {

                        player.control = false;
                        player2.control = true;
                        s_camera.cam.SetPlayer(player2);
                        s_camera.cam.focus = true;
                        camOnPeast = false;
                        player2.MB_STATE = pl_milbert.MILBERT_STATE.MANUAL;

                    }
                    break;

            }
        }

    }

    public void DrawEnemyHP()
    {
        GUI.color = Color.white;
        switch (GetGlobalFlag("M_ACTIVE"))
        {
        
            case 0:
            case 1:
                if (player.targets != null)
                {
                    foreach (BHIII_character c in player.targets)
                    {
                        if (c == null)
                            continue;
                        if (!c.showHealth)
                            continue;
                        if (player.CheckTargetDistance(c, 450))
                        {
                            float offset = 765;
                            Vector3 po = cam.WorldToScreenPoint(c.transform.position); //for (int i = 0; i < c.health; i++)
                            for (int i = 0; i < c.maxHealth; i++)
                                GUI.DrawTexture(new Rect(po.x + (25 * i), -po.y
                                + offset
                                , 20, 20), bossHPBarFull);
                            for (int i = 0; i < c.health; i++)
                                GUI.DrawTexture(new Rect(po.x + (25 * i), -po.y
                                    + offset
                                    , 20, 20), bossHPBar);
                        }
                        else continue;
                    }
                }
                break;

            case 2:
                if (player2.targets != null)
                {
                    foreach (BHIII_character c in player2.targets)
                    {
                        if (c == null)
                            continue;
                        if (!c.showHealth)
                            continue;
                        if (player2.CheckTargetDistance(c, 450))
                        {
                            float offset = 765;
                            Vector3 po = cam.WorldToScreenPoint(c.transform.position); //for (int i = 0; i < c.health; i++)
                            for (int i = 0; i < c.maxHealth; i++)
                                GUI.DrawTexture(new Rect(po.x + (25 * i), -po.y
                                + offset
                                , 20, 20), bossHPBarFull);
                            for (int i = 0; i < c.health; i++)
                                GUI.DrawTexture(new Rect(po.x + (25 * i), -po.y
                                    + offset
                                    , 20, 20), bossHPBar);
                        }
                        else continue;
                    }
                }
                break;

        }
        /*
        */
    }

    /*
case pl_milbert.MILBERT_STATE.SEMI_AUTO:
    player.control = true;
    MilbertGUI.text = "Where should I go? (Press R key on spot + Press F to change)." + "\n";
    if (Input.GetKeyDown(KeyCode.R))
    {
        if (player2.milbertActionQueue.Count > 0)
        {
            if (player2.milbertActionQueue.Peek().type == pl_milbert.m_action.TYPE.FIGHT)
            {
                player2.milbertActionQueue.Dequeue();
                player2.target = null;
                // player2.SetAIFunction(0, player2.IdleState);
            }
        }
        player2.AddPoint();
    }
    if (player2.milbertActionQueue.Count > 0)
    {
        switch (player2.milbertActionQueue.Peek().type)
        {
            case pl_milbert.m_action.TYPE.FIGHT:
                MilbertGUI.text += player2.milbertActionQueue.Peek().obj.name + " will taste my fury!";
                break;

            case pl_milbert.m_action.TYPE.WALK:
                MilbertGUI.text += "I'm off to position: " + player2.milbertActionQueue.Peek().position;
                break;
        }
    }
    break;
    */
    public int testControlBossvar = 29;
    public int testControlBossvar2 = 35;

    private new void OnGUI()
    {
        if (isMainGame)
        {
            switch (GetGlobalFlag("M_ACTIVE"))
            {
                case 2:
                    if (player2 != null)
                    {
                        GUI.color = milbertColour;

                        for (int i = 0; i < player2.maxHealth; i++)
                            GUI.DrawTexture(new Rect(20 + (50 * i), 20, 40, 40), bossHPBarFull);

                        for (int i = 0; i < player2.health; i++)
                        {
                            if (player2.health / player2.maxHealth > 0.25f)
                                GUI.DrawTexture(new Rect(20 + (50 * i), 20, 40, 40), bossHPBar);
                            else
                            {
                                GUI.color = Color.red;
                                GUI.DrawTexture(new Rect(20 + (50 * i) + UnityEngine.Random.Range(-5, 5), 20 + UnityEngine.Random.Range(-5, 5), 40, 40), bossHPBar);
                            }
                        }
                        GUI.color = Color.white;
                    }
                    break;

                case 1:
                    GUI.color = milbertColour;
                    for (int i = 0; i < player2.maxHealth; i++)
                        GUI.DrawTexture(new Rect(300 + (50 * i), 20, 40, 40), bossHPBarFull);

                    for (int i = 0; i < player2.health; i++)
                        GUI.DrawTexture(new Rect(300 + (50 * i), 20, 40, 40), bossHPBar);

                    GUI.color = Color.white;
                    break;
            }
            switch (GetGlobalFlag("M_ACTIVE")) {
                case 0:
                case 1:
                    GUI.color = peastColour;

                    for (int i = 0; i < player.maxHealth; i++)
                        GUI.DrawTexture(new Rect(20 + (50 * i), 20, 40, 40), bossHPBarFull);

                    for (int i = 0; i < player.health; i++)
                    {
                        if (player.health / player.maxHealth > 0.25f)
                            GUI.DrawTexture(new Rect(20 + (50 * i), 20, 40, 40), bossHPBar);
                        else
                        {
                            GUI.color = Color.red;
                            GUI.DrawTexture(new Rect(20 + (50 * i) + UnityEngine.Random.Range(-5, 5), 20 + UnityEngine.Random.Range(-5, 5), 40, 40), bossHPBar);
                        }
                    }
                    GUI.color = Color.white;
                    for (int i = 0; i < playerPeast.maxAmmoPoints; i++)
                    {
                        float a = 1;
                        if ((int)playerPeast.ammoPoints == i)
                            a = (playerPeast.ammoPoints % 1);
                        GUI.color = Color.black;
                        GUI.DrawTexture(new Rect(20 + (45 * i), 60, 40, 40), ammobar);
                        if (Mathf.Ceil(playerPeast.ammoPoints) > i) {
                            GUI.color = Color.white;
                            GUI.DrawTexture(new Rect(20 + (45 * i), 60, 40, 40 * a), ammobar);
                        }
                    }
                    break;
            }

            DrawEnemyHP();

            if (bossDisplayOn)
            {
                if (bossChar != null)
                {
                    float maxHp = 0;
                    float hp = 0;
                    foreach (o_character bc in bossChar) {
                        maxHp += bc.maxHealth;
                        hp += bc.health;
                    }
                    float calc = 20;
                    if (maxHp > 0 && maxHp < 35)
                    {
                        calc = (60 / maxHp) * 10;
                        calc = Mathf.Clamp(calc, 20, testControlBossvar2);
                    }
                    int yposoff = 0;
                    for (int i = 0; i < maxHp; i++) {
                        int xpos = i % 35;
                        if (i % 35 == 0) 
                            yposoff++;
                        if(hp <= i)
                            GUI.DrawTexture(new Rect((30 * xpos), 430 + (yposoff * 20), 25, 25), bossHPBarFull);
                        else
                            GUI.DrawTexture(new Rect((30 * xpos), 430 + (yposoff * 20), 25, 25), bossHPBar);
                    }
                }
            }


            if (playerPeast.oyxegenAmount <= 1) {

                for (int i = 0; i < playerPeast.oyxegenAmount / 10; i++)
                {
                    float a = 1;
                    if ((int)playerPeast.oyxegenAmount / 10 == i)
                        a = (playerPeast.oyxegenAmount / 10 % 1);
                    GUI.DrawTexture(new Rect(20 + (20 * i), 90, 20, 40 * a), bossHPBar);
                }
            }
        }
        base.OnGUI();
    }

    public override void SaveData()
    {
        FileStream fs = new FileStream("save.MF", FileMode.Create);
        BinaryFormatter bin = new BinaryFormatter();
        s_mapManager lev = GameObject.Find("General").GetComponent<s_mapManager>();

        BHIII_save sav = new BHIII_save();

        sav.currentmap = SceneManager.GetActiveScene().name;

        sav.hp = (int)playerPeast.health;
        sav.MAXhp = (int)playerPeast.maxHealth;

        sav.ap = playerPeast.ammoPoints;
        sav.apMax = playerPeast.maxAmmoPoints;

        sav.location = new s_save_vector(player.transform.position.x, player.transform.position.y);
        sav.gbflg = new dat_globalflags(GlobalFlags);

        /*s
        new dat_globalflags(GlobalFlags), (int)player.health, (int)player.maxHealth, lev.mapDat.name,  
        
        if (isFixedSaveArea)
            sav.currentmap = fixedSaveAreaName;
        */

        bin.Serialize(fs, sav);
        fs.Close();
    }
}
