using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState : MonoBehaviour
{

    public Item scriptableObject;
    bool inTrolley;
    public bool grabbed;

    // Start is called before the first frame update
    void Awake()
    {
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
        inTrolley = false;

        transform.localScale = Vector3.one;
    }
}
