using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public GameObject go_player;
    public GameObject go_aura;


    public GameObject go_canvas;
    public GameObject go_menu;

    public Vector3 playerStartPos;
    public Vector3 playerStartRot;

    AuraDetection sc_aura;
    PlayerController_Sonic3 sc_pc;
    ObjectPooler sc_pool;

    ItemSpawnManager sc_spawn;

    float cTime;

    // Start is called before the first frame update
    void Start()
    {
        sc_aura = go_aura.GetComponent<AuraDetection>();
        sc_pc = go_player.GetComponent<PlayerController_Sonic3>();
        sc_pool = GetComponent<ObjectPooler>();
        sc_spawn = GetComponent<ItemSpawnManager>();

        go_canvas.SetActive(true);

        sc_pool.FillPool();

        GameReset();
    }

    public void Checkout()
    {
        go_menu.SetActive(true);

        sc_pc.playable = false;
    }

    public void LevelReset()
    {
        sc_aura.Reset();

        go_player.transform.position = playerStartPos;
        go_player.transform.localRotation = Quaternion.Euler(playerStartRot);

        go_menu.SetActive(false);

        sc_pc.playable = true;

        sc_spawn.GenerateStock(8);

        //reset items!


    }

    public void GameReset()
    {
        LevelReset();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
