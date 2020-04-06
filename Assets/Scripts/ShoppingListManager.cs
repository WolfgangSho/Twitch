using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShoppingListManager : MonoBehaviour
{
    
    public int listSize;

    public int maximumDuplicates;
    public ItemOfEachRarity[] usedRarities;
    
    public GameObject[] go_items;

    ObjectPooler sc_pool;

    TextMeshProUGUI[] text_items;

    List<Item> desiredItems;


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

        //for each rarity type that we're using ask the object pool for some items
        for(int i=0; i<usedRarities.Length; i++)
        {
            desiredItems.AddRange(sc_pool.GetList(usedRarities[i].rarity,usedRarities[i].amount, maximumDuplicates));
        }

        for(int i=0; i<desiredItems.Count;i++)
        {
            text_items[i].text = desiredItems[i].fullName;
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
