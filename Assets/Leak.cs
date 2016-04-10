using UnityEngine;
using System.Collections;

public class Leak : MonoBehaviour
{

    private ParticleSystem particles;
    private Transform surface;
    private AudioSource source;

    public bool leaking = false;
    public Transform fish;

    public AnimationCurve emission;

    public bool AboveSurface
    {
        get { return transform.position.y > surface.position.y; }
    }

    private float flow = 0;

    public float Flow
    {
        get
        {
            if (!leaking || fish != null || AboveSurface)
                return 0;
            return flow;
        }
    }

	// Use this for initialization
	void Awake ()
	{
	    particles = GetComponentInChildren<ParticleSystem>();
	    surface = GameObject.Find("Surface").transform;
	    source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    flow += Time.deltaTime/10;
	    particles.emissionRate = emission.Evaluate(Flow);

	    if (fish && AboveSurface)
	    {
	        Crack();
	    }
	}

    public void Crack()
    {
        source.pitch = Random.Range(-0.8f, 1.2f);
        source.Play();
        flow = 0.01f;
        leaking = true;
        if (fish)
        {
            //this is only destroying the hook
            Destroy(fish.gameObject);
            fish = null;
        }
    }
}
