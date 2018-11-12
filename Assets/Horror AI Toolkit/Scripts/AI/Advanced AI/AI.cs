using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    //Contains AI actions
    public AIactions actionScript;

    Node behaviourTree; //root node
    public BlackBoard currentState = new BlackBoard(); //blackboard to pass into tree

    //public Text statusText; //Text to display over agent, displays their current behaviour

	void Start ()
    {
        actionScript = this.gameObject.GetComponent<AIactions>();
        behaviourTree = CreateBehaviourTree();
        currentState = new BlackBoard();
        //statusText = this.gameObject.transform.GetChild(3).GetChild(0).GetComponent<Text>();
    }

	void FixedUpdate ()
    {
        behaviourTree.Action(currentState); //execute the behaviour tree
    }

    //Set up and create the behaviour tree. Returns a single repeater node.
    //Process the action of this node to begin looping through the tree
    Node CreateBehaviourTree()
    {
        Sequence AmIDead = new Sequence("ImDead", new CheckIsDead(this));

        Sequence AmIStunned = new Sequence("ImStunned", new CheckIsStunned(this));

        Sequence CanPlayerSeeMe = new Sequence("CanPlayerSeeMe", new CheckCanOnlyMoveIfPlayerCantSee(this));

        Sequence CanSeePlayer = new Sequence("CanSeePlayer", new CheckCanSeePlayer(this),
            new ChasePlayer(this));

        Sequence CanHearSound = new Sequence("CanHearSound", new Inverter(new CheckCanSeePlayer(this)), new CheckCanHearSound(this) ,new CheckIfCanSeeLastHeardPosition(this), new Inverter(new LookAtLastHeardPosition(this)));

        Sequence IsCheckingHideInObject = new Sequence("IsCheckingHideInObject", new Selector("HasSelectedHideInObject", 
            new Sequence("HasSelectedHideInObject", new CheckIfCheckingHideInObject(this),
                new Selector("CloseOrFar",
                    new Sequence("IsCloseToHideInObject", new CheckIfCloseToHideInObject(this), new OpenHideInObject(this)),
                    new Sequence("MoveToHideInObject", new MoveToHideInObject(this)))),
            new Sequence("HasNotSelectedHideInObject", new SelectRandomHideIn(this))));

        Sequence SearchLastHeardPosition = new Sequence("SearchLastHeardPosition", new CheckHasRecentlyHeardSound(this),
            new Selector("CloseOrFar",
                new Sequence("IsCloseToSearchPos", new CheckIfCloseToLastHeardPosition(this), new CheckSoundSearchTimer(this)),
                new Sequence("IsFarFromSearchPos", new MoveToLastHeardPostion(this))));

        Sequence SearchLastSeenPosition = new Sequence("SearchLastSeenPosition",new Inverter(new CheckHasRecentlyHeardSound(this)), new CheckHasRecentlySeenPlayer(this),
            new Selector("CloseOrFar",
                new Sequence("IsCloseToSearchPos", new CheckIfCloseToLastSeenPosition(this), new CheckSearchTimer(this)),
                new Sequence("MoveToRandomSearchPos", new IncrementCheatSearchTimer(this), new Selector("CheckHasSelectedRandomPos",
                    new Sequence("HasNotSelectedRandomPos", new Inverter(new CheckHasSelectedSearchPos(this)), new Inverter(new SelectRandomPointNearLastSeen(this))),
                    new Sequence("HasSelectedRandomPos", new CheckHasSelectedSearchPos(this), new Selector("closeOrFar",new CheckIfCloseToSearchPosition(this),
                    new Selector("moveOrIdle", 
                        new Sequence("isIdle", new idleCurrentPosition(this)), 
                        new Sequence("moveToSearch",  new MoveToCurrentSearchPostion(this), new CheckShouldIlde(this)))))))));

        Sequence Patrol = new Sequence("Patrol", new Inverter(new CheckHasRecentlyHeardSound(this)), new Inverter(new CheckCanHearSound(this)), new Inverter(new CheckCanSeePlayer(this)),new CheckShouldIlde(this),
            new Selector("idleOrPatrol", 
                new Sequence("isIdle", new idleCurrentPosition(this)), 
                new Sequence("patrol", new RandomPatrol(this))));

        //The first node, will select one of the above three nodes to enter. Priority left to right. 
        Sequence mainAILoop = new Sequence("", new Inverter(new CheckIsDead(this)), new Inverter(AmIStunned), CanPlayerSeeMe, new Selector("mainAILoop", CanSeePlayer, CanHearSound, IsCheckingHideInObject, SearchLastHeardPosition, SearchLastSeenPosition, Patrol));

        //The root node, will repeat the tree.
        Repeater repeater = new Repeater(mainAILoop);

        //Return the root node of the tree, process this node's action to begin tree execution
        return repeater;
    }
}
