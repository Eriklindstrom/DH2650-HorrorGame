using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sequence : Composite
{
    int activeChild = 0; //Begin checking children from leftmost side

    public Sequence( string compositeName, params Node[] nodes) : base(compositeName, nodes)
    {

    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        //Get the status of the current child
        NodeStatus currStatus = children[activeChild].Action(state);

        //If the node is successful, move on to the next, otherwise end the sequence
        switch(currStatus)
        {
            case NodeStatus.SUCCESS:
                activeChild++;
                break;
            case NodeStatus.FAILURE:
                return NodeStatus.FAILURE;
        }

        //If all the children have been checked, return success.
        //On success, immediately move to next
        if (activeChild >= children.Count)
        {
            return NodeStatus.SUCCESS;
        }
        else if(currStatus == NodeStatus.SUCCESS)
        {
            return OnAction(state);
        }

        return NodeStatus.RUNNING;
    }

    //On reset, set the current child to check back to 0, and perform reset action on all children
    public override void ResetAction()
    {
        activeChild = 0;
        foreach(Node child in children)
        {
            child.Reset();
        }
    }
}
