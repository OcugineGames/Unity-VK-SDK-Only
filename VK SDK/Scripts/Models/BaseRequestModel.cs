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
public class BaseRequestModel{
    public BaseResponseModel response; // Response Model
    public BaseErrorModel error; // Error Model
}

[System.Serializable]
public class BaseResponseModel{
}

[System.Serializable]
public class BaseErrorModel{
    public int error_code = -1; // Error Code
    public string error_msg = ""; // Error Message
}