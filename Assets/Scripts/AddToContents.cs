using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToContents : MonoBehaviour
{
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider c)
    {
        if( mask == (mask| (1 << c.gameObject.layer)))
        {
            c.transform.SetParent(transform);
            c.gameObject.GetComponent<ItemState>().EnterTrolley();
        }
    }

    void OnTriggerExit(Collider c)
    {
        if(c.transform.parent == transform)
        {
            c.transform.SetParent(null);
            c.gameObject.GetComponent<ItemState>().ExitTrolley();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
