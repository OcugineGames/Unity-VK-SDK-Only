using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//===================================================
//  VK Standalone SDK
//  Данный SDK разработан для использования API
//  социальной сети вконтакте для Standalone
//  приложений, разработанных с использованием
//  Unity
//
//  @name           VK Standalone SDK
//  @developer      Ocugine Games
//  @version        0.4.2
//  @build          402
//  @url            https://vk.com/ocugine
//  @license        MIT
//===================================================
//===================================================
//  VK SDK Editor Custom Inspector Class
//===================================================
namespace VK.SDK{
    [CustomEditor(typeof(VKSDK))]
    public class VKSDKInspector : Editor{
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        // Draw Inspector GUI
        public override void OnInspectorGUI(){
            // Draw Image
            GUILayout.Box(Resources.Load("VKSplash") as Texture2D); // Load Resources Icon

            // Draw Ocugine Buttons
            GUILayout.Space(10f);
            if (GUILayout.Button("Перейти к документации", GUILayout.Width(273))){
                Application.OpenURL("https://vk.com/dev/manuals");
            }
            if (GUILayout.Button("Перейти к приложениям", GUILayout.Width(273))){
                Application.OpenURL("https://vk.com/apps?act=manage");
            }

            // Draw Header
            GUILayout.Space(10f);
            GUILayout.Label("Настройка SDK:", EditorStyles.boldLabel);

            // Draw Object Params
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
            serializedObject.ApplyModifiedProperties();
        }
    }
}