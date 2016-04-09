using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tank : MonoBehaviour
{
    private Vector3 waterStart;
    public Transform water;
    public Renderer logo;
    public Renderer gamover;
    [HideInInspector]
    public List<Leak> leaks;

    public float emptY;

    void Start()
    {
        waterStart = water.position;
        StartCoroutine(GameLoop());
    }
    
	IEnumerator GameLoop ()
    {
	    leaks = new List<Leak>(FindObjectsOfType<Leak>());
	    foreach (var leak in leaks)
	    {
	        leak.leaking = false;
	    }

	    water.position = waterStart;

        gamover.enabled = false;

        while (!Input.GetButtonDown("Grab"))
	    {
	        yield return null;
	    }

	    logo.enabled = false;

	    float dificulity = 0;

        yield return new WaitForSeconds(Random.Range(4, 8f));
        while (!gamover.enabled)
	    {
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

            yield return new WaitForSeconds(Random.Range(4, 8f) - Mathf.Clamp(dificulity, 0, 3));
            dificulity += 0.2f;
        }

        yield return new WaitForSeconds(5);
	    StartCoroutine(GameLoop());
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (water.position.y < emptY && !gamover.enabled)
	    {
	        Debug.Log("GameOver");
	        gamover.enabled = true;
	        logo.enabled = true;
	        foreach (var leak in leaks)
	        {
                leak.Crack();
	            leak.leaking = false;
	        }
	    }
	    else
	    {
            water.Translate(0, -leaks.Sum(l => l.Flow) * Time.deltaTime, 0);
        }
	}
}
