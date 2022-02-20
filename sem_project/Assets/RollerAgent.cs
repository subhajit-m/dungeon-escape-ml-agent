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

    public GameObject pillar1;
    public GameObject pillar2;
    public GameObject pillar3;
    public GameObject pillar4;


    public int coinsCollected = 0;
    public bool isTouchingWall = false;
    public bool isTouchingPillar = false;
    public float steps = 0;
    public override void OnEpisodeBegin()
    {
        coinsCollected = 0;
        steps = 0;
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
        coin1.transform.localPosition = new Vector3(Random.value * 8, 0.5f, Random.value * 8);
        coin2.transform.localPosition = new Vector3(Random.value * 8, 0.5f, Random.value * 8);
        coin3.transform.localPosition = new Vector3(Random.value * 8, 0.5f, Random.value * 8);
        coin4.transform.localPosition = new Vector3(Random.value * 8, 0.5f, Random.value * 8);
        coin5.transform.localPosition = new Vector3(Random.value * 8, 0.5f, Random.value * 8);

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

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(coin1.transform.localPosition);
        sensor.AddObservation(coin2.transform.localPosition);
        sensor.AddObservation(coin3.transform.localPosition);
        sensor.AddObservation(coin4.transform.localPosition);
        sensor.AddObservation(coin5.transform.localPosition);
        sensor.AddObservation(pillar1.transform.localPosition);
        sensor.AddObservation(pillar2.transform.localPosition);
        sensor.AddObservation(pillar3.transform.localPosition);
        sensor.AddObservation(pillar4.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    //r(x) = r(pilar)+r(wall)+r(coin)+num_coin
    //weights = 2.5 + 2.5 + 80 + 15

    
    public float forceMultiplier = 1;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        steps++;
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        //float distanceToTarget = Vector3.Distance(this.transform.localPosition, coin1.transform.localPosition);

        // Reached target
        //if (distanceToTarget < 1.42f)
        //{
            //SetReward(1.0f);
            //EndEpisode();
        //}

        if (this.transform.localPosition.y < 0){
            EndEpisode();
        }
        if(isTouchingWall){
            //EndEpisode();
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

        /*GameObject[] coins = {coin1, coin2, coin3, coin4, coin5};
        float rewardFromMovingTowardsCoins = 0;
        for(int i = 0; i<coins.Length; i++){
            GameObject coin = coins[i];
            if(coin.activeSelf){
                float distanceToTarget = Vector3.Distance(this.transform.localPosition, coin.transform.localPosition);
                if(distanceToTarget == 0){
                    rewardFromMovingTowardsCoins+=17f;
                }
                else{
                    rewardFromMovingTowardsCoins+= (17f * (1/distanceToTarget));
                }
            }
            else{
                rewardFromMovingTowardsCoins += 17f;
            }
        }*/

//        float totalReward = (rewardFromCoin + rewardFromNotTouchingWall + rewardFromNotTouchingPillar + rewardFromMovingTowardsCoins)/100f;
        float totalReward = (rewardFromCoin)/100f - (steps*0.0005f);
        Debug.Log(totalReward);
        if(totalReward <= -1){
            SetReward(-1.0f);
            EndEpisode();
        }
        if (coinsCollected >= 3)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else{
            SetReward(totalReward);
        }



        
    }

    //https://stackoverflow.com/questions/52338632/make-an-object-disappear-from-another-object-in-unity-c-sharp#:~:text=You%20can%20do%20that%20with,its%20renderer%20by%20using%20objectToDisappear.
    void OnCollisionEnter(Collision col) 
    { 
        if(col.gameObject.name == "coin1" || col.gameObject.name == "coin2" || col.gameObject.name == "coin3" || col.gameObject.name == "coin4" || col.gameObject.name == "coin5"){
            Debug.Log(col.gameObject.name);
            coinsCollected++;
            Debug.Log(coinsCollected);
            col.gameObject.GetComponent<Renderer>().enabled = false;
            col.gameObject.SetActive(false);
        }

        if(col.gameObject.name == "topwall" || col.gameObject.name == "bottomwall" || col.gameObject.name == "leftwall" || col.gameObject.name == "rightwall"){
            isTouchingWall = true;
        }
        else{
            isTouchingWall = false;
        }

        if(col.gameObject.name == "pillar1" || col.gameObject.name == "pillar2" || col.gameObject.name == "pillar3" || col.gameObject.name == "pillar4"){
            isTouchingPillar = true;
        }
        else{
            isTouchingPillar = false;
        }
    } 

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
