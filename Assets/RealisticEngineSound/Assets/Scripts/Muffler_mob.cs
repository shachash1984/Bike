//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2017 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muffler_mob : MonoBehaviour {

    RealisticEngineSound_mobile res;
    // master volume setting
    [Range(0.1f, 1.0f)]
    public float masterVolume = 1f;
    // pitch multiplier
    [Range(0.5f, 2.0f)]
    public float pitchMultiplier = 1;
    // play time
    [Range(0.5f, 4)]
    public float playTime = 2;
    // audio clips
    public AudioClip offClip;
    public AudioClip onClip;
    // audio sources
    private AudioSource offLoop;
    private AudioSource onLoop;
    // curve settings
    public AnimationCurve mufflerOffVolCurve;
    public AnimationCurve mufflerOnVolCurve;
    // private
    private float clipsValue;
    private int oneShotController = 0;

    void Start()
    {
        res = gameObject.transform.parent.GetComponent<RealisticEngineSound_mobile>();
    }
    void Update()
    {
        clipsValue = res.engineCurrentRPM / res.maxRPMLimit; // calculate % percentage of rpm
        // play on loop
        if (res.gasPedalPressing)
        {
            oneShotController = 1; // prepare for one shoot
        }
        else
        {
            // play off loop
            if (oneShotController == 1)
            {
                if (mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume > 0.09f)
                    oneShotController = 2; // one shot is played, do not play more
                else
                    oneShotController = 0;
            }
        }
        // off loop
        if (mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume > 0.09f)
        {
            if (oneShotController == 2)
            {
                if (offLoop == null)
                {
                    CreateOff();
                }
                else
                {
                    offLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                    offLoop.volume = mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume;
                }
            }
        }
        else
        {
            if (offLoop != null)
                Destroy(offLoop);
        }
        // on loop
        if (mufflerOnVolCurve.Evaluate(clipsValue) * masterVolume > 0.09f)
        {
            if (oneShotController == 1)
            {
                if (onLoop == null)
                {
                    CreateOn();
                }
                else
                {
                    onLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                    onLoop.volume = mufflerOnVolCurve.Evaluate(clipsValue) * masterVolume;
                }
            }
        }
        else
        {
            if (onLoop != null)
                Destroy(onLoop);
        }
        // destroy'em with lazers
        if (oneShotController != 2)
            Destroy(offLoop);
        if (oneShotController != 1)
            Destroy(onLoop);
    }
    // create off loop
    void CreateOff()
    {
        if (offClip != null)
        {
            offLoop = gameObject.AddComponent<AudioSource>();
            offLoop.spatialBlend = 1;
            offLoop.volume = mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume;
            offLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * 2;
            offLoop.clip = offClip;
            offLoop.loop = true;
            offLoop.Play();
            StartCoroutine(Wait2());
        }
    }
    //  create on loop
    void CreateOn()
    {
        if (onClip != null)
        {
            onLoop = gameObject.AddComponent<AudioSource>();
            onLoop.spatialBlend = 1;
            onLoop.volume = mufflerOnVolCurve.Evaluate(clipsValue) * masterVolume;
            onLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            onLoop.clip = onClip;
            onLoop.loop = true;
            onLoop.Play();
            StartCoroutine(Wait1());
        }
    }
    IEnumerator Wait1()
    {
        while (true)
        {
            yield return new WaitForSeconds(playTime); // this is needed to avoid duplicate audio sources at scene start
            oneShotController = 0;
            break;
        }
    }
    IEnumerator Wait2()
    {
        while (true)
        {
            yield return new WaitForSeconds(playTime); // this is needed to avoid duplicate audio sources at scene start
            oneShotController = 0;
            break;
        }
    }
}
