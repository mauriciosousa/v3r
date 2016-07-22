using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;

public class UdpListener : MonoBehaviour
{

    public static string NoneMessage = "0";

    public int Port;

    private UdpClient _udpClient = null;
    private IPEndPoint _anyIP;

    private string _stringToParse = "";

    void Start()
    {
        udpRestart();
    }

    public void udpRestart()
    {
        if (_udpClient != null)
        {
            _udpClient.Close();
        }

        _anyIP = new IPEndPoint(IPAddress.Any, Port);

        _udpClient = new UdpClient(_anyIP);

        _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);

        Debug.Log("[UDPListener] Receiving in port: " + Port);
    }

    public void ReceiveCallback(IAsyncResult ar)
    {
        Byte[] receiveBytes = _udpClient.EndReceive(ar, ref _anyIP);
        _stringToParse = Encoding.ASCII.GetString(receiveBytes);

        _udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
    }

    void Update()
    {
        if (_stringToParse.Length > 0)
        {
            gameObject.GetComponent<SimpleObjController>().setNewTouchMessage(_stringToParse);
        }   
    }

    void OnApplicationQuit()
    {
        if (_udpClient != null) _udpClient.Close();
    }

    void OnQuit()
    {
        OnApplicationQuit();
    }
}