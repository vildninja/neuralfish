using UnityEngine;
using System.Collections;
using System.Linq;

public class Fish : MonoBehaviour
{

    private Rigidbody2D body;
    private Transform surface;

    private Transform target = null;

	// Use this for initialization
	IEnumerator Start ()
	{
	    body = GetComponent<Rigidbody2D>();
	    surface = GameObject.Find("Surface").transform;

	    while (true)
	    {
	        yield return new WaitForSeconds(Random.Range(1f, 2f));
            body.AddForce(new Vector2(Random.value > 0.5f ? 2 : -2, 2), ForceMode2D.Impulse);
	    }
	}

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, 0.3f);
            transform.rotation = target.rotation;
        }
        else if (body.isKinematic)
        {
            body.isKinematic = false;
            GetComponent<Collider2D>().enabled = true;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    body.gravityScale = transform.position.y > surface.position.y ? 1 : 0.1f;
	}

    public void Grab(Transform target)
    {
        body.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        this.target = target;
    }
}
