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
//  @developer      VK
//  @version        0.4.3
//  @build          403
//  @url            https://vk.com/dev
//  @license        MIT
//===================================================
//===================================================
//  VK SDK Settings Model
//===================================================
[System.Serializable]
public class SDKSettingsModel{
    [Tooltip("Режим отладки")] public bool debug_mode = false;                              // SDK Debug Mode
    [Tooltip("Версия API")] public string api_version = "5.103";                            // SDK API Version
    [Tooltip("Platform (Определяется автоматически)")] public string platform = "";         // SDK Platform
    [Tooltip("Интервал проверки авторизации")] public float auth_interval = 3f;             // Auth Interval
    [Tooltip("Таймаут Авторизации")] public float auth_timeout = 300f;                      // Auth timeout
}
