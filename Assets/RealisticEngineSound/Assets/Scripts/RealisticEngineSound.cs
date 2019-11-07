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

public class RealisticEngineSound : MonoBehaviour
{

    // master volume setting
    [Range(0.1f, 1.0f)]
    public float masterVolume = 1f;

    public float engineCurrentRPM = 0.0f;
    public bool gasPedalPressing = false;
    [Range(0.0f, 1.0f)]
    public float gasPedalValue = 1; // (simulated or not simulated) 0 = not pressing = 0 engine volume, 0.5 = halfway pressing (half engine volume), 1 = pedal to the metal (full engine volume)
    // enum for gas pedal setting
    public enum GasPedalValue { Simulated, NotSimulated } // NotSimulated setting is recommended for joystick controlled games
    public GasPedalValue gasPedalValueSetting = new GasPedalValue();
    //
    [Range(1.0f, 15.0f)]
    public float gasPedalSimSpeed = 5.5f; // simulates how fast the player hit the gas pedal
    public float maxRPMLimit = 7000;
    [Range(0.0f, 1.0f)]
    public float dopplerAmount = 0.8f; // 0 = no doppler effect (louder engine without doppler effect), 1 = full doppler effect (less loud with doppler effect)
    // enum for reverb zone setting
    public enum ReverbZone { Off, Generic, PaddedCell, Room, Bathroom, Livingroom, Stoneroom, Auditorium, Concerthall, Cave, Arena, Hangar, CarpetedHallway, Hallway, StoneCorridor, Alley, Forest, City, Mountains, Quarry, Plain, Parkinglot, Sewerpipe, Underwater, Drugged, Dizzy, Psychotic }
    public ReverbZone reverbZoneSetting = new ReverbZone();
    private ReverbZone reverbZoneControll;
    // 
    [Range(0.0f, 0.25f)]
    public float optimisationLevel = 0.01f; // audio source with volume level below this value will be destroyed
    public float minDistance = 1; // within the minimum distance the audiosources will cease to grow louder in volume
    public float maxDistance = 100; // maxDistance is the distance a sound stops attenuating at
    public bool isReversing = false; // is car in reverse gear
    public bool useRPMLimit = true; // enable rpm limit at maximum rpm
    public bool enableReverseGear = true; // enable wistle sound for reverse gear

    // hiden public stuff
    [HideInInspector]
    public float carCurrentSpeed; // needed for straight cut gearbox script
    [HideInInspector]
    public float carMaxSpeed; // needed for straight cut gearbox script
    [HideInInspector]
    public bool isShifting = false; // needed for shifting sounds script

    // idle clip sound
    public AudioClip idleClip;
    public AnimationCurve idleVolCurve;
    public AnimationCurve idlePitchCurve;
    // low rpm clip sounds
    public AudioClip lowOffClip;
    public AudioClip lowOnClip;
    public AnimationCurve lowVolCurve;
    public AnimationCurve lowPitchCurve;
    // medium rpm clip sounds
    public AudioClip medOffClip;
    public AudioClip medOnClip;
    public AnimationCurve medVolCurve;
    public AnimationCurve medPitchCurve;
    // high rpm clip sounds
    public AudioClip highOffClip;
    public AudioClip highOnClip;
    public AnimationCurve highVolCurve;
    public AnimationCurve highPitchCurve;
    // maximum rpm clip sound - if RPM limit is enabled
    public AudioClip maxRPMClip;
    public AnimationCurve maxRPMVolCurve;
    // reverse gear clip sound
    public AudioClip reversingClip;
    public AnimationCurve reversingVolCurve;
    public AnimationCurve reversingPitchCurve;

    // idle audio source
    private AudioSource engineIdle;

    // low rpm audio sources
    private AudioSource lowOff;
    private AudioSource lowOn;

    // medium rpm audio sources
    private AudioSource medOff;
    private AudioSource medOn;

    // high rpm audio sources
    private AudioSource highOff;
    private AudioSource highOn;

    //maximum rpm audio source
    private AudioSource maxRPM;

    // reverse gear audio source
    private AudioSource reversing;

    //private settings
    private float clipsValue;

    private void Start()
    {
        clipsValue = engineCurrentRPM / maxRPMLimit; // calculate % percentage of rpm
        // create and start playing audio sources
        // idle
        if (idleVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (engineIdle == null)
                CreateIdle();
        }
        //
        // low rpm
        if (lowVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (lowOn == null)
                    CreateLowOn();
            }
            else
            {
                if (lowOff == null)
                    CreateLowOff();
            }
        }
        //
        // medium rpm
        if (medVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (medOn == null)
                    CreateMedOn();
            }
            else
            {
                if (medOff == null)
                    CreateMedOff();
            }
        }
        //
        // high rpm
        if (highVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (highOn == null)
                    CreateHighOn();
            }
            else
            {
                if (highOff == null)
                    CreateHighOff();
            }
        }
        //
        // rpm limiting
        if (maxRPMVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (useRPMLimit) // if rpm limit is enabled, create audio source for it
            {
                if (maxRPM == null)
                    CreateRPMLimit();
            }
        }
        //
        // reversing gear sound
        if (enableReverseGear)
        {
            if (isReversing)
            {
                if (reversingVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (reversing == null)
                        CreateReverse();
                }
            }
            else
            {
                if (reversing != null)
                    Destroy(reversing);
            }
        }
        reverbZoneControll = reverbZoneSetting;
        SetReverbZone();
    }
    private void OnDisable() // destroy audio sources if Realistic Engine Sound's script is disabled
    {
        if (engineIdle != null)
            Destroy(engineIdle);
        if (lowOn != null)
            Destroy(lowOn);
        if (lowOff != null)
            Destroy(lowOff);
        if (medOn != null)
            Destroy(medOn);
        if (medOff != null)
            Destroy(medOff);
        if (highOn != null)
            Destroy(highOn);
        if (highOff != null)
            Destroy(highOff);

        if (useRPMLimit)
        {
            if (maxRPM != null)
                Destroy(maxRPM);
        }
        if (enableReverseGear)
        {
            if (reversing != null)
                Destroy(reversing);
        }
        if (gameObject.GetComponent<AudioReverbZone>() != null)
            Destroy(gameObject.GetComponent<AudioReverbZone>());
    }
    private void OnEnable() // recreate all audio sources if Realistic Engine Sound's script is reEnabled
    {
        StartCoroutine(WaitForStart());
        SetReverbZone();
    }
    private void Update()
    {
        clipsValue = engineCurrentRPM / maxRPMLimit; // calculate % percentage of rpm

        // gas pedal value simulation
        if (gasPedalValueSetting == GasPedalValue.Simulated)
        {
            if (gasPedalPressing)
            {
                gasPedalValue = Mathf.Lerp(gasPedalValue, 1, Time.deltaTime * gasPedalSimSpeed);
            }
            else
            {
                gasPedalValue = Mathf.Lerp(gasPedalValue, 0, Time.deltaTime * gasPedalSimSpeed);
            }
        }

        // idle
        if (idleClip != null)
        {
            if (idleVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
            {
                if (engineIdle == null)
                    CreateIdle();
                else
                {
                    engineIdle.volume = idleVolCurve.Evaluate(clipsValue) * masterVolume;
                    engineIdle.pitch = idlePitchCurve.Evaluate(clipsValue);
                }
            }
            else
            {
                Destroy(engineIdle);
            }
        }
        //
        // low rpm
        if (lowVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (lowOnClip != null)
                {
                    if (lowOn == null)
                        CreateLowOn();
                    else
                    {
                        lowOn.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                        lowOn.pitch = lowPitchCurve.Evaluate(clipsValue)  ;
                        if (lowOff != null)
                        {
                            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue)  ;
                            if (lowOff.volume < 0.1f)
                                Destroy(lowOff);
                        }
                    }
                }
            }
            else
            {
                if (lowOffClip != null)
                {
                    if (lowOff == null)
                        CreateLowOff();
                    else
                    {
                        if (!isReversing)
                        {
                            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue)  ;
                        }
                    }
                    if (lowOn != null)
                    {
                        lowOn.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                        lowOn.pitch = lowPitchCurve.Evaluate(clipsValue)  ;
                        if (lowOn.volume < 0.1f)
                            Destroy(lowOn);
                    }
                }
            }
        }
        else
        {
            if (lowOn != null)
                Destroy(lowOn);
            if (lowOff != null)
                Destroy(lowOff);
        }
        //
        // medium rpm
        if (medVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (medOnClip != null)
                {
                    if (medOn == null)
                        CreateMedOn();
                    else
                    {
                        medOn.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                        medOn.pitch = medPitchCurve.Evaluate(clipsValue)  ;
                    }
                    if (medOff != null)
                    {
                        medOff.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                        medOff.pitch = medPitchCurve.Evaluate(clipsValue)  ;
                        if (medOff.volume < 0.1f)
                            Destroy(medOff);
                    }
                }
            }
            else // gas pedal is released
            {
                if (medOffClip != null)
                {
                    if (medOff == null)
                        CreateMedOff();
                    else
                    {
                        if (!isReversing)
                        {
                            medOff.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                            medOff.pitch = medPitchCurve.Evaluate(clipsValue)  ;
                        }
                    }
                    if (medOn != null)
                    {
                        medOn.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                        medOn.pitch = medPitchCurve.Evaluate(clipsValue)  ;
                        if (medOn.volume < 0.1f)
                            Destroy(medOn);
                    }
                }
            }
        }
        else
        {
            if (medOn != null)
                Destroy(medOn);
            if (medOff != null)
                Destroy(medOff);
        }
        //
        // high rpm
        if (highVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (gasPedalPressing)
            {
                if (highOnClip != null)
                {
                    if (highOn == null)
                        CreateHighOn();
                    else
                    {
                        if (!isReversing)
                        {
                            if (maxRPM != null)
                            {
                                if (maxRPM.volume < 0.95f)
                                    highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                else
                                    highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                            }
                            else
                            {
                                highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                            }
                            highOn.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                        }
                    }
                    if (!isReversing)
                    {
                        if (highOff != null)
                        {
                            highOff.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                            highOff.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                            if (highOff.volume < 0.1f)
                                Destroy(highOff);
                        }
                    }
                }
            }
            else // gas pedal is released
            {
                if (highOffClip != null)
                {
                    if (highOff == null)
                        CreateHighOff();
                    else
                    {
                        if (!isReversing)
                        {
                            highOff.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                            highOff.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                        }
                    }
                    if (!isReversing)
                    {
                        if (highOn != null)
                        {
                            if (maxRPM != null)
                            {
                                if (maxRPM.volume < 0.95f)
                                    highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                else
                                    highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                            }
                            else
                            {
                                highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                            }
                            highOn.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                            if (highOn.volume < 0.1f)
                                Destroy(highOn);
                        }
                    }
                }
            }
        }
        else
        {
            if (highOn != null)
                Destroy(highOn);
            if (highOff != null)
                Destroy(highOff);
        }
        //
        // rpm limiting
        if (maxRPMVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
        {
            if (maxRPMClip != null)
            {
                if (useRPMLimit) // if rpm limit is enabled, create audio source for it
                {
                    if (maxRPM == null)
                        CreateRPMLimit();
                    else
                    {
                        maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue) * masterVolume;
                        maxRPM.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                    }
                }
            }
            else // missing rpm limit audio clip
            {
                useRPMLimit = false;
            }
        }
        else
        {
            Destroy(maxRPM);
        }
        //
        // reversing gear sound
        if (enableReverseGear)
        {
            if (reversingClip != null)
            {
                if (isReversing)
                {
                    if (reversingVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                    {
                        if (reversing == null)
                            CreateReverse();
                        else
                        {
                            if (gasPedalPressing)
                            {
                                if (highOn == null)
                                    CreateHighOn();
                                else
                                {
                                    if (maxRPM != null)
                                    {
                                        if (maxRPM.volume < 0.95f)
                                            highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                        else
                                            highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                                    }
                                    else
                                    {
                                        highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                    }
                                    highOn.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                                }
                                if (highOff != null)
                                {
                                    highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                                    highOff.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                                    if (highOff.volume < 0.1f)
                                        Destroy(highOff);
                                }
                            }
                            else // gas pedal is released
                            {
                                if (highOff == null)
                                    CreateHighOff();
                                else
                                {
                                    highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                                    highOff.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                                }
                                if (highOn != null)
                                {
                                    if (maxRPM != null)
                                    {
                                        if (maxRPM.volume < 0.95f)
                                            highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                        else
                                            highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                                    }
                                    else
                                    {
                                        highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                    }
                                    highOn.pitch = highPitchCurve.Evaluate(clipsValue)  ;
                                    if (highOn.volume < 0.1f)
                                        Destroy(highOn);
                                }
                            }
                            // set reversing sound to setted settings
                            reversing.volume = reversingVolCurve.Evaluate(clipsValue) * masterVolume;
                            reversing.pitch = reversingPitchCurve.Evaluate(clipsValue)  ;
                        }
                    }
                    else
                    {
                        if (reversing != null)
                            Destroy(reversing);
                    }
                }
                else
                {
                    if (reversing != null)
                        Destroy(reversing);
                }
            }
            else
            {
                isReversing = false;
                enableReverseGear = false; // disable reversing sound because there is no audio clip for it
            }
        }
        else
        {
            if (isReversing != false)
                isReversing = false;
        }
    }
    private void LateUpdate()
    {
        if (!enableReverseGear)
        {
            if (reversing != null)
            {
                Destroy(reversing); // looks like someone disabled reversing on runtime, destroy this audio source
            }
        }
        // rpm limiting
        if (useRPMLimit) // if rpm limit is enabled, create audio source for it
        {
            if (maxRPM == null)
                CreateRPMLimit();
        }
        else // if disabled, destroy rpm limit's audio source
        {
            if (maxRPM != null)
                Destroy(maxRPM);
        }

        // reverb setting is changed
        if (reverbZoneControll != reverbZoneSetting)
            SetReverbZone();
    }
    void SetReverbZone()
    {
        // reverb zone
        // off
        if (reverbZoneSetting == ReverbZone.Off)
        {
            if (gameObject.GetComponent<AudioReverbZone>() != null)
                Destroy(gameObject.GetComponent<AudioReverbZone>());
        }
        // alley
        if (reverbZoneSetting == ReverbZone.Alley)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Alley;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Alley)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Alley;
            }
        }
        // arena
        if (reverbZoneSetting == ReverbZone.Arena)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Arena;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Arena)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Arena;
            }
        }
        // auditorium
        if (reverbZoneSetting == ReverbZone.Auditorium)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Auditorium;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Auditorium)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Auditorium;
            }
        }
        // bathroom
        if (reverbZoneSetting == ReverbZone.Bathroom)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Bathroom;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Bathroom)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Bathroom;
            }
        }
        // carpeted hallway
        if (reverbZoneSetting == ReverbZone.CarpetedHallway)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.CarpetedHallway;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.CarpetedHallway)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.CarpetedHallway;
            }
        }
        // cave
        if (reverbZoneSetting == ReverbZone.Cave)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Cave;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Cave)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Cave;
            }
        }
        // city
        if (reverbZoneSetting == ReverbZone.City)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.City;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.City)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.City;
            }
        }
        // concerthall
        if (reverbZoneSetting == ReverbZone.Concerthall)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Concerthall;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Concerthall)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Concerthall;
            }
        }
        // dizzy
        if (reverbZoneSetting == ReverbZone.Dizzy)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Dizzy;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Dizzy)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Dizzy;
            }
        }
        // drugged
        if (reverbZoneSetting == ReverbZone.Drugged)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Drugged;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Drugged)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Drugged;
            }
        }
        // forest
        if (reverbZoneSetting == ReverbZone.Forest)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Forest;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Forest)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Forest;
            }
        }
        // generic
        if (reverbZoneSetting == ReverbZone.Generic)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Generic;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Generic)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Generic;
            }
        }
        // hallway
        if (reverbZoneSetting == ReverbZone.Hallway)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Hallway;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Hallway)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Hallway;
            }
        }
        // hangar
        if (reverbZoneSetting == ReverbZone.Hangar)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Hangar;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Hangar)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Hangar;
            }
        }
        // livingroom
        if (reverbZoneSetting == ReverbZone.Livingroom)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Livingroom;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Livingroom)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Livingroom;
            }
        }
        // mountains
        if (reverbZoneSetting == ReverbZone.Mountains)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Mountains;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Mountains)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Mountains;
            }
        }
        // padded cell
        if (reverbZoneSetting == ReverbZone.PaddedCell)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.PaddedCell;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.PaddedCell)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.PaddedCell;
            }
        }
        // parkinglot
        if (reverbZoneSetting == ReverbZone.Parkinglot)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.ParkingLot;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.ParkingLot)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.ParkingLot;
            }
        }
        // plain
        if (reverbZoneSetting == ReverbZone.Plain)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Plain;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Plain)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Plain;
            }
        }
        // psychotic
        if (reverbZoneSetting == ReverbZone.Psychotic)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Psychotic;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Psychotic)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Psychotic;
            }
        }
        // quarry
        if (reverbZoneSetting == ReverbZone.Quarry)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Quarry;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Quarry)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Quarry;
            }
        }
        // room
        if (reverbZoneSetting == ReverbZone.Room)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Room;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Room)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Room;
            }
        }
        // sewerpipe
        if (reverbZoneSetting == ReverbZone.Sewerpipe)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.SewerPipe;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.SewerPipe)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.SewerPipe;
            }
        }
        // stone corridor
        if (reverbZoneSetting == ReverbZone.StoneCorridor)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.StoneCorridor;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.StoneCorridor)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.StoneCorridor;
            }
        }
        // stoneroom
        if (reverbZoneSetting == ReverbZone.Stoneroom)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Stoneroom;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Stoneroom)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Stoneroom;
            }
        }
        // underwater
        if (reverbZoneSetting == ReverbZone.Underwater)
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Underwater;
            }
            else
            {
                if (gameObject.GetComponent<AudioReverbZone>().reverbPreset != AudioReverbPreset.Underwater)
                    gameObject.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Underwater;
            }
        }
        reverbZoneControll = reverbZoneSetting;
    }
    IEnumerator WaitForStart()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f); // this is needed to avoid duplicate audio sources at scene start
            if (engineIdle == null)
                Start();
            break;
        }
    }
    // create audio sources
    // idle
    void CreateIdle()
    {
        if (idleClip != null)
        {
            engineIdle = gameObject.AddComponent<AudioSource>();
            engineIdle.spatialBlend = dopplerAmount;
            engineIdle.volume = idleVolCurve.Evaluate(clipsValue) * masterVolume;
            engineIdle.pitch = idlePitchCurve.Evaluate(clipsValue)  ;
            engineIdle.minDistance = minDistance;
            engineIdle.maxDistance = maxDistance;
            engineIdle.clip = idleClip;
            engineIdle.loop = true;
            engineIdle.Play();
        }
    }
    // low
    void CreateLowOff()
    {
        if (lowOffClip != null)
        {
            lowOff = gameObject.AddComponent<AudioSource>();
            lowOff.spatialBlend = dopplerAmount;
            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue)  ;
            lowOff.minDistance = minDistance;
            lowOff.maxDistance = maxDistance;
            lowOff.clip = lowOffClip;
            lowOff.loop = true;
            lowOff.Play();
        }
    }
    void CreateLowOn()
    {
        if (lowOnClip != null)
        {
            lowOn = gameObject.AddComponent<AudioSource>();
            lowOn.spatialBlend = dopplerAmount;
            lowOn.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
            lowOn.pitch = lowPitchCurve.Evaluate(clipsValue)  ;
            lowOn.minDistance = minDistance;
            lowOn.maxDistance = maxDistance;
            lowOn.clip = lowOnClip;
            lowOn.loop = true;
            lowOn.Play();
        }
    }
    // medium
    void CreateMedOff()
    {
        if (medOffClip != null)
        {
            medOff = gameObject.AddComponent<AudioSource>();
            medOff.spatialBlend = dopplerAmount;
            medOff.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
            medOff.pitch = medPitchCurve.Evaluate(clipsValue)  ;
            medOff.minDistance = minDistance;
            medOff.maxDistance = maxDistance;
            medOff.clip = medOffClip;
            medOff.loop = true;
            medOff.Play();
        }
    }
    void CreateMedOn()
    {
        if (medOnClip != null)
        {
            medOn = gameObject.AddComponent<AudioSource>();
            medOn.spatialBlend = dopplerAmount;
            medOn.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
            medOn.pitch = medPitchCurve.Evaluate(clipsValue)  ;
            medOn.minDistance = minDistance;
            medOn.maxDistance = maxDistance;
            medOn.clip = medOnClip;
            medOn.loop = true;
            medOn.Play();
        }
    }
    // high
    void CreateHighOff()
    {
        if (highOffClip != null)
        {
            highOff = gameObject.AddComponent<AudioSource>();
            highOff.spatialBlend = dopplerAmount;
            highOff.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
            highOff.pitch = highPitchCurve.Evaluate(clipsValue)  ;
            highOff.minDistance = minDistance;
            highOff.maxDistance = maxDistance;
            highOff.clip = highOffClip;
            highOff.loop = true;
            highOff.Play();
        }
    }
    void CreateHighOn()
    {
        if (highOnClip != null)
        {
            highOn = gameObject.AddComponent<AudioSource>();
            highOn.spatialBlend = dopplerAmount;
            highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
            highOn.pitch = highPitchCurve.Evaluate(clipsValue)  ;
            highOn.minDistance = minDistance;
            highOn.maxDistance = maxDistance;
            highOn.clip = highOnClip;
            highOn.loop = true;
            highOn.Play();
        }
    }
    // rpm limit
    void CreateRPMLimit()
    {
        if (maxRPMClip != null)
        {
            maxRPM = gameObject.AddComponent<AudioSource>();
            maxRPM.spatialBlend = dopplerAmount;
            maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue) * masterVolume;
            maxRPM.pitch = highPitchCurve.Evaluate(clipsValue)  ;
            maxRPM.minDistance = minDistance;
            maxRPM.maxDistance = maxDistance;
            maxRPM.clip = maxRPMClip;
            maxRPM.loop = true;
            maxRPM.Play();
        }
    }
    // reversing
    void CreateReverse()
    {
        if (reversingClip != null)
        {
            reversing = gameObject.AddComponent<AudioSource>();
            reversing.spatialBlend = dopplerAmount;
            reversing.volume = reversingVolCurve.Evaluate(clipsValue) * masterVolume;
            reversing.pitch = reversingPitchCurve.Evaluate(clipsValue)  ;
            reversing.minDistance = minDistance;
            reversing.maxDistance = maxDistance;
            reversing.clip = reversingClip;
            reversing.loop = true;
            reversing.Play();
        }
    }
}
