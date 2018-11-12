using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum WaypointNodeType
{
    HALLWAY,
    KITCHEN,
    LIVINGROOM,
    BEDROOM,
    STOREROOM,
    OFFICE,
    BATHROOM,
    LOUNGE,
    GENERIC1,
    GENERIC2,
    GENERIC3,
    GENERIC4,
    GENERIC5
}

public class WaypointNode : MonoBehaviour
{
    Text percentTextBox;
    Text typeTextBox;

    [Tooltip("The type of room or area which this node is marking. Nodes of the same type will share a chance of the AI patrolling to them. This chance will increase the more often AI see the player near a node of the same type.")]
    public WaypointNodeType roomType;

    [HideInInspector]
    public bool isActive = false;
    [HideInInspector]
    public bool hasVisitor = false;
    [HideInInspector]
    public GameObject visitingAI;
    [HideInInspector]
    public AIGroup visitingGroup;
    Renderer render;

    [Tooltip("The max distance between nodes for them to be considered neighbours. Neighbours will slightly effect the patrol chance of this node. Neighbours with a higher chance will increase the chance of this one slightly, while neighbours which currently have a visitor will slightly decrease the chance of this node. Default: 10.")]
    public float maxDistanceToFindNeighbours = 10.0f;

    [Tooltip("All the current neighbours of this node.")]
    public List<WaypointNode> neighbouringWaypointNodes;

    [HideInInspector]
    public float baseSelectionPercentChance = 1.0f;
    [HideInInspector]
    public float selectionPercentModifier = 1.0f;
    [HideInInspector]
    public float currentSelectionPercentChance = 0.0f;

    float minSelectionPercentChance = 1.0f;
    float maxSelectionPercentChance = 100.0f;

	// Use this for initialization
	void Start()
    {
        render = GetComponent<Renderer>();
        percentTextBox = transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        typeTextBox = transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        typeTextBox.text = roomType.ToString();
        FindNeighbours();

        //baseSelectionPercentChance = AIdirector.sharedAIdirector.initialBaseWaypointNodeChance;
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateChanceModifier();
        currentSelectionPercentChance = baseSelectionPercentChance * selectionPercentModifier;
        percentTextBox.text = currentSelectionPercentChance.ToString() + "%";

        if (hasVisitor)
        {
            render.material.color = Color.yellow;
        }
		else if(isActive)
        {
            render.material.color = Color.green;
        }
        else if (!isActive)
        {
            render.material.color = Color.red;
        }

        if (baseSelectionPercentChance > maxSelectionPercentChance)
        {
            baseSelectionPercentChance = maxSelectionPercentChance;
        }
        else if(baseSelectionPercentChance < minSelectionPercentChance)
        {
            baseSelectionPercentChance = minSelectionPercentChance;
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public bool GetActive()
    {
        return isActive;
    }

    public void SetVisitor(GameObject obj)
    {
        hasVisitor = true;
        visitingAI = obj;
    }

    public void setVisitingGroup(AIGroup group)
    {
        hasVisitor = true;
        visitingGroup = group;
    }

    public void RemoveVisitor()
    {
        hasVisitor = false;
        if (visitingAI != null)
            visitingAI = null;

        if (visitingGroup != null)
            visitingGroup = null;
    }

    public void FindNeighbours()
    {
        neighbouringWaypointNodes = new List<WaypointNode>();
        List<WaypointNode> tempList = new List<WaypointNode>();

        foreach(WaypointNode node in AIdirector.sharedAIdirector.allNodes)
        {
            if((Vector3.Distance(node.gameObject.transform.position, this.gameObject.transform.position) < maxDistanceToFindNeighbours) && node != this)
            {
                tempList.Add(node);
            }
        }

        tempList = tempList.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();
        neighbouringWaypointNodes = tempList;
    }

    void UpdateChanceModifier()
    {
        List<WaypointNodeType> roomTypesEffecting = new List<WaypointNodeType>();
        selectionPercentModifier = 1.0f;
        float tempModifier = 1.0f;
        foreach(WaypointNode neighbour in neighbouringWaypointNodes)
        {
            if ((neighbour.baseSelectionPercentChance > baseSelectionPercentChance) && !roomTypesEffecting.Contains(neighbour.roomType))
            {
                tempModifier += (neighbour.baseSelectionPercentChance - baseSelectionPercentChance) / (neighbour.baseSelectionPercentChance + baseSelectionPercentChance);
                roomTypesEffecting.Add(neighbour.roomType);
            }

            if (neighbour.hasVisitor)
            {
                tempModifier -= 0.2f;
            }
        }
        selectionPercentModifier = tempModifier;
    }
}
