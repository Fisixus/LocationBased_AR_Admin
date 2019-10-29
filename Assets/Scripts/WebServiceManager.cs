using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class WebServiceManager : MonoBehaviour
{
    public static WebServiceManager Instance;
    //USER SERVICES
    #region 

    public const string GetAllUsersURI = "https://kctxqs0ws8.execute-api.eu-central-1.amazonaws.com/locationbasedAR/allusers";

    public const string GetUserURI = "https://kctxqs0ws8.execute-api.eu-central-1.amazonaws.com/locationbasedAR/user";

    public const string AddUserURI = "https://kctxqs0ws8.execute-api.eu-central-1.amazonaws.com/locationbasedAR/adduser";

    public const string UpdateUserURI = "https://kctxqs0ws8.execute-api.eu-central-1.amazonaws.com/locationbasedAR/updateuser";

    #endregion
    //SYMBOL SERVICES 
    #region 
    public const string GetSymbolURI = "https://c9bl26g5ji.execute-api.eu-central-1.amazonaws.com/locationbasedAR/symbol";

    public const string GetSymbolsURI = "https://c9bl26g5ji.execute-api.eu-central-1.amazonaws.com/locationbasedAR/symbols";

    public const string GetAllSymbolsURI = "https://c9bl26g5ji.execute-api.eu-central-1.amazonaws.com/locationbasedAR/allsymbols";

    public const string AddSymbolURI = "https://c9bl26g5ji.execute-api.eu-central-1.amazonaws.com/locationbasedAR/addsymbol";

    public const string UpdateSymbolURI = "https://c9bl26g5ji.execute-api.eu-central-1.amazonaws.com/locationbasedAR/updatesymbol";

    public const string DeleteSymbolURI = "https://c9bl26g5ji.execute-api.eu-central-1.amazonaws.com/locationbasedAR/deletesymbol";

    #endregion
    string allUserData = "";
    string userData = "";
    string symbolData = "";
    string symbolsData = "";
    string allSymbolsData = "";

    public string getAllSymbolsData()
    {
        return allSymbolsData;
    }

    public string getAllUserData()
    {
        return allUserData;
    }

    public string getUserData()
    {
        return userData;
    }

    public string getSymbolData()
    {
        return symbolData;
    }

    public string getSymbolsData()
    {
        return symbolsData;
    }

    private void Awake()
    {
        Instance = this;        
    }
    /*
    private void Update()
    {

        StartCoroutine(X());
 
    }

    IEnumerator X()
    {
        using (UnityWebRequest request = UnityWebRequest.Put("http://roboturka.com/", " "))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
    */
    void Start()
    {
        ///JsonFrom Control
        /*
        string jsonData = "{\"UUID\": \"das\", \"SymbolName\":\"Kelebek\", \"Category\":0, \"Latitude\":1434.34, \"Longitude\":34.32, \"Altitude\":4342.12, \"Message\":\"Nays\", \"UserUUID\":\"fsdf\"}";

        Symbol symbol = JsonConvert.DeserializeObject<Symbol>(jsonData);
        Debug.Log(symbol.ToString());
        */

        ///ToJson Control
        /*
        User user = new User();
        string jsonData = JsonConvert.SerializeObject(user);
        Debug.Log(jsonData);
        */
        InvokeRepeating("GetAllUsers", .1f, 5f);
        InvokeRepeating("GetAllSymbols", .1f, 5f);
    }

    public void GetAllUsers()
    { 
        StartCoroutine(GetAllUsersRequest());
    }

    public void GetUser(string UUID)
    {
        StartCoroutine(GetUserRequest(UUID));
    }

    public void AddUser(User User)
    {
        StartCoroutine(AddUserRequest(User));        
    }

    public void UpdateUser(User User)
    {
        StartCoroutine(UpdateUserRequest(User));
    }
    //Give Symbol UUID
    public void GetSymbol(string UUID)
    {
        StartCoroutine(GetSymbolRequest(UUID));
    }
    //Give User UUID
    public void GetSymbols(string UUID)
    {
        StartCoroutine(GetSymbolsRequest(UUID));
    }

    public void GetAllSymbols()
    {
        StartCoroutine(GetAllSymbolsRequest());
    }

    public void AddSymbol(Symbol Symbol)
    {
        StartCoroutine(AddSymbolRequest(Symbol));
    }

    public void UpdateSymbol(Symbol Symbol)
    {
        StartCoroutine(UpdateSymbolRequest(Symbol));
    }

    public void DeleteSymbol(string UUID, string UserUUID)
    {
        StartCoroutine(DeleteSymbolRequest(UUID, UserUUID));
    }



    private IEnumerator GetAllUsersRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetAllUsersURI))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = GetAllUsersURI.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                allUserData = webRequest.downloadHandler.text;
                allUserData = allUserData.Replace("\\", string.Empty);
                allUserData = allUserData.Trim('"');
                /*
                StartCoroutine(GetData());
                recentData = jsonData;
                dataControl = true;
                */
                /*
                var users = JsonConvert.DeserializeObject<List<User>>(jsonData);
                foreach(User user in users)
                {
                    Debug.Log(user + "\n");
                }
                */
                
                //DELEGATION & ACTIONS 

                //OnAllOnlineUserGot(webRequest.downloadHandler.text);

            }
        }
    }



    private IEnumerator GetUserRequest(string UUID)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetUserURI + "?UUID=" + UUID))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = GetUserURI.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                userData = webRequest.downloadHandler.text;
                userData = userData.Replace("\\", string.Empty);
                userData = userData.Trim('"');
            }
        }
    }

    private IEnumerator AddUserRequest(User user)
    {
        string data = JsonConvert.SerializeObject(user);
        /*
        using (UnityWebRequest request = UnityWebRequest.Put(AddUserURI, "{\"UUID\": \"UTKUID\"}"))
        */
        using (UnityWebRequest request = UnityWebRequest.Put(AddUserURI, data))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    private IEnumerator UpdateUserRequest(User user)
    {
        string data = JsonConvert.SerializeObject(user);
        /*using (UnityWebRequest request = UnityWebRequest.Put(UpdateUserURI, "{\"UUID\": \"UTKUID\", \"Username\":\"Utku\", \"Firstname\":\"Erden\", \"Surname\":\"Utku\", \"Password\":\"a\", \"Latitude\":123.123, \"Longitude\":4323.123, \"Altitude\":122.13, \"Online\":true, \"Role\":\"Standart\"}"))
         */
        using (UnityWebRequest request = UnityWebRequest.Put(UpdateUserURI,data))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form update complete!");
            }
        }
    }

    private IEnumerator GetSymbolRequest(string UUID)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetSymbolURI + "?UUID=" + UUID))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = GetSymbolURI.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                symbolData = webRequest.downloadHandler.text;
                symbolData = symbolData.Replace("\\", string.Empty);
                symbolData = symbolData.Trim('"');
            }
        }
    }

    private IEnumerator GetSymbolsRequest(string UUID)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetSymbolsURI + "?UUID=" + UUID))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = GetSymbolsURI.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                symbolsData = webRequest.downloadHandler.text;
                symbolsData = symbolsData.Replace("\\", string.Empty);
                symbolsData = symbolsData.Trim('"');
            }
        }
    }

    private IEnumerator GetAllSymbolsRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetAllSymbolsURI))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = GetAllSymbolsURI.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                allSymbolsData = webRequest.downloadHandler.text;
                allSymbolsData = allSymbolsData.Replace("\\", string.Empty);
                allSymbolsData = allSymbolsData.Trim('"');
            }
        }
    }

    private IEnumerator AddSymbolRequest(Symbol symbol)
    {
        string data = JsonConvert.SerializeObject(symbol);
        /*
        using (UnityWebRequest request = UnityWebRequest.Put(AddSymbolURI, "{\"UUID\": \"AZAZ\", \"UserUUID\":\"UTKUID\"}"))
        */
        using (UnityWebRequest request = UnityWebRequest.Put(AddSymbolURI, data))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    private IEnumerator UpdateSymbolRequest(Symbol symbol)
    {
        string data = JsonConvert.SerializeObject(symbol);
        /*
        using (UnityWebRequest request = UnityWebRequest.Put(UpdateSymbolURI, "{\"UUID\": \"das\", \"SymbolName\":\"Kelebek\", \"Category\":\"Category\", \"Latitude\":1434.34, \"Longitude\":34.32, \"Altitude\":4342.12, \"Message\":\"Nays\"}"))
        */
        using (UnityWebRequest request = UnityWebRequest.Put(UpdateSymbolURI, data))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form update complete!");
            }
        }
    }

    //Get is used instead of Delete because of the authentication problems
    private IEnumerator DeleteSymbolRequest(string UUID, string UserUUID)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DeleteSymbolURI + "?UUID=" + UUID + "&" + "UserUUID=" + UserUUID))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (!webRequest.isNetworkError)
            {
                Debug.Log("Success!!");
            }
            else
            {
                Debug.Log("Fail!!");
            }
        }
    }
}