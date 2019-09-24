using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
public class MessageController : MonoBehaviour
{
    public static MessageController inst;
    public TextMeshProUGUI textmesh;
    string recMessage;
    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        
       


    }

    // Update is called once per frame
    void Update()
    {
        recMessage = NetworkListener.instance.receivedString;
        Debug.Log("message controller received string "+recMessage);
        MessageInfo mes = JsonConvert.DeserializeObject<MessageInfo>(recMessage);
        Debug.Log("***********************"+mes.getAlt().ToString());
        textmesh.text = mes.getAlt().ToString();
        // MessageController messagecontroller = JsonUtility.FromJson<MessageController>(recMessage);
        //string a = JsonConvert.SerializeObject();



        //  textmesh.transform.GetComponent<TextMeshProUGUI>().text = ""+mes.getLat().ToString() + mes.getLot() + mes.getSymbolIndex() ;


    }

    public class MessageInfo
    {
        public float lat;
        public float lot;
        public float alt;
        public string name;
        public int symbolIndex;

        public MessageInfo(float lat, float lot, float alt, string name, int symbolIndex)
        {
            this.lat = lat;
            this.lot = lot;
            this.alt = alt;
            this.name = name;
            this.symbolIndex = symbolIndex;
            ;
        }
        public MessageInfo() { }

        public float getLat()
        {
            return lat;
        }

        public float getLot()
        {
            return lot;
        }
        public float getAlt()
        {
            return alt;
        }
        public string getName()
        {
            return name;
        }
        public int getSymbolIndex()
        {
            return symbolIndex;
        }
    }
}
