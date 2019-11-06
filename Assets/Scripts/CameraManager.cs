using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public float moveSpeedPerPixel = 0.05f;
    public float moveSpeedPerPixelForAndroid = 1000f;
    public float rotationSpeedPerPixel = 0.05f;
    public float zoomSpeedPerWheel = 0.75f;
    public float zoomSpeedPerWheelForAndroid = 200f;
    public float zoomMinDistance = 25f;
    public float zoomMaxDistance = 200f;
    public Canvas canvas;
    public TextMeshProUGUI LatLotMouse;
    public AbstractMap mapManager;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    public GameObject userInfoLockPanel;
    public GameObject onlineUsersLockPanel;
    public GameObject searchAreaLockPanel;

    private float yaw = 0.0f;
    private float pitch = 45.0f;
    private EventSystem m_EventSystem;
    private PointerEventData m_PointerEventData;
    private GraphicRaycaster m_Raycaster;
    private List<RaycastResult> UIResults = new List<RaycastResult>();

    bool addSymbolMode = false;
    bool navigateMode = true;
    decimal latitude = 0m;
    decimal longitude = 0m;

    private void Awake()
    {
        Instance = this;
    }

    public decimal getLatitude()
    {
        return latitude;
    }

    public decimal getLongitude()
    {
        return longitude;
    }

    void Start()
    {        
        NavigateMode();
        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    private bool ClickOnUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
        {            
            m_PointerEventData.position = Input.mousePosition;
            m_Raycaster.Raycast(m_PointerEventData, UIResults);
            return UIResults.Count > 0;
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonUp(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touches[0].phase == TouchPhase.Ended)
#endif
        {
            UIResults.Clear();
            m_PointerEventData.position = Input.mousePosition;
            m_Raycaster.Raycast(m_PointerEventData, UIResults);
            return UIResults.Count > 0;
        }
        return false;
    }

    void LateUpdate()
    {
        FindMouseLocationLatLot();
        ///Stop camera options when the panel is open
        if (SymbolManager.Instance.getAddSymbolPanelOpen())
        {
            ZoomCam();
            return;
        }
        
        if (ClickOnUI())
        {                        
            Debug.Log("UICLICK!!!");
            return;

        }
        
        if (navigateMode)
        {
            NavigateCam();
        }
        else if (addSymbolMode)
        {
            ZoomCam();
            SymbolManager.Instance.AddSymbol();
        }
        
    }

    private void NavigateCam()
    {
        PanCam();
        RotateCam();
        ZoomCam();        
    }

    private void PanCam()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        // For moving on camera with left click
        if (Input.GetMouseButton(0))
        {
            Vector3 dy = transform.up * Input.GetAxis("Mouse Y") * moveSpeedPerPixel * transform.position.y;
            Vector3 dx = transform.right * Input.GetAxis("Mouse X") * moveSpeedPerPixel * transform.position.y;
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            Vector3 dy = transform.forward * Input.touches[0].deltaPosition.y * transform.position.y / moveSpeedPerPixelForAndroid;
            Vector3 dx = transform.right * Input.touches[0].deltaPosition.x  * transform.position.y / moveSpeedPerPixelForAndroid;
#endif
            Vector3 dz = Vector3.zero;
            dz = new Vector3(0.0f, dy.y, 0.0f);
            //Debug.Log("dy:" + dy);            
            //Debug.Log("dx:" + dx);
            transform.position -= dx + dy - dz;
        }
    }

    private void RotateCam()
    {
        // For rotating the camera with right click
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(1))

        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        //Rotate For Android/IOS
#endif
    }

    private void ZoomCam()
    {
        // For zooming in and out
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        transform.position = new Vector3(transform.position.x, transform.position.y * (1 - Input.GetAxis("Mouse ScrollWheel") * zoomSpeedPerWheel), transform.position.z);
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if(Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float difference = currentMagnitude - prevMagnitude;
            transform.position = new Vector3(transform.position.x, transform.position.y * (1 - difference / zoomSpeedPerWheelForAndroid), transform.position.z);
        }        
#endif
        var pos = transform.position;
        pos.y = Mathf.Clamp(transform.position.y, zoomMinDistance, zoomMaxDistance);
        transform.position = pos;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    
    private void FindMouseLocationLatLot()
    {
        /*
        Vector3 mousePos = Input.mousePosition;
        Debug.Log("Mousepos:" + mousePos);
        mousePos.z = mousePos.y;
        mousePos.y = 0;
        //mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.Log("Mousepos2:" + mousePos);
        */
        var mousePosScreen = Input.mousePosition;

        mousePosScreen.z = Camera.main.transform.localPosition.y;
        var pos = Camera.main.ScreenToWorldPoint(mousePosScreen);


        Vector2d LatLot = mapManager.WorldToGeoPosition(pos);
        latitude = (decimal)LatLot.x;
        latitude = Math.Truncate(latitude * 100000000000m) / 100000000000m;
        longitude = (decimal)LatLot.y;
        longitude = Math.Truncate(longitude * 100000000000m) / 100000000000m;
        LatLotMouse.text = latitude + ", " + longitude;
    }



    public void AddSymbolMode()
    {
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        GameObject gObj = GameObject.Find("/Canvas/AddOrNavigate/Add");
        ColorUtilityManager.Instance.SetColorofCamMobilityButtons(gObj);
        SymbolManager.Instance.ActivateMarker();
        addSymbolMode = true;
        navigateMode = false;

        userInfoLockPanel.SetActive(true);
        onlineUsersLockPanel.SetActive(true);
        searchAreaLockPanel.SetActive(true);
    }

    public void NavigateMode()
    {
        if (SymbolManager.Instance.getAddSymbolPanelOpen())
        {
            SymbolManager.Instance.CloseAddSymbolAction();
        }
        transform.rotation = Quaternion.Euler(new Vector3(60f, 0f, 0f));
        GameObject gObj = GameObject.Find("/Canvas/AddOrNavigate/Navigate");
        ColorUtilityManager.Instance.SetColorofCamMobilityButtons(gObj);
        addSymbolMode = false;
        navigateMode = true;

        userInfoLockPanel.SetActive(false);
        onlineUsersLockPanel.SetActive(false);
        searchAreaLockPanel.SetActive(false);
    }
}
