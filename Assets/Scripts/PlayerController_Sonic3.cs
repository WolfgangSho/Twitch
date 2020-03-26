using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Sonic3 : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

    public bool playable;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playable)
        {
            transform.Translate(0, 0, Input.GetAxis("Vertical") * -moveSpeed*0.1f);
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed, 0,Space.World);
        }
    }
}
