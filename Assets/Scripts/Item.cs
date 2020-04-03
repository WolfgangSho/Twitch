using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GenericItem", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{

    public string fullName;
    
    public ItemType type;
   
    public GameObject prefab;

    public ItemRarity rarity;

}
