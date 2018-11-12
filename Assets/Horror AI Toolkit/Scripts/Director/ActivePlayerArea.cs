using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePlayerArea : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;

    [Tooltip("The current radius of the APA. This effects the area where enemies will patrol and spawn.")]
    public float radius = 50;
    [Tooltip("The minimum raidus of the APA.")]
    public float minRadius = 20;
    [Tooltip("The maximum raidus of the APA.")]
    public float maxRadius = 50;

    LineRenderer line;

    // Use this for initialization
    void Start ()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
    }

    void Update()
    {
        DrawAreaCircle();
        UpdateNodesInAPA();
        radius = (-AIdirector.sharedAIdirector.GetCurrentStressLevel() / 3) + maxRadius;

        if(radius < minRadius)
        {
            radius = minRadius;
        }
        else if(radius > maxRadius)
        {
            radius = maxRadius;
        }
    }

    public void UpdateNodesInAPA()
    {
        foreach (WaypointNode node in AIdirector.sharedAIdirector.allNodes)
        {
            if(Vector3.Distance(node.transform.position, AIdirector.sharedAIdirector.GetPlayerPosition()) < radius)
            {
                node.SetActive(true);
            }
            else
            {
                node.SetActive(false);

                //Count how many nodes are active, if less than the minimum amount are active then keep this node activated.
                //Ensures that closest nodes remain active even if they aren't directly within the APA. 
                int activeCount = 0;
                foreach(WaypointNode node2 in AIdirector.sharedAIdirector.allNodes)
                {
                    if(node2.isActive)
                    {
                        activeCount++;
                    }
                }
                if(activeCount < AIdirector.sharedAIdirector.minNumberOfActiveNodes)
                {
                    node.SetActive(true);
                }
            }
        }
    }

    void DrawAreaCircle()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = (Mathf.Sin(Mathf.Deg2Rad * angle) * radius) + AIdirector.sharedAIdirector.GetPlayerPosition().x;
            z = (Mathf.Cos(Mathf.Deg2Rad * angle) * radius) + AIdirector.sharedAIdirector.GetPlayerPosition().z;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
}
