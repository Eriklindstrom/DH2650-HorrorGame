using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This decorator node will ensure that its child is always repeated by
//always returning successfully
public class Repeater : Decorator
{
    public Repeater(Node child) : base(child)
    {

    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        //Get the child node's status
        NodeStatus currStatus = child.Action(state);

        //If the child node is not currently running, then perform the reset on both it and its child
        //Should only occur when starting
        if(currStatus != NodeStatus.RUNNING)
        {
            Reset();
            child.Reset();
        }

        //Always return success to ensure that it is repeated
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
        
    }
}

