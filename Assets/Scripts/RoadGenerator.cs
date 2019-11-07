using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

    static public RoadGenerator S;
    [SerializeField] private RoadSegment[] roadSegments;
    private const int SEGMENT_DISTANCE = 20;
    private RoadSegment _farthestSegment;

    private void Awake()
    {
        if (S != null)
            Destroy(this);
        S = this;
        Init();
    }

    public void Init()
    {
        _farthestSegment = roadSegments[roadSegments.Length - 1];
    }

    public void MoveSegment(int index, bool startPos = false)
    {
        //disable the segment before this index
        RoadSegment rs = null;
        if (!startPos)
        {
            if (index == 0)
                rs = roadSegments[roadSegments.Length - 1];
            else
                rs = roadSegments[index - 1];
        }
        else
        {
            return;
        }
        rs.gameObject.SetActive(false);

        //move that segment 20  + farthestSegment.z
        rs.transform.position = new Vector3(0f, 0f, _farthestSegment.transform.position.z + SEGMENT_DISTANCE);

        rs.gameObject.SetActive(true);
        //make this segment farthest segment
        _farthestSegment = roadSegments[rs.index];
        _farthestSegment.triggered = false;
    }

}
