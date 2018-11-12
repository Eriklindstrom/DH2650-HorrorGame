using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIdirector : MonoBehaviour
{
    public enum DirectorStates
    {
        STRESSGAIN,
        STRESSFADE,
        MAINTAINHIGHSTRESS,
        MAINTAINLOWSTRESS
    }
    [Tooltip("The current state of the director. Four states exist, stress gain, which will increase stress levels, maintain high stress, which will keep stress at max, stress fade, which lowers the stress level, and maintain low stress, which keeps stress at its lowest. This is best started at the maintain low stress state.")]
    public DirectorStates currentState;
    public static AIdirector sharedAIdirector;

    [Tooltip("The current stress level, which will be changed according to the director's state. This is best started at 0, along with the director's state starting at maintain low stress.")]
    public float currentStressLevel = 0;
    float maxStressLevel = 100;
    float minStressLevel = 0;

    [Tooltip("The rate at which stress is gained. This will be further increased over time, building up quicker the longer stress is being gained. Default: 0.05.")]
    public float stressGainRate = 0.05f;
    [Tooltip("The rate at which stress is lost. This will be further increased over time, dropping quicking the longer stress is being faded. Default -0.05.")]
    public float stressFadeRate = -0.05f;

    [Tooltip("How long the director will maintain max stress once reached. Default: 15.")]
    public float maintainHighStressDuration = 15;
    float maintainHighStressTimer = 0;
    [Tooltip("How long the director will maintain minimum stress once reached. Default 15.")]
    public float maintainLowStressDuration = 15;
    float maintainLowStressTimer = 0;
    float timeInLowStress = 0;
    float timeInHighStress = 0;

    [Tooltip("The distance from the player the AI enemies must be to increase stress. Default: 10.")]
    public float distanceToAIForStressGain = 10;
    [Tooltip("The distance from the player the AI enemies must be to reduce stress.  Default: 20.")]
    public float distanceToAIForStressLoss = 20;

    [HideInInspector]
    public ActivePlayerArea activePlayerArea;

    Player player;
    [Tooltip("All advanced AI types on the level.")]
    public List<AI> allAdvancedAI;
    [Tooltip("All basic AI types on the level.")]
    public List<BasicAI> allBasicAI;
    [Tooltip("All scout AI types on the level.")]
    public List<ScoutAI> allScoutAI;

    [Tooltip("All basic AI groups currently active.")]
    public List<AIGroup> basicAIGroups;

    [Tooltip("All nodes on the level.")]
    public List<WaypointNode> allNodes;
    [Tooltip("The minimum number of nodes which will remain active. Nodes become inactive once they leave the APA, this value will keep a set amount active. If all nodes are inactive the AI will instead navigate to random points within the APA.")]
    public int minNumberOfActiveNodes = 1;

    [Tooltip("Whether or not the director should group Basic AI.")]
    public bool shouldGroupAI = false;

    [HideInInspector]
    public float currentIdleTimeMultiplier = 1.0f;
    float highStressIdleTimeMultiplier = 3.0f;
    float lowStressIdleTimeMultiplier = 1.0f;
    float maintainHighStressIdleTimeMultiplier = 6.0f;

    [HideInInspector]
    public float currentTimeForPlayerDetectionMultiplier = 1.0f;
    float lowStressTimeForPlayerDetectionMultiplier = 1.0f;
    float highStressTimeForPlayerDetectionMultiplier = 5.0f;
    float maintainHighStressTimeForPlayerDetectionMultiplier = 10.0f;

    [HideInInspector]
    public float currentSpeedMultiplier = 1.0f;
    float lowStressSpeedMultiplier = 1.0f;
    float highStressSpeedMultiplier = 1.8f;
    float maintainHighStressSpeedMultiplier = 2.5f;

    float waypointNodeChangeCooldownDuration = 10.0f;
    float waypointNodeChangeCooldownTimer = 0.0f;

    [HideInInspector]
    public float initialBaseWaypointNodeChance;
    int baseNodeWeighting = 1;
    List<KeyValuePair<WaypointNodeType, float>> allActiveNodeTypes = new List<KeyValuePair<WaypointNodeType, float>>();

    [Tooltip("Whether or not the director should spawn enemies. Enemies spawned are considered temporary, and will be removed as stress decreases.")]
    public bool shouldSpawnEnemies = true;

    [Tooltip("Whether or not the director will spawn enemies which are not in the player's sight without checking for obstucted vision. When enabled, AI will spawn behind the player even if there is nothing between them. When disabled, AI will only spawn if there is something blocking vision between them and the player.")]
    public bool spawnEnemiesOutOfSightWithoutObstructionCheck = true;
    int numberOfSpawnsAtCurrentPoint = 0;
    int maxNumberOfSpawnsAtSpawnPoint = 5;
    float findNewSpawnPointTimer = 0.0f;
    float findNewSpawnPointTimerDuration = 5.0f;

    [Tooltip("The target number of AI between 0 and 25 stress.")]
    public int lowStressMaxEnemySpawns = 0;
    [Tooltip("The target number of AI between 25 and 50 stress.")]
    public int mediumStressMaxEnemySpawns = 2;
    [Tooltip("The target number of AI between 50 and 75 stress.")]
    public int highStressMaxEnemySpawns = 5;
    [Tooltip("The target number of AI between 75 and 100 stress.")]
    public int maxStressMaxEnemySpawns = 10;
    [Tooltip("The current number of enemies which have been spawned.")]
    public int currentNumberOfEnemySpawns = 0;
    [Tooltip("The current target number of enemy spawns.")]
    public int currentMaxNumberOfEnemySpawns = 0;
    [Tooltip("The min distance the AI will be able to spawn from the player. Be careful that this is not lower than the APA, otherwise the spawning will be blocked.")]
    public float minSpawnDistanceFromPlayer = 2.0f;
    [Tooltip("The min distance from the player the AI must be in order to be removed.")]
    public float minDespawnDistanceFromPlayer = 20.0f;
    bool hasFoundSpawnPoint = false;
    Vector3 currentSpawnPoint;

    [Tooltip("An object pooler must be attached to the director object when spawning enemies. Set the object to pool as the enemy to spawn. This must be of the Basic AI type.")]
    public ObjectPooler basicAIObjPooler;

    // Use this for initialization
    void Awake()
    {
        sharedAIdirector = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        activePlayerArea = gameObject.GetComponent<ActivePlayerArea>();
        FindAllAI();
        FindAllWaypointNodes();
        currentState = DirectorStates.STRESSGAIN;
        StartCoroutine("DirectorFSM");

        basicAIGroups = new List<AIGroup>();
        basicAIGroups.Add(new AIGroup());
        basicAIGroups[basicAIGroups.Count - 1].groupAvoidancePriority = Random.Range(11, 99);

        waypointNodeChangeCooldownTimer = waypointNodeChangeCooldownDuration;

        GetActiveRoomTypes();
        if (allActiveNodeTypes.Count() > 0)
        {
            initialBaseWaypointNodeChance = 100 / allActiveNodeTypes.Count();
        }

        basicAIObjPooler = gameObject.GetComponent<ObjectPooler>();

        if(shouldSpawnEnemies)
        {
            basicAIObjPooler.amountToPool = maxStressMaxEnemySpawns;
        }
    }

    void Start()
    {
        if (shouldGroupAI)
        {
            PutAIintoGroups();
        }
    }

    // Update is called once per frame
    void Update()
    {   
        CheckAIdistancesToPlayer();
        if (shouldGroupAI)
        {
            UpdateGroupSight();
            SetGroupDestinations();
            CheckIfGroupsNearDestination();

            if (!CheckAllAIAreGrouped())
            {
                //Debug.Log("group again");
                PutAIintoGroups();
            }
        }

        if(currentStressLevel >= 75)
        {
            currentMaxNumberOfEnemySpawns = maxStressMaxEnemySpawns;
        }
        else if(currentStressLevel >= 50)
        {
            currentMaxNumberOfEnemySpawns = highStressMaxEnemySpawns;
        }
        else if(currentStressLevel >= 25)
        {
            currentMaxNumberOfEnemySpawns = mediumStressMaxEnemySpawns;
        }
        else
        {
            currentMaxNumberOfEnemySpawns = lowStressMaxEnemySpawns;
        }

        UpdateNumberOfTempAISpawns();
        if (shouldSpawnEnemies)
        {
            if(hasFoundSpawnPoint)
            {
                findNewSpawnPointTimer += Time.deltaTime;
                if(findNewSpawnPointTimer >= findNewSpawnPointTimerDuration)
                {
                    hasFoundSpawnPoint = false;
                }
            }

            if(currentNumberOfEnemySpawns < currentMaxNumberOfEnemySpawns)
            {
                SpawnBasicAI();
            }
            else if(currentNumberOfEnemySpawns > currentMaxNumberOfEnemySpawns)
            {
                RemoveBasicAISpawn();
            }
        }

        waypointNodeChangeCooldownTimer += Time.deltaTime;
        UpdateNodeChanceValues();
    }

    //Controls the various states of the enemy character
    IEnumerator DirectorFSM()
    {
        while (true)
        {
            switch (currentState)
            {
                case DirectorStates.STRESSGAIN:
                    GainStress();
                    break;
                case DirectorStates.STRESSFADE:
                    FadeStress();
                    break;
                case DirectorStates.MAINTAINHIGHSTRESS:
                    MaintainHighStress();
                    break;
                case DirectorStates.MAINTAINLOWSTRESS:
                    MaintainLowStress();
                    break;
            }
            yield return null;
        }
    }

    void GainStress()
    {
        timeInHighStress += Time.deltaTime;
        currentIdleTimeMultiplier = highStressIdleTimeMultiplier;
        currentTimeForPlayerDetectionMultiplier = highStressTimeForPlayerDetectionMultiplier;
        currentSpeedMultiplier = highStressSpeedMultiplier;

        currentStressLevel += (stressGainRate * timeInHighStress) * Time.deltaTime;
        if (currentStressLevel > 100)
        {
            currentStressLevel = 100;
            timeInHighStress = 0;
            currentState = DirectorStates.MAINTAINHIGHSTRESS;
        }
    }

    void FadeStress()
    {
        timeInLowStress += Time.deltaTime;
        currentIdleTimeMultiplier = lowStressIdleTimeMultiplier;
        currentTimeForPlayerDetectionMultiplier = lowStressTimeForPlayerDetectionMultiplier;
        currentSpeedMultiplier = lowStressSpeedMultiplier;

        currentStressLevel += (stressFadeRate * timeInLowStress) * Time.deltaTime;
        if (currentStressLevel < 0)
        {
            currentStressLevel = 0;
            timeInLowStress = 0;
            currentState = DirectorStates.MAINTAINLOWSTRESS;
        }
    }

    void MaintainHighStress()
    {
        maintainHighStressTimer += Time.deltaTime;
        currentIdleTimeMultiplier = maintainHighStressIdleTimeMultiplier;
        currentTimeForPlayerDetectionMultiplier = maintainHighStressTimeForPlayerDetectionMultiplier;
        currentSpeedMultiplier = maintainHighStressSpeedMultiplier;

        if (maintainHighStressTimer > maintainHighStressDuration)
        {
            maintainHighStressTimer = 0;
            currentState = DirectorStates.STRESSFADE;
        }
    }

    void MaintainLowStress()
    {
        maintainLowStressTimer += Time.deltaTime;
        currentIdleTimeMultiplier = lowStressIdleTimeMultiplier;
        currentTimeForPlayerDetectionMultiplier = lowStressTimeForPlayerDetectionMultiplier;
        currentSpeedMultiplier = lowStressSpeedMultiplier;

        if (maintainLowStressTimer > maintainLowStressDuration)
        {
            maintainLowStressTimer = 0;
            currentState = DirectorStates.STRESSGAIN;
        }
    }

    public float GetCurrentStressLevel()
    {
        return currentStressLevel;
    }

    public void AddStress(float addAmount)
    {
        currentStressLevel += addAmount;
        if (currentStressLevel > maxStressLevel)
        {
            currentStressLevel = maxStressLevel;
        }
    }

    public void RemoveStress(float removeAmount)
    {
        currentStressLevel -= removeAmount;
        if (currentStressLevel < minStressLevel)
        {
            currentStressLevel = minStressLevel;
        }
    }

    void FindAllAI()
    {
        GameObject[] tempBasicAI = GameObject.FindGameObjectsWithTag("BasicAI");
        GameObject[] tempAdvancedAI = GameObject.FindGameObjectsWithTag("AdvancedAI");
        GameObject[] tempScoutAI = GameObject.FindGameObjectsWithTag("ScoutAI");

        foreach (GameObject obj in tempBasicAI)
        {
            if (!allBasicAI.Contains(obj.GetComponent<BasicAI>()))
            {
                allBasicAI.Add(obj.GetComponent<BasicAI>());
            }
        }

        foreach (GameObject obj in tempAdvancedAI)
        {
            if (!allAdvancedAI.Contains(obj.GetComponent<AI>()))
            {
                allAdvancedAI.Add(obj.GetComponent<AI>());
            }
        }

        foreach(GameObject obj in tempScoutAI)
        {
            if(!allScoutAI.Contains(obj.GetComponent<ScoutAI>()))
            {
                allScoutAI.Add(obj.GetComponent<ScoutAI>());
            }
        }
    }

    void FindAllWaypointNodes()
    {
        GameObject[] tempNodes = GameObject.FindGameObjectsWithTag("WaypointNode");

        foreach (GameObject obj in tempNodes)
        {
            if (!allNodes.Contains(obj.GetComponent<WaypointNode>()))
            {
                allNodes.Add(obj.GetComponent<WaypointNode>());
            }
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    void CheckAIdistancesToPlayer()
    {
        foreach (AI advancedAi in allAdvancedAI)
        {
            if (advancedAi.GetComponent<AIactions>().GetDistanceToPlayer() < distanceToAIForStressGain)
            {
                //Debug.Log("ADVANCED close to player");
                AddStress(0.5f * Time.deltaTime);
            }
            else if (advancedAi.GetComponent<AIactions>().GetDistanceToPlayer() > distanceToAIForStressGain * 2)
            {
                //Debug.Log("ADVANCED far from player");
                RemoveStress(0.5f * Time.deltaTime);
            }

            if (advancedAi.GetComponent<AIactions>().playerInSight)
            {
                //Debug.Log("ADVANCED can see player");
                AddStress(1.0f * Time.deltaTime);
            }
        }

        foreach (BasicAI basicAi in allBasicAI)
        {
            if (basicAi.GetComponent<AIactions>().GetDistanceToPlayer() < distanceToAIForStressGain)
            {
                //Debug.Log("BASIC close to player");
                AddStress(0.2f * Time.deltaTime);
            }
            else if (basicAi.GetComponent<AIactions>().GetDistanceToPlayer() > distanceToAIForStressLoss)
            {
                //Debug.Log("BASIC far from player");
                RemoveStress(0.2f * Time.deltaTime);
            }

            if (basicAi.GetComponent<AIactions>().playerInSight)
            {
                //Debug.Log("BASIC can see player");
                AddStress(1.0f * Time.deltaTime);
            }
        }
    }

    public WaypointNode GetActiveWaypointNode()
    {
        List<WaypointNode> activeNodes = new List<WaypointNode>();

        foreach (WaypointNode node in allNodes)
        {
            if(node.isActive && !node.hasVisitor)
            {
                activeNodes.Add(node);
            }
        }

        if(activeNodes.Count == 0)
        {
            return null;
        }
        else
        {
            //float randomNumber = Random.Range(0, 100);
            activeNodes = activeNodes.OrderBy(x => x.currentSelectionPercentChance).ToList();
            float randomNumber = Random.Range(0, 100);

            for (int i = activeNodes.Count - 1; i > -1; i--)
            { 
               if(randomNumber >= activeNodes[i].currentSelectionPercentChance)
                {
                    //Debug.Log("GOT NODE " + activeNodes[i].gameObject.name + randomNumber);
                    return activeNodes[i];
                }
                
            }

            //Debug.Log("GET NODE AT TOP OF LIST " + randomNumber);
            return activeNodes[activeNodes.Count-1];

            //foreach (WaypointNode node in activeNodes)
            //{
            //    float randomNumber = Random.Range(0, 100);
            //    if (randomNumber >= node.currentSelectionPercentChance)
            //    {
            //        Debug.Log("GOT NODE " + node.gameObject.name + randomNumber);
            //        return node;
            //    }
            //}
        }

    }

    public bool GetPointInAPA(out Vector3 resultPosition, float radiusModifier)
    {
        Vector3 randomPosition= Random.insideUnitSphere * (activePlayerArea.radius * radiusModifier);
        randomPosition += GetPlayerPosition();
        UnityEngine.AI.NavMeshHit hit;

        if (UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 5.0f, 1))
        {
            resultPosition = hit.position;
            return true;
        }

        resultPosition = Vector3.zero;

        return false;
    }

    public void PutAIintoGroups()
    {
        foreach(AIGroup group in basicAIGroups)
        {
            group.ResetGroupDestination();
        }

        foreach (BasicAI ai in allBasicAI)
        {
            if (!ai.GetComponent<AIactions>().isAlive)
            {
                continue;
            }

            ai.GetComponent<AIactions>().ResetDestination();
            if (ai.actionScript != null && !ai.actionScript.isGrouped)
            {
                for (int i = 0; i < basicAIGroups.Count; i++)
                {
                    if (basicAIGroups[i].CheckIfFull() == false && !basicAIGroups[i].groupMembers.Contains(ai.gameObject))
                    {
                        if (basicAIGroups[i].groupMembers.Count > 0 && (Vector3.Distance(ai.transform.position, basicAIGroups[i].averageGroupPosition) < 10))
                        {
                            basicAIGroups[i].AddToGroup(ai.gameObject);
                            basicAIGroups[i].GetAveragePositionOfGroupMembers();
                            ai.actionScript.isGrouped = true;
                            //Debug.Log("close to existing group, add this agent " + ai.name);
                        }
                        else if (basicAIGroups[i].groupMembers.Count == 0)
                        {
                            basicAIGroups[i].AddToGroup(ai.gameObject);
                            basicAIGroups[i].GetAveragePositionOfGroupMembers();
                            ai.actionScript.isGrouped = true;
                            //Debug.Log("group empty, add this agent " + ai.name);

                            //FIND OTHER CLOSE AIS AND ADD THEM HERE UNTIL GROUP IS FULL
                            List<GameObject> tempList = new List<GameObject>();
                            tempList.AddRange(GameObject.FindGameObjectsWithTag("BasicAI"));
                            tempList = tempList.OrderBy(x => Vector3.Distance(ai.gameObject.transform.position, x.transform.position)).ToList();
                            foreach (GameObject obj in tempList)
                            {
                                if (basicAIGroups[i].CheckIfFull() == false && obj.GetComponent<AIactions>().isGrouped == false && (Vector3.Distance(ai.transform.position, obj.transform.position) < 10))
                                {
                                    basicAIGroups[i].GetAveragePositionOfGroupMembers();
                                    basicAIGroups[i].AddToGroup(obj);
                                    obj.GetComponent<AIactions>().isGrouped = true;
                                    //Debug.Log("ADD NEARBY AI TO GROUP " + obj.name);
                                }
                            }
                        }
                    }
                }
            }

            if (ai.actionScript != null && !ai.actionScript.isGrouped)
            {
                basicAIGroups.Add(new AIGroup());
                basicAIGroups[basicAIGroups.Count - 1].groupAvoidancePriority = Random.Range(11, 99);
                //Debug.Log("MAKE NEW GROUP");
            }
        }
    }

    public bool CheckAllAIAreGrouped()
    {
        bool allAreGrouped = true;
        foreach (BasicAI ai in allBasicAI)
        {

            if (ai.actionScript != null && ai.actionScript.isGrouped == false)
            {
                allAreGrouped = false;
            }
        }
        return allAreGrouped;
    }

    public void SetGroupDestinations()
    {
        foreach (AIGroup group in basicAIGroups)
        {
            if (!group.CanAGroupMemberSeePlayer())
            {
                if (!group.hasGotDestination)
                {
                    group.GetNewGroupDestination();
                }
                else
                {
                    group.SetMembersGroupDestination();
                }
            }
        }
    }

    public void CheckIfGroupsNearDestination()
    {
        foreach (AIGroup group in basicAIGroups)
        {
            if (group.hasGotNode)
            {
                if (Vector3.Distance(group.GetAveragePositionOfGroupMembers(), group.currentNode.transform.position) <= 3)
                {
                    //Debug.Log("group is near destination");
                    group.ResetGroupDestination();
                    group.GetNewGroupDestination();
                }
            }
            else if (group.hasGotDestination)
            {
                if (Vector3.Distance(group.GetAveragePositionOfGroupMembers(), group.currentDestination) <= 3)
                {
                    //Debug.Log("group is near destination");
                    group.ResetGroupDestination();
                    group.GetNewGroupDestination();
                }
            }

            if (group.CheckIfDestinationIsOutsideAPA())
            {
                //Debug.Log("Group destination outside APA - Reset");
                group.ResetGroupDestination();
                group.GetNewGroupDestination();
            }
        }
    }

    public void UpdateGroupSight()
    {
        foreach (AIGroup group in basicAIGroups)
        {
            if (group.CanAGroupMemberSeePlayer())
            {
                group.ResetGroupDestination();
                group.SetAllGroupMembersVision();

                if(!group.hasSetAllFlankers())
                {
                    group.SetFlankers(player.transform.position);
                }
            }
            else if(!group.CanAGroupMemberSeePlayer())
            {
                if(group.hasSetSomeFlankers())
                {
                    group.ResetGroupFlankers();
                }
            }

            //if(group.CanAGroupMemberHearSound())
            //{
            //    group.SetAllGroupMembersSoundHeard();
            //}
        }
    }

    public void PrintMemberNames()
    {
        foreach (AIGroup group in basicAIGroups)
        {
            List<GameObject> tempList = new List<GameObject>();
            tempList = group.GetAllMembers();
            foreach (GameObject obj in tempList)
            {
                Debug.Log("member " + obj.name);
            }
        }
    }

    public WaypointNode GetClosestNodeToPosition(Vector3 position)
    {
        List<WaypointNode> tempList = new List<WaypointNode>();

        foreach(WaypointNode node in allNodes)
        {
            if(Vector3.Distance(node.transform.position, position) < 20)
            {
                tempList.Add(node);
            }
        }

        tempList = tempList.OrderBy(x => Vector3.Distance(position, x.transform.position)).ToList();

        if (tempList.Count > 0)
        {
            return tempList[0];
        }

        return null;
    }

    //Increase the weighting of the selected node type, and decrease the weighting of the other node types.
    //This will increase the percent chance of the selected node types.
    public void IncreaseNodeTypeWeighting(WaypointNodeType nodeType)
    {
        for(int i = 0;i<allActiveNodeTypes.Count();i++)
        {
            if(allActiveNodeTypes[i].Key == nodeType)
            {
                KeyValuePair<WaypointNodeType, float> oldKvp = allActiveNodeTypes[i];
                float newValue = oldKvp.Value;
                newValue += 1;
                allActiveNodeTypes[i] = new KeyValuePair<WaypointNodeType, float>(oldKvp.Key, newValue);
            }
            else 
            {
                KeyValuePair<WaypointNodeType, float> oldKvp = allActiveNodeTypes[i];
                if (oldKvp.Value > 2)
                {
                    float newValue = oldKvp.Value;
                    newValue -= 1;
                    allActiveNodeTypes[i] = new KeyValuePair<WaypointNodeType, float>(oldKvp.Key, newValue);
                }
            }
        }
    }

    //Return weightings for each node type to 1, reseting their chances.
    void ResetNodeTypeWeighting()
    {
        for (int i = 0; i < allActiveNodeTypes.Count(); i++)
        {        
            KeyValuePair<WaypointNodeType, float> oldKvp = allActiveNodeTypes[i];
            float newValue = oldKvp.Value;
            newValue = 1;
            allActiveNodeTypes[i] = new KeyValuePair<WaypointNodeType, float>(oldKvp.Key, newValue);         
        }
    }

    //Use a timer to limit how many nodes can have their chances changed within a certain timespan.
    //This prevents nodes having their values changed to often, such as when several groups of enemies spot the player at the same location.
    public bool CheckNodeChangeCooldownTimer()
    {
        if(waypointNodeChangeCooldownTimer >= waypointNodeChangeCooldownDuration)
        {
            waypointNodeChangeCooldownTimer = 0.0f;
            return true;
        }
        return false;
    }

    //Get all node types which are placed on the level.
    void GetActiveRoomTypes()
    {
        foreach(WaypointNode node in allNodes)
        {
            if(!allActiveNodeTypes.Contains(new KeyValuePair<WaypointNodeType, float>(node.roomType, baseNodeWeighting)))
            {
                KeyValuePair<WaypointNodeType, float> temp = new KeyValuePair<WaypointNodeType, float>(node.roomType, baseNodeWeighting);
                allActiveNodeTypes.Add(temp);
            }
        }
    }

    //Upate each node's percent chance value. This is calculated by dividing 100 by the total number of weights and multiplying the node's 
    //by their own weighting. This distributes 100% across all the nodes, increasing their amount by their weighting.
    void UpdateNodeChanceValues()
    {
        float totalWeights = 0;
        foreach (KeyValuePair<WaypointNodeType, float> kvp in allActiveNodeTypes)
        {
            totalWeights += kvp.Value;
        } 

        foreach (WaypointNode node in allNodes)
        {
            foreach (KeyValuePair<WaypointNodeType, float> kvp in allActiveNodeTypes)
            {
                if (node.roomType == kvp.Key)
                {
                    node.baseSelectionPercentChance = (100 / totalWeights) * kvp.Value;
                }
            }
        }
    }

    public bool GetSpawnPosition(out Vector3 spawnPos, float radiusModifier)
    {
        Vector3 tempPosition;
        //Get random point in APA, modified by multiplier
        if(GetPointInAPA(out tempPosition, radiusModifier))
        {
            //First check if this point is far away enough from the player
            if (Vector3.Distance(tempPosition, player.transform.position) >= minSpawnDistanceFromPlayer)
            {
                //Check if point is obstructed from player's view
                bool pointObstructedFromView = false;
                int ignoreAIlayerMask = 1 << 9;
                ignoreAIlayerMask = ~ignoreAIlayerMask;
                if (Physics.Linecast(player.transform.position, new Vector3(tempPosition.x, 1.0f, tempPosition.z), ignoreAIlayerMask))
                {
                    pointObstructedFromView = true;
                }

                if ((!spawnEnemiesOutOfSightWithoutObstructionCheck && pointObstructedFromView) || (spawnEnemiesOutOfSightWithoutObstructionCheck && InfiniteCameraCanSeePoint(Camera.main, tempPosition) && pointObstructedFromView) || (spawnEnemiesOutOfSightWithoutObstructionCheck && !InfiniteCameraCanSeePoint(Camera.main, tempPosition)))
                {
                    //Once a point has been found, check if that point can path to the player's current location to ensure that it is valid
                    UnityEngine.AI.NavMeshPath tempPath = new UnityEngine.AI.NavMeshPath();
                    if (UnityEngine.AI.NavMesh.CalculatePath(tempPosition, new Vector3(player.transform.position.x, 0.0f, player.transform.position.z), UnityEngine.AI.NavMesh.AllAreas, tempPath))
                    {
                        if (tempPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                        {
                            spawnPos = tempPosition;
                            return true;
                        }
                    }
                }
            }
        }

        spawnPos = Vector3.zero;
        return false;
    }

    void CreateNewBasicAI(Vector3 spawnPos)
    {
        GameObject newBasciAI = basicAIObjPooler.GetPooledObject();
        if(newBasciAI != null)
        {
            newBasciAI.transform.position = spawnPos;
            newBasciAI.transform.rotation = Quaternion.identity;
            newBasciAI.SetActive(true);
            newBasciAI.GetComponent<AIactions>().isTempSpawn = true;
            newBasciAI.GetComponent<AIactions>().currentHealth = newBasciAI.GetComponent<AIactions>().maxHealth;
            newBasciAI.GetComponent<AIactions>().isAlive = true;
            newBasciAI.GetComponent<BasicAI>().RestartFSM();
            allBasicAI.Add(newBasciAI.GetComponent<BasicAI>());
        }
    }

    void SpawnBasicAI()
    {
        if (hasFoundSpawnPoint)
        {
            //Debug.Log("Spawn");
            Vector3 tempSpawnPoint = (Random.insideUnitSphere * 2) + currentSpawnPoint;

            //Select random area around spawn point
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(tempSpawnPoint, out hit, 2.0f, 1))
            {
                //Check if point is obstructed from player's view
                bool pointObstructedFromView = false;
                int ignoreAIlayerMask = 1 << 9;
                ignoreAIlayerMask = ~ignoreAIlayerMask;
                if (Physics.Linecast(player.transform.position, new Vector3(hit.position.x, 1.0f, hit.position.z), ignoreAIlayerMask))
                {
                    pointObstructedFromView = true;
                }

                if ((!spawnEnemiesOutOfSightWithoutObstructionCheck && pointObstructedFromView) || (spawnEnemiesOutOfSightWithoutObstructionCheck && InfiniteCameraCanSeePoint(Camera.main, hit.position) && pointObstructedFromView) || (spawnEnemiesOutOfSightWithoutObstructionCheck && !InfiniteCameraCanSeePoint(Camera.main, hit.position)))
                {
                    //Ensure that there is a path between selected position and the player
                    UnityEngine.AI.NavMeshPath tempPath = new UnityEngine.AI.NavMeshPath();
                    if (UnityEngine.AI.NavMesh.CalculatePath(hit.position, new Vector3(player.transform.position.x, 0.0f, player.transform.position.z), UnityEngine.AI.NavMesh.AllAreas, tempPath))
                    {
                        //Path is valid, so spawn AI
                        if (tempPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                        {
                            CreateNewBasicAI(hit.position);

                            numberOfSpawnsAtCurrentPoint++;
                            if (numberOfSpawnsAtCurrentPoint >= maxNumberOfSpawnsAtSpawnPoint)
                            {
                                hasFoundSpawnPoint = false;
                                numberOfSpawnsAtCurrentPoint = 0;
                            }

                        }
                    }
                }            
            }
        }
        else
        {
            if (GetSpawnPosition(out currentSpawnPoint, 1.5f))
            {
                hasFoundSpawnPoint = true;
                findNewSpawnPointTimer = 0.0f;
            }
        }
    }

    bool InfiniteCameraCanSeePoint(Camera camera, Vector3 point)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }

    void UpdateNumberOfTempAISpawns()
    {
        currentNumberOfEnemySpawns = 0;
        foreach(BasicAI ai in allBasicAI)
        {
            if(ai.actionScript.isTempSpawn)
            {
                currentNumberOfEnemySpawns++;
            }
        }
    }

    void RemoveBasicAISpawn()
    {
        for(int i = 0; i<allBasicAI.Count(); i++)
        {
            if(allBasicAI[i].actionScript.isTempSpawn)
            {
                int layerMask = 1 << 9;
                layerMask = ~layerMask;

                //If AI cannot be seen by player, and are far enough away from the player, despawn them
                if (Physics.Linecast(player.transform.position, new Vector3(allBasicAI[i].transform.position.x, allBasicAI[i].transform.position.y + 1.0f, allBasicAI[i].transform.position.z),layerMask) && Vector3.Distance(player.transform.position, allBasicAI[i].transform.position) >= minDespawnDistanceFromPlayer)
                {
                    DestroyBasicAI(allBasicAI[i]);
                    return;
                }
            }
        }
    }

    public void RemoveFromGroup(BasicAI ai)
    {
        foreach (AIGroup group in basicAIGroups)
        {
            if (group.groupMembers.Contains(ai.gameObject))
            {
                group.groupMembers.Remove(ai.gameObject);
            }
        }
    }

    public void DestroyBasicAI(BasicAI ai)
    {
        if(allBasicAI.Contains(ai))
        {
            allBasicAI.Remove(ai);
        }

        RemoveFromGroup(ai);

        //foreach (BasicAI basicAI in allBasicAI)
        //{
        //    if(basicAI.actionScript.alliesInSight.Contains(ai.gameObject))
        //    {
        //        basicAI.actionScript.alliesInSight.Remove(ai.gameObject);
        //    }
        //}

        //foreach(AI advancedAI in allAdvancedAI)
        //{
        //    if(advancedAI.actionScript.alliesInSight.Contains(ai.gameObject))
        //    {
        //        advancedAI.actionScript.alliesInSight.Remove(ai.gameObject);
        //    }
        //}

        GameObject[] tempDoorList = GameObject.FindGameObjectsWithTag("Door");
        for(int i = 0; i < tempDoorList.Count();i++)
        {
            if(tempDoorList[i].GetComponent<Door>().allNearbyAI.Contains(ai.gameObject))
            {
                tempDoorList[i].GetComponent<Door>().allNearbyAI.Remove(ai.gameObject);
            }
        }

        ai.actionScript.playerInSight = false;
        ai.gameObject.SetActive(false);
    }

    public void DestroyAdvancedAI(AI ai)
    {
        if (allAdvancedAI.Contains(ai))
        {
            allAdvancedAI.Remove(ai);
        }

        //foreach (BasicAI basicAI in allBasicAI)
        //{
        //    if (basicAI.actionScript.alliesInSight.Contains(ai.gameObject))
        //    {
        //        basicAI.actionScript.alliesInSight.Remove(ai.gameObject);
        //    }
        //}

        //foreach (AI advancedAI in allAdvancedAI)
        //{
        //    if (advancedAI.actionScript.alliesInSight.Contains(ai.gameObject))
        //    {
        //        advancedAI.actionScript.alliesInSight.Remove(ai.gameObject);
        //    }
        //}

        GameObject[] tempDoorList = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < tempDoorList.Count(); i++)
        {
            if (tempDoorList[i].GetComponent<Door>().allNearbyAI.Contains(ai.gameObject))
            {
                tempDoorList[i].GetComponent<Door>().allNearbyAI.Remove(ai.gameObject);
            }
        }

        ai.actionScript.playerInSight = false;
        ai.gameObject.SetActive(false);
    }

    public void DestroyScoutAI(ScoutAI ai)
    {
        if (allScoutAI.Contains(ai))
        {
            allScoutAI.Remove(ai);
        }

        //foreach (BasicAI basicAI in allBasicAI)
        //{
        //    if (basicAI.actionScript.alliesInSight.Contains(ai.gameObject))
        //    {
        //        basicAI.actionScript.alliesInSight.Remove(ai.gameObject);
        //    }
        //}

        //foreach (AI advancedAI in allAdvancedAI)
        //{
        //    if (advancedAI.actionScript.alliesInSight.Contains(ai.gameObject))
        //    {
        //        advancedAI.actionScript.alliesInSight.Remove(ai.gameObject);
        //    }
        //}

        GameObject[] tempDoorList = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < tempDoorList.Count(); i++)
        {
            if (tempDoorList[i].GetComponent<Door>().allNearbyAI.Contains(ai.gameObject))
            {
                tempDoorList[i].GetComponent<Door>().allNearbyAI.Remove(ai.gameObject);
            }
        }

        ai.actionScript.playerInSight = false;
        ai.gameObject.SetActive(false);
    }
}
