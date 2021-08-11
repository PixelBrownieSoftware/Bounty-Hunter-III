using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class u_spawner : s_utility
{
    public float maxTimer;
    public string character;
    public int capacity;
    public int limitToSpawn;
    public bool limit;

    int currentCapacity;
    float timer;
    public List<o_character> characters = new List<o_character>();
    public bool isOn = false;

    public new void Update()
    {
        if (isOn)
        {
            foreach (o_character c in characters)
            {
                if (c.health == 0)
                {
                    characters.Remove(c);
                }
            }

            timer += Time.deltaTime;
            if (timer >= maxTimer)
            {
                if (limit)
                {
                    if (characters.Count > limitToSpawn)
                    {
                        timer = 0;
                        return;
                    }
                }
                //characters.Add((o_character)ll_BHIII.LevEd.SpawnObject(character, transform.position, Quaternion.identity));
                timer = 0;
            }
        }
    }
}
