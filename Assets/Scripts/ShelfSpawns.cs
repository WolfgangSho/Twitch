using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfSpawns : MonoBehaviour
{    
    public GameObject[] go_TopSpawns;
    public GameObject[] go_MiddleSpawns;
    public GameObject[] go_BottomSpawns;

    Vector3[] topPositions;
    Vector3[] middlePositions;
    Vector3[] bottomPositions;


    // Start is called before the first frame update
    void Start()
    {
        topPositions = new Vector3[go_TopSpawns.Length];

        for(int i=0; i<go_TopSpawns.Length;i++)
        {
            topPositions[i] = go_TopSpawns[i].transform.position;
        }


        middlePositions = new Vector3[go_MiddleSpawns.Length];

        for(int i=0; i<go_MiddleSpawns.Length;i++)
        {
            middlePositions[i] = go_MiddleSpawns[i].transform.position;
        }


        bottomPositions = new Vector3[go_BottomSpawns.Length];

        for(int i=0; i<go_BottomSpawns.Length;i++)
        {
            bottomPositions[i] = go_BottomSpawns[i].transform.position;
        }
        
    }

    public Vector3 GetPos(ShelfNo s, int e)
    {
        if(s == ShelfNo.Top)
        {
            return topPositions[e];
        }
        else if(s == ShelfNo.Middle)
        {
            return middlePositions[e];
        }
        else if(s == ShelfNo.Bottom)
        {
            return bottomPositions[e];
        }
        else
        {
            Debug.LogError("Incorrect Shelf Number");
            return Vector3.zero;
        }
    }

    public int GetMaxSpawns()
    {
        int number = topPositions.Length + middlePositions.Length + bottomPositions.Length;

        return number;
    }

    public int GetMaxTop()
    {
        return topPositions.Length;
    }
    public int GetMaxMiddle()
    {
        return middlePositions.Length;
    }

    public int GetMaxBottom()
    {
        return bottomPositions.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum ShelfNo
{
    Top,
    Middle,
    Bottom
}
