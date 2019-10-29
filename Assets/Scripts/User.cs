using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum Role
{
    Admin,
    Standart,
    Deleted
}


public class User
{
    [JsonProperty]
    private string UUID = " ";

    public string Username = " ";
    public string Firstname = " ";
    public string Surname = " ";
    public string Password = " ";
    public decimal Latitude = 0.0m;
    public decimal Longitude = 0.0m;
    public decimal Altitude = 0.0m;
    public bool Online = false;
    public Role Role = Role.Standart;
    public List<string> SymbolUUIDs = new List<string>();

    [JsonIgnore]
    public List<Symbol> Symbols = new List<Symbol>();

    [JsonIgnore]
    public string getUUID { get => UUID;}

    public User()
    {
        UUID = generateUUID();
    }

    //Deep Copy
    public User(User user)
    {
        UUID = user.getUUID;
        Username = user.Username;
        Firstname = user.Firstname;
        Surname = user.Surname;
        Password = user.Password;
        Latitude = user.Latitude;
        Longitude = user.Longitude;
        Altitude = user.Altitude;
        Online = user.Online;
        Role = user.Role;

        Symbols.Clear();
        for (int i = 0; i < Symbols.Count; i++)
        {
            Symbols.Add(new Symbol(user.Symbols[i]));
        }

        SymbolUUIDs.Clear();
        for(int i = 0; i < SymbolUUIDs.Count; i++)
        {
            SymbolUUIDs.Add(user.SymbolUUIDs[i]);
        }
    }

    public string generateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public override string ToString()
    {
        return UUID + " " + Username + " " + Firstname + " " + Surname + " " + Latitude + " " + Longitude + " " + Altitude + " " + Online + " " + (int)Role + " " + SymbolUUIDs.Count;
    }
}
