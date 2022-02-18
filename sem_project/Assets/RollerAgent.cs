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
    public override void OnEpisodeBegin()
    {
        coinsCollected = 0;
        // If the Agent fell, zero its momentum
        if (coinsCollected == 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

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

    public float forceMultiplier = 5;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Debug.Log("Called actionreceived");
        // Actions, size = 2
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

        if(coinsCollected == 0){
            //SetReward(0.0f);
        }
        else if(coinsCollected == 0){
            SetReward(0.33f);
        }
        else if(coinsCollected == 0){
            SetReward(0.66f);
        }
        else if (coinsCollected >= 3)
        {
            SetReward(1.0f);
            EndEpisode();
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
    } 

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
