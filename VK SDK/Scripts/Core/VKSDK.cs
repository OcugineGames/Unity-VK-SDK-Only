using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using ICODES.STUDIO.WWebView;
using System.Net;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

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
        private static string _oauth_uri = "https://oauth.vk.com/authorize";            // VK OAuth URI
        private static string _api_uri = "https://api.vk.com/method/";                  // VK API Gateway URI
        private static string _sdk_settings_path;                                       // VK SDK Settings Path
        private static string _app_settings_path;                                       // VK SDK Application Settings Path
        private static string _auth_data_path;                                          // VK Authentication Data Path

        // WebView Params
        private bool _webViewInitialized = false;                                       // WebView Initialization Status
        private GameObject _webViewObject = null;                                       // WebView Game Object
        private WebViewObject _macWVObj = null;                                         // MacOS WebView Object
        private WWebView _winWVObj = null;                                              // Windows WebView Object

        //============================================================
        //  @class      VKSDK
        //  @method     Awake()
        //  @type       Internal Void
        //  @usage      Call before scene initialized
        //============================================================
        void Awake(){
            // Set Settings Paths
            Debug.Log("Инициализация VK SDK...");
            _sdk_settings_path = Application.persistentDataPath + "/sdk_settings.vkconf"; // SDK Settings
            _app_settings_path = Application.persistentDataPath + "/app_settings.vkconf"; // App Settings
            _auth_data_path = Application.persistentDataPath + "/app_auth.vkconf"; // Authentication Data

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
            _webViewObject = _initializeWebView(); // Initialize WebView
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
            /*if (application.auto_crash_reporting && type != LogType.Warning && type != LogType.Log && this.reports != null){ // Auto Crash reporting
                bool critical = (type == LogType.Error || type == LogType.Assert) ? false : true; // Critical Error
                string message = logString + ". \n\n Stack Trace: " + stackTrace; // Log Message
                string code = "UERROR:";
                if (type == LogType.Error)
                    code = code + " Error";
                if (type == LogType.Assert)
                    code = code + " Assert";
                if (type == LogType.Exception)
                    code = code + " Exception";
            }*/
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

            // Load App Settings
            string _app_cfg = LoadSDKConfigs(_app_settings_path, true); // Load
            if (_app_cfg != null) application = JsonUtility.FromJson<ApplicationSettingsModel>(_app_cfg); // Deserialize Application Configs

            // Load Authentication Data
            if (application.autosave_data){
                string _auth_data = LoadSDKConfigs(_auth_data_path, true); // Load
                if (_auth_data != null) authentication = JsonUtility.FromJson<AuthenticationModel>(_auth_data); // Deserialize Authenitcation Data
            }

            // Initialize Platform
            if (settings.platform.Length < 1){ // Platform is not defined
                settings.platform = "other";
                if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) settings.platform = "mac";
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) settings.platform = "windows";
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _getWebView()
        //  @type       Private Void
        //  @usage      Initialize WebView
        //============================================================
        private GameObject _initializeWebView(){
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer){
                if (!_webViewInitialized){ // WebView is Not Initialized
                    _webViewObject = new GameObject(); // Create Object
                    _webViewObject.transform.SetSiblingIndex((this.gameObject.transform.GetSiblingIndex() + 1)); // Sort
                    _winWVObj = _webViewObject.AddComponent<WWebView>(); // Create Component and Set Link
                    _webViewObject.SetActive(false);
                    return _webViewObject; // Return
                } else {
                    return _webViewObject; // Return
                }
            } else {
                if (!_webViewInitialized){ // WebView is Not Initialized
                    _webViewObject = new GameObject(); // Create Object
                    _webViewObject.transform.SetSiblingIndex((this.gameObject.transform.GetSiblingIndex()+1)); // Sort
                    _macWVObj = _webViewObject.AddComponent<WebViewObject>(); // Create Component and set Link
                    _macWVObj.Init(cb: (msg) => {
                            Debug.Log(string.Format("CallFromJS[{0}]", msg));
                        },
                        err: (msg) => {
                            OnMacNavigationError(msg);
                            Debug.Log(string.Format("CallOnError[{0}]", msg));
                        },
                        started: (msg) => {
                            Debug.Log(string.Format("CallOnStarted[{0}]", msg));
                        },
                        ld: (msg) => {
                            OnMacNavigationComplete(msg);
                            Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                            #if UNITY_EDITOR_OSX || !UNITY_ANDROID
                            #if true
                            _macWVObj.EvaluateJS(@"
                                      if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                                        window.Unity = {
                                          call: function(msg) {
                                            window.webkit.messageHandlers.unityControl.postMessage(msg);
                                          }
                                        }
                                      } else {
                                        window.Unity = {
                                          call: function(msg) {
                                            window.location = 'unity:' + msg;
                                          }
                                        }
                                      }
                                    ");
                            #else
                                    webView.EvaluateJS(@"
                                      if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                                        window.Unity = {
                                          call: function(msg) {
                                            window.webkit.messageHandlers.unityControl.postMessage(msg);
                                          }
                                        }
                                      } else {
                                        window.Unity = {
                                          call: function(msg) {
                                            var iframe = document.createElement('IFRAME');
                                            iframe.setAttribute('src', 'unity:' + msg);
                                            document.documentElement.appendChild(iframe);
                                            iframe.parentNode.removeChild(iframe);
                                            iframe = null;
                                          }
                                        }
                                      }
                                    ");
                            #endif
                            _macWVObj.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                            #endif
                        }, enableWKWebView: true);
                    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                        _macWVObj.bitmapRefreshCycle = 1;
                    #endif
                    _macWVObj.SetMargins(0, 0, 0, 0);
                    _macWVObj.SetVisibility(false);
                    _webViewObject.SetActive(false);
                    return _webViewObject; // Return
                } else { // Initialized
                    return _webViewObject; // Return
                }
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
                Debug.Log("Файл конфигураций \"" + path + "\" не найден. Будут использованы настройки из инспектора или по-умолчанию.");
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
            Debug.Log("Файл конфигураций \"" + path + "\" был успешно загружен с устройства.");
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
            Debug.Log("Файл конфигураций успешно сохранен в: " + path);
        }

        //============================================================
        //  @class      VKSDK
        //  @method     Start()
        //  @type       Internal Void
        //  @usage      Call when scene initialized
        //============================================================
        void Start(){

        }

        //============================================================
        //  @class      VKSDK
        //  @method     Update()
        //  @type       Internal Void
        //  @usage      Call every tick
        //============================================================
        void Update(){
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
                Debug.Log("VK API Response: " + jr);
            }, (BaseErrorModel err) => {
                if (error != null)
                    error(err);
            }));
        }

        //============================================================
        //  @class      VKSDK
        //  @method     ShowLoginWindow()
        //  @type       Public void
        //  @usage      Show Authentication Window
        //============================================================
        private bool _handlersInitialized = false; // Authentication WebView Handlers Flag
        private bool _isAuthenticationProcess = false; // Authentication Process Flag
        private string _finalAuthURL = ""; // Final Authenrication URL
        public delegate void OnVKAuthDoneDelegate(string access_token);
        public delegate void OnVKAuthErrorDelegate(BaseErrorModel error);
        public event OnVKAuthDoneDelegate OnAuthenticationComplete;
        public event OnVKAuthErrorDelegate OnAuthenticationError;
        public void ShowLoginWindow(){
            // Show General Object
            _webViewObject.SetActive(true);

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

            // Detect Platform
            _isAuthenticationProcess = true;
            _finalAuthURL = _oauth_uri + "?client_id=" + application.app_id + "&display=" + application.display + "&scope=" + _scopes + "&response_type=" + application.response_type + "&v=" + settings.api_version + "&state=" + application.state;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer){ // Windows
                if (!_handlersInitialized){
                    _winWVObj.OnNavigationCompleted += OnWindowsNavigationComplete;
                    _winWVObj.OnNavigationFailed += OnWindowsNavigationError;
                    _winWVObj.OnClose += OnWindowsWebViewClosed;
                    _handlersInitialized = true;
                }
                _winWVObj.Navigate(_finalAuthURL);
                _winWVObj.Show(); // Show
            } else { // Mac
                _macWVObj.LoadURL(_finalAuthURL);
                _macWVObj.SetVisibility(true); // Set Visibility
            }
        }
        //============================================================
        //  @class      VKSDK
        //  @method     CloseLoginWindow()
        //  @type       Public void
        //  @usage      Show Authentication Window
        //============================================================
        public void CloseLoginWindow(){
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer){ // Windows
                _isAuthenticationProcess = false;
                _winWVObj.Stop(); // Stop Loading
                _winWVObj.Hide(); // Hide
            } else {
                _isAuthenticationProcess = false;
                _macWVObj.SetVisibility(false); // Set Visibility
            }

            // Hide General Object
            _webViewObject.SetActive(false);
        }

        // Private Handlers for WebView
        private void OnWindowsNavigationComplete(WWebView webView, string data){
            _CompleteAuthentication(data, true);
        }
        private void OnWindowsNavigationError(WWebView webView, int code, string url){
            if (_isAuthenticationProcess){
                _isAuthenticationProcess = false;
                BaseErrorModel _error = new BaseErrorModel();
                _error.error_code = code;
                _error.error_msg = "Ошибка авторизации: не удалось перейти на веб-страницу "+url;
                if(OnAuthenticationError!=null) OnAuthenticationError(_error);
            }
        }
        private bool OnWindowsWebViewClosed(WWebView webView){
            if (_isAuthenticationProcess){
                _isAuthenticationProcess = false;
                BaseErrorModel _error = new BaseErrorModel();
                _error.error_code = 999;
                _error.error_msg = "Ошибка авторизации: пользователь закрыл окно авторизации";
                if (OnAuthenticationError != null)
                    OnAuthenticationError(_error);
                return true;
            } else {
                return true;
            }
        }
        private void OnMacNavigationComplete(string data){
            _CompleteAuthentication(data, false);
        }
        private void OnMacNavigationError(string message){
            if (_isAuthenticationProcess){
                _isAuthenticationProcess = false;
                BaseErrorModel _error = new BaseErrorModel();
                _error.error_code = 500;
                _error.error_msg = "Ошибка авторизации: " + message;
                if (OnAuthenticationError != null) OnAuthenticationError(_error);
            }
        }
        private void _CompleteAuthentication(string data, bool is_windows){
            Debug.Log("WebView Navigated to: " + data);
            NameValueCollection _keys = new NameValueCollection(); // Create Collection

            // Check URL
            if (data.Contains("https://oauth.vk.com/blank.html"))
            { // Has URL
                _keys = _parseQueryString(data);
            }
            if (_isAuthenticationProcess && data != _finalAuthURL){
                if (_keys["access_token"] == null && _keys.Count > 0){
                    _isAuthenticationProcess = false;
                    BaseErrorModel _error = new BaseErrorModel();
                    _error.error_code = 998;
                    _error.error_msg = "Ошибка авторизации: сервер не вернул токен доступа пользователя";
                    if (OnAuthenticationError != null)
                        OnAuthenticationError(_error);
                } else {
                    _isAuthenticationProcess = false;
                    authentication.access_token = _keys["access_token"];
                    authentication.expires_in = (_keys["expires_in"] != null) ? int.Parse(_keys["expires_in"]) : 0;
                    authentication.user_id = (_keys["user_id"] != null) ? int.Parse(_keys["user_id"]) : 0;
                    if (OnAuthenticationComplete != null)
                        OnAuthenticationComplete(authentication.access_token);
                    if (is_windows){
                        _winWVObj.Hide(); // Hide WebView
                    } else {
                        _macWVObj.SetVisibility(false);
                    }

                    // Hide General Window
                    _webViewObject.SetActive(false);

                    // Save Configuration
                    if (application.autosave_data){
                        SaveSDKConfigs(_auth_data_path, JsonUtility.ToJson(authentication), true);
                    }
                }
            }
        }

        //============================================================
        //  @class      VKSDK
        //  @method     _parseQueryString()
        //  @type       Private static void
        //  @usage      Parse URL Params
        //  @args       (string) url - Requested URL
        //============================================================
        private static NameValueCollection _parseQueryString(string url){
            NameValueCollection nvc = new NameValueCollection();
            // remove anything other than query string from url
            if (url.Contains("#")){
                url = url.Substring(url.IndexOf('#') + 1);
            }
            foreach (string vp in Regex.Split(url, "&")){
                string[] singlePair = Regex.Split(vp, "=");
                if (singlePair.Length == 2){
                    nvc.Add(singlePair[0], singlePair[1]);
                } else {
                    nvc.Add(singlePair[0], string.Empty);
                }
            }
            return nvc;
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
                    Debug.Log("VK SDK Error: " + response.error.error_msg);
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

