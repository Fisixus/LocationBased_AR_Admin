using Mapbox.Unity.Location;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json;
using Mapbox.Unity.Map;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    public GameObject contentPanelOnlines;
    public GameObject contentPanelUserInfo;
    public GameObject onlineUserButton;
    public GameObject userAvatar;
    public AbstractMap mapManager;
    public GameObject LatInputField;
    public GameObject LotInputField;

    #region USER INFO PANEL
    public GameObject UserInfo;
    public TextMeshProUGUI UsernameData;
    public TextMeshProUGUI FirstnameData;
    public TextMeshProUGUI SurnameData;
    public TextMeshProUGUI UserLatitudeData;
    public TextMeshProUGUI UserLongitudeData;
    public TextMeshProUGUI UserAltitudeData;
    public GameObject SymbolData;
    #endregion
    #region SYMBOL INFO PANEL
    public GameObject SymbolInfo;
    public TextMeshProUGUI SymbolOwnerData;
    public TextMeshProUGUI SymbolNameData;
    public TextMeshProUGUI CategoryData;
    public TextMeshProUGUI SymbolLatitudeData;
    public TextMeshProUGUI SymbolLongitudeData;
    public TextMeshProUGUI SymbolAltitudeData;
    public TextMeshProUGUI MessageData;
    #endregion
    private List<User> onlineUsers = new List<User>();
    private Vector3 cameraStartingPos;
    private Quaternion cameraStartingRotation;
    private bool camFocusOnFirstOnline = true;

    //This keeps last selected user
    private User selectedUser = null;
    //This keeps last selected symbol
    private Symbol selectedSymbol = null;


    private void Awake()
    {
        cameraStartingPos = Camera.main.transform.position;
        cameraStartingRotation = Camera.main.transform.rotation;
    }

    private void Start()
    {
        InvokeRepeating("RefreshOnlineUsers", 4.0f, 5f);             
    }

    private void CreateOnlineUserButtonsandAvatars()
    {        
        foreach(User user in onlineUsers)
        {
            ///For the starting camera focus
            if (camFocusOnFirstOnline)
            {
                Camera.main.transform.position = cameraStartingPos;
                Camera.main.transform.rotation = cameraStartingRotation;
                mapManager.UpdateMap(new Vector2d((double)user.Latitude, (double)user.Longitude), 16f);
                selectedUser = user;
                RefreshUserInfo();
            }
            /// For refreshing user info                
            if (selectedUser != null)
            {
                if (user.Username.Equals(UsernameData.text))
                {
                    RefreshUserInfo();
                    ///TODO There could be an option about focusing selected user's location
                    //UpdateCameraPosition();
                }
            }

            /// For refreshing symbol info                
            if (selectedSymbol != null)
            {
                if (user.Username.Equals(UsernameData.text))
                {
                    RefreshSymbolInfo();
                }
            }

            GameObject avatar = Instantiate(userAvatar, mapManager.GeoToWorldPosition(new Vector2d((double)user.Latitude, (double)user.Longitude), true), Quaternion.identity);
            avatar.name = user.Username;
            //avatar.transform.GetChild(0).GetComponent<Renderer>().material = selectedUserMaterial;
            GameObject goButton = (GameObject)Instantiate(onlineUserButton);
            goButton.transform.SetParent(contentPanelOnlines.transform, false);
            goButton.GetComponentInChildren<TextMeshProUGUI>().text = user.Username;
            goButton.GetComponent<Button>().onClick.AddListener(LocateUserLocation);
            goButton.GetComponent<Button>().onClick.AddListener(GetUserInfoAndSetColors);
            /*
            if(selectedUser != null)
            {
                //get info ????
                if (user.getUUID.Equals(selectedUser.getUUID))
                {
                    //give color red to avatar
                }
                else
                {
                    //give color yellow to avatar  
                }
            }
            */
            camFocusOnFirstOnline = false;
        }
        if (onlineUsers.Count > 0)
        {            
            /// ALL Button
            GameObject allButton = (GameObject)Instantiate(onlineUserButton);
            allButton.transform.SetParent(contentPanelOnlines.transform, false);
            allButton.GetComponentInChildren<TextMeshProUGUI>().text = "ALL";
            allButton.GetComponent<Button>().onClick.AddListener(AllInfo);

            ColorUtilityManager.Instance.SetColorofOnlineUserButtons(selectedUser);
            ColorUtilityManager.Instance.SetColorofAvatars(selectedUser);
        }
    }

    private void DeleteOnlineUserButtonsandAvatars()
    {        
        var onlineButtons = GameObject.FindGameObjectsWithTag("OnlineButtons");
        var userAvatars = GameObject.FindGameObjectsWithTag("UserAvatar");
        var onlineButtonsandUserAvatars = onlineButtons.Concat(userAvatars).ToArray();
        foreach (GameObject gObj in onlineButtonsandUserAvatars)
        {
            Destroy(gObj);
        }        
    }

    private void FindOnlineUsers()
    {
        onlineUsers.Clear();
        bool onlineUserInfoPanelOpenedPersonLeft = true;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        foreach (var user in allUsers)
        {
            if (user.Online)
            {
                onlineUsers.Add(user);     
                if(selectedUser != null && selectedUser.Username.ToLower().Equals(user.Username.ToLower()))
                {
                    onlineUserInfoPanelOpenedPersonLeft = false;
                }
            }
        }
        if (onlineUsers.Count == 0) camFocusOnFirstOnline = true;

        //TODO Not Working Right
        if (onlineUserInfoPanelOpenedPersonLeft)
        {
            CloseUserInfo();
            CloseSymbolInfo();
            DeleteSymbolImages();
            selectedUser = null;
            selectedSymbol = null;
        }
    }

    private void RefreshOnlineUsers()
    {
        FindOnlineUsers();
        DeleteOnlineUserButtonsandAvatars();
        CreateOnlineUserButtonsandAvatars();
    }

    
    /// Online User Button's OnClick Event
    private void LocateUserLocation()
    {
        mapManager.ResetMap();        
        GameObject gObj = EventSystem.current.currentSelectedGameObject;
        string buttonText = gObj.GetComponentInChildren<TextMeshProUGUI>().text.ToString();
        foreach (User user in onlineUsers)
        {
            ///Which locates the user location that belongs to clicked button 
            if (buttonText.ToLower().Equals(user.Username.ToLower()))
            {
                ///Find Location of user and move the camera there
                Camera.main.transform.position = cameraStartingPos;
                Camera.main.transform.rotation = cameraStartingRotation;
                mapManager.UpdateMap(new Vector2d((double)user.Latitude, (double)user.Longitude), 16f);
                /// This is the solution of altitude problem of avatar
                RefreshOnlineUsers();
                break;
            }
        }        
    }

    private void AllInfo()
    {
        selectedUser = null;
        selectedSymbol = null;
        CloseUserInfo();
        CloseSymbolInfo();
        /*
        mapManager.ResetMap();
        float totalLatitude = 0f;
        float totalLongitude = 0f;
        //TODO Mathematical stuff like this
        foreach (User user in onlineUsers)
        {
            totalLatitude += user.Latitude;
            totalLongitude += user.Longitude;
        }
        totalLatitude /= onlineUsers.Count;
        totalLongitude /= onlineUsers.Count; 
        Debug.Log("TotalLatitude:" + totalLatitude);
        Debug.Log("TotalLongitude:" + totalLongitude);
        mapManager.UpdateMap(new Vector2d(totalLatitude, totalLongitude), 16f);
    }
    */
    }

    public void Search()
    {        
        decimal result = 0m;
        decimal Lot = 0m; 
        decimal Lat = 0m;
        if (decimal.TryParse(LatInputField.GetComponent<InputField>().text.ToString(), out result))
        {
            Lat = decimal.Parse(LatInputField.GetComponent<InputField>().text.ToString().Trim(), CultureInfo.InvariantCulture.NumberFormat);
        }
        if (decimal.TryParse(LotInputField.GetComponent<InputField>().text.ToString().ToString(), out result))
        {
            Lot = decimal.Parse(LotInputField.GetComponent<InputField>().text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
        }
        if(Lot != 0m && Lat != 0m)
        {
            CloseUserInfo();
            CloseSymbolInfo();
            mapManager.UpdateMap(new Vector2d((double)Lat, (double)Lot), 16f);
            RefreshOnlineUsers();
        }
        else
        {
            Debug.Log("You only can give decimal type of input!");
        }
            
    }
    
    public void GetUserInfoAndSetColors()
    {
        UserInfo.SetActive(true);
        GameObject gObj = EventSystem.current.currentSelectedGameObject;        
        string buttonText = gObj.GetComponentInChildren<TextMeshProUGUI>().text.ToString();
        
        foreach (User user in onlineUsers)
        {
            ///Which brings user info that belongs to clicked button 
            if (buttonText.ToLower().Equals(user.Username.ToLower()))
            {
                selectedUser = user;
                ///This part is for separating  userInfo and symbolInfo panels.
                if(selectedSymbol != null)
                {
                    if (selectedUser.getUUID.Equals(selectedSymbol.UserUUID))
                    {
                        RefreshSymbolInfo();
                    }
                    else
                    {
                        selectedSymbol = null;
                        CloseSymbolInfo();
                    }
                }               
                RefreshUserInfo();               
                break;
            }
        }
        ColorUtilityManager.Instance.SetColorofOnlineUserButtons(selectedUser);
        ColorUtilityManager.Instance.SetColorofAvatars(selectedUser);
    }

    private void RefreshUserInfo()
    {
        DeleteSymbolDatas();
        DeleteSymbolImages();
        selectedUser.Symbols.Clear();

        UsernameData.text = selectedUser.Username;
        FirstnameData.text = selectedUser.Firstname;
        SurnameData.text = selectedUser.Surname;
        UserLatitudeData.text = selectedUser.Latitude.ToString();
        UserLongitudeData.text = selectedUser.Longitude.ToString();
        UserAltitudeData.text = selectedUser.Altitude.ToString();
        RefreshUserSymbols();
    }

    private void RefreshUserSymbols()
    {       
        string data = WebServiceManager.Instance.getAllSymbolsData();
        List<Symbol> allSymbols = JsonConvert.DeserializeObject<List<Symbol>>(data);
        
        foreach(Symbol symbol in allSymbols)
        {
            if (symbol.UserUUID.Equals(selectedUser.getUUID))
            {
                selectedUser.Symbols.Add(symbol);
                GameObject symbolInfo = (GameObject)Instantiate(SymbolData);
                symbolInfo.transform.SetParent(contentPanelUserInfo.transform, false);
                symbolInfo.name = symbol.SymbolName;
                symbolInfo.GetComponentInChildren<TextMeshProUGUI>().text = symbol.SymbolName;
                symbolInfo.GetComponent<Button>().onClick.AddListener(GetSymbolInfo);
                symbolInfo.GetComponent<Button>().onClick.AddListener(LocateSymbolLocation);
                SymbolManager.Instance.CreateSymbolImageOnMap(symbol);
            }
        }
    }

    //TODO In the glass application, there should be a control about SymbolName, a user have to give his symbols with unique names.
    private void GetSymbolInfo()
    {
        SymbolInfo.SetActive(true);
        GameObject gObj = EventSystem.current.currentSelectedGameObject;
        string buttonText = gObj.GetComponentInChildren<TextMeshProUGUI>().text.ToString();
        foreach (Symbol symbol in selectedUser.Symbols)
        {
            ///Which brings user info that belongs to clicked button 
            if (buttonText.ToLower().Equals(symbol.SymbolName.ToLower()))
            {
                selectedSymbol = symbol;
                RefreshSymbolInfo();
                break;
            }
        }        
    }

    private void RefreshSymbolInfo()
    {
        SymbolOwnerData.text = selectedUser.Username;
        SymbolNameData.text = selectedSymbol.SymbolName;
        CategoryData.text = selectedSymbol.Category.ToString();
        SymbolLatitudeData.text = selectedSymbol.Latitude.ToString();
        SymbolLongitudeData.text = selectedSymbol.Longitude.ToString();
        SymbolAltitudeData.text = selectedSymbol.Altitude.ToString();
        MessageData.text = selectedSymbol.Message;
    }

    //TODO In the eyeglass application, there should be a control about SymbolName, a user have to give his symbols with unique names. 
    private void LocateSymbolLocation()
    {
        GameObject gObj = EventSystem.current.currentSelectedGameObject;
        string buttonText = gObj.GetComponentInChildren<TextMeshProUGUI>().text.ToString();
        foreach (Symbol symbol in selectedUser.Symbols)
        {
            ///Which locates the selected user's symbol location that belongs to clicked button 
            if (buttonText.ToLower().Equals(symbol.SymbolName.ToLower()))
            {
                ///Find Location of selected user's symbol and move the camera there
                Camera.main.transform.position = cameraStartingPos;
                Camera.main.transform.rotation = cameraStartingRotation;
                mapManager.UpdateMap(new Vector2d((double)symbol.Latitude, (double)symbol.Longitude), 16f);
                /// This is the solution of altitude problem of avatar
                RefreshOnlineUsers();
                break;
            }
        }
    }

    private void DeleteSymbolDatas()
    {
        var symbolDatas = GameObject.FindGameObjectsWithTag("SymbolData");
        foreach (GameObject gObj in symbolDatas)
        {
            Destroy(gObj);
        }
    }

    private void DeleteSymbolImages()
    {
        var symbolImages = GameObject.FindGameObjectsWithTag("SymbolImage");
        foreach (GameObject gObj in symbolImages)
        {
            Destroy(gObj);
        }
    }

    public void CloseUserInfo()
    {
        DeleteSymbolDatas();
        UserInfo.SetActive(false);
    }

    public void CloseSymbolInfo()
    {
        SymbolInfo.SetActive(false);
    }

}
