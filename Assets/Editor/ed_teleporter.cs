using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using MagnumFoudation;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(o_trigger))]
[CanEditMultipleObjects]
public class ed_teleporter : Editor
{
    s_mapEventholder mapdat;

    public override void OnInspectorGUI()
    {
        if (mapdat == null)
            mapdat = GameObject.Find("MapObject").GetComponent<s_mapEventholder>();
        o_trigger tra = (o_trigger)target;

        base.OnInspectorGUI();

        for (int i = 0; i < Labelmap().Count; i++)
        {
            if (GUILayout.Button(Labelmap()[i].Item1))
            {
                tra.LabelToJumpTo = Labelmap()[i].Item2;
                tra.stringLabelToJumpTo = Labelmap()[i].Item1;
            }
        }
    }

    List<Tuple<string, int>> Labelmap()
    {
        List<MagnumFoudation.ev_details> te = mapdat.Events;

        List<Tuple<string, int>> maploc = new List<Tuple<string, int>>();
        for (int i = 0; i < te.Count; i++)
        {
            if (te[i].eventType == -1)
                maploc.Add(new Tuple<string, int>(te[i].string0, i));
        }
        return maploc;
    }

    /*
    List<string> LocationsMap()
    {
        //TODO: GET MAPS THEMSELVES INSTEAD
        SceneManager.getscene("Scenes/");
        List<TextAsset> te = mapdat.jsonMaps;

        List<string> maploc = new List<string>();
        for (int i = 0; i < te.Count; i++)
        {
            maploc.Add(te[i].name);
        }
        return maploc;
    }

    List<string> PositionsOnMap(string n)
    {
        List<string> maploc = new List<string>();
        TextAsset te = mapdat.jsonMaps.Find(x => x.name == n);

        s_map m = JsonUtility.FromJson<s_map>(te.text);

        for (int i = 0; i < m.tilesdata.Count; i++)
        {
            //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
            switch (m.tilesdata[i].TYPENAME)
            {
                case "teleport_object":
                    maploc.Add(m.tilesdata[i].name);
                    break;
            }
        }
        return maploc;
    }
    */
}
/*
[CustomEditor(typeof(o_maptransition))]
[CanEditMultipleObjects]
public class ed_teleporter : Editor
{
    s_map mapdat;

    public override void OnInspectorGUI()
    {
        if (mapdat == null)
            mapdat = GameObject.Find("General").GetComponent<s_leveledit>().mapDat;

        base.OnInspectorGUI();
        o_maptransition tra = (o_maptransition)target;
        EditorGUILayout.LabelField("Location to teleport to: " + tra.sceneToTransferTo);
        
        for (int i = 0; i < LocationsMap().Count; i++)
        {
            if (GUILayout.Button(LocationsMap()[i]))
            {
               // tra.sceneToTransferTo = LocationsMap()[i];
            }
        }
        EditorGUILayout.LabelField("Teleportation point " + tra.teleportObj);
        for (int i = 0; i < PositionsMap().Count; i++)
        {
            if (GUILayout.Button(PositionsMap()[i]))
            {
                tra.teleportObj = PositionsMap()[i];
            }
        }
    }

    List<string> LocationsMap()
    {
        List<string> maploc = new List<string>();
        for (int i = 0; i < mapdat.tilesdata.Count; i++)
        {
            if (mapdat.tilesdata[i].TYPENAME == "mapteleport")
                maploc.Add(mapdat.tilesdata[i].mapname);
        }
        return maploc;
    }
    List<string> PositionsMap()
    {
        List<string> maploc = new List<string>();
        if (GameObject.Find("General").GetComponent<s_leveledit>().mapDat != null) {

            s_object[] tilesInMap = GameObject.Find(GameObject.Find("General").GetComponent<s_leveledit>().mapDat.name).transform.Find("Tiles").GetComponentsInChildren<s_object>();
            
            for (int i = 0; i < tilesInMap.Length; i++)
            {
                //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
                if (tilesInMap[i].ID == "teleport_object")
                    maploc.Add(tilesInMap[i].name);
            }
        }

        return maploc;
    }
}
*/
