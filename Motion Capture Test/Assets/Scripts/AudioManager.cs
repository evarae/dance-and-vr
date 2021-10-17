using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;


    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds){
            s. source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play (string name){
        Sound s = Array.Find(sounds, sound => sound.name ==name);
        s.source.Play();
    }

    public void Loop (string name){
        Sound s = Array.Find(sounds, sound => sound.name ==name);
        s.source.loop = true;
    }

    public void stopLoop (string name){
        Sound s = Array.Find(sounds, sound => sound.name ==name);
        s.source.loop = false;
    }

    public void SetVolume (string name, float newVol){

        if(newVol > 1 || newVol < 0){
            return;
        }

        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = newVol;
    }

    public bool isPlaying(string name){
        Sound s = Array.Find(sounds, sound => sound.name ==name);
        if (s == null){
            return false;
        } else {
            return s.source.isPlaying;
        }
    }
}
