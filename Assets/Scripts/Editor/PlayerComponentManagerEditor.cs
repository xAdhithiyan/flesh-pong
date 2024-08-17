using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
[CustomEditor(typeof(PlayerComponentManager))]
public class PlayerComponentManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerComponentManager manager = (PlayerComponentManager)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Load PCM"))
        {
            PlayerComponentManager.UpdateScripts();
        }
    }
}
