using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngineInternal;

public class Player : MonoBehaviour
{

    private Rigidbody2D body;
    private Transform surface;

    public Transform grabber;
    private Transform currentGrab;

	// Use this for initialization
	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        surface = GameObject.Find("Surface").transform;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.gravityScale = transform.position.y > surface.position.y ? 2 : 0.1f;

        var move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        body.AddForce(move * 5);

        if (body.velocity.magnitude > 0.1f)
        {
            var rot = Mathf.Atan2(body.velocity.y, body.velocity.x)*Mathf.Rad2Deg - 90;
            body.MoveRotation(Mathf.MoveTowardsAngle(transform.eulerAngles.z, rot, 10));
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var fishes = FindObjectsOfType<Fish>().Where(f => !f.GetComponent<Rigidbody2D>().isKinematic);
            foreach (var fish in fishes)
            {
                if (fish.GetComponent<Collider2D>().OverlapPoint(grabber.position))
                {
                    currentGrab = new GameObject("hook").transform;
                    fish.Grab(currentGrab);
                }
            }
        }
        
        if (currentGrab != null)
        {
            currentGrab.position = grabber.position;
            currentGrab.rotation = grabber.rotation;
        }
    }
}
