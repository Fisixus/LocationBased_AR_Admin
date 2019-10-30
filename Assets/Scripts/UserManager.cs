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
}
