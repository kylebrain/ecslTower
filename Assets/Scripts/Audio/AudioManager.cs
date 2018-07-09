using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;
    public string[] volumes;
    public AudioMixer mainMixer;

    private static AudioManager instance;

    // Use this for initialization
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
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
        foreach (string str in volumes)
        {
            mainMixer.SetFloat(str, PlayerPrefs.GetFloat(str, 0f));
        }
        PlayLoop("MusicLoop", "MusicIntro");
    }

    public static void Play(string name, float pitchRange = 0)
    {
        Sound s = instance.GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return;
        }
        if (pitchRange != 0)
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
        Sound s = instance.GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return;
        }
        if (mute)
        {
            s.source.volume = 0;
        }
        else
        {
            s.source.volume = s.volume;
        }

    }

    public static float GetLength(string name)
    {
        Sound s = instance.GetSound(name);
        if (s == null)
        {
            Debug.LogWarning("Could not find the sound: " + name);
            return -1;
        }
        return s.source.clip.length;
    }

    public Sound GetSound(string name)
    {
        return Array.Find(instance.sounds, sound => sound.name == name);
    }

    public static void PlayLoop(string loop, string intro = null)
    {
        AudioSource loopSource;
        AudioSource introSource;
        if (string.IsNullOrEmpty(loop))
        {
            Debug.LogError("Must have a valid loop!");
            return;
        } else
        {
            loopSource = instance.GetSound(loop).source;
        }

        if (!string.IsNullOrEmpty(intro))
        {
            introSource = instance.GetSound(intro).source;
        } else
        {
            introSource = null;
        }

        instance.StartCoroutine(instance.PlayIntro(loopSource, introSource));
    }

    IEnumerator PlayIntro(AudioSource loop, AudioSource intro = null)
    {
        if(intro == null)
        {
            yield return null;
        } else
        {
            intro.loop = false;
            intro.Play();
            yield return new WaitForSeconds(intro.clip.length / intro.pitch);
        }
        loop.loop = true;
        loop.Play();
    }

}
