using UnityEngine;

public class dragon_walk : MonoBehaviour
{

    private Vector3 target;

    private Rigidbody rb;

    public float walkSpeed = 1;
    private Vector3 dirToGo;

    private int strategy;
    private bool strategy1forward = false;
    private bool strategy2forward = false;

    void strategy1Forward()
    {
        this.transform.localPosition = new Vector3(-7.4f, 0.5f, 0f);
        target = new Vector3(7.4f, 0.5f, 0f);
    }
    void strategy1Reverse()
    {
        this.transform.localPosition = new Vector3(7.4f, 0.5f, 0f);
        target = new Vector3(-7.4f, 0.5f, 0f);
    }

    void strategy2Forward()
    {
        this.transform.localPosition = new Vector3(0f, 0.5f, -7.4f);
        target = new Vector3(0f, 0.5f, 7.4f);
    }
    void strategy2Reverse()
    {
        this.transform.localPosition = new Vector3(0f, 0.5f, 7.4f);
        target = new Vector3(0f, 0.5f, -7.4f);
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        strategy = Random.Range(0f, 1f) < .5f ? 0 : 1;
        //Debug.Log(strategy);
        //strategy = 1;
        if (strategy == 0)
        {
            this.strategy1Forward();
            this.strategy1forward = true;
        }
        else
        {
            this.strategy2Forward();
            this.strategy2forward = true;
        }
        GameEvent.current.onNewEpisodeBeginAgent += onNewEpisode;
    }

    public void onNewEpisode()
    {
        GameEvent.current.onNewEpisodeBeginAgent -= onNewEpisode;
        this.Start();
    }

    // Update is called once per frame
    void Update()
    {

        dirToGo = this.target - this.transform.localPosition;
        dirToGo.y = 0;
        rb.rotation = Quaternion.LookRotation(dirToGo);
        rb.MovePosition(transform.position + transform.forward * walkSpeed * Time.deltaTime);

        float distanceToTarget = Vector3.Distance(this.target, this.transform.localPosition);
        //Debug.Log(distanceToTarget);
        if (distanceToTarget <= 0.3f)
        {
            if (strategy == 0)
            {
                if (this.strategy1forward)
                {
                    this.strategy1Reverse();
                    this.strategy1forward = false;
                }
                else
                {
                    this.strategy1Forward();
                    this.strategy1forward = true;
                }
                
            }
            else
            {
                if (this.strategy2forward)
                {
                    this.strategy2Reverse();
                    this.strategy2forward = false;
                }
                else
                {
                    this.strategy2Forward();
                    this.strategy2forward = true;
                }
            }
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "agent")
        {
            this.onNewEpisode();
        }
    }
}
