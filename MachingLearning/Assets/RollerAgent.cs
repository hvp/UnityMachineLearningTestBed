using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAgent : Agent {

    Rigidbody rBody;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void AgentReset()
    {
        if(transform.position.y < -1.0) // i.e. below floor
        {
            transform.position = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
                
        }
        else
        {
            Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        }
    }

    public override void CollectObservations()
    {
        Vector3 relativePostion = Target.position - transform.position;

        AddVectorObs(relativePostion.x / 5);
        AddVectorObs(relativePostion.z / 5);

        // Distance to edges of platform

        AddVectorObs((this.transform.position.x + 5) / 5);
        AddVectorObs((this.transform.position.x - 5) / 5);
        AddVectorObs((this.transform.position.z + 5) / 5);
        AddVectorObs((this.transform.position.z - 5) / 5);

        // Agent velocity
        AddVectorObs(rBody.velocity.x / 5);
        AddVectorObs(rBody.velocity.z / 5);

    }

    public float speed = 10;
    private float previousDistance = float.MaxValue;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // rewards
        float distanceToTarget = Vector3.Distance(transform.position, Target.position);

        // reached target
        if(distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        // Getting closer
        if(distanceToTarget < previousDistance)
        {
            AddReward(0.1f);
        }

        // time pentaly;
        AddReward(-0.05f);

        // fell
        if(transform.position.y < -1.0f)
        {
            AddReward(-1.0f);
            Done();
        }

        previousDistance = distanceToTarget;

        // actions, size = 2
        Vector3 ControlSignal = Vector3.zero;
        ControlSignal.x = vectorAction[0];
        ControlSignal.z = vectorAction[1];
        rBody.AddForce(ControlSignal * speed);

    }

}
