using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngineInternal;

public class Player : MonoBehaviour
{

    private Rigidbody2D body;
    private Transform surface;
    private Tank tank;

    public Transform grabber;
    private Transform currentGrab;

	// Use this for initialization
	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        surface = GameObject.Find("Surface").transform;
	    tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.gravityScale = transform.position.y > surface.position.y ? 2 : 0.1f;

        var move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        body.AddForce(move * 5);

        if (body.velocity.magnitude > 0.1f || move.magnitude > 0.1f)
        { 
            float rot = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90;
            if (move.magnitude < 0.1f)
                rot = Mathf.Atan2(body.velocity.y, body.velocity.x)*Mathf.Rad2Deg - 90;
            body.MoveRotation(Mathf.MoveTowardsAngle(transform.eulerAngles.z, rot, 10));
        }
    }

    void Update()
    {
        if (currentGrab == null)
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

        if (transform.position.y < surface.position.y && Input.GetButtonDown("Grab"))
        {
            var move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (move.magnitude > 0.1f)
            {
                body.AddForce(move.normalized * 7, ForceMode2D.Impulse);
            }
        }
        
        if (currentGrab != null)
        {
            currentGrab.position = grabber.position;
            currentGrab.rotation = grabber.rotation;

            foreach (var leak in tank.leaks)
            {
                if (leak != null && leak.Flow > 0 &&
                    Vector2.Distance(currentGrab.position, leak.transform.position) < 1.5f)
                {
                    leak.fish = currentGrab;
                    currentGrab.position = leak.transform.position;
                    currentGrab.rotation = leak.transform.rotation;
                    currentGrab = null;
                    break;
                }
            }
        }
    }
}
