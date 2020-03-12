using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
//  VK SDK Settings Model
//===================================================
[System.Serializable]
public class SDKSettingsModel{
    [Tooltip("Версия API")] public string api_version = "5.103";                            // SDK API Version
    [Tooltip("Platform (Определяется автоматически)")] public string platform = "";         // SDK Platform
    [Tooltip("Таймаут Авторизации")] public int auth_timeout = 30000;                       // Auth timeout
}
