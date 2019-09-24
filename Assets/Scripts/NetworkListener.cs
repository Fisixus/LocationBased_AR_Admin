using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using TMPro;
using System.Threading;
using System;
using Newtonsoft.Json;
public class NetworkListener : MonoBehaviour
{

    public static NetworkListener instance;
    public Thread receiveThread;
    public UdpClient listener;
    string response;
    public string receivedString;
    string senderString = "Hello";
    public IPAddress ip = IPAddress.Parse("192.168.3.200");
    public String host = "192.168.3.200";
    public int port = 80;
    public IPEndPoint REP;

    IAsyncResult ar1;
    private void Awake()
    {
        instance = this;
    }
    public MessageController.MessageInfo a;

    private void Start()
    {

       
        
        try
        {

            
            receiveThread = new Thread(new ThreadStart(receiveCB));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            //        //UdpClient sender = new UdpClient(port);
            //        //IPEndPoint endIP = new IPEndPoint(ip, port);
            //        //byte[] sendData = System.Text.Encoding.UTF8.GetBytes(senderString);
            //        ////sender.Send(sendData, sendData.Length, endIP);
            //        //Debug.Log("gönderilen ip adresi: " + endIP);
        }
        catch (Exception aaa)
        {
            Debug.Log("Gönderim hatası: " + aaa.ToString() + "---" + aaa);
             receiveThread.Abort();
        }

    }
    private void Update()
    {
       

    }
    private void receiveCB()
    {
        Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        listener = new UdpClient(port);
        REP = new IPEndPoint(ip, port);
        soUdp.Bind(REP);
        while (true)
        {
            
            try
            {
                EndPoint eP = (REP);
                byte[] recData = new byte[1024];
                //listener.Connect(REP);
                int recivedBytes =soUdp.ReceiveFrom(recData,ref eP);
                Debug.Log(" dinleme başarılı "+recData);
                receivedString = System.Text.Encoding.UTF8.GetString(recData); 
                a =JsonConvert.DeserializeObject<MessageController.MessageInfo>(receivedString); 
                Debug.Log("Latitude" + a.getLat() + " Longitude" + a.getLot());
                Debug.Log("Gelen veri boyutu: " + recData.Length);
                response = receivedString;
                Debug.Log("Alınan Veri: " + receivedString);
            }
            catch (Exception e)
            {
                receiveThread.Abort();
                Debug.Log("Veri Alınamıyor" + e.ToString());

            }

            //ar1 = listener.BeginReceive(recieveCB, null);

        }
    }
    private void OnApplicationQuit()
    {
        //Thread.CurrentThread.Abort();
        receiveThread.Abort();
    }
    public object GetMessage()
    {
        return a;
    }
}
