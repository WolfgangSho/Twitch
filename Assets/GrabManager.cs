using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    public GameObject go_grabmesh;

    public GameObject go_player;

    public float actionRadius;
    Vector3 playerPos;

    Raycasting sc_grabmesh;
    
    Transform activeItem;

    public LayerMask itemMask,grabMeshMask;

    bool grabbing;

    // Start is called before the first frame update
    void Start()
    {
        grabbing = false;

        sc_grabmesh = go_grabmesh.GetComponent<Raycasting>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = go_player.transform.position;

        if(!grabbing)
        {
            if(Input.GetMouseButtonDown(0))
            {
                
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray,out hit,Mathf.Infinity,itemMask,QueryTriggerInteraction.Ignore))
                {
                    if(Vector3.Magnitude(hit.point - playerPos) < actionRadius)
                    {
                        activeItem = hit.transform;
                        grabbing = true;
                    }
                }
            }
        }
        else
        {
            Vector3 newPos = sc_grabmesh.GrabPos();

            Vector3 distance = newPos - playerPos;


            if(Vector3.Magnitude(distance) > actionRadius)
            {
                Vector3 direction = distance.normalized;

                newPos = playerPos + (direction * actionRadius);
            }

            activeItem.position = newPos;

            if(Input.GetMouseButtonUp(0))
            {
                grabbing = false;

             //   Destroy(gameObject);
            }
        }
    }
}
