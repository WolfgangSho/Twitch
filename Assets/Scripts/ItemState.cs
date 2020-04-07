using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState : MonoBehaviour
{
    
    public Item scriptableObject;
    bool inTrolley;
    public bool grabbed;

 //   Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
      //  rb = GetComponent<Rigidbody>();

        inTrolley = false;
        grabbed = false;


    }

    public Item GetSO()
    {
        return scriptableObject;
    }

    public void EnterTrolley()
    {
        inTrolley = true;

        transform.localScale = Vector3.one;
    }

    public void ExitTrolley()
    {
        Debug.Log("hello");
        inTrolley = false;
     //   rb.isKinematic = false;
     //   rb.mass = 100;
     //   rb.detectCollisions = true;

        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        /*
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
        }*/
    }
}
