using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;



public static class ReorderableListUtility
{
    //  
    // Copyright (c) 2016 Siyuan Wang.
    // Licensed under the Apache License  Version 2.0. See LICENSE file in the project root for full license information.  
    //
    public static ReorderableList CreateAutoLayout(SerializedProperty property, float columnSpacing = 10f)
    {
        return CreateAutoLayout(property, true, true, true, true, null, null, columnSpacing);
    }

    public static ReorderableList CreateAutoLayout(SerializedProperty property, string[] headers, float?[] columnWidth = null, float columnSpacing = 10f)
    {
        return CreateAutoLayout(property, true, true, true, true, headers, columnWidth, columnSpacing);
    }

    public static ReorderableList CreateAutoLayout(SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, float columnSpacing = 10f)
    {
        return CreateAutoLayout(property, draggable, displayHeader, displayAddButton, displayRemoveButton, null, null, columnSpacing);
    }

    public static ReorderableList CreateAutoLayout(SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, string[] headers, float?[] columnWidth = null, float columnSpacing = 10f)
    {
        var list = new ReorderableList(property.serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);
        var colmuns = new List<Column>();

        list.drawElementCallback = DrawElement(list, GetColumnsFunc(list, headers, columnWidth, colmuns), columnSpacing);
        list.drawHeaderCallback = DrawHeader(list, GetColumnsFunc(list, headers, columnWidth, colmuns), columnSpacing);

        return list;
    }

    public static bool DoLayoutListWithFoldout(ReorderableList list, string label = null)
    {
        var property = list.serializedProperty;
        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label != null ? label : property.displayName);
        if (property.isExpanded)
        {
            list.DoLayoutList();
        }

        return property.isExpanded;
    }

    private static ReorderableList.ElementCallbackDelegate DrawElement(ReorderableList list, System.Func<List<Column>> getColumns, float columnSpacing)
    {
        return (rect, index, isActive, isFocused) =>
        {
            var property = list.serializedProperty;
            var columns = getColumns();
            var layouts = CalculateColumnLayout(columns, rect, columnSpacing);

            var arect = rect;
            arect.height = EditorGUIUtility.singleLineHeight;
            for (var ii = 0; ii < columns.Count; ii++)
            {
                var c = columns[ii];

                arect.width = layouts[ii];
                EditorGUI.PropertyField(arect, property.GetArrayElementAtIndex(index).FindPropertyRelative(c.PropertyName), GUIContent.none);
                arect.x += arect.width + columnSpacing;
            }
        };
    }

    private static ReorderableList.HeaderCallbackDelegate DrawHeader(ReorderableList list, System.Func<List<Column>> getColumns, float columnSpacing)
    {
        return (rect) =>
        {
            var columns = getColumns();

            if (list.draggable)
            {
                rect.width -= 15;
                rect.x += 15;
            }

            var layouts = CalculateColumnLayout(columns, rect, columnSpacing);
            var arect = rect;
            arect.height = EditorGUIUtility.singleLineHeight;
            for (var ii = 0; ii < columns.Count; ii++)
            {
                var c = columns[ii];

                arect.width = layouts[ii];
                EditorGUI.LabelField(arect, c.DisplayName);
                arect.x += arect.width + columnSpacing;
            }
        };
    }

    private static System.Func<List<Column>> GetColumnsFunc(ReorderableList list, string[] headers, float?[] columnWidth, List<Column> output)
    {
        var property = list.serializedProperty;
        return () =>
        {
            if (output.Count <= 0 || list.serializedProperty != property)
            {
                output.Clear();
                property = list.serializedProperty;

                if (property.isArray && property.arraySize > 0)
                {
                    var it = property.GetArrayElementAtIndex(0).Copy();
                    var prefix = it.propertyPath;
                    var index = 0;
                    if (it.Next(true))
                    {
                        do
                        {
                            if (it.propertyPath.StartsWith(prefix))
                            {
                                var c = new Column();
                                c.DisplayName = (headers != null && headers.Length > index) ? headers[index] : it.displayName;
                                c.PropertyName = it.propertyPath.Substring(prefix.Length + 1);
                                c.Width = (columnWidth != null && columnWidth.Length > index) ? columnWidth[index] : null;

                                output.Add(c);
                            }
                            else
                            {
                                break;
                            }

                            index += 1;
                        }
                        while (it.Next(false));
                    }
                }
            }

            return output;
        };
    }

    private static List<float> CalculateColumnLayout(List<Column> columns, Rect rect, float columnSpacing)
    {
        var autoWidth = rect.width;
        var autoCount = 0;
        foreach (var column in columns)
        {
            if (column.Width.HasValue)
            {
                autoWidth -= column.Width.Value;
            }
            else
            {
                autoCount += 1;
            }
        }

        autoWidth -= (columns.Count - 1) * columnSpacing;
        autoWidth /= autoCount;

        var widths = new List<float>(columns.Count);
        foreach (var column in columns)
        {
            if (column.Width.HasValue)
            {
                widths.Add(column.Width.Value);
            }
            else
            {
                widths.Add(autoWidth);
            }
        }

        return widths;
    }

    private struct Column
    {
        public string DisplayName;
        public string PropertyName;
        public float? Width;
    }
}

/*
[CustomEditor(typeof(BHIII_character), true, isFallback = true)]
[CanEditMultipleObjects]
public class ed_character : Editor
{
    private ReorderableList reOList;
    BHIII_character targ;
    List<BHIII_character.s_AIAction> AI = new List<BHIII_character.s_AIAction>();
    string directoryMove;
    SerializedObject so;
    bool[] listOfBools;
    private int lineHeightSpace = 20;

    private void OnEnable()
    {
        targ = (BHIII_character)target;
        SerializedProperty property = serializedObject.FindProperty("actionAI");
        
        reOList = new ReorderableList(serializedObject, property, true, true, true, true);
        AI = targ.actionAI;

        reOList.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "AI", EditorStyles.boldLabel);
        };
        reOList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = reOList.serializedProperty.GetArrayElementAtIndex(index);
                
                serializedObject.Update();
                var pptyIt = serializedObject.FindProperty("actionAI").GetArrayElementAtIndex(index);
                BHIII_character.s_AIAction ac = targ.actionAI[index];
                string elmDesc = "";
                switch (ac.action) 
                {
                    case BHIII_character.s_AIAction.ACTION_TYPE.END:
                        elmDesc = " End label";
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.MOVE_FORWARD:
                        elmDesc = "Move forward if distance to target is " + ac.float0;
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.MOVE_BACK:
                        if (!ac.boolean0)
                            elmDesc = "Move away from target if distance is: " + ac.float0;
                        else
                            elmDesc = " away from target for: " + ac.float0 + " seconds";
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.DASH:
                        elmDesc = "Dash if distance to target is " + ac.float1 + " for " + ac.float0 + " seconds.";
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.WHEN:
                        if (FindLabel(ac.string0) == null)
                            elmDesc = "LABEL NOT DEFINED";
                        else {
                            switch (ac.int0) {
                                case 0:
                                    elmDesc = "When local variable is " + ac.int1 + ", jump to label: " + FindLabel(ac.string0);
                                    break;
                                case 1:
                                    elmDesc = "When Health is is " + ac.float0 + ", jump to label: " + FindLabel(ac.string0);
                                    break;
                                case 2:
                                    elmDesc = "When distance to target is less than " + ac.float0 + ", jump to label: " + FindLabel(ac.string0);
                                    break;
                            }
                        }
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.RANDOM_NUM:
                        elmDesc = "Set local variable to value between " + ac.int0 + " to " + ac.int1;
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.LABEL:
                        elmDesc = ac.string0 + ":";
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.JUMP:
                        elmDesc = "Jump to label: " + ac.string0;
                        break;
                    case BHIII_character.s_AIAction.ACTION_TYPE.CUSTOM_FUNCTION:
                        elmDesc = "Excecute custom function: " + ac.string0;
                        break;
                }
                if (ac.simultaneous)
                    elmDesc += " simultaneous = true";
                else
                    elmDesc += " simultaneous = false";
                EditorGUI.LabelField(new Rect(rect.x + 60, rect.y, 400, 20), elmDesc);
                listOfBools[index] = EditorGUI.Toggle(new Rect(rect.x, rect.y, 160, 20), listOfBools[index]);
                if (listOfBools[index])
                {
                    ac.action = (BHIII_character.s_AIAction.ACTION_TYPE)EditorGUI.EnumPopup(new Rect(rect.x, rect.y + 30, 80, 20), ac.action);
                    ac.simultaneous = EditorGUILayout.Toggle(ac.simultaneous, "Simultaneous");
                    switch (ac.action) 
                    {
                        case BHIII_character.s_AIAction.ACTION_TYPE.MOVE_FORWARD:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 0, 40), "Move towards target if distance is: " + ac.float0);
                            DrawProperty("float0", ref rect, pptyIt, "Distance");
                            //ac.int1 = pptyIt.GetArrayElementAtIndex(index).FindPropertyRelative("int0").intValue; //EditorGUI.PropertyField(new Rect(rect.x, rect.y + 250, 170, 60), );
                            // ac.string0 = 
                            ac.float0 = EditorGUILayout.FloatField("Test4", ac.float0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.WHEN:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            DrawProperty("string0", ref rect, pptyIt, "Label name");
                            ac.int0 = EditorGUILayout.IntField("Cond: ", ac.int0);
                            ac.int1 = EditorGUILayout.IntField("Number coniditon", ac.int1);
                            ac.float0 = EditorGUILayout.FloatField("Distance", ac.float0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.RANDOM_NUM:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            ac.int0 = EditorGUILayout.IntField("Number min: ", ac.int0);
                            ac.int1 = EditorGUILayout.IntField("Number max: ", ac.int1);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.LABEL:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            ac.string0 = EditorGUILayout.TextField("Label name ", ac.string0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.MOVE_BACK:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            ac.boolean0 = EditorGUILayout.Toggle("Distance? ", ac.boolean0);
                            if(ac.boolean0)
                                ac.float0 = EditorGUILayout.FloatField("Seconds: ", ac.float0);
                            else
                                ac.float0 = EditorGUILayout.FloatField("Distance: ", ac.float0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.DASH:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            ac.float1 = EditorGUILayout.FloatField("Distance: ", ac.float1);
                            ac.float0 = EditorGUILayout.FloatField("Time: ", ac.float0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.JUMP:
                            //new Rect(rect.x, rect.y + 100, 0, 40),
                            ac.string0 = EditorGUILayout.TextField("Label name ", ac.string0);
                            break;
                    }
                    bool showChilderen = true;
                    int i = 1;
                    while (pptyIt.NextVisible(showChilderen))
                    {
                        //EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeightSpace), pptyIt);

                        i++;
                        if (pptyIt.isArray)
                        {
                            showChilderen = pptyIt.isExpanded;
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            };

        reOList.elementHeightCallback = (int ind) =>
        {
            float height = 0;
            int i = 2;
            bool showChilderen = true;
            var element = reOList.serializedProperty.GetArrayElementAtIndex(ind);
            if (listOfBools[ind])
            {
                serializedObject.Update();
                SerializedProperty pptyIt = serializedObject.GetIterator();

                while (pptyIt.NextVisible(showChilderen))
                {

                    i++;
                    if (pptyIt.isArray)
                    {
                        showChilderen = pptyIt.isExpanded;
                    }
                }
                serializedObject.ApplyModifiedProperties();

            }
            height = lineHeightSpace * i;
            return height;
        };
    }

    public void DrawProperty(string str, ref Rect rect, SerializedProperty prop, string desc) {
        EditorGUI.LabelField(new Rect(rect.x , rect.y + 100, 130,
            EditorGUIUtility.singleLineHeight), desc);
        EditorGUI.PropertyField(
            new Rect(rect.x + 90, rect.y + 100, 130, 
            EditorGUIUtility.singleLineHeight),
            prop.FindPropertyRelative(str), GUIContent.none);
        rect.y += 50;
    }
    public void DrawProperty(string str, ref Rect rect, SerializedProperty prop)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 100, 130,
            EditorGUIUtility.singleLineHeight), str);
        EditorGUI.PropertyField(
            new Rect(rect.x + 90, rect.y + 100, 130,
            EditorGUIUtility.singleLineHeight),
            prop.FindPropertyRelative(str), GUIContent.none);
        rect.y += 50;
    }

    public string FindFromEnd(int ind)
    {
        for (int i = ind; i > -1; i--)
        {
            if (AI[i].action == BHIII_character.s_AIAction.ACTION_TYPE.LABEL)
                return AI[i].string0;
        }
        return null;
    }
    public string FindLabel(string label)
    {
        for (int i = 0; i < AI.Count; i++)
        {
            if (AI[i].action == BHIII_character.s_AIAction.ACTION_TYPE.LABEL)
                if(label == AI[i].string0)
                    return AI[i].string0;
        }
        return null;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (AI != null)
        {
            for (int i = 0; i < AI.Count; i++)
            {
            }
        }
        serializedObject.Update();
        if (listOfBools == null)
            listOfBools = new bool[reOList.count];
        if (listOfBools.Length < reOList.count)
            listOfBools = new bool[reOList.count];
        reOList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}
[CustomEditor(typeof(BHIII_character))]
[CanEditMultipleObjects]
public class ed_character : Editor
{
    public string FindLabel(int ind, BHIII_character tra)
    {
        for (int i = ind; i > -1; i--)
        {
            if (tra.actionAI[i].action == BHIII_character.s_AIAction.ACTION_TYPE.LABEL)
                return tra.actionAI[i].string0;
        }
        return null;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BHIII_character tra = (BHIII_character)target;

        if (tra != null)  {
            if (tra.actionAI != null) {
                EditorGUILayout.LabelField("AI");
                for (int i = 0; i < tra.actionAI.Count; i++) {
                    BHIII_character.s_AIAction a = tra.actionAI[i];
                    switch (a.action)
                    {
                        case BHIII_character.s_AIAction.ACTION_TYPE.LABEL:
                            EditorGUILayout.LabelField(i + " - " + a.action.ToString() + ": " + a.string0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.MOVE_FORWARD:
                            if(!a.boolean0)
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " towards target if distance is: " + a.float0);
                            else
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " towards target for: " + a.float0 + " seconds");
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.MOVE_BACK:
                            if (!a.boolean0)
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " away from target if distance is: " + a.float0);
                            else
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " away from target for: " + a.float0 + " seconds");
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.DASH:
                            EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " towards target for: " + a.float0 + " seconds, if distance is " + a.float1);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.SHOOT:
                            if(!a.boolean0)
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " towards target for: " + a.float0 + " seconds, if distance is " + a.float1);
                            else
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " towards target for: " + a.float0 + " seconds, if distance is " + a.float1 + ", in direction: ");
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.JUMP:
                            EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " to label: " + a.string0);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.WAIT:
                            EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " for " + a.float0 + " seconds.");
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.END:
                            if (FindLabel(i, tra) == null)
                                EditorGUILayout.LabelField("LABEL NOT DEFINED");
                            else
                                EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " and jump back to label: " + FindLabel(i, tra));
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.RANDOM_NUM:
                            EditorGUILayout.LabelField(i + " - " + a.action.ToString() + " from: " + a.int0 + " to " + a.int1);
                            break;
                        case BHIII_character.s_AIAction.ACTION_TYPE.WHEN:
                            string let = i + " - " + a.action.ToString();
                            switch (a.int0) 
                            {
                                case 0:
                                    if (FindLabel(i, tra) == null)
                                        EditorGUILayout.LabelField("LABEL NOT DEFINED");
                                    else
                                        EditorGUILayout.LabelField(let + " when local variable is " + a.int1 + ", jump to label: " + FindLabel(i, tra));
                                    break;

                                case 1:
                                    if (tra.maxHealth > 0)
                                    {
                                        int calculatedHP = Mathf.RoundToInt(a.float0 * tra.maxHealth);
                                        if (FindLabel(i, tra) == null)
                                            EditorGUILayout.LabelField("LABEL NOT DEFINED");
                                        else
                                            EditorGUILayout.LabelField(let + " when health is " + calculatedHP + ", jump to label: " + FindLabel(i, tra));
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }
    }

}
*/

