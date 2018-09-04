using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TurretAgent :  Agent {




    public override void CollectObservations()
    {

        AddVectorObs(_rigidbody.angularVelocity.x);
        AddVectorObs(_rigidbody.angularVelocity.y);
    }


    public override void AgentReset()
    {
        
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Vector3 RotationControl = Vector3.zero;
        RotationControl.x = vectorAction[0];
        RotationControl.y = vectorAction[1];

        print(RotationControl);

        _rigidbody.AddTorque(RotationControl, ForceMode.Acceleration);
    }

    private Rigidbody _rigidbody;

    // Use this for initialization
    void Start () {

        _rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
