using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This decorator node will invert the result of its child.
//For example, if its child returns success, this node will instead return failure to its parent.
public class Inverter : Decorator
{
    public Inverter(Node child) : base(child)
    {

    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        //Get the state of the child, if it is running, continue until it has finished.
        //If it has returned success, then instead return failure.
        //If it has returned failure, then instead return success
        switch(child.Action(state))
        {
            case NodeStatus.RUNNING:
                return NodeStatus.RUNNING;
            case NodeStatus.SUCCESS:
                return NodeStatus.FAILURE;
            case NodeStatus.FAILURE:
                return NodeStatus.SUCCESS;
        }

        Debug.Log("SHOULD NOT GET HERE");
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
        
    }
}