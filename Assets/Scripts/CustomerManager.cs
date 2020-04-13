using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CustomerManager : MonoBehaviour
{
    ///In charge of both spawning customers and managing there movement.

    public float minWaitTime;

    public float maxWaitTime;
    
    public GameObject customerHolder;

    public Vector3 minBounds;

    public Vector3 maxBounds;

    public float maxNavDistance;
    
    public GameObject[] customerPrefabs;
    
    public List<CustomerInfo> spawnedCustomers;

    ItemSpawnManager sc_aisles;

    int nextUniqueKey;
    
    // Start is called before the first frame update
    void Awake()
    {
        sc_aisles = GetComponent<ItemSpawnManager>();

        spawnedCustomers = new List<CustomerInfo>();
    }

    void Start()
    {

    }

    public void GenerateCustomers(int amount)
    {
        nextUniqueKey = 0;

        foreach(CustomerInfo customer in spawnedCustomers)
        {
            customer.Destroy();
            spawnedCustomers.Remove(customer);
        }
        
        ///make existing customers

        for(int i=0; i<amount;i++)
        {
            float xRand = Random.Range(minBounds.x,maxBounds.x);

            float yRand = Random.Range(minBounds.y,maxBounds.y);

            float zRand = Random.Range(minBounds.z,maxBounds.z);

            Vector3 spawnAttempt = new Vector3(xRand,yRand,zRand);

            NavMeshHit nmh = new NavMeshHit();

            if(NavMesh.SamplePosition(spawnAttempt,out nmh,maxNavDistance,NavMesh.AllAreas))
            {
                //determine spawn position
                Vector3 position = new Vector3(nmh.position.x,yRand,nmh.position.z);

                //choose a prefab
                GameObject prefab = customerPrefabs[Random.Range(0,customerPrefabs.Length)];

                //get aisle they will spawn in
                AisleID aID = sc_aisles.GetNearestAisleID(position);

                //set the customer state on start
                CustomerStates state = CustomerStates.Start;

                //Instantiate the GameObject

                GameObject g = (GameObject)Instantiate(prefab, position, Quaternion.identity, customerHolder.transform);

                //Get agent
                NavMeshAgent agent = g.GetComponent<NavMeshAgent>();

                AnimationNavigationMatch script = g.GetComponent<AnimationNavigationMatch>();

                script.SetManager(this);
            
                script.uniqueKey = nextUniqueKey;

                //Finally create a Customerinfo and add it to the list
                spawnedCustomers.Add(new CustomerInfo(nextUniqueKey,g,agent,script,aID,state));

                Finished(nextUniqueKey);

                nextUniqueKey++;
            }
            else
            {
                Debug.LogError("No point found on the NavMesh close enoguh to the randomly chosen position: " + spawnAttempt.ToString());
                i--;
            }
        }

        ///set off a coroutine to have more customers enter
    }

    public void Finished(int key)
    {
        List<CustomerInfo> current = spawnedCustomers.Where(cust => cust.key == key).ToList();

        if(current.Count > 1)
        {
            Debug.LogError("Multiple customers found with the same unique key " + key);
        }
        else if(current.Count == 0)
        {
            Debug.LogError("No customers found with the key " + key);
        }
        else
        {
          //  Debug.Log(current[0].go.name + " is finished!");

          switch(current[0].currentState)
          {
            case CustomerStates.Start:
                changeState(current[0],CustomerStates.Waiting);
                break;
            case CustomerStates.Waiting:
                changeState(current[0],CustomerStates.Searching);
                break;
            case CustomerStates.Searching:
                changeState(current[0],CustomerStates.Perusing);
                break;
            case CustomerStates.Perusing:
                changeState(current[0],CustomerStates.Waiting);
                break;
          }
        }

    }

    void changeState(CustomerInfo customer, CustomerStates newState)
    {
        customer.currentState = newState;

        //Debug.Log(customer.go.name + " has begun " + newState.ToString());

        switch(newState)
        {
            case CustomerStates.Waiting:
                StartCoroutine(CustomerWaiting(customer.key));
                break;
            case CustomerStates.Searching:
                AisleID nextAisle = sc_aisles.GetTravelAisle(customer.GetPosition(),customer.currentAisle,false);
                customer.currentAisle = nextAisle;
                customer.agent.destination = nextAisle.GetNearestNode(customer.GetPosition(), AisleSide.Left | AisleSide.Right);
                break;
            case CustomerStates.Perusing:
                Vector3 target = customer.currentAisle.GetFurthestAisleExit(customer.GetPosition());
                customer.agent.destination = target;
                break;
        }
    }

    IEnumerator CustomerWaiting(int key)
    {
     //   Debug.Log(key + " Start of CR: " + Time.time);

        yield return new WaitForSeconds(Random.Range(minWaitTime,maxWaitTime));

     //   Debug.Log(key + " End of CR: " + Time.time);

        Finished(key);
    }




    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
public class CustomerInfo
{   
    public int key;
    public GameObject go;

    public NavMeshAgent agent;

    public AnimationNavigationMatch script;

    public AisleID currentAisle;

    public CustomerStates currentState;

    public CustomerInfo(int _key, GameObject _go, NavMeshAgent _agent, AnimationNavigationMatch _script, AisleID _currentAisle, CustomerStates _currentState)
    {
        key = _key;
        go = _go;
        agent = _agent;
        script = _script;
        currentAisle = _currentAisle;
        currentState = _currentState;
    }

    public Vector3 GetPosition()
    {
        return go.transform.position;
    }

    public void Destroy()
    {
        Object.Destroy(go);
    }
}

public enum CustomerStates
{
    Start, //only set when customer is new
    Waiting, //waiting for an instruction
    Perusing, //looking at nearby items
    Inspecting, //heading to a specific item and pausing to look at it
    Grabbing, //Grabbing an item
    Searching, //travelling to a new Aisle
    Checkout, //ehading to and using the checkout



}
