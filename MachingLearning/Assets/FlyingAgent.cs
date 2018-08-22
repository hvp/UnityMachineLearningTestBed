using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FlyingAgent : Agent
{


    Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public Transform Target;

    public override void AgentReset()
    {
        if (!_isInsideArena) // Agent flew out of the arena
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;


            _isInsideArena = true;
        }
        else
        {
            Target.position = new Vector3(Random.value * 8 - 4, Random.Range(0.5f, 9.5f), Random.value * 8 - 4);
        }



    }
    public override void CollectObservations()
    {
        Vector3 relativePostion = Target.position - transform.position;

        AddVectorObs(relativePostion.x / 5);
        AddVectorObs(relativePostion.z / 5);
        AddVectorObs(relativePostion.y / 5);

        // Distance to edges of platform

        AddVectorObs((this.transform.position.x + 5) / 5);
        AddVectorObs((this.transform.position.x - 5) / 5);
        AddVectorObs((this.transform.position.z + 5) / 5);
        AddVectorObs((this.transform.position.z - 5) / 5);
        AddVectorObs((this.transform.position.y + 5) / 5);
        AddVectorObs((this.transform.position.y - 5) / 5);

        // Agent velocity
        AddVectorObs(rigidbody.velocity.x / 5);
        AddVectorObs(rigidbody.velocity.z / 5);
        AddVectorObs(rigidbody.velocity.y / 5);

        // Agent rotation
        AddVectorObs(rigidbody.angularVelocity.x);
        AddVectorObs(rigidbody.angularVelocity.y);
    }

    public float speed = 100;
    public float rotSpeed = 0.5f;
    private float previousDistance = float.MaxValue;
    private float previousDirection = float.MaxValue;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // rewards
        float distanceToTarget = Vector3.Distance(transform.position, Target.position);
        float directionToTarget = Vector3.Dot(transform.forward, (Target.position - transform.position).normalized);
       

        // time pentaly;
        AddReward(-0.05f);

        // fell
        if (!_isInsideArena)
        {
            AddReward(-1.0f);
            Done();
        }

        // Getting closer
        if (distanceToTarget < previousDistance)
        {
            AddReward(0.1f);
        }

        previousDistance = distanceToTarget;

        if(directionToTarget > previousDirection)
        {
            AddReward(0.1f);
        }

        previousDirection = directionToTarget;

        // actions, size = 6

        // Movement
        Vector3 ControlSignal = Vector3.zero;
        ControlSignal.x = vectorAction[0];
        ControlSignal.z = vectorAction[1];

        // altitude 
        ControlSignal.y = vectorAction[2] + ( -1 * vectorAction[6]); 

        rigidbody.AddForce(ControlSignal * speed);

        // Rotation 
        Vector3 RotationControlSignal = Vector3.zero;
        RotationControlSignal.x = vectorAction[3];
        RotationControlSignal.y = vectorAction[4];

        rigidbody.AddTorque(RotationControlSignal * rotSpeed, ForceMode.Force);

        // Shooting
        if (vectorAction[5] > 0.1f) // not sure what to do here....
            FireBullet();
    }

    private bool _isInsideArena = true;

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Arena")
        {
            _isInsideArena = false;
        }
    }

    public LayerMask ProjectileLayerMask;
    public LineRenderer Linerenderer;
    public void FireBullet()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.green, 0.1f);
        Linerenderer.SetPosition(0, ray.origin);
        Linerenderer.SetPosition(1, ray.direction * 10);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, 10, ProjectileLayerMask))
        {
          
            if (raycastHit.transform.gameObject.name == "Target")
            {
                AddReward(1.0f);
              
                Done();
            }

        }


    }

}
