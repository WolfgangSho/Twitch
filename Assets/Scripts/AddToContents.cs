using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToContents : MonoBehaviour
{
    public LayerMask mask;

    public GameObject go_GM;

    ShoppingListManager sc_list;

    // Start is called before the first frame update
    void Awake()
    {
        sc_list = go_GM.GetComponent<ShoppingListManager>();
    }

    void OnTriggerEnter(Collider c)
    {
        if( mask == (mask| (1 << c.gameObject.layer)))
        {
            
            c.transform.SetParent(transform);
            c.gameObject.GetComponent<ItemState>().EnterTrolley();

            sc_list.AddToTrolley(c.gameObject.GetComponent<ItemState>().scriptableObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if(c.transform.parent == transform)
        {
            c.transform.SetParent(null);
            c.gameObject.GetComponent<ItemState>().ExitTrolley();

            sc_list.RemoveFromTrolley(c.gameObject.GetComponent<ItemState>().scriptableObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
