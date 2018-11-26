using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArm : MonoBehaviour {
    [SerializeField] private Transform from;
    [SerializeField] private Transform to;
    float speed = 0.1f;
    void Update()
    {
        /*if(transform.rotation != to.rotation)
            transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, Time.time * speed);
        else
        {
            transform.rotation = Quaternion.Lerp(to.rotation, from.rotation, Time.time * speed);
        }*/
    }
}
