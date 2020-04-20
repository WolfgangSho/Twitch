using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float maxTurnAngle;

    float centre = 180f;

    float minY,maxY;

    // Start is called before the first frame update
    void Awake()
    {
        minY = centre - maxTurnAngle;
        maxY = centre + maxTurnAngle;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,Input.GetAxis("Mouse X")*2,0),Space.World);

        Vector3 rot = transform.localEulerAngles;


        if(rot.y < minY)
        {
            rot.y = minY;
        }

        if(rot.y > maxY)
        {
            rot.y = maxY;
        }

        transform.localEulerAngles = rot;
    }
}
