using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tank : MonoBehaviour
{
    public Transform water;
    public List<Leak> leaks;

    public float emptY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    water.Translate(0, - leaks.Sum(l => l.Flow) * Time.deltaTime, 0);
	    if (water.position.y < emptY)
	    {
	        Debug.Log("GameOver");
	    }
	}
}
