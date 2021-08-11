using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

/*

public class s_leveldatabase : MonoBehaviour
{
    public s_map current_map;
    public List<s_map> maps = new List<s_map>();
    s_leveledit led;

    private void Awake()
    {
        string path = @"C:\Users\hamza\Own Games\Unity\InDevelopment\Codename - Magnum Foundation\Assets\Levels.txt";
        string file = JsonUtility.ToJson(maps[0]);
        //FileStream fs = new FileStream("Levels.txt", FileMode.Create);
        StreamWriter st = new StreamWriter(path, true);
        st.WriteLine(file);
        st.Close();
        //AssetDatabase.ImportAsset("Levels.txt");

        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();


        led = GetComponent<s_leveledit>();
    }



    void SaveMap()
    {
    }


}
*/

