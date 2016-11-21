using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FakeNet {

    private float m_latency;
    private float m_jitter;
    private float m_packetLoss;
    private int packetCounter = 0;
    private float currentTime = 0.0f;
    private List <Message> messages;

    public FakeNet(float l, float j, float pL)
    {
        m_latency = l / 1000; // convert to milliseconds
        m_jitter = j;
        m_packetLoss = pL;
        messages = new List<Message>(); 
    }
	
	// Update is called once per frame
	public void Update ()
    {
        currentTime = Time.time;
        //only process if there's a backlog of messages
        if (messages.Count > 0)
        {
            ProcessMessages();
        }             
    }

    public void Send (Message message)
    {
        m_jitter = Random.Range(-50 / 1000, 50 / 1000);
        message.setTimeStamp(currentTime + m_latency + m_jitter);
        messages.Add(message);
    }

    void ProcessMessages ()
    {
        foreach (Message m in messages)
        {
            if (m.getTimeStamp() < currentTime)
            {
                if (packetCounter == m_packetLoss)
                {
                    GameObject.Find("GameController").GetComponent<Net>().Send(m.getData());
                    packetCounter = 0;
                }
                else
                {
                    packetCounter++;
                }

                messages.Remove(m);
                break; //break out of the loop, list modified
            }
        }
    }
}
