using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEngineSimulator : MonoBehaviour {

    public RealisticEngineSound_mobile res;
    public bool gasPedalPressing = false;
    public float maxRPM = 7000;
    public float idle = 900;
    public float rpm = 0;
    public float accelerationSpeed = 1000f;
    public float decelerationSpeed = 1200f;

    private void Start()
    {
        
        res = GetComponent<RealisticEngineSound_mobile>();
        rpm = idle;
        //gasPedalPressing = true;
        
    }

    void Update()
    {
        gasPedalPressing = Input.touchCount > 0;
        if (gasPedalPressing)
        {
            if (rpm <= maxRPM)
                rpm = Mathf.Lerp(rpm, rpm + accelerationSpeed * PlayerMovement.S.movementSpeed * Time.deltaTime*2, Time.deltaTime);
        }
        else if (rpm > idle)
        {
            rpm = Mathf.Lerp(rpm, rpm - decelerationSpeed * PlayerMovement.S.movementSpeed * Time.deltaTime*2, Time.deltaTime);
        }
        else
            rpm = idle;
        res.gasPedalPressing = gasPedalPressing;
        res.engineCurrentRPM = rpm;
    }
}
