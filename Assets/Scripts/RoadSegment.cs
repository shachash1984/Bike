using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour {

    public int index;
    public bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            triggered = true;
            bool isStarting = transform.position.z == 0 ? true : false;
            RoadGenerator.S.MoveSegment(index, isStarting);
        }
    }
}
