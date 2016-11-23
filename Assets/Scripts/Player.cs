using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Player : MonoBehaviour {

    struct PositionalData
    {
        public float x;
        public float y;
        public float r;
    }
    PositionalData startPosData;
    PositionalData endPosData;
    float elapsedPacketTime;
    const float PacketSendInterval = (1000f / 10f) / 1000f;

    //public string charName;
    public Collider2D otherPlayerCollider;
    private string charName = "player";
    Rigidbody2D rb;
    public bool chasing;
    private bool showText = false;
    private float currentTime = 0.0f, elapsedTime = 0.0f, showTextDuration = 1.5f, chaseTime = 0.0f;
    private string text = "", textShadow = "", textColor = "";
    private GUIStyle guiStyle = new GUIStyle();

    private float radius;

    private bool collided = false;

    const float DefaultPosData = -1000f;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        guiStyle.richText = true;   
        guiStyle.alignment = TextAnchor.MiddleCenter;

        radius = this.GetComponent<CircleCollider2D>().bounds.size.x / 2;

        Debug.Log(radius);

        textColor = "<color=#668cff>";

        startPosData.x = DefaultPosData;


        Physics2D.IgnoreCollision(otherPlayerCollider, GetComponent<Collider2D>());
    }

    // Update is called once per frame
    public void SelfUpdate(Vector2 player2Pos)
    {
        currentTime = Time.time; //update time 
        //CheckCollision(player2Pos);

        //teleport the player to the other side of the screen if they go through the edge holes
        if (rb.transform.position.x < -8.6f)
        {
            rb.transform.position = new Vector2(8.6f, rb.position.y);
        }
        else if (rb.transform.position.x > 8.6f)
        {
            rb.transform.position = new Vector2(-8.6f, rb.position.y);
        }
        else if (rb.transform.position.y < -4.85f)
        {
            rb.transform.position = new Vector2(rb.position.x, 4.85f);
        }
        else if (rb.transform.position.y > 4.85f)
        {
            rb.transform.position = new Vector2(rb.position.x, -4.85f);
        }

        if (startPosData.x != DefaultPosData)
        {
            //float percent = (Time.time - endPosData.t) / (endPosData.t - startPosData.t);
            elapsedPacketTime += Time.deltaTime;
            float percent = elapsedPacketTime / PacketSendInterval;
            rb.transform.position = new Vector2(Mathf.Lerp(startPosData.x, endPosData.x, percent), Mathf.Lerp(startPosData.y, endPosData.y, percent));
            rb.rotation = Mathf.Lerp(startPosData.r, endPosData.r, percent);
        }
    }

    private void CheckCollision(Vector2 player2Pos)
    {
        if (rb.position.x + radius + radius > player2Pos.x
        && rb.position.x < player2Pos.x + radius + radius
        && rb.position.y + radius + radius > player2Pos.y
        && rb.position.y < player2Pos.y + radius + radius 
        && collided == false)
        {
            collided = true;
        }
    }

    public void HandleInput(string[] directions)
    {
        //loop through the array for each direction in directions
        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] != "")
            {
                rb.drag = 0;
                //add forces depending on the current direction in directionsSplit[i]
                if (directions[i] == "Right")
                {
                    if (rb.velocity.x < 20.0f)
                    {
                        rb.AddForce(new Vector2(20, 0));
                    }
                }
                else if (directions[i] == "Down")
                {
                    if (rb.velocity.y > -20.0f)
                    {
                        rb.AddForce(new Vector2(0, -20));
                    }
                }
                else if (directions[i] == "Left")
                {
                    if (rb.velocity.x > -20.0f)
                    {
                        rb.AddForce(new Vector2(-20, 0));
                    }
                }
                else if (directions[i] == "Up")
                {
                    if (rb.velocity.y < 20.0f)
                    {
                        rb.AddForce(new Vector2(0, 20));
                    }
                }
            }
        }

        rb.drag = 0.75f;
    }

    public void DeserializeData(string data)
    {
        string[] splitData;
        splitData = data.Split('|');

        if (elapsedPacketTime > PacketSendInterval * 1.5)
        {
            rb.position = new Vector2(float.Parse(splitData[0]), float.Parse(splitData[1]));
            rb.rotation = float.Parse(splitData[2]);
            endPosData.x = float.Parse(splitData[0]);
            endPosData.y = float.Parse(splitData[1]);
            endPosData.r = float.Parse(splitData[2]);
            Debug.Log("ye");
        }

        elapsedPacketTime = 0f;
        if (startPosData.x == DefaultPosData)
        {
            startPosData.x = float.Parse(splitData[0]);
            startPosData.y = float.Parse(splitData[1]);
            startPosData.r = float.Parse(splitData[2]);
            endPosData = startPosData;
        }
        else
        {
            startPosData = endPosData;
            endPosData.x = float.Parse(splitData[0]);
            endPosData.y = float.Parse(splitData[1]);
            endPosData.r = float.Parse(splitData[2]);
        }
    }

    public string SerializeData()
    {
        return (rb.position.x + "|" + rb.position.y + "|" + rb.rotation + "|" + rb.velocity.x + "|" + rb.velocity.y);
    }

    //display the GUI, in this case it shows text on screen when conditions are right
    void OnGUI()
    {
        if (showText && currentTime - elapsedTime < showTextDuration)
        {           
            GUI.Label(new Rect(0, Screen.height / 2 + 6, Screen.width, 100), textShadow, guiStyle);
            GUI.Label(new Rect(0, Screen.height / 2, Screen.width, 100), text, guiStyle);           
        }        
    }  

    void OnCollisionEnter2D(Collision2D collisionObject)
    {          
        if (collisionObject.gameObject.tag == "Player")
        {
            //force the 2 players apart slightly on collision         
            //rb.AddForce(collisionObject.contacts[0].normal * 5.0f, ForceMode2D.Impulse);          
            if (chasing == true)
            {
                chaseTime = currentTime - elapsedTime;          
                
                //use rich text to create multi coloured strings and set font size
                text = "<size=52><color=yellow>It took </color>" + textColor + charName + " " + "</color><color=yellow>" + chaseTime.ToString("F2") + " seconds to catch the other player!</color></size>";
                textShadow = "<size=52><color=black>It took " + charName + " " + chaseTime.ToString("F2") + " seconds to catch the other player!</color></size>";

                elapsedTime = currentTime;
                showText = true;
                collided = false;
                chasing = false;
                Component halo = GetComponent("Halo"); halo.GetType().GetProperty("enabled").SetValue(halo, false, null); //disable halo to indicate running
            }
            else
            {
                elapsedTime = currentTime;
                collided = false;
                chasing = true;
                showText = false;
                Component halo = GetComponent("Halo"); halo.GetType().GetProperty("enabled").SetValue(halo, true, null); //enable halo
            }            
        }
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }
}
