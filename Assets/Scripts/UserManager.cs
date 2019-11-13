using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating("CallAllUsers", .1f, 5f);
    }

    public void CallAllUsers()
    {
        WebServiceManager.Instance.GetAllUsers();
    }

    public string FindUserUUIDbyUsername(string symbolOwnerName)
    {        
        string userUUID = string.Empty;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        for(int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].Username.ToLower().Trim().Equals(symbolOwnerName.ToLower().Trim()))
            {
                userUUID = allUsers[i].getUUID;
            }
        }
        return userUUID;
    }

    public string FindUsernamebyUserUUID(string userUUID)
    {
        string username = string.Empty;
        string data = WebServiceManager.Instance.getAllUserData();
        List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(data);
        for (int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].getUUID.Equals(userUUID))
            {
                username = allUsers[i].Username;
            }
        }
        return username;
    }

    public List<string> GetUsersSymbolNames(User dataUser)
    {
        string data = WebServiceManager.Instance.getAllSymbolsData();
        List<Symbol> allSymbols = JsonConvert.DeserializeObject<List<Symbol>>(data);
        List<string> userSymbolNames = new List<string>();
        
        foreach (Symbol s in allSymbols)
        {
            if (s.UserUUID.Equals(dataUser.getUUID))
            {
                userSymbolNames.Add(s.SymbolName);
            }
        }
        return userSymbolNames;
    }
}
