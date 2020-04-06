using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasting : MonoBehaviour
{
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {/*
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log(GrabPos().ToString());
        }*/
    }

    public Vector3 GrabPos()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(ray.direction.y >= 0)
        {
            float up = -0.01f;

            ray.direction = new Vector3(ray.direction.x,up,ray.direction.z);
        }

        if(Physics.Raycast(ray,out hit,Mathf.Infinity,mask,QueryTriggerInteraction.Collide))
        {
            return hit.point;
        }
        else
        {
            Debug.LogError("Invalid raycast");
            return new Vector3();
        }

    }
}
