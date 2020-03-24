using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraDetection : MonoBehaviour
{
    public GameObject growth;
    public GameObject fill;

    float maxDiam;

    public float maxCount;
    public float delayTime;
    public float shrinkMult;

    float waitTime;

    float fillCount;
    float newScale;
    float yScale;

    // Start is called before the first frame update
    void Start()
    {
        maxDiam = fill.transform.localScale.x;

        if (fill.transform.localScale.x != fill.transform.localScale.z)
        {
            Debug.LogError("Bitch, your aura fill is not circular you dumb fuck");
        }

        fillCount = 0;
        newScale = 0;
        yScale = growth.transform.localScale.y;

        waitTime = delayTime;

        UpdateFill();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(waitTime > 0)
        {
            waitTime -= Time.fixedTime;
        }
        else
        {
            fillCount -= shrinkMult;

            if(fillCount < 0)
            {
                fillCount = 0;
            }

            UpdateFill();
        }
    }

    void OnTriggerStay(Collider c)
    {
        fillCount++;

        waitTime = delayTime;

        if(fillCount >= maxCount)
        {
            Debug.Log("I BEEN IFECTED");
            fillCount = 0;
        }

        UpdateFill();

    }

    void UpdateFill()
    {
        newScale = fillCount / maxCount * maxDiam;

        growth.transform.localScale = new Vector3(newScale, yScale, newScale);
    }
}
