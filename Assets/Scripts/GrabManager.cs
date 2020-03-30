using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    public GameObject go_grabmesh;

    public GameObject go_player;

    public float actionRadius;

    public float itemHeight;

    Vector3 playerPos;

    Raycasting sc_grabmesh;
    
    Transform activeItem;
    Rigidbody activeBody;

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
        playerPos = new Vector3(go_player.transform.position.x,itemHeight,go_player.transform.position.z);

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
                        activeBody = hit.rigidbody;

                        activeBody.velocity = Vector3.zero;
                        activeBody.useGravity = false;
                        activeBody.isKinematic = false;

                        activeItem.gameObject.GetComponent<Item>().grabbed = true;

                        grabbing = true;
                    }
                }
            }
        }
        else
        {
            Vector3 newPos = sc_grabmesh.GrabPos();

            Vector3 distance = newPos - playerPos;

           // Debug.Log(activeBody.velocity);


            if(Vector3.Magnitude(distance) > actionRadius)
            {
                Vector3 direction = distance.normalized;

                newPos = playerPos + (direction * actionRadius);
            }

            activeItem.position = newPos;

            if(Input.GetMouseButtonUp(0))
            {
                activeBody.useGravity = true;

                activeItem.gameObject.GetComponent<Item>().grabbed = false;
                
                grabbing = false;

             //   Destroy(gameObject);
            }
        }
    }
}
