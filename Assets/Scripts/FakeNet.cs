using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FakeNet {

    struct Message
    {
        public string data;
        public float sendTimestamp;
    }
    private float m_latency;
    private float m_jitter;
    private float m_packetLoss;
    private List <Message> messages;
    Net net;

    public FakeNet(float l, float j, float pL)
    {
        messages = new List<Message>(); 
        net = new Net();
        m_latency = l / 1000f; // convert to milliseconds
        m_jitter = j;
        m_packetLoss = pL;
    }
    public void Send (string data)
    {
        Message m;
        //m_jitter = Random.Range(-50 / 1000, 50 / 1000);
        m.sendTimestamp = Time.time + m_latency + m_jitter;
        m.data = data;
        messages.Add(m);
    }

    public void ProcessMessages()
    {
        float currentTime = Time.time;
        foreach (Message m in messages)
        {
            if (m.sendTimestamp < currentTime)
            {
                if (Random.Range(0, 101) >= m_packetLoss)
                {
                    net.Send(m.data);
                }

                messages.Remove(m);
                break; //break out of the loop, list modified
            }
        }
    }

    public void SetupA()
    {
        net.SetupA();
    }

    public void SetupB()
    {
        net.SetupB();
    }

    public void SetupIPs()
    {
        net.SetupIPs();
    }

    public void Connect()
    {
        net.Connect();
    }

    public bool GetConnected()
    {
        return net.GetConnected();
    }

    public string Receive()
    {
        return net.Receive();
    }
}
