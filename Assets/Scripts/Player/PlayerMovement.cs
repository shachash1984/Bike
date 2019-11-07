using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    #region Public Fields
    static public PlayerMovement S;
    [Range(0f, 50f)] public float movementSpeed = 25f;
    [Range(1f, 20f)] public float accelerationSpeed = 1f;
    [Range(0f, 100f)] public float turnSpeed = 30f;
    [Range(0f, 100f)] public float tiltAmount = 25f;
    public float yAxisLimit = 0.3f;
    #endregion

    #region Private Fields
    private const float MAX_SPEED = 60f;
    private const float NORMAL_SPEED = 25f;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }

    private void Start()
    {
        movementSpeed = 0f;
    }

    private void FixedUpdate()
    {
        MoveForward(movementSpeed);
    }

    private void Update()
    {
        MoveSideways(turnSpeed);
        if (Input.touchCount > 0)
        {
            if (movementSpeed > MAX_SPEED)
                movementSpeed = MAX_SPEED;
            else if (movementSpeed < MAX_SPEED)
                movementSpeed += Time.deltaTime * accelerationSpeed;
        }
        else
        {
            if (movementSpeed > NORMAL_SPEED)
                movementSpeed -= Time.deltaTime * accelerationSpeed;
            else if (movementSpeed < NORMAL_SPEED)
                movementSpeed += Time.deltaTime * accelerationSpeed;//NORMAL_SPEED;
        }
    }
    #endregion

    #region Private Methods
    private void MoveForward(float speed)
    {
        Vector3 movementDirection = new Vector3(0f, 0f, Time.deltaTime * speed);
        transform.position += movementDirection;
    }

    private void MoveSideways(float speed)
    {
        Vector3 movementDirection = new Vector3(Input.acceleration.x * Time.deltaTime * speed, 0f, 0f);
        transform.position += movementDirection;
        Tilt(Input.acceleration.x);
    }

    private void Tilt(float dir)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -dir * tiltAmount));
        ClampYAxis();
    }

    private void ClampYAxis()
    {
        if (transform.position.y != yAxisLimit)
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, yAxisLimit, transform.position.z), 0.5f);
    }

    #endregion


}
