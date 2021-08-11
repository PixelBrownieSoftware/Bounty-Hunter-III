using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

[System.Serializable]
public class BHIII_save : dat_save {
    public int apMax;
    public float ap;
    public BHIII_save()
    {
    }
    public BHIII_save(dat_globalflags gbflg,int health, int MAXhp, string currentmap, Vector2 location, int apMax, float ap)
    : base (gbflg, health, MAXhp, currentmap, location)
    {
        this.apMax = apMax;
        this.ap = ap;
    }
}

public class BHIII_mainmenu : s_mainmenu {
    public override void LoadSave<T>()
    {
        base.LoadSave<T>();
    }

    public override void CallLoadSave()
    {
        LoadSave<BHIII_save>();
    }

    public override bool SaveExist()
    {
        return base.SaveExist();
    }

    public void OnGUI()
    {
        base.OnGUI();
    }

    private void Start()
    {
        _buttonList = new strBool[1] { new strBool("Dash direction", true, "Dash in direction of mouse", "Dash in direction of keys") };
        _keyList = new string[6] { "left", "right", "up", "down", "dash" , "select" };
        if (PlayerPrefs.GetInt("firstTime") == 0) {
            s_globals.SetButtonKeyPref("left", KeyCode.A);
            s_globals.SetButtonKeyPref("right", KeyCode.D);
            s_globals.SetButtonKeyPref("up", KeyCode.W);
            s_globals.SetButtonKeyPref("down", KeyCode.S);
            s_globals.SetButtonKeyPref("dash", KeyCode.Space);
            s_globals.SetButtonKeyPref("select", KeyCode.E);
            PlayerPrefs.SetInt("firstTime", 1);
        }
    }

}

/*
public class s_mainmenu : MonoBehaviour {

    public static dat_save save;
    public static bool isload = false;

    private void OnGUI()
    {
        if (GUILayout.Button("Start game"))
        {
            isload = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Game");
        }

        if (File.Exists("sav.MF"))
        {
            if (GUILayout.Button("Load game"))
            {
                isload = true;
                FileStream fs = new FileStream("sav.MF", FileMode.Open);
                BinaryFormatter bin = new BinaryFormatter();

                save = (dat_save)bin.Deserialize(fs);
                
                fs.Close();
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main Game");
            }
        }
        
    }
}
*/
