using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void functionptr(Color colour);
[System.Serializable]
public struct gui_button
{
    public gui_button(Vector2Int vec, functionptr function, Color colour)
    {
        position = vec;
        funct = function;
        this.colour = colour;
    }
    public Vector2Int position;
    public Color colour;
    public functionptr funct;
}

/*
public class s_gui : MonoBehaviour {

    public Vector2Int menuchoice;
    int menuchoice_1D;
    int[,] cells;
    public Texture2D text;

    public Text textstr;
    public BHIII_character character;
    public static List<BHIII_character> othercharacter = new List<BHIII_character>();
    public static BHIII_character allycharacter;
    o_item selected_Item;
    public Vector2 bossHPPOS = new Vector2(180,250);
    float bosshposoffset;
    public float offsetCell;

    int cell_x, cell_y;
    int inv_leng = 0;
    bool menu = false;
    bool x_limit;
    bool open_menu = false;
    public o_plcharacter play;

    public Color playerColour;
    List<Vector2Int> cell_colours = new List<Vector2Int>();
    List<gui_button> buttons = new List<gui_button>();
    List<gui_element> gui_elements = new List<gui_element>();

    public static void ClearBossHPList()
    {
        othercharacter.Clear();
    }
    public static void AddToBossHPList(BHIII_character ch)
    {
        othercharacter.Add(ch);
    }

    void Start ()
    {
        bossHPPOS = new Vector2(325, 620);
        cell_x = 20;
        cell_y = 10;
        play = GameObject.Find("Player").GetComponent<o_plcharacter>();
        buttons.Add(new gui_button(new Vector2Int(4,6), ChangeCOlour, Color.red));
        buttons.Add(new gui_button(new Vector2Int(12, 3), ChangeCOlour, Color.magenta));
        buttons.Add(new gui_button(new Vector2Int(9, 5), ChangeCOlour, Color.blue));
    }

    public static void RemoveCharacters(bool top) {
        if (top)
        {
            allycharacter = null;
        }
        else {
            othercharacter = null;
        }
    }

    public void ChangeCOlour(Color colou)
    {
       character.GetComponent<SpriteRenderer>().color = colou;
    }
    
    
    public static void DisplayNotificationText(string n, float t)
    {
        TextShowNotification = n;

        if (t < 0)
            stayOn = true;
        else
            stayOn = false;

        timer = t;
        NotificationBox.enabled = true;
    }
    public static void HideNotificationText()
    {
        TextShowNotification = "";
        timer = 0;
        NotificationBox.enabled = false;
    }

    private void OnGUI()
    {
        if (play)
            DrawInARow((int)play.health, 23, new Vector2(0, 0), playerColour);
        if (othercharacter != null)
        {
            int combinedHP = 0;
            int MaxCombinedHP = 0; ;
            foreach (BHIII_character c in othercharacter)
            {
                MaxCombinedHP += (int)c.maxHealth;
                combinedHP += (int)c.health;
            }

            if (MaxCombinedHP > 20)
            {
                float divisior = MaxCombinedHP - 20;
                float offset = 0;
                for (int i = 0; i < divisior; i++)
                {
                    offset += offsetCell;
                }
                bosshposoffset = offset;
            }
            //if(MaxCombinedHP > 54)
            DrawInARow(MaxCombinedHP, 23, bossHPPOS + new Vector2(bosshposoffset, 0), Color.gray);
            DrawInARow(combinedHP, 23, bossHPPOS + new Vector2(bosshposoffset, 0), Color.white);
        }
        if (s_globals.GetGlobalFlag("M_ACTIVE") == 1)
        {
            if (allycharacter != null)
            {
                DrawInARow((int)allycharacter.health, 23, new Vector2(0, 200), new Color(0.1f, 0.4f, 0.55f));
            }
            else
            {
                allycharacter = GameObject.Find("Milbert").GetComponent<BHIII_character>();
            }
        }

        if (textstr != null)
        {

            textstr.text = "";
            string id = "";
            int ind = 0;
            textstr.text += "Press E to interact with blocks with down arrows" + "\n";
            textstr.text += "Money: " + s_globals.Money + "\n";
            textstr.text += "Player states: " + play.CHARACTER_STATE.ToString() + "\n";
            textstr.text += "FPS: " + Time.frameCount + " Target FPS: " + Application.targetFrameRate + "\n";
            if (open_menu)
            {
                textstr.text += "Press Z to equip " + "\n";
                textstr.text += "Press tab to close item menu" + "\n";
            }
            else {
                textstr.text += "Press tab to open item menu " + "\n";
            }
            foreach (o_item i in s_globals.inventory_unique)
            {
                if (i.name == id)
                    continue;
                id = i.name;
                if (open_menu)
                {
                    if (ind == menuchoice.y)
                    {
                        selected_Item = i;
                        textstr.text += "->";
                    }
                }

                if (i.TYPE == o_item.ITEM_TYPE.CONSUMABLE)
                {
                    textstr.text += i.name + " x " + s_globals.inventory.FindAll(x => x.name == i.name && x.TYPE == i.TYPE).Count + "\n";
                }
                else
                {
                    textstr.text += i.name + " x " + s_globals.inventory.FindAll(x => x.name == i.name && x.TYPE == i.TYPE).Count + " Key item" + "\n";
                }
                ind++;
            }
            

            if (cells != null)
                DrawInMulti(new Vector2Int(cell_x, cell_y), 20);

        }
    }

    public void CreateCells(int x, int y)
    {
        cell_x = x;
        cell_y = y;

        cells = new int[cell_x, cell_y];
    }

    public void CreateCells(int quantity, int lim, bool x_limit)
    {
        int q = Mathf.CeilToInt(quantity/ lim);
        this.x_limit = x_limit;

        cell_x = lim;
        cell_y = q;

        cells = new int[cell_x, cell_y];
    }

    public void ResetData()
    {
        cells = null;
    }

    public void DrawInARow(int count, float spacing, Vector2 offset)
    {
        for (int i = 0; i < count; i++)
        {
            DrawObject(new Rect(30 + (spacing * i) + offset.x, 30 + offset.y, 30, 30), text, Color.white);
        }
    }
    public void DrawInARow(int count, float spacing, Vector2 offset, Color colour)
    {
        for (int i = count; i > 0; i--)
        {
            DrawObject(new Rect(30 + (spacing * i) + offset.x, 30 + offset.y, 30, 30), text, colour);
        }
    }
    
    public void DrawInMulti(int quantity ,float spacing)
    {
        int tot = cell_x * cell_y - quantity;
        int x_y = 0;
        int y_x = 0;
        for (int i = 0; i < tot; i++)
        {
            if (i % cell_y == 0)
            {
                y_x = 0;
                x_y++;
            }
            y_x++;

            if (menuchoice.x * menuchoice.y > tot)
            {
                menuchoice.x = 0;
                menuchoice.y = 0;
            }

            print(menuchoice);
            DrawObject(new Rect(30 + spacing * x_y,  spacing * y_x, 30, 30), text, Color.white);
            if (menuchoice.x == x_y && menuchoice.y == y_x)
            {
                DrawObject(new Rect(30 + spacing * x_y,  spacing * y_x, 30, 30), text, Color.red);
            }


        }
    }
    public int PickFromList(int amount)
    {
        if (cells == null)
        {
            cell_x = 1;
            cell_y = amount;
            cells = new int[cell_x, cell_y];
        }

        return menuchoice.y;
    }

    public void DrawInMulti(Vector2 count, float spacing)
    {
        for (int x = 0; x < count.x; x++)
        {
            for (int y = 0; y < count.y; y++)
            {
                if (menuchoice == new Vector2Int(x, y))
                {
                    DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.red);
                    if (Input.GetKeyDown(KeyCode.N))
                    {
                        foreach (gui_button butt in buttons)
                        {
                            if (butt.position == menuchoice)
                            {
                                butt.funct(butt.colour);
                            }
                        }


                        cell_colours.Add(new Vector2Int(x, y));
                        DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.blue);
                    }
                }
                else
                {
                    DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.white);

                    foreach (Vector2Int cell in cell_colours)
                    {
                        if (x == cell.x && y == cell.y)
                        {
                            DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, Color.green);
                        }
                    }
                    foreach (gui_button button in buttons)
                    {
                        if (x == button.position.x && y == button.position.y)
                        {
                            DrawObject(new Rect(spacing * x, spacing * y, 30, 30), text, button.colour);
                        }
                    }
                }


            }
        }
    }

    public void DrawTextGUI(string letter)
    {
        GUI.TextArea(new Rect(90, 90, 140, 80), letter);
    }

    public void DrawText(string letter)
    {
        GetComponent<Text>().text = letter;
    }

    public void DrawObject(Rect rectan, Texture2D thing, Color colour)
    {
        GUI.color = colour;
        if(thing != null)
            GUI.DrawTexture(rectan, thing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            menuchoice.y += 1;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            menuchoice.y -= 1; //(int)Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.RightArrow))
            menuchoice.x += 1; //(int)Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            menuchoice.x -= 1;
        
        menuchoice.x = Mathf.Clamp(menuchoice.x, 0, cell_x);
        menuchoice.y = Mathf.Clamp(menuchoice.y, 0, s_globals.inventory_unique.Count - 1);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!open_menu)
            {
                Time.timeScale = 0;
                open_menu = true;
            }
            else
            {
                Time.timeScale = 1;
                open_menu = false;
            }
        }
        if (open_menu)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (selected_Item != null)
                {
                    if (selected_Item.TYPE == o_item.ITEM_TYPE.CONSUMABLE)
                    {
                        play.heal_item = selected_Item;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            menu = !menu;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            cell_colours.Clear();
        }

    }
}

[System.Serializable]
public class gui_element
{
    public Rect rectan;

    public gui_element(Rect rectan)
    {
        this.rectan = rectan;
    }

    void idk() { //DrawObject(new Rect(new Vector2(90, 90), new Vector2(90, 90)), text); 
    }

    public void DrawObject(Texture2D thing)
    {
        GUI.DrawTexture(rectan, thing);
    }

}
*/
