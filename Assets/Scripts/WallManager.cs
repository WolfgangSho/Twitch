using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public LayerMask mask;
    public GameObject go_Player;
    public GameObject LeftWall,RightWall,BottomWall,TopWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, (go_Player.transform.position - Camera.main.transform.position));

        LeftWall.SetActive(true);
        RightWall.SetActive(true);
        BottomWall.SetActive(true);
        TopWall.SetActive(true);

        
        if(Physics.Raycast(ray,out hit,Mathf.Infinity,mask,QueryTriggerInteraction.Collide))
        {
            hit.transform.gameObject.SetActive(false);
        }
        
    }
}
