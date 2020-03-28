using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    //Rows contain Aisles which contain shelves which contain shelfNos which contain spawn points

    //Density per Aisle is max 200 (actually 198)

    public int shelvesperS;

    public int topSpawnNo;
    public int middleSpawnNo;

    public int bottomSpawnNo;
    
    public GameObject ItemStore;
    public GameObject[] LeftRow;
    public GameObject[] MiddleRow;
    public GameObject[] RightRow;

    public int variance;

    public int[] busyMM;
    public int[] emptyMM;

    public DayBulk[] bulk;
    ObjectPooler pool;

    int currentDensity;

    int aisleDCounter;

    void Awake()
    {
        pool = GetComponent<ObjectPooler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GenerateStock(int day)
    {
        aisleDCounter = 0;

        ///Clear avaiable shelves

        currentDensity = 0;

        pool.Refresh();

        for(int i=0; i<LeftRow.Length; i++)
        {
            LeftRow[i].SetActive(false);
        }

        for(int i=0; i<MiddleRow.Length; i++)
        {
            MiddleRow[i].SetActive(false);
        }

        for(int i=0; i<RightRow.Length; i++)
        {
            RightRow[i].SetActive(false);
        }

        ///Fill Busy Aisles

        int busyCount = bulk[day].noOfBusyAisles;

        List<Row> bag = new List<Row>();

        bag.Add(Row.Left); bag.Add(Row.Left); bag.Add(Row.Left);

        bag.Add(Row.Middle); bag.Add(Row.Middle); bag.Add(Row.Middle); bag.Add(Row.Middle); bag.Add(Row.Middle); bag.Add(Row.Middle);

        bag.Add(Row.Right); bag.Add(Row.Right); bag.Add(Row.Right);

        for(int i=0; i<busyCount;i++)
        {
            int result = Random.Range(0,bag.Count);

            Row r = bag[i];

            bag.RemoveAt(result);

            int aisleNo = GetFreeAisle(r);

            int density = Random.Range(busyMM[0],busyMM[1]);

            SpawnAisle(r,aisleNo,density);

        }

        ///Fill Empty Aisles

        int emptyCount = bulk[day].noOfEmptyAisles;

        bag.Clear();

        for(int i=0; i<LeftRow.Length;i++)
        {
            if(LeftRow[i].activeSelf == false)
            {
                bag.Add(Row.Left);
            }
        }

        for(int i=0; i<MiddleRow.Length;i++)
        {
            if(MiddleRow[i].activeSelf == false)
            {
                bag.Add(Row.Middle);
            }
        }

        for(int i=0; i<RightRow.Length;i++)
        {
            if(RightRow[i].activeSelf == false)
            {
                bag.Add(Row.Right);
            }
        }

        for(int i=0; i<emptyCount;i++)
        {
            int result = Random.Range(0,bag.Count);

            Row r = bag[i];

            bag.RemoveAt(result);

            int aisleNo = GetFreeAisle(r);

            int density = Random.Range(emptyMM[0],emptyMM[1]);

            SpawnAisle(r,aisleNo,density);
        }


        ///Determine number of aisles left and density left

        ///Half remaining aisles get 2/3

        ///Other half gets 1/3

        Debug.Log(aisleDCounter + " sum of aisle densities. " + currentDensity + " items spawned. Diff: " + (currentDensity - aisleDCounter));



    }

    int GetFreeAisle(Row r)
    {
        GameObject[] tempRow = new GameObject[0];

        switch(r)
        {
            case Row.Left:
                tempRow = LeftRow;
                break;
            case Row.Middle:
                tempRow = MiddleRow;
                break;
            case Row.Right:
                tempRow = RightRow;
                break;
            default:
                Debug.LogError("Not a recognised Row");
                break;
        }

        List<int> freeI = new List<int>();

        for(int i=0; i<tempRow.Length;i++)
        {
            if(tempRow[i].activeSelf == false)
            {
                freeI.Add(i);
            }
        }

        return freeI[Random.Range(0,freeI.Count)];
    }



    void SpawnAisle(Row r,int aisle, int density)
    {
        aisleDCounter += density;
     //   Debug.Log("AD: " + density);

        switch(r)
        {
            case Row.Left:
                SpawnAisle(LeftRow,aisle,density);
                break;
            case Row.Middle:
                SpawnAisle(MiddleRow,aisle,density);
                break;
            case Row.Right:
                SpawnAisle(RightRow,aisle,density);
                break;
            default:
                Debug.LogError("Not a recognised Row");
                break;
        }

    }

    void SpawnAisle(GameObject[] row, int aisle, int density)
    {
        row[aisle].SetActive(true);

        //setup pointers to aisle, sides, and shelves

        GameObject go_aisle = row[aisle];

        GameObject go_left = go_aisle.transform.GetChild(0).gameObject;
        GameObject go_right = go_aisle.transform.GetChild(1).gameObject;

        GameObject[] leftShelves = new GameObject[go_left.transform.childCount];

        for(int i=0; i<leftShelves.Length;i++)
        {
            leftShelves[i] = go_left.transform.GetChild(i).gameObject;
        }

        GameObject[] rightShelves = new GameObject[go_right.transform.childCount];

        for(int i=0; i<rightShelves.Length;i++)
        {
            rightShelves[i] = go_right.transform.GetChild(i).gameObject;
        }
     
        //one side gets 2/3rds the other gets 1/3

        float d = (float)density;
        
        int sideADensity = Mathf.CeilToInt(d/3f);

        int sideBDensity = (2 * sideADensity) - 1;

        if(sideBDensity > 99)
        {
            sideADensity += sideBDensity - 99;
        
            sideBDensity = 99;
        }

        if(Random.value > 0.5f)
        {
            SpawnSide(leftShelves,sideADensity);
            SpawnSide(rightShelves,sideBDensity);
        }
        else
        {
            SpawnSide(leftShelves,sideBDensity);
            SpawnSide(rightShelves,sideADensity);
        }
    }

    void SpawnSide(GameObject[] shelves, int density)
    {
        //reference to shelf scripts

        ShelfSpawns[] spawns = new ShelfSpawns[shelves.Length];

        for(int i=0; i<spawns.Length; i++)
        {
            spawns[i] = shelves[i].GetComponent<ShelfSpawns>();
        }

        //determine density


        float d = (float)density;

        int shelfDensity = Mathf.CeilToInt(d/(float)shelvesperS) - 1;

        d = (float)shelfDensity;

        d = d/20f;

        int topDensity = Mathf.CeilToInt(d * 9);
        int middleDensity = Mathf.CeilToInt(d * 7);
        int bottomDensity = Mathf.CeilToInt(d * 4);

        if(topDensity > topSpawnNo)
        {
            middleDensity += topDensity - topSpawnNo;

            topDensity = topSpawnNo;
        }

        if(middleDensity > middleSpawnNo)
        {
            bottomDensity += middleDensity - middleSpawnNo;
            middleDensity = middleSpawnNo;
        }

        if(bottomDensity > bottomSpawnNo)
        {
            Debug.Log("bot: " + bottomDensity);
            bottomDensity = bottomSpawnNo;
        }

         List<int> bag = new List<int>();

        for(int i=0; i<spawns.Length; i++)
        {
            //ask for top tier positions

            bag.Clear();

            for(int j=0; j<topSpawnNo; j++)
            {
                bag.Add(j);
            }

            for(int j=0; j<topDensity; j++)
            {
                int index = bag[Random.Range(0,bag.Count)];

                bag.Remove(index);

                SpawnItem(spawns[i].GetPos(ShelfNo.Top,index));
            }

            //ask for mid tier positions

            bag.Clear();

            for(int j=0; j<middleSpawnNo; j++)
            {
                bag.Add(j);
            }

            for(int j=0; j<middleDensity; j++)
            {
                int index = bag[Random.Range(0,bag.Count)];

                bag.Remove(index);

                SpawnItem(spawns[i].GetPos(ShelfNo.Middle,index));
            }

            //ask for bottom tier positions

            bag.Clear();

            for(int j=0; j<bottomSpawnNo; j++)
            {
                bag.Add(j);
            }

            for(int j=0; j<bottomDensity; j++)
            {
                int index = bag[Random.Range(0,bag.Count)];

                bag.Remove(index);

                SpawnItem(spawns[i].GetPos(ShelfNo.Bottom,index));
            }
        }
    }

    void SpawnItem(Vector3 pos)
    {
        currentDensity++;

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
