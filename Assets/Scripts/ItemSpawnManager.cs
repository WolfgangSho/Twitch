using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    //Rows contain Aisles which contain shelves which contain shelfNos which contain spawn points

    //Density per Aisle is max 200 (actually 198)

    public string leftNodeStart;
    public string rightNodeStart;
    
    public int shelvesperS;

    public int topSpawnNo;
    public int middleSpawnNo;
    public int bottomSpawnNo;
    
    public GameObject itemStore;
    public GameObject[] leftRow;
    public GameObject[] middleRow;
    public GameObject[] rightRow;

    public List<AisleID> travelIDs;
    public int variance;

    public int[] busyMM;
    public int[] emptyMM;

    public DayBulk[] bulk;
    ObjectPooler pool;

    int currentDensity;

    int aisleDCounter;

    int spawnIndex, spawnsLeft;

    int spawnedTop, spawnedMiddle, spawnedBottom;


    void Awake()
    {
        pool = GetComponent<ObjectPooler>();

        travelIDs = new List<AisleID>();
    
        for(int i=0; i<leftRow.Length;i++)
        {
            AisleID current = new AisleID(Row.Left,i);

            current.leftNodes = GetNodes(leftRow[i],true);
            current.rightNodes = GetNodes(leftRow[i],false);

            travelIDs.Add(current);
        }

        for(int i=0; i<middleRow.Length;i++)
        {
            AisleID current = new AisleID(Row.Middle,i);

            current.leftNodes = GetNodes(middleRow[i],true);
            current.rightNodes = GetNodes(middleRow[i],false);
            
            travelIDs.Add(current);
        }

        for(int i=0; i<rightRow.Length;i++)
        {
            AisleID current = new AisleID(Row.Right,i);

            current.leftNodes = GetNodes(rightRow[i],true);
            current.rightNodes = GetNodes(rightRow[i],false);
            
            travelIDs.Add(current);
        }
    }

    public void Start()
    {
        //test
        //SpawnSlot(null,60);

    }

    public int CustomerBulk(int day)
    {
        return bulk[day].noOfStartingCustomers;
    }

    public AisleID GetTravelAisle(Vector3 position, AisleID currentAisle, bool stayInRow)
    {
        List<AisleID> candidates = new List<AisleID>();

        if(stayInRow)
        {
            foreach(AisleID a in travelIDs)
            {
                if(a.row == currentAisle.row && a.aisleNo != currentAisle.aisleNo)
                {
                    candidates.Add(a);
                }
            }
        }
        else
        {
            foreach(AisleID a in travelIDs)
            {
                if(a.row != currentAisle.row)
                {
                    candidates.Add(a);
                }
            }
        }

        AisleID option = candidates[Random.Range(0,candidates.Count)];

        return option;

    }

    public AisleID GetNearestAisleID(Vector3 position)
    {
        float minDistance = Mathf.Infinity;

        AisleID chosenID = new AisleID(Row.Left,0);

        foreach(AisleID aID in travelIDs)
        {
            Vector3 nearestNode = aID.GetNearestNode(position, AisleSide.Left | AisleSide.Right);

            float distance = Vector3.Distance(nearestNode,position);

            if(distance < minDistance)
            {
                chosenID = aID;
                minDistance = distance;

           //     Debug.Log("Pos: " + position + " nearestNode: " + nearestNode);
            }
        }

        if(minDistance == Mathf.Infinity)
        {
            Debug.LogError("No close Aisle found for input position: " + position);
        }

        return chosenID;
    }

    public void GenerateStock(int day)
    {
        aisleDCounter = 0;

        ///Clear avaiable shelves

        currentDensity = 0;

        pool.Refresh();

        for(int i=0; i<leftRow.Length; i++)
        {
            leftRow[i].SetActive(false);
        }

        for(int i=0; i<middleRow.Length; i++)
        {
            middleRow[i].SetActive(false);
        }

        for(int i=0; i<rightRow.Length; i++)
        {
            rightRow[i].SetActive(false);
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

        int aislesLeft = leftRow.Length + middleRow.Length + rightRow.Length - busyCount - emptyCount;

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

        for(int i=0; i<leftRow.Length;i++)
        {
            if(leftRow[i].activeSelf == false)
            {
                bag.Add(Row.Left);
            }
        }

        for(int i=0; i<middleRow.Length;i++)
        {
            if(middleRow[i].activeSelf == false)
            {
                bag.Add(Row.Middle);
            }
        }

        for(int i=0; i<rightRow.Length;i++)
        {
            if(rightRow[i].activeSelf == false)
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
                tempRow = leftRow;
                break;
            case Row.Middle:
                tempRow = middleRow;
                break;
            case Row.Right:
                tempRow = rightRow;
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

        AisleID id = new AisleID(r,freeI[Random.Range(0,freeI.Count)]);

        return id;
    }



    void SpawnAisle(AisleID id, int density)
    {
        aisleDCounter += density;
     //   Debug.Log("AD: " + density);

        Row r = id.row;
        int aisle = id.aisleNo;

        switch(r)
        {
            case Row.Left:
                SpawnAisle(leftRow[aisle],density);
              //  SpawnAisle(leftRow,aisle,density);
                break;
            case Row.Middle:
                SpawnAisle(middleRow[aisle],density);
            //    SpawnAisle(middleRow,aisle,density);
                break;
            case Row.Right:
                SpawnAisle(rightRow[aisle],density);
             //   SpawnAisle(rightRow,aisle,density);
                break;
            default:
                Debug.LogError("Not a recognised Row");
                break;
        }

    }

    void SpawnAisle(GameObject aisle, int density)
    {
        aisle.SetActive(true);

        //divide density by number of shelves per side
        int divDensity = Mathf.FloorToInt((float)density/shelvesperS);
        int remDensity = density - ((shelvesperS-1)*divDensity);

        //make shelf spawn slot references
        Slot[] slots = new Slot[shelvesperS];

        GameObject go_left = aisle.transform.GetChild(0).gameObject;
        GameObject go_right = aisle.transform.GetChild(1).gameObject;

        for(int i=0; i<shelvesperS;i++)
        {
            slots[i] = new Slot();

            slots[i].left = go_left.transform.GetChild(i).gameObject.GetComponent<ShelfSpawns>();
            slots[i].right = go_right.transform.GetChild(i).gameObject.GetComponent<ShelfSpawns>();

            //spawn slots with required density
            if(i == 0)
            {
                SpawnSlot(slots[i], remDensity);
            }
            else
            {
                SpawnSlot(slots[i], divDensity);
            }
        }
    }

    void SpawnSlot(Slot chosenSlot, int density)
    {
        //one side gets 2/3rds the other gets 1/3
        float d = (float)density;
        
        int sideADensity = Mathf.CeilToInt(d/3f);

        int sideBDensity = (density - sideADensity);

        int maxSpawns = Mathf.Min(chosenSlot.left.GetMaxSpawns(),chosenSlot.right.GetMaxSpawns());

        if(sideBDensity > maxSpawns)
        {
            sideADensity += sideBDensity - maxSpawns;
        
            sideBDensity = maxSpawns;

            if(sideADensity > maxSpawns)
            {
                Debug.LogError("Too much density sent to slot");

                sideADensity = maxSpawns;
            }
        }

        GameObject[] spawnObjects = pool.AssignSlot(density);

        GameObject[] spawnA = spawnObjects.Take(sideADensity).ToArray();
        GameObject[] spawnB = spawnObjects.Skip(sideADensity).Take(sideBDensity).ToArray();

       // Debug.Log(spawnA.Length + " " + spawnB.Length);

        if(Random.value > 0.5f)
        {
            SpawnShelf(chosenSlot.left,spawnA, false);
            SpawnShelf(chosenSlot.right,spawnB, true);
        }
        else
        {
            SpawnShelf(chosenSlot.left,spawnB, false);
            SpawnShelf(chosenSlot.right,spawnA, true);
        }
    }

    void SpawnShelf(ShelfSpawns shelf, GameObject[] spawns, bool flip)
    {
        spawnIndex = 0;
        spawnsLeft = spawns.Length;
        int iterations = 0;

        int maxTop = shelf.GetMaxTop();
        int maxMiddle = shelf.GetMaxMiddle();
        int maxBottom = shelf.GetMaxBottom();

        spawnedTop = 0;
        spawnedMiddle = 0;
        spawnedBottom = 0;

        while(spawnsLeft > 0)
        {
            if(spawnedTop == maxTop && spawnedMiddle == maxMiddle && spawnedBottom == maxBottom)
            {
                Debug.LogError("Shelf full, excess: " + spawnsLeft);
                spawnsLeft = 0;
            }

            if(spawnsLeft > 1)
            {
                if(spawnedTop < maxTop)
                {
                    SpawnItem(shelf, ShelfNo.Top, spawns[spawnIndex], flip);

                    //assign second
                    if(spawnedTop < maxTop)
                    {
                        SpawnItem(shelf, ShelfNo.Top, spawns[spawnIndex], flip);
                    }
                }

                if(spawnsLeft > 1)
                {
                    if(spawnedMiddle < maxMiddle)
                    {
                        SpawnItem(shelf, ShelfNo.Middle, spawns[spawnIndex], flip);

                        //assign second
                        if(spawnedMiddle < maxMiddle)
                        {
                            SpawnItem(shelf, ShelfNo.Middle, spawns[spawnIndex], flip);
                        }
                    }
                }

                if(spawnsLeft > 0)
                {
                    if(spawnedBottom < maxBottom)
                    {
                        SpawnItem(shelf, ShelfNo.Bottom, spawns[spawnIndex], flip);
                    }
                }
            }
            else
            {
                //exactly 1 left
                if(spawnedTop < maxTop)
                {
                    SpawnItem(shelf, ShelfNo.Top, spawns[spawnIndex], flip);
                }
                else if(spawnedMiddle < maxMiddle)
                {
                    SpawnItem(shelf, ShelfNo.Middle, spawns[spawnIndex], flip);
                }
                else if(spawnedBottom < maxBottom)
                {
                    SpawnItem(shelf, ShelfNo.Bottom, spawns[spawnIndex], flip);
                }
                else
                {
                    Debug.LogError("Can't place last item.");
                    spawnsLeft = 0;
                }

            }
            
            iterations++;

            if(iterations > 30000)
            {
                Debug.LogError("While loop stuck.");
                break;
            }

        }

    }

    void SpawnItem(ShelfSpawns shelf, ShelfNo loc, GameObject spawn, bool flip)
    {
        spawnIndex++;
        spawnsLeft--;
        currentDensity++;

        Vector3 pos; 

        switch(loc)
        {
            case ShelfNo.Top:
                pos = shelf.GetPos(loc,spawnedTop);
                spawnedTop++;
                break;
            case ShelfNo.Middle:
                pos = shelf.GetPos(loc,spawnedMiddle);
                spawnedMiddle++;
                break;
            case ShelfNo.Bottom:
                pos = shelf.GetPos(loc,spawnedBottom);
                spawnedBottom++;
                break;
            default:
                pos = Vector3.zero;
                Debug.LogError("Incorrect Shelf Number");
            break;
        }

        spawn.transform.position = pos;

        if(flip)
        {
            spawn.transform.rotation = Quaternion.Euler(0,180,0);
        }
        else
        {
            spawn.transform.rotation = Quaternion.Euler(0,0,0);
        }

        spawn.SetActive(true);

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

    public List<Vector3> GetNodes(GameObject aisle, bool left)
    {
        List<Vector3> nodes = new List<Vector3>();

        for(int i=0; i<aisle.transform.childCount; i++)
        {
            string n = aisle.transform.GetChild(i).name;

            if(left)
            {
                if(n.Contains(leftNodeStart))
                {
                    nodes.Add(aisle.transform.GetChild(i).position);
                }
            }
            else
            {
                if(n.Contains(rightNodeStart))
                {
                    nodes.Add(aisle.transform.GetChild(i).position);
                }
            }
        }

        return nodes;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Slot
{
    public ShelfSpawns left;

    public ShelfSpawns right;
}

[System.Serializable]
public class DayBulk
{
    public int baseDensity;

    public int noOfBusyAisles;

    public int noOfEmptyAisles;

    public int noOfStartingCustomers;

}

[System.Serializable]
public class AisleID
{
    public Row row;

    public int aisleNo;

    public List<Vector3> leftNodes;

    public List<Vector3> rightNodes;

    public AisleID(Row r, int no)
    {
        row = r;
        aisleNo = no;
    }

    public Vector3 GetFurthestAisleExit(Vector3 position)
    {
        //figure out which side of the aisle we are closest to
        Vector3 nearestLeft = GetNearestNode(position, AisleSide.Left);
        float leftDist = Vector3.Distance(position,nearestLeft);
        Vector3 nearestRight = GetNearestNode(position,AisleSide.Right);
        float rightDist = Vector3.Distance(position,nearestRight);

        AisleSide furthestSide = leftDist < rightDist ? AisleSide.Right : AisleSide.Left;

        //get closest exit node for the other side of the aisle#

        return GetNearestNode(position,furthestSide);
    }

    public Vector3 GetNearestNode(Vector3 position, AisleSide sides)
    {
        List<Vector3> nodes = new List<Vector3>();

        if(sides.HasFlag(AisleSide.Left))
        {
            nodes.AddRange(leftNodes);
        }

        if(sides.HasFlag(AisleSide.Right))
        {
            nodes.AddRange(rightNodes);
        }

        if((int)sides == 0)
        {
            Debug.LogError("No nodes to search for when locating nearest node.");
        }

        Vector3 chosenNode = position;

        float minDistance = Mathf.Infinity;

        foreach(Vector3 n in nodes)
        {
            float distance = Vector3.Distance(n,position);

            if(distance < minDistance)
            {
                chosenNode = n;
                minDistance = distance;
            }
        }

        return chosenNode;
    }
}

[System.Flags]
public enum AisleSide
{
    Left = 1,
    Right = 2
}

public enum Row
{
    Left,
    Middle,
    Right
}
