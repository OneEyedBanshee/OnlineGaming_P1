using UnityEngine;
using System.Collections;

public class boost : MonoBehaviour
{
    public byte direction;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	public void SelfUpdate ()
    {

        pulsateLight();
    }

    void pulsateLight()
    {
        //get the renderer so the material of the object can be accessed
        Renderer renderer = GetComponent<Renderer>();
        Material mat = renderer.material;
        //Make the emission property of the materal pulsate
        float emission = Mathf.PingPong(Time.time, 0.75f);
        Color baseColor = Color.green * 0.75f;
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor);
    }

    void OnTriggerEnter2D(Collider2D player)//(Collision2D Player)
    { 
        //depending on the boostpad direction add force, reduce motion then force if moving in the opposing direction to pad flow
        if (direction == 0)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.x < 0)
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);            

            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(1000, 0));           
        }
        else
        {
            if (player.GetComponent<Rigidbody2D>().velocity.x > 0)            
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);            
            
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1000, 0));           
        }
    }
}
