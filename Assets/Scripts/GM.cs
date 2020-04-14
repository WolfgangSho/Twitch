using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public GameObject go_player;
    public GameObject go_aura;


    public GameObject go_canvas;
    public GameObject go_menu;

    public GameObject go_menu_GameOver;
    public GameObject go_menu_Checkout;

    public Vector3 playerStartPos;
    public Vector3 playerStartRot;

    AuraDetection sc_aura;
    PlayerController_Sonic3 sc_pc;
    ObjectPooler sc_pool;
    ShoppingListManager sc_list;
    ItemSpawnManager sc_spawn;
    CustomerManager sc_customer;

    float cTime;

    // Start is called before the first frame update
    void Start()
    {
        sc_aura = go_aura.GetComponent<AuraDetection>();
        sc_pc = go_player.GetComponent<PlayerController_Sonic3>();
        sc_pool = GetComponent<ObjectPooler>();
        sc_list = GetComponent<ShoppingListManager>();
        sc_spawn = GetComponent<ItemSpawnManager>();
        sc_customer = GetComponent<CustomerManager>();

        go_canvas.SetActive(true);

        sc_pool.FillPool();

        GameReset();
    }

    public void Checkout()
    {
        Debug.Log("HDHDHD");
        sc_list.UpdateMenu();
        SetMenu(MenuState.Checkout);
    }

    public void NextDay()
    {
        Debug.Log("NEXT DAY");
    }

    public void LevelReset()
    {
        sc_aura.Reset();

        go_player.transform.position = playerStartPos;
        go_player.transform.localRotation = Quaternion.Euler(playerStartRot);

        SetMenu(MenuState.Clear);

        sc_list.PickItems();

        sc_spawn.GenerateStock(8);

        sc_customer.GenerateCustomers(100);

        //reset items! Done?


    }

    public void GameReset()
    {
        LevelReset();
    }

    public void GameOver()
    {
        SetMenu(MenuState.GameOver);
    }
        
    void SetMenu(MenuState state)
    {
        //clear all menu states
        
        go_menu_GameOver.SetActive(false);
        go_menu_Checkout.SetActive(false);


        if(state == MenuState.Clear)
        {
            go_menu.SetActive(false);
            sc_pc.playable = true;
        }
        else
        {
            go_menu.SetActive(true);
            sc_pc.playable = false;

            switch(state)
            {
                case MenuState.GameOver:
                    go_menu_GameOver.SetActive(true);
                    break;
                case MenuState.Checkout:
                    go_menu_Checkout.SetActive(true);
                    break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum MenuState
{
    Clear,
    Checkout,
    GameOver
}
