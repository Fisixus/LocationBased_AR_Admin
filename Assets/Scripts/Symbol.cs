using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public enum Category
{
    Ambulance,
    Barrier,
    Blast,
    Bomb,
    Car,
    Construction,
    Ditch,
    Fire,
    Firstaid,
    Hunting,
    Male,
    Office_building,
    Police,
    Soldier,
    Traffic_light
}


public class Symbol
{
    [JsonProperty]
    private string UUID = " ";

    public string SymbolName = " ";
    public Category Category = Category.Ambulance;
    public decimal Latitude = 0.0m;
    public decimal Longitude = 0.0m;
    public decimal Altitude = 0.0m;
    public string Message = " ";
    public string UserUUID = " ";

    [JsonIgnore]
    public string getUUID { get => UUID;}

    public Symbol()
    {
        UUID = generateUUID();
    }

    //Deep Copy
    public Symbol(Symbol symbol)
    {
        UUID = symbol.getUUID;
        SymbolName = symbol.SymbolName;
        Category = symbol.Category;
        Latitude = symbol.Latitude;
        Longitude = symbol.Longitude;
        Altitude = symbol.Altitude;
        Message = symbol.Message;
        UserUUID = symbol.UserUUID;
    }

    public string generateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public override string ToString()
    {
        return UUID + " " + SymbolName + " " + (int)Category + " " + Latitude + " " + Longitude + " " + Altitude + " " + Message + " " + UserUUID ;
    }
}
