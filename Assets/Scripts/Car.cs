using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Car : MonoBehaviour {

    public float forwardSpeed = 15f;
    public short currentLane = 0;
    public const float LANE_POSITION = 6.75f;
    private const float LANE_SWITCH_ANGLE = 15f;
    private Coroutine _switchLaneCoroutine;
    public int switchLaneTimer = 300;
    [SerializeField] private float _turnSpeed = 1f;
    [SerializeField] private float _laneChangingSpeed = 2.25f;
    [SerializeField] private float _timeBeforeAllignment = 1.25f;
    [SerializeField] private float _laneAllignmentSpeed = 0.5f;


    private void FixedUpdate()
    {
        MoveForward(forwardSpeed);
    }

    private void Update()
    {
        
        if (Time.frameCount % switchLaneTimer == 0)
            _switchLaneCoroutine = StartCoroutine(SwitchLanes(GetNextLane()));
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject, 1f);
    }

    private void MoveForward(float speed)
    {
        Vector3 movementDirection = new Vector3(0f, 0f, Time.deltaTime * speed);
        transform.position += movementDirection;
    }

    public IEnumerator SwitchLanes(short nextLane)
    {
        if(nextLane != currentLane)
        {
            if (nextLane == -1 || (nextLane == 0 && currentLane == 1))
            {
                transform.DORotate(new Vector3(0f, -LANE_SWITCH_ANGLE, 0f), _turnSpeed);
                transform.DOMoveX(nextLane * LANE_POSITION, _laneChangingSpeed);
            }
                
            else if (nextLane == 1 || nextLane == 0 && currentLane == -1)
            {
                transform.DORotate(new Vector3(0f, LANE_SWITCH_ANGLE, 0f), _turnSpeed);
                transform.DOMoveX(nextLane * LANE_POSITION, _laneChangingSpeed);
            }
            yield return new WaitForSeconds(_timeBeforeAllignment);
            transform.DORotate(Vector3.zero, _laneAllignmentSpeed);
            currentLane = nextLane;
        }
    }

    private short GetNextLane()
    {
        int newLane = Random.Range(-1, 2);
        while (currentLane == newLane)
        {
            newLane = Random.Range(-1, 2);
        }
        return (short)newLane;
    }

    
}
