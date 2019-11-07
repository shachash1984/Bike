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

public class ShiftingSound : MonoBehaviour {

    RealisticEngineSound res;
    // master volume setting
    [Range(0.1f, 1.0f)]
    public float masterVolume = 1f;

    public AudioClip shiftingSoundClip;
    private AudioSource shiftingSound;
    private int playOnce = 0;
    void Start ()
    {
        res = gameObject.transform.parent.GetComponent<RealisticEngineSound>();
	}
	
	void Update ()
    {
		if(res.isShifting)
        {
            if (playOnce == 0)
            {
                CreateShiftSound();
                playOnce = 1;
            }
        }
        else
        {
            playOnce = 0;
            if (shiftingSound != null)
                Destroy(shiftingSound);
        }
	}
    void CreateShiftSound()
    {
        shiftingSound = gameObject.AddComponent<AudioSource>();
        shiftingSound.spatialBlend = 1;
        shiftingSound.volume = masterVolume;
        shiftingSound.pitch = Random.Range(0.69f, 1.69f);
        shiftingSound.loop = false;
        shiftingSound.clip = shiftingSoundClip;
        shiftingSound.Play();
    }
}
