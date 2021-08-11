using System.Collections;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct s_icon
{
    public string name;
    public List<Sprite> sprites;
}
/*
public class gui_dialogue : s_gui
{
    #region VARIABLES
    public List<s_icon> icons = new List<s_icon>();
    public Text textthing;
    public Stack<token> commands = new Stack<token>();
    public List<token> listoftokens = new List<token>();
    public Image muggshot;
    public GameObject anim;

    public enum TEXT_STATE
    {
        TYPING,
        INPUT
    }
    public TEXT_STATE TEXST;

    int current_text = 0;
    public bool done_event = false;
    public float speed = 0.05f;
    public bool automatic { get; set; }
    public float default_speed;

    public bool readtext { get; set; }

    public string[] disptxt;
    bool istyping = true;
    public string textTarg;
    public GameObject fad;
    public tokendef[] tokendictionary;

    GameObject txbx;
    public RectTransform trans;
    Text textB;

    #endregion

    public void Start()
    {
        textB = GameObject.Find("Dialogue").GetComponent<Text>();
        txbx = GameObject.Find("TextBox");
        txbx.SetActive(false);
        tokendictionary = new tokendef[9];
        readtext = false;
        default_speed = speed;
        textthing = GameObject.Find("Dialogue").GetComponent<Text>();
        tokendictionary[0] = (new tokendef(@"\" + "<pwait=" + @"[-+]?([0-9]*\.?[0-9])+" + ">", "<pwait>", 0, false));
        tokendictionary[1] = (new tokendef(@"\" + "<pcol=" + "(#[a-f0-9]{8})" + ">", "<pcol>", 1, true));
        tokendictionary[2] = (new tokendef(@"\" + "<pita>", "<pita>", 2, true));
        tokendictionary[3] = (new tokendef(@"\" + "</end>", "</end>", 5, true));
        tokendictionary[4] = (new tokendef(@"\" + "<ptxspd=" + @"[-+]?([0-9]*\.?[0-9][0-9])+" + ">", "<pspeed>", 3, false));
        tokendictionary[5] = (new tokendef(@"\" + "<panim=" + @"([0-9])" + ">", "<pspeed>", 4, false));
        tokendictionary[6] = (new tokendef(@"\" + "<pmovechar=" + @"'(\d+)" + "," + @"(\d+)'" + ">", "geswge", 6, false));
        tokendictionary[7] = (new tokendef(@"\" + "<pcharsel=" + @"(\w+)" + "," + @"(\d+)" + ">", "geswge", 7, false));
        tokendictionary[8] = (new tokendef(@"\" + "<pfade=" + @"'(\d+),(\d+),(\d+),(\d+)'" + "," + @"(\d+)" + ">", "geswge", 8, false));
    }
    
    #region COPILATION
    public void CompileText()
    {
        string replace = "";

        replace += "<endlin>" + "|";
        for (int l = 0; l < tokendictionary.Length; l++)
        {
            if (l < tokendictionary.Length - 1)
                replace += tokendictionary[l].regularex.ToString() + "|";
            else if (l == tokendictionary.Length - 1)
                replace += tokendictionary[l].regularex.ToString();
        }

        string[] str = new string[] { "<endlin>" };

        disptxt = textTarg.Split(str, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < disptxt.Length; i++)
        {
            ExtractTokens(disptxt[i], i);
        }

        for (int i = 0; i < disptxt.Length; i++)
        {
            disptxt[i] = Regex.Replace(disptxt[i], replace, "", RegexOptions.None);
        }
    }

    public void ExtractTokens(string text, int ind)
    {
        Match mch;
        foreach (tokendef tokdef in tokendictionary)
        {
            string tokentext;
            int offset = 0;

            string replace = "";
            for (int l = 0; l < tokendictionary.Length; l++)
            {
                if (tokendictionary[l].regularex.ToString() == tokdef.regularex.ToString())
                {
                    continue;
                }
                else if (l < tokendictionary.Length - 1)
                    replace += tokendictionary[l].regularex.ToString() + "|";
                else if (l == tokendictionary.Length - 1)
                    replace += tokendictionary[l].regularex.ToString();
            }
            tokentext = Regex.Replace(text, replace, "", RegexOptions.None);

            //print(tokentext);

            mch = Regex.Match(tokentext, tokdef.regularex.ToString());

            if (!mch.Success)
                continue;

            while (mch.Success != false)
            {
                // print("match: " + mch.Value + " match length: " + mch.Length);

                switch (tokdef.tokentype)
                {
                    case 1:
                        CreateToken((token.TOKENTYPE)tokdef.tokentype, (string)GetCaptureType(tokdef, mch), mch.Index - offset, ind);
                        break;
                    case 7:
                        CreateToken((token.TOKENTYPE)tokdef.tokentype, (StringAndInt)GetCaptureType(tokdef, mch), mch.Index - offset, ind);
                        break;

                    case 3:
                    case 0:
                        CreateToken((token.TOKENTYPE)tokdef.tokentype, null, mch.Index - offset, ind, (float)GetCaptureType(tokdef, mch));
                        break;

                    case 4:
                        CreateToken((token.TOKENTYPE)tokdef.tokentype, null, mch.Index - offset, ind, (int)GetCaptureType(tokdef, mch));
                        break;

                    case 6:
                        CreateToken((token.TOKENTYPE)tokdef.tokentype, null, mch.Index - offset, ind, (Vector2Int)GetCaptureType(tokdef, mch));
                        break;
                        
                    case 8:
                        //StringAndInt returntp = (StringAndInt)GetCaptureType(tokdef, mch);

                        CreateToken((token.TOKENTYPE)tokdef.tokentype, null, mch.Index - offset, ind, (Color)GetCaptureType(tokdef, mch), Convert.ToSingle(mch.Groups[5].Value));
                        break;
                }
                offset += mch.Length;
                mch = mch.NextMatch();
            }

        }
    }

    public object GetCaptureType(tokendef token, Match mch)
    {
        try
        {
            switch (token.tokentype)
            {
                case 1:
                    return Convert.ToString(mch.Groups[1].Value.ToString());

                case 3:
                case 0:
                    return Convert.ToSingle(mch.Groups[1].Value);

                case 4:
                    return Convert.ToInt32(mch.Groups[1].Value);

                case 6:
                    return new Vector2Int(Convert.ToInt32(mch.Groups[1].Value), Convert.ToInt32(mch.Groups[2].Value));
                case 7:
                    return new StringAndInt(Convert.ToInt32(mch.Groups[2].Value), mch.Groups[1].Value);
                case 8:
                    return new Color(Convert.ToInt32(mch.Groups[1].Value), Convert.ToInt32(mch.Groups[2].Value), Convert.ToInt32(mch.Groups[3].Value), Convert.ToInt32(mch.Groups[4].Value));
            }
        }
        catch (Exception e) { print(e.Message); }

        return "";
    }

    /// <summary>
    /// Processed during compile-time, this creates the list of tokens that are put in effect into the
    /// run time thing.
    /// </summary>
    /// <param name="tok">The token that was found.</param>
    /// <param name="beginlin">The line that the effect of this token begins</param>
    /// <param name="ind">Which part of the dialgoue the token applies to</param>
    public void CreateToken(token.TOKENTYPE enu, string tok, int beginlin, int ind)
    {
        //TODO:
        //DO A FOREACH LOOP OF THE TOKENS OF WHICH INSTRUCTIONS TO INCLUDE

        foreach (tokendef tokens in tokendictionary)
        {
            if (tokens.tokentype == (int)enu)
            {
                token newtok = new token();
                newtok.b_line = beginlin; //- tok.Length;
                newtok.dialogue_num = ind;
                newtok.tokentype = (token.TOKENTYPE)tokens.tokentype;
                newtok.capturedat = tok;
                newtok.time = 0;
                newtok.number = 0;
                listoftokens.Add(newtok);
            }

        }
    }
    public void CreateToken(token.TOKENTYPE enu, StringAndInt st, int beginlin, int ind)
    {
        //TODO:
        //DO A FOREACH LOOP OF THE TOKENS OF WHICH INSTRUCTIONS TO INCLUDE

        foreach (tokendef tokens in tokendictionary)
        {
            if (tokens.tokentype == (int)enu)
            {
                token newtok = new token();
                newtok.b_line = beginlin; //- tok.Length;
                newtok.dialogue_num = ind;
                newtok.tokentype = (token.TOKENTYPE)tokens.tokentype;
                newtok.capturedat = st.words;
                newtok.number = st.integer;
                newtok.time = 0;
                newtok.number = 0;
                listoftokens.Add(newtok);
            }

        }
    }
    public void CreateToken(token.TOKENTYPE enu, string tok, int beginlin, int ind, float time)
    {
        foreach (tokendef tokens in tokendictionary)
        {
            if (tokens.tokentype == (int)enu)
            {
                token newtok = new token();
                newtok.b_line = beginlin; //- tok.Length;
                newtok.dialogue_num = ind;
                newtok.tokentype = (token.TOKENTYPE)tokens.tokentype;
                newtok.capturedat = tok;
                newtok.time = time;
                newtok.number = 0;
                listoftokens.Add(newtok);
            }
        }
    }
    public void CreateToken(token.TOKENTYPE enu, string tok, int beginlin, int ind, Color colour, float time)
    {
        foreach (tokendef tokens in tokendictionary)
        {
            if (tokens.tokentype == (int)enu)
            {
                token newtok = new token();
                newtok.b_line = beginlin; //- tok.Length;
                newtok.dialogue_num = ind;
                newtok.tokentype = (token.TOKENTYPE)tokens.tokentype;
                newtok.capturedat = tok;
                newtok.colour = colour;
                newtok.time = time;
                newtok.number = 0;
                listoftokens.Add(newtok);
            }
        }
    }
    public void CreateToken(token.TOKENTYPE enu, string tok, int beginlin, int ind, Vector2 vect)
    {
        foreach (tokendef tokens in tokendictionary)
        {
            if (tokens.tokentype == (int)enu)
            {
                token newtok = new token();
                newtok.b_line = beginlin; //- tok.Length;
                newtok.dialogue_num = ind;
                newtok.tokentype = (token.TOKENTYPE)tokens.tokentype;
                newtok.capturedat = tok;
                newtok.position = vect;
                newtok.number = 0;
                listoftokens.Add(newtok);
            }
        }
    }

    /// <summary>
    /// This checks for every single token per line and see if it matches the 
    /// begining line of the respective token. 
    /// If it does, it applies the respective effect of the token to the text.
    /// NOTE: This uses the current line in the raw text rather than the text you see
    /// </summary>
    /// <param name="currentline"></param>
    public token.TOKENTYPE ParseTxt(int currentline, int text_index)
    {
        token.TOKENTYPE currenttok = token.TOKENTYPE.ANIMATION;
        foreach (token tok in listoftokens)
        {
            if (currentline == tok.b_line && tok.dialogue_num == text_index)
            {
                print("Commands " + tok.tokentype);
                currenttok = tok.tokentype;

                if (tok.tokentype == token.TOKENTYPE.END)
                    commands.Pop();
                else
                    commands.Push(tok);
            }

        }
        TypeLine(currentline);
        //  if (commands.Peek().tokentype == token.TOKENTYPE.WAIT)
        // 
        return currenttok;
    }
    #endregion

    #region TYPE
    void TypeLine(int currentline)
    {
        Stack<token> temp_stk = new Stack<token>();
        foreach (token stack in commands)
        {
            temp_stk.Push(stack);
        }

        //This is for the line by line check
        while (temp_stk.Count > 0)
        {
            token.TOKENTYPE currenttok = temp_stk.Peek().tokentype;

            switch (currenttok)
            {
                case token.TOKENTYPE.SELECT_CHARACTER:
                    Set(temp_stk.Peek().capturedat, temp_stk.Peek().number);
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.WAIT:
                    speed = (float)temp_stk.Peek().time;
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.COLOUR:

                    textthing.text += "<color=" + temp_stk.Peek().capturedat + ">";
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.ITALICS:

                    textthing.text += "<i>";
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.TXTSPD:

                    default_speed = (float)temp_stk.Peek().time;
                    speed = default_speed;
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.MOVEPOS:
                    if (anim != null)
                        anim.transform.position = temp_stk.Peek().position;
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.ANIMATION:
                    //anim.GetComponent<o_actor>().anim_enum = (int)temp_stk.Peek().time;
                    print((int)temp_stk.Peek().time);
                    temp_stk.Pop();
                    break;

                case token.TOKENTYPE.FADE:
                    Color fadecol = fad.GetComponent<Image>().color;
                    fadecol.a = fad.GetComponent<Image>().color.a;
                    StartCoroutine(Fade(fadecol, temp_stk.Peek().colour, (float)temp_stk.Peek().time));
                    temp_stk.Pop();
                    break;
            }
        }

        textthing.text += disptxt[current_text][currentline];

        foreach (token tok in commands)
        {
            switch (tok.tokentype)
            {
                case token.TOKENTYPE.COLOUR:

                    textthing.text += "</color>";
                    break;

                case token.TOKENTYPE.ITALICS:

                    textthing.text += "</i>";
                    break;
            }
        }
    }

    IEnumerator Fade(Color fromcolour, Color tocolour, float time)
    {
        float timer = 0;
        while (timer < time)
        {
            print("yue");
            timer = timer + Time.deltaTime;
            fad.GetComponent<Image>().color = Color.Lerp(fromcolour, tocolour, timer);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    #endregion
    
    public void Set(string iconName, int number)
    {
        trans.sizeDelta = new Vector2(880, trans.sizeDelta.y);
        trans.position = new Vector2(706, trans.position.y);
        if (iconName == "none" || iconName == null)
        {
            RemoveIcon();
            return;
        }
        muggshot.enabled = true;
        s_icon ic = icons.Find(x => x.name == iconName);
        for (int i = 0; i < ic.sprites.Count; i++)
        {
            if (i == number)
                muggshot.sprite = ic.sprites[i];
        }

    }

    public void RemoveIcon()
    {
        trans.sizeDelta = new Vector2(995, trans.sizeDelta.y);
        trans.position = new Vector2(618.3f, trans.position.y);
        muggshot.enabled = false;
    }

    public IEnumerator DisplayDialogue(string textasset)
    {
        done_event = false;
        txbx.SetActive(true);
        textTarg = textasset;
        CompileText();
        bool istyping = true;
        bool isSkipped = false;
        current_text = 0;
        while (current_text != disptxt.Length)
        {
            if (istyping)
            {
                for (int i = 0; i < disptxt[current_text].Length; i++)
                {
                    if (Input.GetKeyDown(KeyCode.X) && !automatic)
                    {
                        if (!isSkipped)
                        {
                            textthing.text = "";
                            istyping = true;
                            current_text++;
                            break;
                        }
                    }
                    token.TOKENTYPE toktype = ParseTxt(i, current_text);
                    //if(!isSkipped)
                        yield return new WaitForSeconds(speed);

                    if (commands.Count > 0)
                    {
                        switch (commands.Peek().tokentype)
                        {
                            case token.TOKENTYPE.WAIT:
                                speed = default_speed;
                                commands.Pop();
                                break;

                            case token.TOKENTYPE.SELECT_CHARACTER:
                            case token.TOKENTYPE.MOVEPOS:
                            case token.TOKENTYPE.ANIMATION:
                            case token.TOKENTYPE.TXTSPD:
                            case token.TOKENTYPE.FADE:
                                commands.Pop();
                                break;
                        }
                    }
                }
                isSkipped = false;
                istyping = false;
            }

            yield return new WaitForSeconds(Time.deltaTime);

            if (automatic)
            {
                yield return new WaitForSeconds(0.8f);
                textthing.text = "";
                istyping = true;
                current_text++;
            }
            else if (Input.GetKeyDown(KeyCode.Space)) {
                textthing.text = "";
                istyping = true;
                current_text++;
            }

        }
        RemoveIcon();
        txbx.SetActive(false);
        done_event = true;
    }
}
*/