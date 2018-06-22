using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    

    private static AudioManager instance;

	// Use this for initialization
	void Awake () {

        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

		foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.outputAudioMixerGroup = sound.mixer;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
	}

    private void Start()
    {
        Play("Theme");
    }

    public static void Play (string name, float pitchRange = 0)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return;
        }
        if(pitchRange != 0)
        {
            s.source.pitch = VaryPitch(pitchRange);
        }
        s.source.Play();
    }

    public static float VaryPitch(float pitchRange)
    {
        return 1f + UnityEngine.Random.Range(pitchRange, pitchRange);
    }

    public static void Mute(string name, bool mute = true)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return;
        }
        if (mute)
        {
            s.source.volume = 0;
        } else
        {
            s.source.volume = s.volume;
        }
        
    }

    public static float GetLength(string name)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return -1;
        }
        return s.source.clip.length;
    }

}
