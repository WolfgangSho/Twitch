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

            AisleID aisle = GetFreeAisle(r);

            int density = Random.Range(busyMM[0],busyMM[1]);

            SpawnAisle(aisle,density);

        }

        ///Fill Empty Aisles

        int emptyCount = bulk[day].noOfEmptyAisles;

        for(int i=0; i<emptyCount;i++)
        {
            AisleID aisle = GetFreeAisle();

            int density = Random.Range(emptyMM[0],emptyMM[1]);

            SpawnAisle(aisle,density);
        }

        ///Determine number of aisles left and density left

        int aislesLeft = LeftRow.Length + MiddleRow.Length + RightRow.Length - busyCount - emptyCount;

        int varianceAmount = Random.Range(-variance,variance+1);
        
        int remainingDensity = bulk[day].baseDensity + varianceAmount - currentDensity;

    //    Debug.Log("variance amount: " + varianceAmount + " remaining density: " + remainingDensity + " current density: " + currentDensity);

        int ABCounter = 0;

        ///Half remaining aisles get 2/3

        int alphaA = Mathf.FloorToInt((float)aislesLeft/2f);

        int densityAlpha = Mathf.CeilToInt(2f * (float)remainingDensity / 3f);

        int totalAlpha = densityAlpha;

        int densityPerAAisle = Mathf.CeilToInt((float)densityAlpha/(float)alphaA);

        for(int i=0; i<alphaA;i++)
        {
            AisleID aisle = GetFreeAisle();

            if(densityAlpha > densityPerAAisle)
            {
                SpawnAisle(aisle,densityPerAAisle);

                ABCounter += densityPerAAisle;

                densityAlpha -= densityPerAAisle;
            }
            else
            {
                SpawnAisle(aisle,densityAlpha);

                ABCounter += densityAlpha;
            }
        }
    //    Debug.Log("requested alphas: " + ABCounter);

        ABCounter = 0;

        ///Other half gets 1/3
        int betaA = aislesLeft - alphaA;

        int densityBeta = remainingDensity - totalAlpha;

        int densityPerBAisle = Mathf.CeilToInt((float)densityBeta/(float)betaA);

        for(int i=0; i<betaA;i++)
        {
             AisleID aisle = GetFreeAisle();

            if(densityBeta > densityPerBAisle)
            {
                SpawnAisle(aisle,densityPerBAisle);

                ABCounter +=densityPerBAisle;

                densityBeta -= densityPerBAisle;
            }
            else
            {
                SpawnAisle(aisle,densityBeta);

                ABCounter += densityBeta;
            }
        }

        Debug.Log(bulk[day].baseDensity + varianceAmount + " planned density. " + 
            aisleDCounter + " sum of requested densities. " + 
            currentDensity + " items spawned. Planned to spawned Diff: " + 
            (bulk[day].baseDensity + varianceAmount - currentDensity));

    //    Debug.Log("requested betas: " + ABCounter);




    }

    AisleID GetFreeAisle()
    {
        List<Row> bag = new List<Row>();

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

        int result = Random.Range(0,bag.Count);

        Row r = bag[result];

        AisleID id = GetFreeAisle(r);

        return id;

    }

    AisleID GetFreeAisle(Row r)
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

        AisleID id = new AisleID{row = r, ailseNo = freeI[Random.Range(0,freeI.Count)]};

        return id;
    }



    void SpawnAisle(AisleID id, int density)
    {
        aisleDCounter += density;
     //   Debug.Log("AD: " + density);

        Row r = id.row;
        int aisle = id.ailseNo;

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
            SpawnSide(leftShelves,sideADensity, false);
            SpawnSide(rightShelves,sideBDensity, true);
        }
        else
        {
            SpawnSide(leftShelves,sideBDensity, false);
            SpawnSide(rightShelves,sideADensity, true);
        }
    }

    void SpawnSide(GameObject[] shelves, int density, bool flip)
    {
        //reference to shelf scripts

        ShelfSpawns[] spawns = new ShelfSpawns[shelves.Length];

        for(int i=0; i<spawns.Length; i++)
        {
            spawns[i] = shelves[i].GetComponent<ShelfSpawns>();
        }

        //determine density


        float d = (float)density;

        int shelfDensity = Mathf.CeilToInt(d/(float)shelvesperS) - 2;

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

                SpawnItem(spawns[i].GetPos(ShelfNo.Top,index),flip);
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

                SpawnItem(spawns[i].GetPos(ShelfNo.Middle,index),flip);
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

                SpawnItem(spawns[i].GetPos(ShelfNo.Bottom,index), flip);
            }
        }
    }

    void SpawnItem(Vector3 pos, bool flip)
    {
        currentDensity++;

        GameObject g = pool.GetItem();

        g.transform.position = pos;

        if(flip)
        {
            g.transform.rotation = Quaternion.Euler(0,180,0);
        }
        else
        {
            g.transform.rotation = Quaternion.Euler(0,0,0);
        }

        g.SetActive(true);

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

public class AisleID
{
    public Row row;

    public int ailseNo;
}

public enum Row
{
    Left,
    Middle,
    Right
}
