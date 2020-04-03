using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    
    public int commonWeight;
    public int uncommonWeight;
    public int rareWeight;

    public int minBagSize;

    
    public RaritySpawnAmount[] raritySpawns;
    public GameObject store;
    public Item[] itemsToSpawn;
    
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

        foreach(ItemType it in System.Enum.GetValues(typeof(ItemType)))
        {
            nextAvailable.Add(0);

            poolTypes.Add(new List<GameObject>());
        }


        for(int i=0; i<itemsToSpawn.Length;i++)
        {
            
            IEnumerable<int> spawnAmount = raritySpawns.Where(spawns => spawns.rarity == itemsToSpawn[i].rarity).Select(spawns => spawns.amount);

            
            if(spawnAmount.Count() != 1)
            {
                Debug.LogError("More or less than one inital spawn amount found for rarity " + itemsToSpawn[i].rarity);
            }


            for(int j=0; j<spawnAmount.First();j++)
            {
                GameObject g = (GameObject)Instantiate(itemsToSpawn[i].prefab);
                g.SetActive(false);
                g.transform.parent = store.transform;

                poolTypes[(int)itemsToSpawn[i].type].Add(g);
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

    public GameObject GetItem()
    {
        if(bag.Count < minBagSize)
        {
            ResetRarityBag();
        }

        int index = Random.Range(0,bag.Count);

        ItemRarity rarity = bag[index];

        bag.RemoveAt(index);

        List<ItemType> candidates = itemsToSpawn.Where(p => p.rarity == rarity).Select(p => p.type).ToList();

        return GetNext(candidates[Random.Range(0,candidates.Count)]);
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
        }
        */
        
    }
}

[System.Serializable]
public class RaritySpawnAmount
{
    public ItemRarity rarity;

    public int amount;
}

public enum ItemType
{
    Cereal,
    Snacks,
    MiscBoxed,
    Bread,
    ToiletRoll,
    MiscPack,
    TinCan
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Key
}
