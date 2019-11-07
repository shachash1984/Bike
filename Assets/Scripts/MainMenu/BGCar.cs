using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCar : MonoBehaviour {

    public float movementSpeed = 15f;
    [SerializeField] private bool _outOfSight = false;
    private const float MAX_DISTANCE = 60f;
	


	void FixedUpdate () {
        CheckVisibility();
        MoveForward(movementSpeed);
	}

    private void MoveForward(float speed)
    {
        Vector3 movementDirection = new Vector3(0f, 0f, Time.deltaTime * speed);
        transform.position += movementDirection;
    }

    private void CheckVisibility()
    {
        if (transform.position.z >= MAX_DISTANCE)
            Destroy(gameObject);
    }
}
