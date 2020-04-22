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
//  VK SDK Base Request Model
//===================================================
[System.Serializable]
public class AuthenticationModel{
    public string access_token = ""; // Access Token
    public int expires_in = 0;  // Expires Time
    public int user_id = 0;     // User ID
}
