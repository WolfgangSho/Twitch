using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    
    public int numberOfSlots;
    
    public int commonWeight;
    public int uncommonWeight;
    public int rareWeight;

    public int minBagSize;

    
    public RaritySpawnAmount[] raritySpawns;
    public GameObject store;
    public Item[] itemsToSpawn;
    
    List<int> nextAvailable;

    [SerializeField]
    public List<List<GameObject>> pooledItems;

    List<ItemRarity> bag;

    public SlotRarityChances[] slotChances;
    List<Item> slotBag;
    int[] slotAmounts;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FillPool()
    {
        ResetRarityBag();

        ResetSlotBag();

        nextAvailable = new List<int>();

        pooledItems = new List<List<GameObject>>();

        for(int i=0; i<itemsToSpawn.Length;i++)
        {
            nextAvailable.Add(0);

            pooledItems.Add(new List<GameObject>());
            
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

                pooledItems[i].Add(g);
            }
        }
    }

    public void Refresh()
    {
        ResetRarityBag();

        ResetSlotBag();

        for(int i=0; i<pooledItems.Count;i++)
        {
            nextAvailable[i] = 0;

            for(int j=0; j<pooledItems[i].Count;j++)
            {
                pooledItems[i][j].SetActive(false);
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

        List<int> candidates = new List<int>();

        for(int i=0; i<itemsToSpawn.Length;i++)
        {
            if(itemsToSpawn[i].rarity == rarity)
            {
                candidates.Add(i);
            }
        }

        return GetNext(candidates[Random.Range(0,candidates.Count)]);
    }

    public GameObject GetNext(int index)
    {
        GameObject g;

        int next = nextAvailable[index];
        
        if(next < pooledItems[index].Count)
        {
            g = pooledItems[index][nextAvailable[index]];

            nextAvailable[index]++;
        }
        else
        {
            Debug.LogError("Out of " + itemsToSpawn[index]);
            g = null;
        }

        return g;
    }

    void ResetSlotBag()
    {
        //make slot available int array
        slotAmounts = new int[itemsToSpawn.Length];
        List<int> fullElements = new List<int>();

        SlotRarityChances src;
        Item it;

        int slots;

        for(int i=0; i<slotAmounts.Length;i++)
        {
            it = itemsToSpawn[i];
            src = slotChances.First(r => r.rarity == it.rarity);

            slots = src.gaurenteed;

            if(Random.value < src.chance1)
            {
                slots++;
            }

            if(Random.value < src.chance2)
            {
                slots++;
            }

            if(slots == src.gaurenteed + 2)
            {       
                fullElements.Add(i);
            }

            slotAmounts[i] = slots;
        }

        

      //  Debug.Log("Before alteration: " + slotAmounts.Sum());

        int iterations = 0;

        while(slotAmounts.Sum() != numberOfSlots)
        {
            int index = -1;

            if(fullElements.Count > 0)
            {
                index = fullElements[Random.Range(0,fullElements.Count)];
            }
            else
            {
                while(index < 0)
                {
                    int tempIndex = Random.Range(0,slotAmounts.Length);

                    if(slotAmounts[tempIndex] > 1)
                    {
                        index = tempIndex;
                    }
                }
            }

      //      Debug.Log(fullElements.Count);
      //      Debug.Log("index is " + index);

            if(slotAmounts.Sum() > numberOfSlots)
            {
                slotAmounts[index]--;
                fullElements.Remove(index);
            }
            
            if(slotAmounts.Sum() < numberOfSlots)
            {
                slotAmounts[Random.Range(0,slotAmounts.Length)]++;
            }

            iterations++;

            if(iterations > 5000)
            {
                Debug.LogError("Trapped in while loop");
                break;
            }
        }

     //   Debug.Log("After alteration: " + slotAmounts.Sum());

        slotBag = new List<Item>();

        for(int i=0; i<itemsToSpawn.Length; i++)
        {
            for(int j=0; j<slotAmounts[i]; j++)
            {
                slotBag.Add(itemsToSpawn[i]);
            }
        }
    }

    public GameObject[] AssignSlot(int density)
    {
        int itemIndex = -1;

        while(itemIndex < 0)
        {
            int tempIndex = Random.Range(0,slotAmounts.Length);

            if(slotAmounts[tempIndex] > 0)
            {
                itemIndex = tempIndex;

                slotAmounts[tempIndex]--;
            }
        }

        GameObject[] prefabs = new GameObject[density];

        for(int i=0; i<prefabs.Length; i++)
        {
            prefabs[i] = GetNext(itemIndex);
        }

        return prefabs;
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

    public List<Item> GetList(ItemRarity rarity, int amount, int maxDuplicates)
    {
        List<Item> selected = new List<Item>();
        
        List<Item> options = itemsToSpawn.Where(t => t.rarity == rarity).ToList();

        int[] duplicates = new int[options.Count];

        for(int i=0; i<amount; i++)
        {
            int index = Random.Range(0,options.Count);
            
            Item candidate  = options[index];

            if(duplicates[index] < maxDuplicates)
            {
                selected.Add(candidate);
                duplicates[index]++;
            }
            else
            {
                //Debug.Log("too many boi " + duplicates[index]);
                i--;
            }
        }

        return selected;
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

[System.Serializable]
public class SlotRarityChances
{
    public ItemRarity rarity;

    public float chance1;

    public float chance2;

    public int gaurenteed;
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Key
}
