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
//  VK SDK Authentication Code Response Model
//===================================================
[System.Serializable]
public class AuthCodeModel{
    public string auth_code = "";           // VK Authentication Code
    public string device_id = "";           // VK Device ID
    public long expires_in = 0;             // VK Code Expires Time
    public string auth_url = "";            // VK Auth Url
    public string device_name = "";         // VK Device Name
}