using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingSteeringAgent : AbstractSteeringAgent
{
    float offset = 1.0f;
    Vector3 desired;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // TODO Add your solution here. Feel free to add your own variables or helper methods.
        //      By accessing "Position" property, you can retrieve the world position of the agent.
        //      You can use "LookDirection" property to retrieve the direction in which the agent is facing.
        //      To move the agent, modify the "Velocity" property storing the direction in which the agent should go,
        //      whereas its magnitude should be equal to agent's speed per second (use "maxSpeed" variable).
        //      You can use "SetRotationImmediate" and "SetRotationTransition" functions to set the rotation / "LookDirection" of the agent.
        //
        //      Example code:
        //      SetRotationTransition(Vector3.right); // SetRotationImmediate(Vector3.right)
        //      Velocity = LookDirection * maxSpeed;
        if(WillColide() == true)
        {
            SetRotationTransition(desired);
        }
        else
        {
            float rotace = (Random.value - Random.value)*Mathf.PI * 2.0f;
            Vector3 rotaceVector = new Vector3(Mathf.Sin(rotace), 0.0f, Mathf.Cos(rotace)) * 0.02f;
            rotaceVector += LookDirection;
            rotaceVector.Normalize();
            SetRotationImmediate(rotaceVector);
            ///If the solution is too much twitchy for you then I have second option. Just comment lines above (Vector3 rotaceVector to SetRotationImmediate)
            /*Vector3 rotaceVector = new Vector3(Mathf.Sin(rotace), 0.0f, Mathf.Cos(rotace));
            rotaceVector += LookDirection;
            rotaceVector.Normalize();
            SetRotationTransition(rotaceVector);*/
            
        }
        Velocity = LookDirection * maxSpeed;
    }

    protected bool WillColide()
    {
        if (Mathf.Abs(Position.x) + offset >= 5.0f || Mathf.Abs(Position.z) + offset >= 5.0f)
        {
            desired = new Vector3(0.0f, 0.0f, 0.0f) - Position;
            return true;
        }
        return false;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
}
