using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Selector : Composite
{
    int chosenChild = 0; //Begin checking children from leftmost side

    public Selector(string compositeName, params Node[] nodes) : base(compositeName, nodes)
    {

    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        //If the child counter has passed the number of child nodes, then stop processing children
        if (chosenChild >= children.Count)
        {
            return NodeStatus.FAILURE;
        }

        //Get the status of the current child
        NodeStatus currStatus = children[chosenChild].Action(state);

        //If the child has passed successfully, then select the current child node.
        //Otherwise, move on to the next
        switch (currStatus)
        {
            case NodeStatus.SUCCESS:
                return NodeStatus.SUCCESS;

            case NodeStatus.FAILURE:
                chosenChild++;
                return OnAction(state);
        }
        return NodeStatus.RUNNING;
    }

    //On reset set current child to check to 0, ensuring it starts from leftmost again, 
    //and perform reset on all child nodes
    public override void ResetAction()
    {
        chosenChild = 0;
        foreach (Node child in children)
        {
            child.Reset();
        }
    }
}