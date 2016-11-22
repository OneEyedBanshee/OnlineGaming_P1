using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class Net
{
    //private string[] ips = { "149.153.106.174", "149.153.106.153" }; //my own and Stephen's IP addresses, mine is index 0, his is index 1
    private string[] ips = { "127.0.0.1", "127.0.0.1" }; //my own and Stephen's IP addresses, mine is index 0, his is index 1
    private string ip;
    private string ip_remote;
    private int port;
    private int port_remote;
    private ConnectionConfig config;
    private byte channelId;
    private int socketId;
    private int clientConnectionId;
    private bool connected = false;

    // Use this for initialization
    public Net ()
    {
        NetworkTransport.Init();
        config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Reliable);
	}

    public void SetupA()
    {        
        //local connection
        ip = ips[0];
        ip_remote = ips[1];
        port = 8001;
        port_remote = 8000;

        GameObject.Find("SetupAButt").GetComponent<Button>().interactable = false;
        GameObject.Find("SetupBButt").GetComponent<Button>().interactable = false;

        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, port);
        Debug.Log("Me: " + ip + ":" + port);
    }

    public void SetupB()
    {        
        //other pc connection
        ip = ips[1];
        ip_remote = ips[0];
        port = 8000;
        port_remote = 8001;

        GameObject.Find("SetupAButt").GetComponent<Button>().interactable = false;
        GameObject.Find("SetupBButt").GetComponent<Button>().interactable = false;

        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, port);
        Debug.Log("Me: " + ip + ":" + port);
    }

    public void SetupIPs()
    {
        if (Network.player.ipAddress == ips[0])
        {
            //local connection
            ip = ips[0];
            ip_remote = ips[1];
            port = 8001;
            port_remote = 8000;
        }
        else
        {
            //other pc connection
            ip = ips[1];
            ip_remote = ips[0];
            port = 8000;
            port_remote = 8001;
        }

        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, port);
        Debug.Log("Me: " + ip + ":" + port);
    }

    public void Connect()
    {      
        byte error = 0;
        clientConnectionId = NetworkTransport.Connect(socketId, ip_remote, port_remote, 0, out error);
        connected = true;

        GameObject.Find("SetupAButt").SetActive(false);
        GameObject.Find("SetupBButt").SetActive(false);
        GameObject.Find("ConnectButt").SetActive(false);
    }

    public bool GetConnected()
    {
        return connected;
    }

    public void Send(string message)
    {
        //send the message
        byte error = 0;
        byte[] buffer = new byte[500];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        channelId = 0;
        NetworkTransport.Send(socketId, clientConnectionId, channelId, buffer, (int)stream.Position, out error);
    }

    public string Receive()
    {
        string data = "";

        int socketId_remote;
        int clientConnectionId_remote;
        int channelId_remote;
        int bufferSize = 500;
        byte[] recBuffer = new byte[500];

        int dataSize;
        byte error;
        NetworkEventType receivedData = NetworkTransport.Receive(out socketId_remote,
                                                                 out clientConnectionId_remote,
                                                                 out channelId_remote, recBuffer,
                                                                 bufferSize,
                                                                 out dataSize,
                                                                 out error);
        switch (receivedData)
        {
            case NetworkEventType.Nothing:         //1
                break;
            case NetworkEventType.ConnectEvent:    //2              
                Debug.Log("Connect event");
                break;
            case NetworkEventType.DataEvent:       //3
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                data = formatter.Deserialize(stream) as string;           
                break;
            case NetworkEventType.DisconnectEvent: //4
                break;
        }

        return data;
    }
}