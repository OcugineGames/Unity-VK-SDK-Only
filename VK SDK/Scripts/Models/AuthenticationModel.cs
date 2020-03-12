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
//  VK SDK Base Request Model
//===================================================
[System.Serializable]
public class AuthenticationModel{
    public string access_token = ""; // Access Token
    public int expires_in = 0;  // Expires Time
    public int user_id = 0;     // User ID
}
