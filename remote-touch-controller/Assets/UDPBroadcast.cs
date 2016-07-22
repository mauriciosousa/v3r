using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class UdpBroadcast
{
    private string _address;
    private int _port;

    private IPEndPoint _remoteEndPoint;
    private UdpClient _udp;

    private DateTime _lastSent;

    private bool _streaming = false;

    public UdpBroadcast(int port, int sendRate = 100)
    {
        _lastSent = DateTime.Now;
        reset(port, sendRate);
    }

    public void reset(int port, int sendRate = 100)
    {
        try
        {
            _port = port;

            _remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, _port);
            _udp = new UdpClient();
            _streaming = true;

            Debug.Log("[UDP Broadcast] Sending at port: " + _port);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
    }

    public void send(string line)
    {
        if (_streaming)
        {
            try
            {
                //if (DateTime.Now > _lastSent.AddMilliseconds(TrackerProperties.Instance.sendInterval))
                {
                    byte[] data = Encoding.UTF8.GetBytes(line);

                    _udp.Send(data, data.Length, _remoteEndPoint);

                    _lastSent = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[UDP Send] " + e.Message);
                Debug.LogError(e.StackTrace);
            }
        }
    }
}