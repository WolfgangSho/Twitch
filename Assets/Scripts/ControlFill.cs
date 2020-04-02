using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlFill : MonoBehaviour
{
    public float maxValue;
    public float minValue;

    float fillDiff;

    RectTransform rect;

    // Start is called before the first frame update
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        fillDiff = maxValue - minValue;
    }

    public void SetFill(float level)
    {
        if(level < 0 || level > 100)
        {
            Debug.LogError("Incorrect sanitizer level supplied");
        }
        else
        {
            float multiplier = level/100f;

            int height = Mathf.FloorToInt((multiplier * fillDiff) + minValue);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x,(float)height);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
