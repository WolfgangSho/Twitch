using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ShoppingListManager : MonoBehaviour
{
    
    public int listSize;

    public int maximumDuplicates;
    public ItemOfEachRarity[] usedRarities;
    
    public GameObject[] go_items;

    ObjectPooler sc_pool;

    TextMeshProUGUI[] text_items;

    List<Item> desiredItems;

    bool[] striked;

    List<Item> trolleyContents;

    List<UniqueItemCount> listIndex;

    // Start is called before the first frame update
    void Awake()
    {
        sc_pool = GetComponent<ObjectPooler>();

     //   text_items = go_items.GetComponent<TextMesh>();

        text_items = new TextMeshProUGUI[go_items.Length];

        for(int i=0; i<go_items.Length;i++)
        {
            text_items[i] = go_items[i].GetComponent<TextMeshProUGUI>();
        }

        int sizeCheck = 0;

        for(int i=0; i<usedRarities.Length; i++)
        {
            sizeCheck += usedRarities[i].amount;
        }

        if(sizeCheck != listSize)
        {
            Debug.LogError("Sum of Items requested of each rartiy in shopping list does not match the shopping list size set");
        }

        if(text_items.Length != listSize)
        {
            Debug.LogError("Number of TextMeshPro elements does not equal the shopping list size set");
        }
    }

    //called by Game Manager
    public void PickItems()
    {
        desiredItems = new List<Item>();
        trolleyContents = new List<Item>();
        listIndex = new List<UniqueItemCount>();

        //for each rarity type that we're using ask the object pool for some items
        for(int i=0; i<usedRarities.Length; i++)
        {
            desiredItems.AddRange(sc_pool.GetList(usedRarities[i].rarity,usedRarities[i].amount, maximumDuplicates));
        }

        striked = new bool[desiredItems.Count];

        UpdateList();

        for(int i=0; i<desiredItems.Count;i++)
        {
            striked[i] = false;
            text_items[i].text = desiredItems[i].fullName;

            int listIndexElement = -1;

            for(int j=0; j<listIndex.Count;j++)
            {
                if(listIndex[j].inList == desiredItems[i])
                {
                    listIndexElement = j;
                }
            }
            
            if(listIndexElement == -1)
            {
                listIndex.Add(new UniqueItemCount(desiredItems[i],i));
            }
            else
            {
                listIndex[listIndexElement].Add(i);
            }
        }
    }

    public void AddToTrolley(Item it)
    {
        trolleyContents.Add(it);
        UpdateList();
    }

    public void RemoveFromTrolley(Item it)
    {
        trolleyContents.Remove(it);
        UpdateList();
    }

    void UpdateList()
    {
        for(int i=0; i<striked.Length; i++)
        {
            striked[i] = false;
        }

        for(int i=0;i < listIndex.Count;i++)
        {
            Item it = listIndex[i].inList;

            int inTrolley = 0;

            for(int j=0; j<trolleyContents.Count;j++)
            {
                if(trolleyContents[j] == it)
                {
                    inTrolley++;
                }
            }

            for(int j=0; j<listIndex[i].indices.Count;j++)
            {
                if(j < inTrolley)
                {
                    striked[listIndex[i].indices[j]] = true;
                }
            }
        }

        for(int i=0; i<striked.Length;i++)
        {
            if(striked[i])
            {
                text_items[i].fontStyle = FontStyles.Strikethrough;
            }
            else
            {
                text_items[i].fontStyle = FontStyles.Normal;
            }
        }       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class ItemOfEachRarity
{
    public ItemRarity rarity;

    public int amount;
}

public class UniqueItemCount
{
    public Item inList;

    public List<int> indices;

    public UniqueItemCount(Item it, int index)
    {
        inList = it;

        indices = new List<int>();

        indices.Add(index);
    }

    public void Add(int index)
    {
        indices.Add(index);
    }
}
