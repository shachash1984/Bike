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

public class ShiftingSoundRandom : MonoBehaviour {

    RealisticEngineSound res;
    // master volume setting
    [Range(0.1f, 1.0f)]
    public float masterVolume = 1f;
    // shift sound clips
    public AudioClip[] shiftingSoundClips;

    private AudioSource shiftingSound;
    private int playOnce = 0;

    void Start()
    {
        res = gameObject.transform.parent.GetComponent<RealisticEngineSound>();
        // create and set audio source for shifting sounds
        shiftingSound = gameObject.AddComponent<AudioSource>();
        shiftingSound.spatialBlend = 1;
        shiftingSound.loop = false;
        shiftingSound.Stop();
    }
    private void OnDisable() // destroy audio sources if disabled
    {
        if (shiftingSound != null)
            Destroy(shiftingSound);
    }
    private void OnEnable() // recreate all audio sources if Realistic Engine Sound's script is reEnabled
    {
        StartCoroutine(WaitForStart());
    }
    void Update()
    {
        if (res.isShifting)
        {
            // play shift sound only once
            if (playOnce == 0)
            {
                CreateShiftSound();
                playOnce = 1;
            }
        }
        else
        {
            playOnce = 0; // waiting for next gear shifting
        }
    }
    IEnumerator WaitForStart()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f); // this is needed to avoid duplicate audio sources at scene start
            if (shiftingSound == null)
                Start();
            break;
        }
    }
    // choose and set a random shifting sound
    void CreateShiftSound()
    {
        if (shiftingSound != null)
        {
            shiftingSound.clip = shiftingSoundClips[Random.Range(0, shiftingSoundClips.Length)]; // random clip
            shiftingSound.volume = masterVolume;
            shiftingSound.pitch = Random.Range(0.69f, 1.69f); // random pitch
            shiftingSound.Play();
        }
    }
}
