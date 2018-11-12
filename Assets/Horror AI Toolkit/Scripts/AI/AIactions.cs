using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIactions : MonoBehaviour
{
    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent agent;
    [HideInInspector]
    public UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter character;

    [HideInInspector]
    public GameObject player;

    [Tooltip("If the AI is currently alive or not.")]
    public bool isAlive = true;

    [Tooltip("Wether or not the AI will respawn on death. If enabled, they will respawn within the APA after a set amount of time. If disabled, the AI will be permentantly removed from the game.")]
    public bool respawns = false;

    [Tooltip("The length of time before the AI will respawn.")]
    public float respawnTimerDuration = 15.0f;
    float respawnTimer = 0.0f;


    [Tooltip("The field of view of this AI's vision. Vision is only calculated once the player enters this AI's trigger collider.")]
    public float fieldOfViewAngle = 140f;
    [HideInInspector]
    public bool playerInSight;
    [HideInInspector]
    public bool soundIsHeard;
    [Tooltip("Whether or not the AI will use a detailed vision detection. This will target different points on the player, rather than just their center. This will impact framerate if used on too many AI at once.")]
    public bool shouldUseDetailedVision = false;

    [Tooltip("Whether or not the AI can hear sounds. If disabled they will ignore all sound emitters.")]
    public bool canHearSounds = true;

    [HideInInspector]
    public soundEmitter lastHeardSoundEmitter;
    [Tooltip("The sound emitters which this AI has searched. They will not search any emitters in this list again. The player type will never be included in this list.")]
    public List<soundEmitter> checkedSoundEmitters = new List<soundEmitter>();
    [Tooltip("The sound emitter types which this AI is ignoring. Once the AI has searched too many sound emitters of the same type they will ignore all sounds of that type in the future.")]
    public List<SoundType> ignoreSoundTypes = new List<SoundType>();
    [Tooltip("The number of times the AI must search emitters of the same type before they begin ignoring them.")]
    public int numberOfSoundChecksBeforeIgnore = 2;


    [HideInInspector]
    public bool isIdle = false;
    [Tooltip("Whether or not the AI should randomly idle during patrol. This will cause them to stop and look around.")]
    public bool shouldRandomlyIdle = true;

    [HideInInspector]
    public List<HideInObject> hideInObjectsInSight = new List<HideInObject>();
    [HideInInspector]
    public HideInObject hideInObjectToCheck;

    [HideInInspector]
    public List<GameObject> alliesInSight = new List<GameObject>();

    //Patrol variables
    [Tooltip("The patrol movement speed of the AI.")]
    public float patrolSpeed = 0.5f;
    float turnSpeed = 3.0f;

    //Chase variables
    [Tooltip("The chase movement speed of the AI.")]
    public float chaseSpeed = 1.1f;
    [Tooltip("The flank movement speed of the AI. This is only used when AI are grouped and flankers are selected. Set this either the same as the chase speed, or slightly higher.")]
    public float flankSpeed = 1.15f;

    //Search variables
    [Tooltip("The search movement speed of the AI.")]
    public float searchSpeed = 0.8f;
    float searchTimer = 0.0f;
    [Tooltip("The length of time the AI will search for after seeing the player. Default: 5.0.")]
    public float searchTimerDuration = 5.0f;
    float searchSoundTimer = 0.0f;
    [Tooltip("The length of time the AI will search a sound for. This is the time required for the emitter to be added to the ignore list. Default: 5.0.")]
    public float searchSoundTimerDuration = 5.0f;

    float lookAtTimer = 0.0f;
    float lookAtTimerDuration = 2.0f;

    [HideInInspector]
    public float closeToPositionRange = 0.5f;

    [Tooltip("The range in which the AI will search around after seeing the player. Default: 10.0.")]
    public float searchRange = 10.0f;
    [Tooltip("The range the AI must be in to begin searching a sound emitter. Default: 5.0.")]
    public float searchSoundRange = 5.0f;

    bool isInCrouchArea;

    bool isPlayerTooClose;
    float playerTooCloseRange = 3.0f;

    float chanceToOpenHideInObject = 0.001f;
    float chanceToOpenHideInObjectPlayerIsIn = 0.005f;
    float openHideInTimer = 0.0f;
    float openHideInTimerDuration = 5.0f;
    float openHideInTimerDurationMin = 5.0f;
    float openHideInTimerDurationMax = 35.0f;

    float cheatSearchTimer = 0.0f;
    float cheatSearchTimerDuration = 8.0f;

    float idleChance = 0.05f;
    float idleTimer = 0.0f;
    float idleTimerDuration = 10.0f;
    float idleDelayTimer = 0.0f;
    float idleDelayTimerDuration = 10.0f;
    float idleDelayTimerDurationMin = 10.0f;
    float idleDelayTimerDurationMax = 50.0f;
    Vector3 idleLookAtPoint;
    bool hasFoundLookAt = false;

    float recalulateSightTimer = 0.0f;
    [Tooltip("The time between vision recalculation. Set to 0 for the best AI behaviour, set slightly higher for better performance. Default: 0.5.")]
    public float recalculateSightTimerDuration = 0.5f;

    [HideInInspector]
    public WaypointNode lastNode;
    [HideInInspector]
    public WaypointNode currentNode;
    [HideInInspector]
    public bool hasGotNode = false;
    [HideInInspector]
    public bool hasGotRandomPosition = false;
    Vector3 randomPatrolPosition;

    [HideInInspector]
    public AIdirector director;
    UnityEngine.AI.NavMeshPath currentPath;
    bool hasFoundPath = false;
    float recalculatePathTimer = 0.0f;

    [Tooltip("The time between path recalculation. Lower values will result in the best behaviour but will impact framerate. Default 1.0.")]
    public float recalculatePathTimerDuration = 1.0f;

    [HideInInspector]
    public bool isGrouped;
    [HideInInspector]
    public Vector3 groupTargetPosition;

    float playerInSightTimer;
    float timeInSightForPlayerDetection = 2.0f;
    [HideInInspector]
    public bool playerInSightForDuration = false;

    [HideInInspector]
    public bool isFleeing = false;
    [HideInInspector]
    public bool hasFoundFleePosition = false;
    [HideInInspector]
    public Vector3 fleeToPosition;
    float fleeTimer;
    [Tooltip("How long the AI will flee from the player for. Currently only used by Scout AI.")]
    public float fleeTimerDuration = 15.0f;

    [HideInInspector]
    public bool isSignallingAllies = false;
    [Tooltip("The range at which the AI will alert nearby allies. Currently only used by Scout AI.")]
    public float signalAlliesRange = 30.0f;
    float signalAlliesDuration = 1.0f;
    float signalAlliesTimer = 0.0f;
    [HideInInspector]
    public bool hasBeenSignaled;
    float hasBeenSignaledTimer = 0.0f;
    float hasBeenSignaledDuration = 3.0f;

    [HideInInspector]
    public int initialAvoidancePriority;

    float timeDestinationOutOfAPA;
    float timeDestinationOutOfAPAforReset = 5.0f;
    bool destinationIsOutOfAPA = false;

    [HideInInspector]
    public bool playerFirstSighting = true;
    float resetPlayerFirstSightingTimer = 0.0f;
    float resetPlayerFirstSightingDuration = 5.0f;

     [HideInInspector]
    public bool isFlanker = false;
    [HideInInspector]
    public float flankDirection = 90;
    [HideInInspector]
    public float flankDistance = 4;

    [Tooltip("Whether or not this is a temporary spawn. Any AI spawned by the director will be considered temporary, and will be removed to match the target number of AI.")]
    public bool isTempSpawn = false;
    [Tooltip("Whether or not the AI can open hide in objects. Currently only applies to Advanced AI.")]
    public bool canOpenHideInObjects = false;

    [Tooltip("Whether or not the AI can only move if the player has no vision of them. Currently only applies to Advanced AI.")]
    public bool canOnlyMoveIfPlayerCantSee = false;

    [Tooltip("The amount of damage inflicted to the player when this AI attacks.")]
    public float attackDamage = 25.0f;
    [Tooltip("The range this AI must be in to attack the player.")]
    public float attackRange = 2.5f;
    [Tooltip("The time between this AI's attacks.")]
    public float attackCooldownDuration = 5.0f;
    float attackCooldownTimer = 0.0f;

    [Tooltip("Whether or not this AI can take damage.")]
    public bool canTakeDamage = true;
    [Tooltip("The max health of this AI.")]
    public float maxHealth = 100.0f;
    [Tooltip("The current health of this AI.")]
    public float currentHealth = 0.0f;
    [Tooltip("Whether or not damage stuns this AI. This can apply even if the AI cannot take damage.")]
    public bool damageStuns = false;
    [Tooltip("The time the AI is stunned for.")]
    public float stunDuration = 3.0f;
    float stunTimer = 0.0f;
    [HideInInspector]
    public bool isStunned = false;
    [HideInInspector]
    public bool hasAttacked = false;

    [Tooltip("The audio clip which plays when this AI attacks.")]
    public AudioClip attackSound;
    AudioSource audioSource;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();

        agent.autoTraverseOffMeshLink = false;
        agent.updatePosition = true;
        agent.updateRotation = false; //rotation is handled by animation
        agent.avoidancePriority = Random.Range(2, 99);
        initialAvoidancePriority = agent.avoidancePriority;

        director = GameObject.FindGameObjectWithTag("AIdirector").GetComponent<AIdirector>();
        player = GameObject.FindGameObjectWithTag("Player");
        idleDelayTimerDuration = Random.Range(idleDelayTimerDurationMin, idleDelayTimerDurationMax);
        openHideInTimerDuration = Random.Range(openHideInTimerDurationMin, openHideInTimerDurationMax);

        currentPath = new UnityEngine.AI.NavMeshPath();
        isGrouped = false;

        currentHealth = maxHealth;

        if (GetComponent<AudioSource>() != null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        recalulateSightTimer += Time.deltaTime;
        agent.enabled = true;
        attackCooldownTimer += Time.deltaTime;

        //UnityEngine.AI.NavMeshHit navMeshHit; 

        //if (UnityEngine.AI.NavMesh.SamplePosition(agent.transform.position, out navMeshHit, 0.1f, UnityEngine.AI.NavMesh.AllAreas))
        //{
        //    if (navMeshHit.mask == 8)
        //    {
        //        isInCrouchArea = true;
        //        //Debug.Log("Crouch");
        //    }
        //    else
        //    {
        //        isInCrouchArea = false;
        //    }
        //}

        if (GetDistanceToPlayer() < 3.0f)
        {
            isPlayerTooClose = true;
        }
        else
        {
            isPlayerTooClose = false;
        }

        if(hasBeenSignaled)
        {
            playerInSight = true;
            hasBeenSignaledTimer += Time.deltaTime;
            if(hasBeenSignaledTimer >= hasBeenSignaledDuration)
            {
                hasBeenSignaledTimer = 0.0f;
                hasBeenSignaled = false;
                playerInSight = false;
            }
        }

        if (playerInSight)
        {
            hasGotNode = false;
            if (currentNode != null)
            {
                currentNode.RemoveVisitor();
                currentNode = null;
            }

            playerInSightTimer += Time.deltaTime * director.currentTimeForPlayerDetectionMultiplier;
            if(playerInSightTimer >= timeInSightForPlayerDetection)
            {
                playerInSightForDuration = true;
                if (playerFirstSighting == true)
                {
                    IncreaseChanceOfClosestNodeToPlayer();
                    playerFirstSighting = false;
                }
                resetPlayerFirstSightingTimer = 0.0f;
            }
        }
        else
        {
            playerInSightTimer = 0;
            playerInSightForDuration = false;

            if (playerFirstSighting == false)
            {
                resetPlayerFirstSightingTimer += Time.deltaTime;
                if (resetPlayerFirstSightingTimer >= resetPlayerFirstSightingDuration)
                {
                    resetPlayerFirstSightingTimer = 0.0f;
                    playerFirstSighting = true;
                }
            }
        }

        if (isFleeing)
        {
            if(playerInSight)
            {
                fleeTimer = 0;
            }

            fleeTimer += Time.deltaTime;
            if (fleeTimer >= fleeTimerDuration)
            {
                isFleeing = false;
            }
        }
        else
        {
            fleeTimer = 0;
            hasFoundFleePosition = false;
        }

        if(isSignallingAllies)
        {
            signalAlliesTimer += Time.deltaTime;
            if(signalAlliesTimer >= signalAlliesDuration)
            {
                isSignallingAllies = false;
                signalAlliesTimer = 0.0f;
            }
        }
        else
        {
            signalAlliesTimer = 0.0f;
        }

        if(isStunned)
        {
            stunTimer += Time.deltaTime;
            if(stunTimer >= stunDuration)
            {
                stunTimer = 0.0f;
                isStunned = false;
            }
        }

        CheckNearbyAllies();
        if(hasAttacked)
        {
            hasAttacked = false;
        }

        if(!gameObject.activeInHierarchy)
        {
            Debug.Log("im inactive");
        }
    }

    void OnEnable()
    {
        if (canOpenHideInObjects)
        {
            Player.EnterHideInBroadcast += CanSeePlayerEnterHideIn;
        }
    }

    void OnDisable()
    {
        if (canOpenHideInObjects)
        {
            Player.EnterHideInBroadcast -= CanSeePlayerEnterHideIn;
        }
    }

    //If player in enemy's sight
    void OnTriggerStay(Collider other)
    {
        //recalulateSightTimer += 0.1f * Time.deltaTime;

        if (other.gameObject == player && !other.isTrigger)
        {
            //playerIsHeard = false;
            //playerInSight = false;
            if (recalulateSightTimer >= recalculateSightTimerDuration)
            {     
                recalulateSightTimer = 0.0f;
                Vector3 direction = other.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                //if player is within field of view OR is too close
                if (((angle < fieldOfViewAngle * 0.5f) && !player.GetComponent<Player>().isHiding) || (isPlayerTooClose && !player.GetComponent<Player>().isHiding))
                {
                    if (shouldUseDetailedVision)
                    {
                        DetailedVisionCheck();
                    }
                    else
                    {
                        SimpleVisionCheck();
                    }
                }
                else
                {
                    playerInSight = false;
                }
            }
        }
        else if (canOpenHideInObjects && (other.tag == "HideInObject" && !hideInObjectsInSight.Contains(other.GetComponent<HideInObject>())))
        {
            RaycastHit hit;
            int layerMask = ((1 << 8) | ( 1 << 9));
            layerMask = ~layerMask;
            if ((Physics.Linecast(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), out hit)))
            {
                if (hit.collider == other)
                {
                    if (!hideInObjectsInSight.Contains(other.GetComponent<HideInObject>()))
                        hideInObjectsInSight.Add(other.GetComponent<HideInObject>());
                }
            }
        }
        //Enables vision of nearby allies, disabled due to large performance impact. 
        //else if (other != this.GetComponent<Collider>() && (other.tag == "BasicAI" || other.tag == "AdvancedAI" || other.tag == "ScoutAI") && !alliesInSight.Contains(other.gameObject))
        //{
        //    RaycastHit hit;
        //    if ((Physics.Linecast(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), out hit)))
        //    {
        //        if (hit.collider == other)
        //        {
        //            if (!alliesInSight.Contains(other.gameObject))
        //            {
        //                Debug.Log("can see ally");
        //                alliesInSight.Add(other.gameObject);
        //            }
        //        }
        //    }
        //}
    }

    //if object leaves detection range, they cannot be seen
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
            //playerIsHeard = false;
        }
        else if (other.gameObject.tag == "HideInObject")
        {
            if (hideInObjectsInSight.Contains(other.GetComponent<HideInObject>()))
                hideInObjectsInSight.Remove(other.GetComponent<HideInObject>());
        }
        else if (other.tag == "BasicAI" || other.tag == "AdvancedAI" || other.tag == "ScoutAI")
        {          
            if (alliesInSight.Contains(other.gameObject))
                alliesInSight.Remove(other.gameObject);

        }
    }

    //Move between waypoints randomly
    public void Patrol()
    {
        CheckIfDestinationIsOutsideAPA();

        if (hasGotNode && !hasGotRandomPosition)
        {
            if (Vector3.Distance(this.transform.position, currentNode.transform.position) >= 2) //if too far from waypoint, move closer
            {
                MoveToPosition(currentNode.transform.position, patrolSpeed);               
            }
            else if (Vector3.Distance(this.transform.position, currentNode.transform.position) <= 2) //if too close, move to next waypoint
            {
                lastNode = currentNode;
                hasGotNode = false;
                currentNode.RemoveVisitor();
                currentNode = null;
            }
        }
        else if(!hasGotRandomPosition)
        {
            currentNode = director.GetActiveWaypointNode();
            if (currentNode != null && currentNode != lastNode)
            {
                if (CheckCanPathToPosition(currentNode.transform.position))
                {
                    hasGotNode = true;
                    currentNode.SetVisitor(this.gameObject);
                    hasGotRandomPosition = false;
                }                                  
            }
        }

        if(!hasGotNode)
        {
            if (!hasGotRandomPosition)
            {
                Vector3 tempPos;
                hasGotRandomPosition = director.GetPointInAPA(out tempPos, 1.0f);
                if(hasGotRandomPosition)
                {
                    randomPatrolPosition = tempPos;
                    hasGotNode = false;
                    if (currentNode != null)
                    {
                        currentNode.RemoveVisitor();
                        currentNode = null;
                    }
                }
            }
            else
            {
                if (Vector3.Distance(this.transform.position, randomPatrolPosition) >= 2) //if too far from waypoint, move closer
                {
                    MoveToPosition(randomPatrolPosition, patrolSpeed);                
                }
                else if (Vector3.Distance(this.transform.position, randomPatrolPosition) <= 2) //if too close, move to next waypoint
                {
                    hasGotRandomPosition = false;
                }
            }
        }
    }

    public void GroupPatrol()
    {
        CheckIfDestinationIsOutsideAPA();
        MoveToPosition(groupTargetPosition, patrolSpeed);
    }

    public void ForceFindNewDestination()
    {
        hasGotNode = false;
        hasGotRandomPosition = false;
        ForceRecalculatePath();
    }

    public void ForceRecalculatePath()
    {
        recalculatePathTimer = recalculatePathTimerDuration;
    }

    public void LookAtPoint(Vector3 LookAt)
    {
        StopMovement();

        //Face enemy towards investigate position over time
        Vector3 lookDirection = LookAt - transform.position;
        lookDirection.y = 0;
        Quaternion toRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
    }

    public bool CheckIfCanSeePoint(Vector3 position)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        if(Physics.Linecast(transform.position, position, layerMask))
        {
            return false;
        }
        return true;
    }

    public float GetDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
        return distanceToPlayer;
    }

    public void MoveToPosition(Vector3 position, float speed)
    {
        //UnityEngine.AI.OffMeshLinkData data = agent.currentOffMeshLinkData;
        //UnityEngine.AI.OffMeshLink link = data.offMeshLink;
        if (playerInSight)
        {
            recalculatePathTimer += Time.deltaTime * 4;
        }
        else
        {
            recalculatePathTimer += Time.deltaTime;
        }
        if(recalculatePathTimer >= recalculatePathTimerDuration)
        {
            if (CheckCanPathToPosition(position))
            {
                //Debug.Log("path found");
                agent.SetDestination(position);
                recalculatePathTimer = 0.0f;
            }
            else
            {
                //Debug.Log("path failed");
                hasGotNode = false;
                hasGotRandomPosition = false;
            }                  
        }

        if (agent.isOnOffMeshLink)
        {
            //Debug.Log("IS ON LINK");
            UnityEngine.AI.OffMeshLinkData data = agent.currentOffMeshLinkData;
            UnityEngine.AI.OffMeshLink link = data.offMeshLink;

            //Debug.Log("link name " + link.name.ToString());
            if (link.activated)
            {
                if (link.area == UnityEngine.AI.NavMesh.GetAreaFromName("DestroyWall"))
                {
                    //Debug.Log("on destroy wall link");
                    StopMovement();
                    DestroyClosestDestroyableWall();
                }
                else if (link.area == UnityEngine.AI.NavMesh.GetAreaFromName("DoorWalkThrough"))
                {
                    //Debug.Log("on door link");
                    StopMovement();
                    OpenClosestDoor();               
                }
            }
            else
            {
                agent.Warp(transform.position); //warp to current position to leave the offmesh link
            }
        }
        else
        {
            agent.speed = speed * director.currentSpeedMultiplier;
            character.Move(agent.desiredVelocity, isInCrouchArea, false);
        }
    }

    public void StopMovement()
    {
        agent.enabled = true;
        agent.speed = 0;
        if(agent.isOnNavMesh)
        {
            agent.SetDestination(this.transform.position);
            character.Move(agent.desiredVelocity, isInCrouchArea, false);
        }      
        agent.enabled = false;
        if (GetComponent<Rigidbody>().velocity.magnitude < .01)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public bool CheckIfCloseToPosition(Vector3 position, float range)
    {
        if (CalculatePathLength(transform.position, position) <= range)
        {
            return true;
        }
        return false;
    }

    public bool CheckIfCloseToPositionCheap(Vector3 position, float range)
    {
        if (Vector3.Distance(this.transform.position, position) < range)
        {
            return true;
        }
        return false;
    }

    public bool CheckSearchTimer()
    {
        searchTimer += Time.deltaTime;

        if(searchTimer >= searchTimerDuration)
        {
            searchTimer = 0.0f;
            return true;
        }
        return false;
    }

    public bool CheckSearchSoundTimer()
    {
        searchSoundTimer += Time.deltaTime;

        if (searchSoundTimer >= searchSoundTimerDuration)
        {
            searchSoundTimer = 0.0f;
            return true;
        }
        return false;
    }

    public void ResetSearchTimer()
    {
        searchTimer = 0.0f;
    }

    public void ResetSoundSearchTimer()
    {
        searchSoundTimer = 0.0f;
    }

    public bool CheckLookAtTimer()
    {
        lookAtTimer += Time.deltaTime;

        if (lookAtTimer >= lookAtTimerDuration)
        {
            lookAtTimer = 0.0f;
            return true;
        }
        return false;
    }

    public bool CheckIdleTimer()
    {
        idleTimer += Time.deltaTime * director.currentIdleTimeMultiplier;

        if (idleTimer >= idleTimerDuration)
        {
            idleTimer = 0.0f;
            return true;
        }
        return false;
    }

    public void ResetIdleTimers()
    {
        idleTimer = 0.0f;
        lookAtTimer = 0.0f;
        idleDelayTimer = 0.0f;
        idleDelayTimerDuration = Random.Range(idleDelayTimerDurationMin, idleDelayTimerDurationMax);
        isIdle = false;
        hasFoundLookAt = false;
    }

    public bool CheckCheatSearchTimer(out float currentTimer)
    {
        cheatSearchTimer += Time.deltaTime;

        //Debug.Log("timer " + cheatSearchTimer.ToString());

        if(cheatSearchTimer >= cheatSearchTimerDuration)
        {
            //Debug.Log("cheat timer over");
            //cheatSearchTimer = 0.0f;
            currentTimer = cheatSearchTimer;
            return true;
        }
        currentTimer = cheatSearchTimer;
        return false;
    }

    public void ResetCheatSearchTimer()
    {
        cheatSearchTimer = 0.0f;
    }

    //Find the length to a position, based on nav mesh movement.
    //For example, sound can only be heard through walls if enemty's path is close enough to player
    public float CalculatePathLength(Vector3 startPosition, Vector3 targetPosition)
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

        if (agent.enabled)
            agent.CalculatePath(targetPosition, path);

        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        allWayPoints[0] = startPosition;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];

        }

        float pathLength = 0f;

        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }

    public Vector3 GetDirectionToPosition(Vector3 targetPosition, Vector3 startPosition)
    {
        Vector3 directionToPoint = (targetPosition - startPosition).normalized;
        //Debug.DrawLine(startPosition, startPosition + directionToPoint * 10, Color.red, 1.0f);

        return directionToPoint;
    }

    public bool SelectRandomPointInArea(Vector3 startPosition, float radius, out Vector3 resultPosition)
    {
        Vector3 randomDirection = Random.insideUnitSphere * (radius);
        randomDirection += startPosition;
        UnityEngine.AI.NavMeshHit hit;

        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, 5.0f, 1))
        {
            resultPosition = hit.position;
            return true;
        }

        resultPosition = Vector3.zero;

        return false;

        //Debug.Log("random position " + finalPosition);
        //Debug.Log("actual position " + startPosition);
    }

    public void OpenHideInObject()
    {
        hideInObjectToCheck.OpenDoorInstant();
        hideInObjectToCheck = null;
    }

    public void CanSeePlayerEnterHideIn()
    {
        if(playerInSight)
        {      
            hideInObjectToCheck = player.GetComponent<Player>().objectHiddenIn.GetComponent<HideInObject>();           
        }
    }

    public HideInObject RandomlySelectHideInObject()
    {
        foreach(HideInObject obj in hideInObjectsInSight)
        {
            if((!obj.isOpenFully) && (obj.playerIsIn) && (obj.isOpen) && (Random.Range(0,100) < chanceToOpenHideInObjectPlayerIsIn))
            {
                return obj;
            }
            else if ((!obj.isOpenFully) && (Random.Range(0, 100) < chanceToOpenHideInObject))
            {
                return obj;
            }
        }

        return null;
    }

    public bool CheckOpenHideInTimer()
    {
        openHideInTimer += Time.deltaTime;

        if(openHideInTimer >= openHideInTimerDuration)
        {
            return true;
        }
        return false;
    }

    public void ResetOpenHideInTimer()
    {
        openHideInTimer = 0.0f;
        openHideInTimerDuration = Random.Range(openHideInTimerDurationMin, openHideInTimerDurationMax);
    }

    public void DestroyClosestDestroyableWall()
    {
        //Debug.Log("Destroy wall");
        GameObject[] allDestructableWalls;
        allDestructableWalls = GameObject.FindGameObjectsWithTag("DestroyWall");
        GameObject closestWall = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject obj in allDestructableWalls)
        {
            Vector3 diff = obj.transform.position - position;
            float currDistance = diff.sqrMagnitude;
            if (currDistance < distance)
            {
                closestWall = obj;
                distance = currDistance;
            }
        }
        closestWall.GetComponent<DestroyWall>().ReduceHealth(50);
    }

    public void OpenClosestDoor()
    {
        //Debug.Log("Open closest door");
        GameObject[] allDoors;
        allDoors = GameObject.FindGameObjectsWithTag("Door");
        GameObject closestDoor = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject obj in allDoors)
        {
            Vector3 diff = obj.transform.position - position;
            float currDistance = diff.sqrMagnitude;
            if (currDistance < distance && (!obj.GetComponent<Door>().isOpen || (obj.GetComponent<Door>().isOpen && obj.GetComponent<Door>().playerIsUsing)))
            {
                closestDoor = obj;
                distance = currDistance;
            }
        }

        closestDoor.GetComponent<Door>().navLink.activated = false;

        if(!closestDoor.GetComponent<Door>().StartOpening())
        {
            closestDoor.GetComponent<Door>().BreakDownDoor();
        }

        ForceRecalculatePath();
    }

    public bool CheckShouldIdle()
    {
        if (!isIdle)
        {
            idleDelayTimer += Time.deltaTime;

            if (idleDelayTimer >= idleDelayTimerDuration)
            {
                if (Random.Range(0, 100) <= idleChance)
                {
                    //Debug.Log("should idle");
                    isIdle = true;
                    idleDelayTimer = 0.0f;
                    idleDelayTimerDuration = Random.Range(idleDelayTimerDurationMin, idleDelayTimerDurationMax);
                    return true;
                }
            }
        }
        return false;
    }

    //Find a point to look at and face it. Will not face direction if wall is too close
    public bool IdleCurrentPosition()
    {
        if (CheckLookAtTimer() || !hasFoundLookAt)
        {
            idleLookAtPoint = randomPointInCircle(this.transform, 5, Random.Range(-45, 45)); //use randomPointInCircle to ensure that the AI only looks at a point in front of it, and does not fully spin in one turn
            Vector3 directionToLookAt = GetDirectionToPosition(idleLookAtPoint, transform.position);
            RaycastHit hit;
            float minDistanceToObjects = 8.0f;
            if (Physics.Raycast(transform.position, directionToLookAt, out hit, minDistanceToObjects))
            {
                //Debug.Log("Looking at wall or something");
                hasFoundLookAt = false;
            }
            else
            {
                hasFoundLookAt = true;
            }
        }

        if (CheckIdleTimer())
        {
            isIdle = false;
            hasFoundLookAt = false;
        }

        if (hasFoundLookAt)
        {
            LookAtPoint(idleLookAtPoint);

            //reset current node the AI is visiting if idle is successful.
            hasGotNode = false;
            if (currentNode != null)
            {
                currentNode.RemoveVisitor();
                currentNode = null;
            }
            hasGotRandomPosition = false;
            return true;
        }

        return false;
    }

    public Vector3 randomPointInCircle(Transform trans, float radius, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 position = trans.right * Mathf.Sin(rad) + trans.forward * Mathf.Cos(rad);
        return trans.position + position * radius;
    }

    void DetailedVisionCheck()
    {
        int ignoreAIlayerMask = (1 << 9);
        ignoreAIlayerMask = ~ignoreAIlayerMask;

        //Check three linecasts from the agent's head to two points of the player, one lower, one middle and one higher. The player can only be seen if two or more of these linecasts hit the player
        RaycastHit hitLower;
        RaycastHit hitMiddle;
        RaycastHit hitUpper;

        //Add one to this value when a linecast hits, if more than two hit, the player can be seen
        int hitCounter = 0;

        float yOriginPos = transform.position.y + 0.5f;
        if (!character.m_Crouching)
        {
            yOriginPos = transform.position.y + 1.5f;
        }

        //Check lower raycast
        if ((Physics.Linecast(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z), out hitLower, ignoreAIlayerMask)))
        {
            if (hitLower.collider.gameObject == player)
            {
                hitCounter += 2; //If AI can see player's legs, spot them instantly.
            }

            if (hitLower.collider.gameObject == player)
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z), Color.green);
            }
            else
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z), Color.red);
            }

        }

        //Check middle raycast
        if ((Physics.Linecast(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z), out hitMiddle, ignoreAIlayerMask)))
        {
            if (hitMiddle.collider.gameObject == player)
            {
                hitCounter += 2; //If AI can see player's middle, spot them instantly.
            }

            if (hitMiddle.collider.gameObject == player)
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z), Color.green);
            }
            else
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z), Color.red);
            }

        }

        //Check upper raycast
        if ((Physics.Linecast(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.75f, player.transform.position.z), out hitUpper, ignoreAIlayerMask)))
        {
            if (hitUpper.collider.gameObject == player)
            {
                hitCounter++; //Only increase hit counter by one, meaning more of the player needs to be visible.
            }

            if (hitUpper.collider.gameObject == player)
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.75f, player.transform.position.z), Color.green);
            }
            else
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.75f, player.transform.position.z), Color.red);
            }
        }

        if (hitCounter >= 2)
        {
            playerInSight = true;
            lastHeardSoundEmitter = null;
            hideInObjectToCheck = null;
        }
        else
        {
            playerInSight = false;
        }
    }

    void SimpleVisionCheck()
    {
        int ignoreAIlayerMask = (1 << 9);
        ignoreAIlayerMask = ~ignoreAIlayerMask;

        float yOriginPos = transform.position.y + 0.5f;
        if (!character.m_Crouching)
        {
            yOriginPos = transform.position.y + 1.5f;
        }

        RaycastHit hitMiddle;

        //Check middle raycast
        if ((Physics.Linecast(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z), out hitMiddle, ignoreAIlayerMask)))
        {
            if (hitMiddle.collider.gameObject == player)
            {
                playerInSight = true;
                lastHeardSoundEmitter = null;
                hideInObjectToCheck = null;
            }
            else
            {
                playerInSight = false;
            }

            if (hitMiddle.collider.gameObject == player)
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z), Color.green);
            }
            else
            {
                Debug.DrawLine(new Vector3(transform.position.x, yOriginPos, transform.position.z), new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z), Color.red);
            }

        }
    }

    public void RunAwayFromPosition(Vector3 runAwayFromPos)
    {
        Vector3 dirToPos = GetDirectionToPosition(runAwayFromPos, this.transform.position);
        if (!hasFoundFleePosition)
        {
            hasFoundFleePosition = SelectRandomPointInArea(transform.position - dirToPos * 4, 10, out fleeToPosition);

            if(!CheckCanPathToPosition(fleeToPosition))
            {
                hasFoundFleePosition = false;
            }
        }

        if(hasFoundFleePosition)
        {
            MoveToPosition(fleeToPosition, chaseSpeed);
            if(Vector3.Distance(transform.position, fleeToPosition) < 3)
            {
                hasFoundFleePosition = false;
            }
        }
    }

    public void SignalNearbyAllies(float signalRange)
    {
        foreach (BasicAI basicAI in AIdirector.sharedAIdirector.allBasicAI)
        {
            if (Vector3.Distance(transform.position, basicAI.transform.position) < signalRange)
            {
                if (basicAI.actionScript.CheckCanPathToPosition(player.transform.position))
                {
                    //Debug.Log("signalled ally");
                    basicAI.actionScript.hasBeenSignaled = true;
                }
            }
        }

        foreach (AI advancedAI in AIdirector.sharedAIdirector.allAdvancedAI)
        {
            if (Vector3.Distance(transform.position, advancedAI.transform.position) < signalRange)
            {
                if (advancedAI.actionScript.CheckCanPathToPosition(player.transform.position))
                {
                    //Debug.Log("signalled ally");
                    advancedAI.actionScript.hasBeenSignaled = true;
                }
            }
        }
    }

    public bool CheckCanPathToPosition(Vector3 pos)
    {
        UnityEngine.AI.NavMeshPath tempPath = new UnityEngine.AI.NavMeshPath();
        agent.enabled = true;
        if (agent.CalculatePath(pos, tempPath))
        {
            if (tempPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
            {
                return true;
            }
        }

        return false;
    }

    public void IncreaseChanceOfClosestNodeToPlayer()
    {
        WaypointNode node;
        node = AIdirector.sharedAIdirector.GetClosestNodeToPosition(player.transform.position);
        if(node != null && director.CheckNodeChangeCooldownTimer())
        {
            //Debug.Log("INCREASE CHANCE OF CLOSEST NODE " + node.name.ToString());
            AIdirector.sharedAIdirector.IncreaseNodeTypeWeighting(node.roomType);
        }
    }  

    public void ResetDestination()
    {
        hasGotNode = false;
        if (currentNode != null)
        {
            currentNode.RemoveVisitor();
            currentNode = null;
        }
        ForceFindNewDestination();
    }

    public bool GetIsDesinationOutsideAPA()
    {
        return destinationIsOutOfAPA;
    }

    void CheckIfDestinationIsOutsideAPA()
    {
        if (hasGotNode)
        {
            if (!currentNode.isActive)
            {
                //Debug.Log("Node now inactive");
                timeDestinationOutOfAPA += Time.deltaTime;
                if (timeDestinationOutOfAPA >= timeDestinationOutOfAPAforReset)
                {
                    isIdle = true;
                    destinationIsOutOfAPA = true;
                    ResetDestination();
                }
            }
            else
            {
                timeDestinationOutOfAPA = 0.0f;
                destinationIsOutOfAPA = false;
            }
        }
        else if (hasGotRandomPosition)
        {
            if (Vector3.Distance(randomPatrolPosition, player.transform.position) > director.activePlayerArea.radius)
            {
                //Debug.Log("Random point now out of APA");
                timeDestinationOutOfAPA += Time.deltaTime;
                if (timeDestinationOutOfAPA >= timeDestinationOutOfAPAforReset)
                {
                    isIdle = true;
                    destinationIsOutOfAPA = true;
                    ResetDestination();
                }
            }
            else
            {
                timeDestinationOutOfAPA = 0.0f;
                destinationIsOutOfAPA = false;
            }
        }

        if(isGrouped)
        {
            if (Vector3.Distance(groupTargetPosition, player.transform.position) > director.activePlayerArea.radius)
            {
                //Debug.Log("Group destination now out of APA");
                timeDestinationOutOfAPA += Time.deltaTime;
                if (timeDestinationOutOfAPA >= timeDestinationOutOfAPAforReset)
                {
                    isIdle = true;
                    destinationIsOutOfAPA = true;
                }
            }
            else
            {
                timeDestinationOutOfAPA = 0.0f;
                destinationIsOutOfAPA = false;
            }
        }
    }

    void CheckNearbyAllies()
    {
        foreach(GameObject ally in alliesInSight)
        {
            //if (ally.GetComponent<AIactions>().playerInSight)
            //{
            //    playerInSight = true;
            //}

            //if (ally.GetComponent<AIactions>().playerInSightForDuration)
            //{
            //    playerInSightForDuration = true;
            //}
        }
    }

    bool CheckIfPlayerCanSeeMe()
    {
        bool pointObstructedFromView = false;
        int ignoreAIlayerMask = ((1 << 9) | (1 << 8));
        ignoreAIlayerMask = ~ignoreAIlayerMask;

        RaycastHit hit;
        if (Physics.Linecast(Camera.main.transform.position, new Vector3(this.transform.position.x, this.transform.position.y + 1.0f,this.transform.position.z), out hit, ignoreAIlayerMask))
        {
            //Debug.Log("VIEW IS OBSTUCTED " + hit.collider.gameObject.name.ToString());
            pointObstructedFromView = true;
        }

        if (!pointObstructedFromView && (InfiniteCameraCanSeePoint(Camera.main, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z)) || InfiniteCameraCanSeePoint(Camera.main, new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z)) || InfiniteCameraCanSeePoint(Camera.main, new Vector3(this.transform.position.x, this.transform.position.y + 2.0f, this.transform.position.z))))
        {
            //Debug.Log("PLAYER CAN SEE ME ");
            return true;
        }

        return false;
    }

    bool InfiniteCameraCanSeePoint(Camera camera, Vector3 point)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }

    public bool CheckCanMove()
    {
        //if (GetDistanceToPlayer() < 1.5f)
        //{
        //    return false;
        //}

        if (!CheckIfPlayerCanSeeMe())
        {
            return true;
        }
        return false;
    }

    public bool AttackPlayer()
    {
        if(attackCooldownTimer >= attackCooldownDuration)
        {
            if(attackSound != null)
            {
                audioSource.PlayOneShot(attackSound, 1);
            }

            attackCooldownTimer = 0.0f;
            player.GetComponent<Player>().LoseHealth(attackDamage);
            hasAttacked = true;
            return true;
        }
        return false;
    }

    public void TakeDamage(float healthLoss)
    {
        if (damageStuns)
        {
            if(!isStunned)
            {
                isStunned = true;
            }
        }
        
        if(canTakeDamage)
        {
            currentHealth -= healthLoss;
            if (currentHealth <= 0)
            {
                if (!respawns)
                {
                    AIdirector director = GameObject.FindGameObjectWithTag("AIdirector").GetComponent<AIdirector>();
                    if (this.tag == "AdvancedAI")
                    {
                        director.DestroyAdvancedAI(gameObject.GetComponent<AI>());

                    }
                    else if (this.tag == "BasicAI")
                    {
                        director.DestroyBasicAI(gameObject.GetComponent<BasicAI>());
                    }
                    else if(this.tag == "ScoutAI")
                    {
                        director.DestroyScoutAI(gameObject.GetComponent<ScoutAI>());
                    }
                }
                else
                {
                    isAlive = false;
                    if (isGrouped)
                    {
                        director.RemoveFromGroup(gameObject.GetComponent<BasicAI>());
                    }
                }
            }
        }
    }

    public bool CheckRespawnTimer()
    {
        respawnTimer += Time.deltaTime;
        if(respawnTimer >= respawnTimerDuration)
        {
            return true;
        }
        return false;
    }

    public void ResetRespawnTimer()
    {
        respawnTimer = 0.0f;
    }

    public void WarpToPosition(Vector3 position)
    {
        gameObject.transform.position = position;
        agent.Warp(position);
    }

    public void Respawn(Vector3 respawnPos)
    {
        ResetRespawnTimer();
        WarpToPosition(respawnPos);
        isAlive = true;
        currentHealth = maxHealth;
    }

    public void Deactivate()
    {
        WarpToPosition(new Vector3(-1000, -1000, -1000));
        StopMovement();
    }
}

