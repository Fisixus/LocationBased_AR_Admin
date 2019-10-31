using Mapbox.Unity.Map;
using Mapbox.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SymbolManager : MonoBehaviour
{
    public static SymbolManager Instance;
    public List<Texture> textureList;
    public AbstractMap mapManager;

    public GameObject symbolImage;

    public GameObject addSymbolPanelPreb;
    public GameObject addSymbolMarker;
    public Canvas canvas;

    private GameObject addSymbolPanel;
    private EventSystem m_EventSystem;
    private PointerEventData m_PointerEventData;
    private GraphicRaycaster m_Raycaster;
    private List<RaycastResult> UIResults = new List<RaycastResult>();

    private bool addSymbolPanelOpen = false;

    public void setAddSymbolPanelOpen(bool value)
    {
        addSymbolPanelOpen = value;
    }

    public bool getAddSymbolPanelOpen()
    {
        return addSymbolPanelOpen;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    private void LateUpdate()
    {
        MoveAddSymbolPanel();
    }

    private void MoveAddSymbolPanel()
    {
        if (!addSymbolPanelOpen) return;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
        {
            m_PointerEventData.position = Input.mousePosition;
            m_Raycaster.Raycast(m_PointerEventData, UIResults);
            for (int i = 0; i < UIResults.Count; i++)
            {
                //Debug.Log("Name:" + UIResults[i].gameObject.name);
                if (UIResults[i].gameObject.name.Equals("MovePanel"))
                {
                    addSymbolPanel.transform.position = Input.mousePosition;
                }
            }
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonUp(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touches[0].phase == TouchPhase.Ended)
#endif
        {
            UIResults.Clear();
        }
    }

    public void CreateSymbolImageOnMap(Symbol symbol)
    {
        switch (symbol.Category)
        {
            case Category.Ambulance:
                SetImage(symbol, 0);                
                break;
            case Category.Barrier:
                SetImage(symbol, 1);
                break;
            case Category.Blast:
                SetImage(symbol, 2);
                break;
            case Category.Bomb:
                SetImage(symbol, 3);
                break;
            case Category.Car:
                SetImage(symbol, 4);
                break;
            case Category.Construction:
                SetImage(symbol, 5);
                break;
            case Category.Ditch:
                SetImage(symbol, 6);
                break;
            case Category.Fire:
                SetImage(symbol, 7);
                break;
            case Category.Firstaid:
                SetImage(symbol, 8);
                break;
            case Category.Hunting:
                SetImage(symbol, 9);
                break;
            case Category.Male:
                SetImage(symbol, 10);
                break;
            case Category.Office_building:
                SetImage(symbol, 11);
                break;
            case Category.Police:
                SetImage(symbol, 12);
                break;
            case Category.Soldier:
                SetImage(symbol, 13);
                break;
            case Category.Traffic_light:
                SetImage(symbol, 14);
                break;
            default:
                break;
        }
    }

    private void SetImage(Symbol symbol, int textureNo)
    {
        
        symbolImage.GetComponentInChildren<RawImage>().texture = textureList[textureNo];
        GameObject image = Instantiate(symbolImage, mapManager.GeoToWorldPosition(new Vector2d((double)symbol.Latitude, (double)symbol.Longitude), true), Quaternion.identity);
        image.name = symbol.SymbolName;
        image.tag = "SymbolImage";
    }
    
    public void AddSymbol()
    {
        addSymbolMarker.transform.position = Input.mousePosition;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonUp(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount == 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
#endif
        {
            addSymbolMarker.SetActive(false);

            addSymbolPanel = Instantiate(addSymbolPanelPreb, Input.mousePosition, Quaternion.identity, canvas.transform);
            addSymbolPanel.name = "AddSymbolPanel";


            addSymbolPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(CloseAddSymbolAction);
            addSymbolPanel.transform.GetChild(1).GetComponentInChildren<Button>().onClick.AddListener(AddSymbolAction);

            //This is for filling lat lot values automatically
            addSymbolPanel.transform.Find("ScrollView/ContentPanel/LatitudeDATA").GetComponent<InputField>().text = CameraManager.Instance.getLatitude().ToString();
            addSymbolPanel.transform.Find("ScrollView/ContentPanel/LongitudeDATA").GetComponent<InputField>().text = CameraManager.Instance.getLongitude().ToString();
            addSymbolPanel.transform.Find("ScrollView/ContentPanel/AltitudeDATA").GetComponent<InputField>().text = "0.0";

            FillUserDropdown();
            addSymbolPanelOpen = true;
        }

    }

    private void FillUserDropdown()
    {
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        TMP_Dropdown symbolOwnerDropdown = addSymbolPanel.transform.Find("ScrollView/ContentPanel/SymbolOwnerDATA").GetComponent<TMP_Dropdown>();
        symbolOwnerDropdown.options.Clear();
        foreach(User user in allUsers)
        {
            if(user.Role == Role.Standart)
                symbolOwnerDropdown.options.Add(new TMP_Dropdown.OptionData() { text = user.Username });
        }
    }

    public void AddSymbolAction()
    {
        Category category;
        decimal result;

        string symbolNameDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/SymbolNameDATA").GetComponent<InputField>().text.Trim();
        //TODO geo info must writed automatically by WorldToGeoPosition method
        string latitudeDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/LatitudeDATA").GetComponent<InputField>().text.Trim();
        string longitudeDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/LongitudeDATA").GetComponent<InputField>().text.Trim();
        string altitudeDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/AltitudeDATA").GetComponent<InputField>().text.Trim();
        string messageDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/MessageDATA").GetComponentInChildren<TMP_InputField>().text.Trim();

        TMP_Dropdown categoryDrowdown = addSymbolPanel.transform.Find("ScrollView/ContentPanel/CategoryDATA").GetComponent<TMP_Dropdown>();
        string categoryDATA = categoryDrowdown.options[categoryDrowdown.value].text;

        //TODO This would be multi selected dropbox for the multiple assigns
        TMP_Dropdown symbolOwnerDropdown = addSymbolPanel.transform.Find("ScrollView/ContentPanel/SymbolOwnerDATA").GetComponent<TMP_Dropdown>();
        string symbolOwnerDATA = symbolOwnerDropdown.options[symbolOwnerDropdown.value].text;


        bool nameIsValid = true;


        //List<User> selectedUsers = new List<User>();
        User dataUser = null;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);

        foreach(User user in allUsers)
        {
            if (user.Username.Equals(symbolOwnerDATA))
            {
                //TODO dataUser should be list of selected users
                dataUser = user;
                break;
            }
        }
        List<string> userSymbolNames = UserManager.Instance.GetUsersSymbolNames(dataUser);

        for(int i=0; i < userSymbolNames.Count; i++)
        {
            if (userSymbolNames[i].ToLower().Trim().Equals(symbolNameDATA.ToLower().Trim()))
            {
                nameIsValid = false;
                break;
            }

        }

        bool postControl = (decimal.TryParse(longitudeDATA.ToString().Trim(), out result)) && (decimal.TryParse(latitudeDATA.ToString().Trim(), out result)) && (decimal.TryParse(altitudeDATA.ToString().Trim(), out result)) && (Enum.TryParse(categoryDATA, out category)) && (nameIsValid) && (!symbolNameDATA.Equals(""));
        /*
        Debug.Log("longControl:" + (decimal.TryParse(longitudeDATA.ToString().Trim(), out result)));
        Debug.Log("latControl:" + (decimal.TryParse(latitudeDATA.ToString().Trim(), out result)));
        Debug.Log("altControl:" + (decimal.TryParse(altitudeDATA.ToString().Trim(), out result)));
        Debug.Log("categoryControl:" + (Enum.TryParse(categoryDATA, out category)));
        Debug.Log("NameNull:" + (symbolNameDATA != null));
        Debug.Log("NameValid:" + (nameIsValid));
        */
        if (postControl)
        {
            Symbol symbol = new Symbol();
            //TODO For every selected user
            symbol.SymbolName = symbolNameDATA;
            //Debug.Log("SymbolName:" + symbolNameDATA);
            symbol.Latitude = decimal.Parse(latitudeDATA.Replace(',','.'), CultureInfo.InvariantCulture.NumberFormat);
            //Debug.Log("LatData:" + latitudeDATA);
            symbol.Longitude = decimal.Parse(longitudeDATA.Replace(',','.'), CultureInfo.InvariantCulture.NumberFormat);
            //Debug.Log("LongData:" + longitudeDATA);
            symbol.Altitude = decimal.Parse(altitudeDATA.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
            symbol.Category = (Category)Enum.Parse(typeof(Category), categoryDATA);
            if (messageDATA.Equals("")) messageDATA = "-";
            symbol.Message = messageDATA;
            //Debug.Log("MessageData:" + messageDATA);
            //Debug.Log("SymbolOwner:" + symbolOwnerDATA);
            symbol.UserUUID = UserManager.Instance.FindUserUUIDbyUsername(symbolOwnerDATA);
            //Debug.Log("UserUUID:" + symbol.UserUUID);
            WebServiceManager.Instance.AddSymbol(symbol);
            CloseAddSymbolAction();
        }

        else
        {
            Debug.Log("This request is not eligible!");
        }

    }

    public void ActivateMarker()
    {
        addSymbolMarker.transform.position = Input.mousePosition;        
        addSymbolMarker.SetActive(true);
    }

    public void CloseAddSymbolAction()
    {
        Destroy(addSymbolPanel);
        addSymbolPanelOpen = false;
        CameraManager.Instance.NavigateMode();
    }

    public void DeleteSymbol()
    {

    }

}
