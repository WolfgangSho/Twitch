using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AuraDetection : MonoBehaviour
{
    public GameObject go_SanitizerFill;

    public GameObject go_GM;

    public float maxRisk;
    public float shrinkDelay;
    public float shrinkMult;
    public float drainSpeed;

    public GameObject go_AlertLeft;
    public GameObject go_AlertRight;
    public GameObject go_AlertTop;
    public GameObject go_AlertBottom;

    float waitTime;

    Animator anim_AlertLeft;
    Animator anim_AlertRight;
    Animator anim_AlertTop;
    Animator anim_AlertBottom;

    float riskCount;
    float fillPercent; //between 0 and 100

    ControlFill fill;

    GM gameM;

    // Start is called before the first frame update
    void Awake()
    {
        fill = go_SanitizerFill.GetComponent<ControlFill>();
        gameM = go_GM.GetComponent<GM>();

        anim_AlertLeft = go_AlertLeft.GetComponent<Animator>();
        anim_AlertRight = go_AlertRight.GetComponent<Animator>();
        anim_AlertTop= go_AlertTop.GetComponent<Animator>();
        anim_AlertBottom = go_AlertBottom.GetComponent<Animator>();

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

            if(riskCount == 0)
            {
                anim_AlertLeft.SetBool("Danger",false);
                anim_AlertRight.SetBool("Danger",false);
                anim_AlertTop.SetBool("Danger",false);
                anim_AlertBottom.SetBool("Danger",false);
            }
        }
    }

    void OnTriggerStay(Collider c)
    {
        anim_AlertLeft.SetBool("Danger",true);
        anim_AlertRight.SetBool("Danger",true);
        anim_AlertTop.SetBool("Danger",true);
        anim_AlertBottom.SetBool("Danger",true);

        riskCount+= Time.fixedDeltaTime;

        waitTime = shrinkDelay;

    //    Debug.Log("current risk: " + riskCount);

        if(riskCount >= maxRisk)
        {
         //   Debug.Log("I BEEIN IFECTED");

            fillPercent -= drainSpeed;

            UpdateFill();
        }
    }

    void UpdateFill()
    {
        if(fillPercent < 0)
        {
            gameM.GameOver();
        }
        else
        {
            fill.SetFill(fillPercent);
        }
    }
}
