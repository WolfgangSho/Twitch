using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    bool inTrolley;
    public bool grabbed;

    float maxVel;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        maxVel = Mathf.Infinity;

        inTrolley = false;
        grabbed = false;

    }

    public void EnterTrolley()
    {
        inTrolley = true;
    }

    public void ExitTrolley()
    {
        Debug.Log("hello");
        inTrolley = false;
        rb.isKinematic = false;
        rb.mass = 100;
     //   rb.detectCollisions = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(inTrolley && !grabbed)
        {
            if(rb.velocity.y > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x,0,rb.velocity.z);
            }

            if(Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                rb.isKinematic = true;
                rb.mass = 0;
          //      rb.detectCollisions = false;
            }
        }
    }
}
