using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class Net : MonoBehaviour
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
    private Game gameScript;

    public Button SetupAButton;
    public Button SetupBButton;
    public Button ConnectButton;

    // Use this for initialization
    void Start ()
    {
        gameScript = GetComponent<Game>();
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
        gameScript.setPlayer(0);

        SetupAButton.interactable = false;
        SetupBButton.interactable = false;

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
        gameScript.setPlayer(1);

        SetupAButton.interactable = false;
        SetupBButton.interactable = false;

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

    // Update is called once per frame
    public void SelfUpdate()
    {
        Receive();        
    }

    public void Connect()
    {      
        byte error = 0;
        clientConnectionId = NetworkTransport.Connect(socketId, ip_remote, port_remote, 0, out error);
        connected = true;
        ConnectButton.interactable = false;

        Vector3 pos = SetupAButton.transform.position;
        pos.x += 1000;

        SetupAButton.transform.position = pos;
        SetupBButton.transform.position = pos;
        ConnectButton.transform.position = pos;
    }

    public bool getConnected()
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

    void Receive()
    {
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
                //connecting = false;
                break;
            case NetworkEventType.DataEvent:       //3
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                gameScript.movePlayer(message);             
                break;
            case NetworkEventType.DisconnectEvent: //4
                break;
        }
    }
}

//spent 20 minutes
//spent another 15

