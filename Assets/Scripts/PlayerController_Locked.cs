using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Locked : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

    public GameObject cam;

    Vector3 oldPos, nextPos, moveVec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        oldPos = transform.position;
        transform.Translate(0,0,Input.GetAxis("Vertical") * -moveSpeed * 0.1f);
        nextPos = transform.position;

        moveVec = nextPos - oldPos;

        cam.transform.Translate(moveVec,Space.World);

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed, 0, Space.World);

    }
}
