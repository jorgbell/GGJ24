using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("AudioManager instanced");
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.pitch = s.minPitch;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    private void Start()
    {
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("NO SE ENCUENTRA EL AUDIO: " + name);
            return;

        }
        if (s.minPitch != s.maxPitch)
        {
            s.source.pitch = UnityEngine.Random.Range(s.minPitch, s.maxPitch);
        }
        s.source.Play();
        Debug.Log("Esto suena");
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("NO SE ENCUENTRA EL AUDIO: " + name);
            return;

        }
        if (s.source.isPlaying) s.source.Stop();
    }

}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1.0f;
    [Range(1f, 3f)]
    public float minPitch = 1.0f;
    [Range(1f, 3f)]
    public float maxPitch = 1.0f;

    public bool loop;



    [HideInInspector]
    public AudioSource source;
}


