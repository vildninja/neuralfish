using UnityEngine;
using System.Collections;

public class Leak : MonoBehaviour
{

    private ParticleSystem particles;
    private Transform surface;

    public bool leaking = false;
    public Transform fish;

    public bool AboveSurface
    {
        get { return transform.position.y > surface.position.y; }
    }

    public float Flow
    {
        get { return 0; }
    }

	// Use this for initialization
	void Awake ()
	{
	    particles = GetComponentInChildren<ParticleSystem>();
	    surface = GameObject.Find("Surface").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void Crack()
    {
        leaking = true;
        if (fish)
        {
            
        }
    }
}
