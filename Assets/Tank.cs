using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tank : MonoBehaviour
{
    public Transform water;
    public Renderer logo;
    public Renderer gamover;
    [HideInInspector]
    public List<Leak> leaks;

    public float emptY;

	// Use this for initialization
	IEnumerator Start ()
    {
	    leaks = new List<Leak>(FindObjectsOfType<Leak>());

	    while (!Input.GetButtonDown("Fire1"))
	    {
	        yield return null;
	    }

	    logo.enabled = false;
	    gamover.enabled = false;

        while (!gamover.enabled)
	    {
	        yield return new WaitForSeconds(Random.Range(4, 8f));
            leaks.RemoveAll(l => l.AboveSurface);

	        int fishLeft = 6 - leaks.Count(l => l.fish != null);

	        if (fishLeft > 1)
	        {
	            leaks[Random.Range(0, leaks.Count)].Crack();
	        }
	        else
	        {
                // make sure to create a leak with one of the fish
	            Leak leak;
	            do
	            {
	                leak = leaks[Random.Range(0, leaks.Count)];
	            } while (!leak.fish);

                leak.Crack();
	        }
	    }
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (water.position.y < emptY)
	    {
	        Debug.Log("GameOver");
	        gamover.enabled = true;
	        logo.enabled = true;
	        foreach (var leak in leaks)
	        {
	            leak.leaking = false;
	        }
	    }
	    else
	    {
            water.Translate(0, -leaks.Sum(l => l.Flow) * Time.deltaTime, 0);
        }
	}
}
