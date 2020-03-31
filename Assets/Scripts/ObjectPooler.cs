using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    
    public int commonWeight;
    public int uncommonWeight;
    public int rareWeight;

    public int minBagSize;

    
    public GameObject store;
    public Swimmer[] swimmers;

    
    List<int> nextAvailable;

    [SerializeField]
    public List<List<GameObject>> poolTypes;

    List<ItemRarity> bag;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FillPool()
    {
        ResetRarityBag();

        nextAvailable = new List<int>();

        poolTypes = new List<List<GameObject>>();

        for(int i=0; i<swimmers.Length;i++)
        {
            nextAvailable.Add(0);

            poolTypes.Add(new List<GameObject>());

            for(int j=0; j<swimmers[i].spawnAmount;j++)
            {
                GameObject g = (GameObject)Instantiate(swimmers[i].prefab);
                g.SetActive(false);
                g.transform.parent = store.transform;

                poolTypes[i].Add(g);
            }
        }
    }

    public void Refresh()
    {
        ResetRarityBag();

        for(int i=0; i<poolTypes.Count;i++)
        {
            nextAvailable[i] = 0;

            for(int j=0; j<poolTypes[i].Count;j++)
            {
                poolTypes[i][j].SetActive(false);
            }
        }
    }


    public GameObject GetNext(ItemType t)
    {
        int type = (int)t;
        int next = nextAvailable[type];

        GameObject g;
        
        if(next < poolTypes[type].Count)
        {
            g = poolTypes[type][nextAvailable[type]];

            nextAvailable[type]++;
        }
        else
        {
            Debug.LogError("Out of " + System.Enum.GetName(typeof(ItemType),type));
            g = null;
        }

        return g;
    }

    public GameObject GetItem()
    {
        if(bag.Count < minBagSize)
        {
            ResetRarityBag();
        }

        int index = Random.Range(0,bag.Count);

        ItemRarity rarity = bag[index];

        bag.RemoveAt(index);

        List<ItemType> candidates = new List<ItemType>();

        foreach(Swimmer s in swimmers)
        {
            if(s.rarity == rarity)
            {
                candidates.Add(s.type);
            }
        }

        return GetNext(candidates[Random.Range(0,candidates.Count)]);
    }

    void ResetRarityBag()
    {
        bag = new List<ItemRarity>();

        for(int i=0; i<commonWeight; i++)
        {
            bag.Add(ItemRarity.Common);
        }

        for(int i=0; i<uncommonWeight; i++)
        {
            bag.Add(ItemRarity.Uncommon);
        }

        for(int i=0; i<rareWeight; i++)
        {
            bag.Add(ItemRarity.Rare);
        }

    }



    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetMouseButtonDown(0))
        {
            GameObject gTest = GetNext(ItemType.TinCan);

            gTest.SetActive(true);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Refresh();
        }*/
        
    }
}

[System.Serializable]
public class Swimmer
{
    public ItemType type;
   
    public GameObject prefab;

    public int spawnAmount;

    public ItemRarity rarity;
}

public enum ItemType
{
    Cereal,
    Bread,
    TinCan
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Key
}
