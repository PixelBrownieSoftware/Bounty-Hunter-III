using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_BGM : MonoBehaviour
{
    public AudioClip sound;
    public AudioSource src;

    private void Start()
    {
        src.loop = true;
        src.clip = sound;
        src.Play();
    }
}
