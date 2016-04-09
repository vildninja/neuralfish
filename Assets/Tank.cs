using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tank : MonoBehaviour
{
    public Transform water;
    [HideInInspector]
    public List<Leak> leaks;

    public float emptY;

	// Use this for initialization
	IEnumerator Start ()
    {
	    leaks = new List<Leak>(FindObjectsOfType<Leak>());

	    while (true)
	    {
	        yield return new WaitForSeconds(Random.Range(2, 6f));
            leaks.RemoveAll(l => l.AboveSurface);

	        leaks[Random.Range(0, leaks.Count)].Crack();
	    }
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (water.position.y < emptY)
	    {
	        Debug.Log("GameOver");
	    }
	    else
	    {
            water.Translate(0, -leaks.Sum(l => l.Flow) * Time.deltaTime, 0);
        }
	}
}
