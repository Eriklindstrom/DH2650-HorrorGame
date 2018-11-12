using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Context for each node, contains variables which can pass between nodes
public class BlackBoard
{
    public Vector3 lastHeardSoundLocation;
    public bool hasRecentlyHeardSound = false;

    public Vector3 lastSeenPlayerLocation;
    public Vector3 lastSeenPlayerDirection;
    public bool hasRecentlySeenPlayer = false;

    public Vector3 currentSearchPosition;
    public Vector3 lastSearchPosition;
    public bool hasSelectedSearchPosition = false;

    public bool hasSelecetedSoundSearchPosition = false;
    public Vector3 currentSoundSearchPosition;
}

//The current status of the node, either still running, returning failure or returning success
//Pass into root node's action, and will pass down to all others
public enum NodeStatus
{
    FAILURE,
    SUCCESS,
    RUNNING
}

//Base class for all nodes
public abstract class Node
{
    public bool isStart = true; //Whether or not the node has just started running

    public abstract NodeStatus OnAction(BlackBoard state); //What the node will do when it is called to process
    public abstract void ResetAction(); //Called on reset, can perform actions on reset here. Must be overrided in each node

    //The action of the node, calls the on action function 
    public virtual NodeStatus Action(BlackBoard state)
    {
        NodeStatus currStatus = OnAction(state);

        isStart = false;

        if (currStatus != NodeStatus.RUNNING)
        {
            Reset();
        }

        return currStatus;
    }

    //Resets the node, starting it again and performing a reset action, calls the resetaction function
    public void Reset()
    {
        isStart = true;
        ResetAction();
    }
}

//class for decorator nodes, the inverter and repeater nodes
public abstract class Decorator : Node
{
    protected Node child; //Decorator nodes can only store one child

    //Constructor, set given node as its child
    public Decorator(Node node)
    {
        child = node;
    }
}

//class for composite nodes, the selector and sequence nodes
public abstract class Composite : Node
{
    protected List<Node> children = new List<Node>(); //Composite nodes can have many children, unlike decorator nodes
    public string compositeName; //The name of the node, helps with managing nodes when creating tree

    //Constructor, give a list of nodes as its child
    public Composite(string name, params Node[] nodes)
    {
        compositeName = name;
        children.AddRange(nodes); //Add all nodes to their children list
    }

    public override NodeStatus Action(BlackBoard state)
    {
        NodeStatus currStatus = base.Action(state);

        return currStatus;
    }
}

//class for leaf nodes, these nodes will perform the actions for the AI
public abstract class Leaf : Node
{
    public AI AIagent; //The AI itself, where the tree is created. Also allows reference to agent actions

    //Constructor, assign an AI agent
    public Leaf(AI aiagent)
    {
        AIagent = aiagent;
    }
}
