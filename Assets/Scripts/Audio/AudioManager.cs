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

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
	}

    private void Start()
    {
        Play("Theme");
    }

    public static void Play (string name)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return;
        }
    }

}
