using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    //Rows contain Aisles which contain shelves which contain shelfNos which contain spawn points

    public GameObject ItemStore;
    public GameObject[] LeftRow;
    public GameObject[] MiddleRow;
    public GameObject[] RightRow;

    
    public int variance;

    public int[] busyMM;
    public int[] emptyMM;




    
    
    public DayBulk[] bulk;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GenerateStock()
    {

    }

    void SpawnAisle(Row r,int aisle, int density)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class DayBulk
{
    public int baseDensity;

    public int noOfBusyAisles;

    public int noOfEmptyAisles;

}

public enum Row
{
    Left,
    Middle,
    Right
}
