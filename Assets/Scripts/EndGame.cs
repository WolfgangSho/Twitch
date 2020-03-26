using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameObject go_gm;
    GM sc_gm;

    // Start is called before the first frame update
    void Start()
    {
        sc_gm = go_gm.GetComponent<GM>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider c)
    {
        sc_gm.Checkout();
    }

}
