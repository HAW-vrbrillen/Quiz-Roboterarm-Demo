using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

public class TagFilterWindow : EditorWindow
{
    string tagToFilter = "YourTagHere";

    [MenuItem("Window/Tag Filter")]
    public static void ShowWindow()
    {
        GetWindow<TagFilterWindow>("Tag Filter");
    }

    void OnGUI()
    {
        tagToFilter = EditorGUILayout.TagField("Tag to filter", tagToFilter);

        if (GUILayout.Button("Show GameObjects"))
        {
            List<GameObject> taggedObjects = new List<GameObject>();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tagToFilter))
            {
                taggedObjects.Add(obj);
                Debug.Log(obj.name); // oder fügen Sie eine Logik hinzu, um die Objekte in einem benutzerdefinierten Fenster anzuzeigen
            }
        }
    }
}
#endif
