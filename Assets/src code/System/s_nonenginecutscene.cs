using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_nonenginecutscene : MonoBehaviour
{
    public Text TextToDispl;
    public bool isskippable = false;
    public AudioSource src;
    public GUISkin guiStyle;
    public SpriteRenderer render;
    public Image fadeFX;
    public AudioSource audSrc;
    bool fadeIsWhite = false;

    [System.Serializable]
    public struct cut_element {
        public Sprite img;
        public float delay;
        public string text;
        public AudioClip sound;
        public enum ELEMENT_TYPE { 
            IMAGE,
            IMAGE_TEXT,
            TEXT,
            FADE,
            END,
            PLAY_SOUND,
            PLAY_SOUND_LOOP,
            CHANGE_SCENE
        }
        public ELEMENT_TYPE el_type;
    }
    public cut_element[] cutScene;

    public void Start()
    {
        StartCoroutine(PlayScene());
    }

    IEnumerator PlayScene() {
        foreach (cut_element c in cutScene) {
            switch (c.el_type) {
                case cut_element.ELEMENT_TYPE.FADE:
                    yield return StartCoroutine(FadeToWhite());
                    break;
                case cut_element.ELEMENT_TYPE.IMAGE_TEXT:
                    yield return StartCoroutine(DisplayPictureAndText(c));
                    break;
                case cut_element.ELEMENT_TYPE.TEXT:
                    yield return StartCoroutine(DisplayTextEnumerator(c));
                    break;
                case cut_element.ELEMENT_TYPE.IMAGE:
                    yield return StartCoroutine(DisplayTextEnumerator(c));
                    break;
                case cut_element.ELEMENT_TYPE.END:
                    yield return StartCoroutine(DisplayTextEnumeratorPerm(c));
                    break;
                case cut_element.ELEMENT_TYPE.PLAY_SOUND:
                    yield return StartCoroutine(DisplayTextEnumeratorPerm(c));
                    break;
                case cut_element.ELEMENT_TYPE.PLAY_SOUND_LOOP:
                    PlaySoundLoop(c.sound);
                    break;
                case cut_element.ELEMENT_TYPE.CHANGE_SCENE:
                    ChangeScene();
                    break;
            }
        }
    }

    public void OnGUI()
    {
        if (isskippable)
        {
            if (GUI.Button(new Rect(20, 20, 100, 60), "Skip", guiStyle.GetStyle("Box")))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
            }
        }
    }
    public void PlaySoundLoop(AudioClip cl)
    {
        src.clip = cl;
        src.loop = true;
        src.Play();
    }

    public void ChangeScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
    IEnumerator DisplayTextEnumerator(cut_element c)
    {
        TextToDispl.color = Color.clear;
        TextToDispl.text = c.text;
        if (!fadeIsWhite)
            TextToDispl.color = Color.white;
        else
            TextToDispl.color = Color.black;
        yield return new WaitForSeconds(c.delay);
        TextToDispl.color = Color.clear;
        yield return new WaitForSeconds(0.4f);
    }
    IEnumerator DisplayTextEnumeratorPerm(cut_element c)
    {
        TextToDispl.color = Color.clear;
        TextToDispl.text = c.text;
        yield return new WaitForSeconds(c.delay);
        TextToDispl.color = Color.white;
    }
    IEnumerator FadeToWhite()
    {
        TextToDispl.color = Color.clear;
        float t = 0;
        while (fadeFX.color != Color.white)
        {
            fadeFX.color = Color.Lerp(Color.clear, Color.white, t);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        fadeIsWhite = true;
        yield return new WaitForSeconds(0.5f);
        TextToDispl.color = Color.black;
    }
    IEnumerator DisplayPictureAndText(cut_element c)
    {
        render.color = Color.clear;
        TextToDispl.color = Color.clear;
        TextToDispl.text = c.text;
        render.sprite = c.img;
        render.color = Color.white;
        TextToDispl.color = Color.white;
        yield return new WaitForSeconds(c.delay);
        TextToDispl.color = Color.clear;
        render.color = Color.clear;
        yield return new WaitForSeconds(0.4f);

    }
    IEnumerator PlaySound(cut_element c)
    {
        src.loop = false;
        src.PlayOneShot(c.sound);
        yield return new WaitForSeconds(c.delay);
        render.color = Color.clear;
        yield return new WaitForSeconds(0.4f);
    }


}
