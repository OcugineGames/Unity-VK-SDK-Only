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
public class BaseRequestModel{
    public BaseResponseModel response; // Response Model
    public BaseErrorModel error; // Error Model
}

[System.Serializable]
public class BaseResponseModel{
    /* Empty Model */
}

[System.Serializable]
public class BaseErrorModel{
    public int error_code = -1; // Error Code
    public string error_msg = ""; // Error Message
}