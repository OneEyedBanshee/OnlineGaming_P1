using UnityEngine;

public class Game : MonoBehaviour
{        
    private Player player1;    
    private Player player2;
    private Player myPlayer, remotePlayer; //the game controls one player, it's player
    private FakeNet net;
    private float lastSendTime;
    const float PacketSendInterval = (1000f / 10f) / 1000f;

    // Use this for initialization
    void Start ()
    {
        Screen.SetResolution(1980, 1080, false);
        //initialise the net object so we can use Net functions        
        player1 = GameObject.Find("Player1").GetComponent<Player>();
        player2 = GameObject.Find("Player2").GetComponent<Player>();

        net = new FakeNet(0, 0, 0);
        lastSendTime = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        net.ProcessMessages();
        string message = net.Receive();
        HandleInput();        

        if (net.GetConnected())
        {
            myPlayer.SelfUpdate(remotePlayer.GetPosition());
            remotePlayer.SelfUpdate(myPlayer.GetPosition());

            if (message != "")
            {
                remotePlayer.DeserializeData(message);
            }

            if (Time.time - lastSendTime > PacketSendInterval)
            {
                net.Send(myPlayer.SerializeData());
                lastSendTime = Time.time;
            }
        }
    }

    private void HandleInput()
    {
        //check if game is connected and a key is pressed, no point sending input based data continually
        if (net.GetConnected())
        {
            if (Input.anyKey)
            {
                //store each of the possible directions in an array
                string[] keys = new string[4];

                if (Input.GetKey(KeyCode.D))
                {
                    keys[0] = "Right";
                }

                if (Input.GetKey(KeyCode.S))
                {
                    keys[1] = "Down";
                }

                if (Input.GetKey(KeyCode.A))
                {
                    keys[2] = "Left";
                }

                if (Input.GetKey(KeyCode.W))
                {
                    keys[3] = "Up";
                }
                myPlayer.HandleInput(keys);
            }            
        }               
    }

    public void SetupA()
    {
        myPlayer = player1;
        remotePlayer = player2;
        net.SetupA();
    }

    public void SetupB()
    {
        myPlayer = player2;
        remotePlayer = player1;
        net.SetupB();
    }

    public void Connect()
    {
        net.Connect();
    }

}
