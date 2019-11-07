using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform target;
    [SerializeField] private Vector3 _distanceFromTarget;

    private void Start()
    {
        target = FindObjectOfType<PlayerMovement>().transform;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 wantedPos = target.position + _distanceFromTarget;
        wantedPos.x = 0;
        transform.position = Vector3.Lerp(transform.position, wantedPos, 0.75f);
    }
}
