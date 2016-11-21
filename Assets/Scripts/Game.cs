using UnityEngine;

public class Game : MonoBehaviour
{        
    private Net netScript;
    private Player player1;    
    private Player player2;
    private Player myPlayer, remotePlayer; //the game controls one player, it's player
    private boost pad1;
    private boost pad2;
    public KeyCode up;
    public KeyCode right;
    public KeyCode down;
    public KeyCode left;
    private bool inputBased = false;
    FakeNet fakeNet = new FakeNet(200, 0, 1);

    // Use this for initialization
    void Start ()
    {
        Screen.SetResolution(1980, 1080, false);
        //initialise the net object so we can use Net functions        
        player1 = GameObject.Find("Player1").GetComponent<Player>();
        player2 = GameObject.Find("Player2").GetComponent<Player>();
        pad1 = GameObject.Find("Pad1").GetComponent<boost>();
        pad2 = GameObject.Find("Pad2").GetComponent<boost>();
        netScript = GetComponent<Net>();
    }

    //tell the game which player it controls
    public void setPlayer(byte id)
    {
        if (id == 0)
        {
            myPlayer = player1;
            remotePlayer = player2;
        }
        else
        {
            myPlayer = player2;
            remotePlayer = player1;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        netScript.SelfUpdate();
        HandleInput();        
        pad1.SelfUpdate();
        pad2.SelfUpdate();
        fakeNet.Update();

        if (netScript.getConnected())
        {
            myPlayer.SelfUpdate(remotePlayer.GetComponent<CircleCollider2D>().bounds.center);
            remotePlayer.SelfUpdate(myPlayer.GetComponent<CircleCollider2D>().bounds.center);

            if (!inputBased)
            {
                string[] data = new string[3];
                data[0] = (myPlayer.GetComponent<Rigidbody2D>().position.x).ToString();
                data[1] = (myPlayer.GetComponent<Rigidbody2D>().position.y).ToString();
                data[2] = (myPlayer.GetComponent<Rigidbody2D>().rotation).ToString();
                string concatenatedInput = string.Join("|", data);
                concatenatedInput += "|false";
                //netScript.Send(concatenatedInput);
                fakeNet.Send(new Message(concatenatedInput));
            }
        }
    }

    public void movePlayer(string message)
    {
        remotePlayer.DeserializeData(message);
    }

    private void HandleInput()
    {
        //check if game is connected and a key is pressed, no point sending input based data continually
        if (netScript.getConnected())
        {
            if (Input.anyKey)
            {
                //switch between state and input based on the press of space
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    inputBased = !inputBased;
                }

                //store each of the possible directions in an array
                string[] keys = new string[4];

                if (Input.GetKey(right))
                {
                    keys[0] = "Right";
                }

                if (Input.GetKey(down))
                {
                    keys[1] = "Down";
                }

                if (Input.GetKey(left))
                {
                    keys[2] = "Left";
                }

                if (Input.GetKey(up))
                {
                    keys[3] = "Up";
                }

                string concatenatedInput = string.Join("|", keys);
                concatenatedInput += "|true";

                if (inputBased)
                {                    
                    //netScript.Send(concatenatedInput);
                    fakeNet.Send(new Message(concatenatedInput));
                }
                                
                myPlayer.DeserializeData(concatenatedInput);
            }            
        }               
    }       
}
