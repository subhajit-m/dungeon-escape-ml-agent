using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    //https://answers.unity.com/questions/1659075/how-can-i-stop-cube-from-rolling.html
    void Update()
    {
        this.transform.rotation = Quaternion.identity;
    }

    public GameObject coin1;
    public GameObject coin2;
    public GameObject coin3;
    public GameObject coin4;
    public GameObject coin5;

    public GameObject rightWallReference;

    public GameObject door1;
    public GameObject door2;
    public bool isDoorOpen = false;

    public GameObject dragon;
    public bool dragonCollidedAgent = false;

    public int coinsCollected = 0;
    public float steps = 0;
    public override void OnEpisodeBegin()
    {
        coinsCollected = 0;
        steps = 0;
        dragonCollidedAgent = false;
        isDoorOpen = false;
        // If the Agent fell, zero its momentum
        
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);

        //make them stand on a place because they have been knocked out previously
        coin1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        coin2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        coin3.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        coin4.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        coin5.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        coin1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        coin2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        coin3.GetComponent<Rigidbody>().velocity = Vector3.zero;
        coin4.GetComponent<Rigidbody>().velocity = Vector3.zero;
        coin5.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Move the target to a new spot
        coin1.transform.localPosition = new Vector3((Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10), 0.5f, (Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10));
        coin2.transform.localPosition = new Vector3((Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10), 0.5f, (Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10));
        coin3.transform.localPosition = new Vector3((Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10), 0.5f, (Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10));
        coin4.transform.localPosition = new Vector3((Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10), 0.5f, (Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10));
        coin5.transform.localPosition = new Vector3((Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10), 0.5f, (Random.Range(0f, 1f)<.5f?-1: 1) *Random.value * Random.Range(1, 10));

        //set them active
        coin1.SetActive(true);
        coin2.SetActive(true);
        coin3.SetActive(true);
        coin4.SetActive(true);
        coin5.SetActive(true);

        //show them on scene
        coin1.GetComponent<Renderer>().enabled = true;
        coin2.GetComponent<Renderer>().enabled = true;
        coin3.GetComponent<Renderer>().enabled = true;
        coin4.GetComponent<Renderer>().enabled = true;
        coin5.GetComponent<Renderer>().enabled = true;

        //doors set active
        door1.SetActive(true);
        door2.SetActive(true);

        //render doors
        door1.GetComponent<Renderer>().enabled = true;
        door2.GetComponent<Renderer>().enabled = true;

        GameEvent.current.NewEpisodeBeginAgent();

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(coin1.transform.localPosition.x);
        sensor.AddObservation(coin1.transform.localPosition.z);

        sensor.AddObservation(coin2.transform.localPosition.x);
        sensor.AddObservation(coin2.transform.localPosition.z);
        
        sensor.AddObservation(coin3.transform.localPosition.x);
        sensor.AddObservation(coin3.transform.localPosition.z);
        
        sensor.AddObservation(coin4.transform.localPosition.x);
        sensor.AddObservation(coin4.transform.localPosition.z);

        sensor.AddObservation(coin5.transform.localPosition.x);
        sensor.AddObservation(coin5.transform.localPosition.z);

        sensor.AddObservation(dragon.transform.localPosition.x);
        sensor.AddObservation(dragon.transform.localPosition.z);
        
        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }
    
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        steps++;
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        if (this.transform.localPosition.y < 0){
            SetReward(-1.0f);
            EndEpisode();
            return;
        }
        if(dragonCollidedAgent){
            SetReward(-1.0f);
            EndEpisode();
            return;
        }

        float rewardFromCoin = 0f;

        if(coinsCollected == 0){
            rewardFromCoin = 0f;
        }
        else if(coinsCollected == 1){
            rewardFromCoin = 33f;
        }
        else if(coinsCollected == 2){
            rewardFromCoin = 66f;
        }
        else if (coinsCollected >= 3)
        {
            rewardFromCoin = 100f;
        }


        float totalReward = ((rewardFromCoin)/100f) - (steps*0.0005f);
        //float totalReward = ((rewardFromCoin)/100f);
        //Debug.Log(totalReward);
        if(totalReward <= -1){
            SetReward(-1.0f);
            EndEpisode();
        }
        if (coinsCollected >= 3)
        {
            SetReward(1.0f);
            //EndEpisode();
        }
        else{
            SetReward(totalReward);
        }


        doorOpen();
        if (this.transform.localPosition.x > rightWallReference.transform.localPosition.x + 1f){
            SetReward(1.0f);
            EndEpisode();
            return;
        }
        
    }

    //https://stackoverflow.com/questions/52338632/make-an-object-disappear-from-another-object-in-unity-c-sharp#:~:text=You%20can%20do%20that%20with,its%20renderer%20by%20using%20objectToDisappear.
    void OnCollisionEnter(Collision col) 
    { 
        if(col.gameObject.name == "dragon"){
            dragonCollidedAgent = true;
            //Debug.Log("dragon touched agent");
        }
        else if(col.gameObject.name == "coin1" || col.gameObject.name == "coin2" || col.gameObject.name == "coin3" || col.gameObject.name == "coin4" || col.gameObject.name == "coin5"){
            //Debug.Log(col.gameObject.name);
            coinsCollected++;
            //Debug.Log(coinsCollected);
            col.gameObject.GetComponent<Renderer>().enabled = false;
            col.gameObject.SetActive(false);
        }
    } 
    public void doorOpen()
    {
        if(coinsCollected>=3 && !isDoorOpen){
            int strategy = Random.Range(0f, 1f) < .5f ? 0 : 1;
            if(strategy == 0){
                //open door 1
                door1.gameObject.GetComponent<Renderer>().enabled = false;
                door1.gameObject.SetActive(false);
            }
            else{
                //open door 2
                door2.gameObject.GetComponent<Renderer>().enabled = false;
                door2.gameObject.SetActive(false);
            }
            isDoorOpen = true;
            
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}

