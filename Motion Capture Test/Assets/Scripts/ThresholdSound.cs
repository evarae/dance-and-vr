using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdSound : MonoBehaviour
{
    
    public VelocityTracker velTrack;
    public string soundName = "lowGat";

    [Range(0.1f, 50f)]
    public float threshold = 2f;

    private AudioManager am;
    

    // Start is called before the first frame update
    void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update(){

        float speed = velTrack.getSpeed();

        bool isPlaying = am.isPlaying(soundName);


        if (speed > threshold && isPlaying == false ){
            am.Play(soundName);
            print("reached threshold, speed is: " + speed + ", playing: " + soundName);
        }

    }
}
