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
//  VK SDK Editor Initializer Class
//===================================================
namespace VK.SDK{
    public class VKSDKInitializer : EditorWindow{
        [MenuItem("VK SDK/Добавить на сцену")]
        static void initializeSDKObject(){
            GameObject _sdk = new GameObject("VKSDK");
            _sdk.transform.SetSiblingIndex(0);
            _sdk.AddComponent<VKSDK>();
            Selection.activeGameObject = _sdk;
        }

        [MenuItem("VK SDK/Перейти к документации")]
        static void showDocs(){
            Application.OpenURL("https://vk.com/dev/manuals");
        }

        [MenuItem("VK SDK/Перейти к приложениям")]
        static void showDashboard(){
            Application.OpenURL("https://vk.com/apps?act=manage");
        }
    }
}

