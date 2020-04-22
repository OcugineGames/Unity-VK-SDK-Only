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
//  VK Application Settings Model
//===================================================
[System.Serializable]
public class ApplicationSettingsModel{
    [Header("Настройки приложения")]
    [Tooltip("Сохранять данные авторизации и приложения")] public bool autosave_data = true; // Autosave Data
    [Tooltip("ID приложения")] public int app_id;                                           // Application ID
    [Tooltip("Защищенный ключ")] public string secure_key;                                  // Secure Key
    [Tooltip("Сервисный ключ доступа")] public string service_key;                          // Service Key
    [Space]
    [Header("Права доступа приложения")]
    [Tooltip("Доступ к списку друзей")] public bool friends = true;
    [Tooltip("Доступ к уведомлениям")] public bool notify = true;
    [Tooltip("Доступ к фотографиям")] public bool photos = true;
    [Tooltip("Доступ к аудио-записям")] public bool audio = true;
    [Tooltip("Доступ к видео-записям")] public bool video = true;
    [Tooltip("Доступ к историям")] public bool stories = true;
    [Tooltip("Доступ к wiki-страницам")] public bool pages = true;
    [Tooltip("Доступ к статусу пользователя")] public bool status = true;
    [Tooltip("Доступ к заметкам")] public bool notes = true;
    [Tooltip("Доступ к сообщениям")] public bool messages = true;
    [Tooltip("Доступ к стене")] public bool wall = true;
    [Tooltip("Доступ к расширенным рекламным функциям")] public bool ads = true;
    [Tooltip("Доступ к API в любое время")] public bool offline = true;
    [Tooltip("Доступ к документам")] public bool docs = true;
    [Tooltip("Доступ к группам")] public bool groups = true;
    [Tooltip("Доступ к оповещениям об ответах пользователю")] public bool notifications = true;
    [Tooltip("Доступ к статистике")] public bool stats = true;
    [Tooltip("Доступ к email-адресу")] public bool email = true;
    [Tooltip("Доступ к товарам")] public bool market = true;
}