using UnityEngine;
using System.Collections;

public class Message
{
    string data;
    float sendTimestamp = 0;

	// Use this for initialization
	public Message(string d)
    {
        data = d;
    }
	
	// Update is called once per frame
	void SelfUpdate() {
	
	}

    public float getTimeStamp()
    {
        return sendTimestamp;
    }

    public void setTimeStamp(float ts)
    {
        sendTimestamp = ts;
    }

    public string getData()
    {
        return data;
    }
}
