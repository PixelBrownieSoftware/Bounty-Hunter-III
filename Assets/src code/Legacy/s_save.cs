using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
public class s_save : MonoBehaviour {

    public GameObject loadedLevel;
    public GameObject res;

    GameObject targetLevel;

    struct save_dat
    {
        public List<trigger_obj> triggers;
        public List<o_collidableobject> collidableobjects;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            LoadData();
        }
    }

    public void SaveData()
    {
        if (PrefabUtility.FindPrefabRoot(res) == null)
        {
            PrefabUtility.CreatePrefab("Assets/Levels/" + "Obj.prefab", loadedLevel);
        }
        else
        {
            PrefabUtility.ReplacePrefab(loadedLevel, res);
        }
    }

    public void LoadData()
    {
        if (PrefabUtility.FindPrefabRoot(res) == null)
        {
            PrefabUtility.CreatePrefab("Assets/Levels/" + "Obj.prefab", loadedLevel);
        }
        else
        {
            loadedLevel = PrefabUtility.FindPrefabRoot(res);
        }
    }
}
*/
