using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GM : MonoBehaviour
{
    
    public int maxDays;
    public int day;
    public int score;
    public GameObject go_player;
    public GameObject go_aura;


    public GameObject go_canvas;
    public GameObject go_menu;

    public GameObject go_menu_Start;
    public GameObject go_menu_GameOver;
    public GameObject go_menu_Checkout;

    public GameObject go_menu_GameEnd;

    public GameObject go_gui_day;

    public Vector3 playerStartPos;
    public Vector3 playerStartRot;

    TextMeshProUGUI gui_day;

    AuraDetection sc_aura;
    PlayerController_Sonic3 sc_pc;
    ObjectPooler sc_pool;
    ShoppingListManager sc_list;
    ItemSpawnManager sc_spawn;
    CustomerManager sc_customer;
    GrabManager sc_grab;

    float cTime;

    // Start is called before the first frame update
    void Start()
    {
        gui_day = go_gui_day.GetComponent<TextMeshProUGUI>();

        sc_aura = go_aura.GetComponent<AuraDetection>();
        sc_pc = go_player.GetComponent<PlayerController_Sonic3>();
        sc_pool = GetComponent<ObjectPooler>();
        sc_list = GetComponent<ShoppingListManager>();
        sc_spawn = GetComponent<ItemSpawnManager>();
        sc_customer = GetComponent<CustomerManager>();
        sc_grab = GetComponent<GrabManager>();

        go_canvas.SetActive(true);

        sc_pool.FillPool();

        SetMenu(MenuState.Start);
    }


    public void Checkout()
    {
        score += sc_list.ProcessCheckout();
        SetMenu(MenuState.Checkout);
    }

    public void NextDay()
    {
        day++;

        if(day < maxDays)
        {
            LevelReset();
        }
        else
        {
            sc_list.UpdateGameEndMenu(score);
            SetMenu(MenuState.End);
        }
        
    }

    public void LevelReset()
    {
        gui_day.text = "Day: " + (day+1);

        sc_aura.Reset();

        go_player.transform.position = playerStartPos;
        go_player.transform.localRotation = Quaternion.Euler(playerStartRot);

        SetMenu(MenuState.Clear);

        sc_list.PickItems();

        sc_spawn.GenerateStock(day);

        int customersToSpawn = sc_spawn.CustomerBulk(day);

        sc_customer.GenerateCustomers(customersToSpawn);
    }

    public void GameReset()
    {
        day = 0;
        score = 0;
        LevelReset();
    }

    public void GameOver()
    {
        sc_list.UpdateGameOverMenu(day+1, score);
        SetMenu(MenuState.GameOver);
    }
        
    void SetMenu(MenuState state)
    {
        //clear all menu states
        
        go_menu_Start.SetActive(false);
        go_menu_GameOver.SetActive(false);
        go_menu_Checkout.SetActive(false);
        go_menu_GameEnd.SetActive(false);


        if(state == MenuState.Clear)
        {
            go_menu.SetActive(false);
            sc_pc.playable = true;
            sc_grab.playable = true;
        }
        else
        {
            go_menu.SetActive(true);
            sc_pc.playable = false;
            sc_grab.playable = false;

            switch(state)
            {
                case MenuState.Start:
                    go_menu_Start.SetActive(true);
                    break;
                case MenuState.GameOver:
                    go_menu_GameOver.SetActive(true);
                    break;
                case MenuState.Checkout:
                    go_menu_Checkout.SetActive(true);
                    break;
                case MenuState.End:
                    go_menu_GameEnd.SetActive(true);
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
    Start,
    Checkout,
    GameOver,
    End
}
