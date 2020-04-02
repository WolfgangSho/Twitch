using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraDetection : MonoBehaviour
{
    public GameObject go_SanitizerFill;

    public float maxRisk;
    public float shrinkDelay;
    public float shrinkMult;
    public float drainSpeed;

    float waitTime;

    
    float riskCount;
    float fillPercent; //between 0 and 100

    ControlFill fill;

    // Start is called before the first frame update
    void Awake()
    {
        fill = go_SanitizerFill.GetComponent<ControlFill>();

        fillPercent = 100f;

        riskCount = 0;
        
    }

    public void Reset()
    {
        fillPercent = 100f;

        waitTime = 0;

        riskCount = 0;

        UpdateFill();

    }

    public void AddFill(float amount)
    {
        fillPercent += amount;

        if(fillPercent > 100f)
        {
            fillPercent = 100f;
        }

        UpdateFill();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(waitTime > 0)
        {
            waitTime -= Time.fixedDeltaTime;
        }
        else
        {
            riskCount -= shrinkMult;

            if(riskCount < 0)
            {
                riskCount = 0;
            }
        }
    }

    void OnTriggerStay(Collider c)
    {
        riskCount+= Time.fixedDeltaTime;

        waitTime = shrinkDelay;

    //    Debug.Log("current risk: " + riskCount);

        if(riskCount >= maxRisk)
        {
            Debug.Log("I BEEIN IFECTED");

            fillPercent -= drainSpeed;

            UpdateFill();
        }
    }

    void UpdateFill()
    {
        if(fillPercent < 0)
        {
            Debug.Log("YOU LOSE SIR");
        }
        else
        {
            fill.SetFill(fillPercent);
        }
    }
}
