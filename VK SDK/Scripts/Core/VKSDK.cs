using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//===================================================
//  VK Standalone SDK
//  Данный SDK разработан для использования API
//  социальной сети вконтакте для Standalone
//  приложений, разработанных с использованием
//  Unity
//
//  @name           VK Standalone SDK
//  @developer      Ocugine Games
//  @version        0.4.3
//  @build          403
//  @url            https://vk.com/ocugine
//  @license        MIT
//===================================================

// Todo: Remove warning disabletors
#pragma warning disable CS0618 // For usind deprecated method WWW

//===================================================
//  VK SDK General Class
//===================================================
namespace VK.SDK{
    [AddComponentMenu("VK SDK/SDK Manager")]
    public class VKSDK : MonoBehaviour{
        // Public Variables
        [HideInInspector] public static VKSDK instance = null;                          // VK SDK Instance
        [HideInInspector] public bool initialized = false;                              // VK SDK Initialization Status
        [Tooltip("Настройки приложения")] public ApplicationSettingsModel application;  // VK SDK Application Settings
        [Tooltip("Настройки SDK")] public SDKSettingsModel settings;                    // VK SDK Settings
        [HideInInspector] public AuthenticationModel authentication;                    // VK SDK Authentication Data

        // Private Variables
        private static string _oauth_uri = "https://oauth.vk.com/";                     // VK OAuth URI
        private static string _api_uri = "https://api.vk.com/method/";                  // VK API Gateway URI
        private static string _sdk_settings_path;                                       // VK SDK Settings Path
        private static string _app_settings_path;                                       // VK SDK Application Settings Path
        private static string _auth_data_path;                                          // VK Authentication Data Path
        private static string _log_data_path;                                           // VK SDK Log Path

        // Private Authentication Variables
        private AuthCodeModel _authCode;                                                // VK Auth Code model

        //============================================================
        //  @class      VKSDK
        //  @method     Awake()
        //  @type       Internal Void
        //  @usage      Call before scene initialized
        //============================================================
        void Awake(){
            // Set Settings Paths
            _sdk_settings_path = Application.persistentDataPath + "/sdk_settings.vkconf"; // SDK Settings
            _app_settings_path = Application.persistentDataPath + "/app_settings.vkconf"; // App Settings
            _auth_data_path = Application.persistentDataPath + "/app_auth.vkconf"; // Authentication Data
            _log_data_path = Application.persistentDataPath + "/vk_sdk.log"; // Log Data Path

            // Check Instance of VKSDK
            if (instance == null){ // Instance not found
                instance = this; // Set this object to instance
            } else if (instance == this){ // Has instance
                Destroy(gameObject); // Destroy this Gamebase
            }

            // Cant destroy this object when load another
            // scene for singleton working state
            DontDestroyOnLoad(gameObject); // Don't Destroy Gamebase

            // Initialize Settings
            _initializeSettings();  // Start VK SDK Initialization
            initialized = true;     // Set Initialized Flag
        }

        //============================================================
        //  @class      VKSDK
        //  @method     OnEnable()
        //  @type       Internal Void
        //  @usage      Component Enabled Callback
        //============================================================
        void OnEnable(){
            Application.logMessageReceived += HandleLog;
        }

        //============================================================
        //  @class      VKSDK
        //  @method     OnDisable()
        //  @type       Internal Void
        //  @usage      Component Disabled Callback
        //============================================================
        void OnDisable(){
            Application.logMessageReceived -= HandleLog;
        }

        //============================================================
        //  @class      VKSDK
        //  @method     HandleLog()
        //  @type       Private Void
        //  @usage      Unity Log Handler
        //============================================================
        private void HandleLog(string logString, string stackTrace, LogType type){
            if (settings.debug_mode && type != LogType.Warning && type != LogType.Log){ // Crash Reporting to the Log
                bool critical = (type == LogType.Error || type == LogType.Assert) ? false : true; // Critical Error
                string message = logString + ". \n\n Stack Trace: " + stackTrace; // Log Message
                string code = "Unknown Error Type";
                if (type == LogType.Error)
                    code = "Script Error";
                if (type == LogType.Assert)
                    code = "Script Assert";
                if (type == LogType.Exception)
                    code = "Script Exception";

                // Save Log
                string _error_data = "";
                _error_data += "VK SDK Error Type: " + code + "\n\n";
                _error_data += "================================== \n\n";
                _error_data += message;
                Debug.Log("Возникло исключение при работе скрипта. Данные об ошибке записаны в "+_log_data_path);
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _initializeSettings()
        //  @type       Private Void
        //  @usage      Initialize SDK Settings
        //============================================================
        private void _initializeSettings(){
            // Load SDK Settings
            string _sdk_cfg = LoadSDKConfigs(_sdk_settings_path, true); // Load
            if (_sdk_cfg != null) settings = JsonUtility.FromJson<SDKSettingsModel>(_sdk_cfg); // Deserialize SDK Configs
            if (settings.debug_mode) Debug.Log("Инициализация VK SDK...");

            // Load App Settings
            string _app_cfg = LoadSDKConfigs(_app_settings_path, true); // Load
            if (_app_cfg != null) application = JsonUtility.FromJson<ApplicationSettingsModel>(_app_cfg); // Deserialize Application Configs

            // Load Authentication Data
            if (application.autosave_data){
                string _auth_data = LoadSDKConfigs(_auth_data_path, true); // Load
                if (_auth_data != null) authentication = JsonUtility.FromJson<AuthenticationModel>(_auth_data); // Deserialize Authenitcation Data
                if (settings.debug_mode && _auth_data!=null) Debug.Log("Токен пользователя инициализирован.");
            }

            // Initialize Platform
            if (settings.platform.Length < 1){ // Platform is not defined
                settings.platform = "other";
                if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) settings.platform = "mac";
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) settings.platform = "windows";
                if (settings.debug_mode) Debug.Log("VK SDK инициализирован для платформы: "+settings.platform);
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     LoadSDKConfigs()
        //  @type       Public Void
        //  @usage      Load Configurations
        //  @args       (string) path - Path to Config Files
        //              (bool) encoded - Config Files are Encoded?
        //  @return     (string/null) - Configuration Data
        //============================================================
        public string LoadSDKConfigs(string path, bool encoded = false){
            // Check File Exists
            if (!File.Exists(path)){ // No File
                if (settings.debug_mode) Debug.Log("Файл конфигураций \"" + path + "\" не найден. Будут использованы настройки из инспектора или по-умолчанию.");
                return null;
            }

            // Load File
            string _data = File.ReadAllText(path); // Read All Text

            // Decode File
            if (encoded){ // Only For Encoded Files
                byte[] _decodedBytes = Convert.FromBase64String(_data); // Get Decoded Bytes
                string _decodedData = Encoding.UTF8.GetString(_decodedBytes); // Get Decoded Text
                return _decodedData; // Return Decoded Data
            }

            // Return Data
            if (settings.debug_mode) Debug.Log("Файл конфигураций \"" + path + "\" был успешно загружен с устройства.");
            return _data;
        }

        //============================================================
        //  @class      VKSDK
        //  @method     SaveSDKConfigs()
        //  @type       Public Void
        //  @usage      Save Configurations
        //  @args       (string) path - Path to Config Files
        //              (string) data - Data to Save
        //              (bool) encoded - Encoding File Flag
        //============================================================
        public void SaveSDKConfigs(string path, string data, bool encoded = false){
            // Need Encoded
            if (encoded){ // Only if flag = true 
                byte[] _bytesToEncode = Encoding.UTF8.GetBytes(data); // Get Bytes to Encode
                data = Convert.ToBase64String(_bytesToEncode); // Encode Text
            }

            // Save File
            File.WriteAllText(path, data); // Save datas to file
            if (settings.debug_mode) Debug.Log("Файл конфигураций успешно сохранен в: " + path);
        }

        //============================================================
        //  @class      VKSDK
        //  @method     GoToDocsPage()
        //  @type       Internal Void
        //  @usage      Go to documentation Page
        //============================================================
        [ContextMenu("Перейти к документации")]
        void GoToDocsPage(){
            Application.OpenURL("https://vk.com/dev/manuals");
        }

        //============================================================
        //  @class      VKSDK
        //  @method     GoToAppPage()
        //  @type       Internal Void
        //  @usage      Go to application manage Page
        //============================================================
        [ContextMenu("Управление приложением")]
        void GoToAppPage(){
            int _app_id = (application.app_id>0)?application.app_id:-1;
            if (_app_id > -1){ // Has App ID
                Application.OpenURL("https://vk.com/editapp?id="+_app_id.ToString());
            } else {
                Application.OpenURL("https://vk.com/apps?act=manage");
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     Call()
        //  @type       Public void
        //  @usage      Call VK Method
        //  @args       (string) url - Method Name
        //              (WWWForm) data - Form Data
        //              (callback) complete - On Request Complete
        //              (error) error - On Request Error
        //============================================================
        public delegate void OnVKRequestDone(string json_response);
        public delegate void OnVKRequsetError(BaseErrorModel error);
        public void Call(string method, WWWForm data, OnVKRequestDone complete = null, OnVKRequsetError error = null){
            // Generate Form Data
            WWWForm requestedData = new WWWForm(); // Create WWWForm
            requestedData = data; // Set Base Data
            requestedData.AddField("access_token", authentication.access_token);
            if (settings.api_version.Length>0){
                requestedData.AddField("v", settings.api_version);
            } else {
                throw new Exception("Не указана версия API для работы с VK SDK");
            }

            // Send Request
            StartCoroutine(_sendRequest(_api_uri + method, requestedData, (string jr) => {
                if (complete != null)
                    complete(jr);
                if (settings.debug_mode) Debug.Log("VK API Response: " + jr);
            }, (BaseErrorModel err) => {
                if (error != null)
                    error(err);
            }));
        }

        //============================================================
        //  @class      VKSDK
        //  @method     Auth
        //  @type       Public void
        //  @usage      Authenticate User
        //  @args       (callback) complete - On Auth Complete
        //              (error) error - On Auth Error
        //============================================================
        public delegate void OnVKAuthComplete(string access_token); // Authentication Complete Delegate
        public delegate void OnVKAuthError(BaseErrorModel error); // Authentication Error Delegate
        public void Auth(OnVKAuthComplete complete = null, OnVKAuthError error = null){
            // Generate Scopes
            List<string> _slist = new List<string>();
            if (application.friends) _slist.Add("friends");
            if (application.notify) _slist.Add("notify");
            if (application.photos) _slist.Add("photos");
            if (application.audio) _slist.Add("audio");
            if (application.video) _slist.Add("video");
            if (application.stories) _slist.Add("stories");
            if (application.pages) _slist.Add("pages");
            if (application.status) _slist.Add("status");
            if (application.notes) _slist.Add("notes");
            if (application.messages) _slist.Add("messages");
            if (application.wall) _slist.Add("wall");
            if (application.ads) _slist.Add("ads");
            if (application.offline) _slist.Add("offline");
            if (application.docs) _slist.Add("docs");
            if (application.groups) _slist.Add("groups");
            if (application.notifications) _slist.Add("notifications");
            if (application.stats) _slist.Add("stats");
            if (application.email) _slist.Add("email");
            if (application.market) _slist.Add("market");
            string _scopes = String.Join(",", _slist.ToArray());

            // Get Authentication Code
            StartCoroutine(_getAuthCode(_scopes, (AuthCodeModel auth_code) => { // Authentication Code Successfully received
                _authCode = auth_code; // Set Auth Code Model
                _getAccessToken((string at) =>{ // Token Request Complete
                    complete(at); // Complete
                }, (BaseErrorModel e) => { // Token Request Error
                    if (error != null) error(e);
                });
            }, (BaseErrorModel e) => { // Authentication Code Request Error
                if (error != null) error(e); // Return Error
            }));
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _getAccessToken
        //  @type       Private Void
        //  @usage      Open Authentication Window and Get Access Token
        //  @args       (callback) complete - On Request Complete
        //              (error) error - On Request Error
        //============================================================
        public delegate void OnGetTokenComplete(string access_token); // Token Request Complete
        public delegate void OnGetTokenError(BaseErrorModel error); // Token Request Error
        private bool _isHandled = false; // Handle
        private void _getAccessToken(OnGetTokenComplete complete = null, OnGetTokenError error = null){
            // Generate URL
            string _url = _oauth_uri + "code_auth?stage=check&code=" + _authCode.auth_code; // Set URL

            // Open Window
            _isHandled = false; // Set Handled Flag
            Application.OpenURL(_url); // Open URL

            // Invoke Token Request
            StartCoroutine(_repeatTokenRequest((string at)=> {
                if (complete != null) complete(at); // Complete
            }, (BaseErrorModel e)=> {
                if (error != null) error(e);
            }));
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _getAuthCode()
        //  @type       Private IEnumerator
        //  @usage      Get VK Authentication Code
        //  @args       (callback) complete - On Request Complete
        //              (error) error - On Request Error
        //============================================================
        private delegate void OnAuthCodeCompleted(AuthCodeModel auth_code);
        private delegate void OnAuthCodeError(BaseErrorModel error);
        private IEnumerator _getAuthCode(string scopes, OnAuthCodeCompleted complete = null, OnAuthCodeError error = null){
            string _url = _oauth_uri + "get_auth_code?"; // Get Auth Code
            _url += "scope=" + scopes; // Set Scopes
            _url += "&client_id=" + application.app_id; // Set Application ID
            var request = new WWW(_url); // Create WWW Request
            yield return request; // Send Request

            // Work with Response
            if (request.error != null){ // Request has error
                throw new Exception("Не удалось отправить запрос к серверу VK API. Проверьте соединение с интернетом и попробуйте снова.");
            }else{ // No Errors
                if (request.text.Length < 1){ // Error
                    BaseErrorModel _error = new BaseErrorModel();
                    _error.error_code = 999;
                    _error.error_msg = "Не удалось получить код авторизации VK API. Попробуйте изменить ваш запрос.";
                    error(_error); // Call Error
                } else { // All Right
                    BaseRequestModel response = JsonUtility.FromJson<BaseRequestModel>(request.text); // Get Base Model from Response Text
                    if (response.error.error_code == -1){ // Response not has an errors
                        AuthCodeModel auth_code = JsonUtility.FromJson<AuthCodeModel>(request.text); // Get Authentication Model from Response
                        if (complete != null) complete(auth_code); // Return Complete
                    } else { // Reponse has errors
                        if (error != null) error(response.error); // Show Error
                        if (settings.debug_mode) Debug.Log("VK SDK Error: " + response.error.error_msg);
                    }
                }
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _requestAccessToken()
        //  @type       Private IEnumerator
        //  @usage      Get VK Access Token
        //  @args       (callback) complete - On Request Complete
        //              (error) error - On Request Error
        //============================================================
        private delegate void OnATComplete(string access_token);
        private delegate void OnATError(BaseErrorModel error);
        private IEnumerator _requestAccessToken(OnATComplete complete = null, OnATError error = null) {
            string _url = _oauth_uri + "code_auth_token?"; // Get Access Token URL
            _url += "device_id=" + _authCode.device_id; // Set Device ID
            _url += "&client_id=" + application.app_id; // Set Application ID
            var request = new WWW(_url); // Create WWW Request
            yield return request; // Send Request

            // Work with Response
            if (request.error != null){ // Request has error
                BaseErrorModel _err = new BaseErrorModel();
                _err.error_code = 999;
                _err.error_msg = "Токен не был получен, либо авторизация еще не пройдена. Повторная попытка авторизации...";
                error(_err);
            } else { // No Errors
                if (request.text.Length < 1) { // Error
                    BaseErrorModel _err = new BaseErrorModel();
                    _err.error_code = 999;
                    _err.error_msg = "Сервер передал пустой ответ. Повторная попытка авторизации...";
                } else { // All Right
                    BaseRequestModel response = JsonUtility.FromJson<BaseRequestModel>(request.text); // Get Base Model from Response Text
                    if (response.error.error_code == -1) { // Response not has an errors
                        AuthenticationModel auth = JsonUtility.FromJson<AuthenticationModel>(request.text); // Parse Auth Data
                        if (auth.access_token.Length > 0){ // Has Access Token
                            authentication = auth; // Set Authentication Data
                            _isHandled = true; // Set Handled

                            // Save Configuration
                            if (application.autosave_data){
                                SaveSDKConfigs(_auth_data_path, JsonUtility.ToJson(authentication), true);
                            }

                            // Return Done
                            complete(auth.access_token); // Return Token
                        } else { // Error
                            BaseErrorModel _err = new BaseErrorModel();
                            _err.error_code = 999;
                            _err.error_msg = "Сервер передал ответ без токена доступа. Повторная попытка авторизации...";
                            error(_err);
                        }
                    } else { // Reponse has errors
                        if (error != null) error(response.error); // Show Error
                        if (settings.debug_mode) Debug.Log("VK SDK Error: " + response.error.error_msg);
                    }
                }
            }
        }
        private IEnumerator _repeatTokenRequest(OnATComplete complete = null, OnATError error = null){
            int timeout = 0; // Set Timeout
            while(!_isHandled && timeout != settings.auth_timeout * 1000){ // Not Timeout and Not Token
                StartCoroutine(_requestAccessToken((string access_token) => { // Complete Token
                    _isHandled = true; // Handled
                    if (complete != null) complete(access_token); // Complete Callback
                }, (BaseErrorModel e) => { // Error
                    if (settings.debug_mode) Debug.Log("VK Auth State: " + e.error_msg);
                }));
                timeout++; // Timeout
                yield return new WaitForSeconds(settings.auth_interval); // Set Interval
            }
            if (!_isHandled){
                if (settings.debug_mode) Debug.Log("Таймаут авторизации в приложении VK");
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _sendRequest()
        //  @type       Private IEnumerator
        //  @usage      Send Request to Web Server
        //  @args       (string) url - Requested URL
        //              (WWWForm) data - Form Data
        //              (callback) complete - On Request Complete
        //              (error) error - On Request Error
        //============================================================
        private delegate void OnRequestCompleted(string json_response);
        private delegate void OnRequestError(BaseErrorModel error);
        private IEnumerator _sendRequest(string url, WWWForm data, OnRequestCompleted complete = null, OnRequestError error = null){
            var request = new WWW(url, data); // Create WWW Request
            yield return request; // Send Request

            // Work with Response
            if (request.error != null){ // Request has error
                throw new Exception("Не удалось отправить запрос к серверу VK API. Проверьте соединение с интернетом и попробуйте снова.");
            } else { // No Errors
                BaseRequestModel response = JsonUtility.FromJson<BaseRequestModel>(request.text); // Get Base Model from Response Text
                if (response.error.error_code == -1)
                { // Response not has an errors
                    if (complete != null)
                        complete(request.text); // Return Complete
                } else
                { // Reponse has errors
                    if (error != null)
                        error(response.error); // Show Error
                    if (settings.debug_mode) Debug.Log("VK SDK Error: " + response.error.error_msg);
                }
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     LoadImage()
        //  @type       Public IEnumerator
        //  @usage      Load an Image to the Container
        //============================================================
        public delegate void OnImageLoaded(); // Image Loaded
        public delegate void OnImageLoadError(string code); // Image Loading Error
        public IEnumerator LoadImage(string url, Image image, OnImageLoaded complete = null, OnImageLoadError error = null){
            url = url.Replace(@"\", ""); // Replace Wrong Slashes
            var request = new WWW(url); // Create WWW Request
            yield return request; // Send Request

            // Work with Response
            if (request.error != null){ // Request has error
                error("Не удалось загрузить изображение. Пожалуйста, проверьте соединение с интернетом и попробуйте снова."); // Do error
            } else { // No Errors
                image.sprite = Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), new Vector2(0, 0));
                if (complete != null) complete();
            }
        }
    }
}