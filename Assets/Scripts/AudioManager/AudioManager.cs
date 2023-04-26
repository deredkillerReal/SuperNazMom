using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("why are there two Audio Manager");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixerGroup;

        }
    }
    private void Start()
    {
        Play("Music2");
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source.isPlaying)
        {

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixerGroup;
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
            s.source.Play();
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.Play();
    }
    public void Stop(string sound,int toStop)
    {
        Sound[] s = Array.FindAll(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: '" + name + "' not found! couldnt stop.");
            return;
        }
        if (toStop == 0) toStop = s.Length;
        for (int i = 0; i < toStop; i++)
        {
        s[i].source.Stop();

        }
    }
    public void SetVolume(float a)
    {
        Debug.Log(a);

    }

}
