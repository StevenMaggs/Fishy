using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        UpdateVolumes();

        instance.Play("Background", GameManager.instance.volumeMusic);
    }

    public void Play(string name, float volume)
    {
        Sound s = System.Array.Find(sounds, x => x.name == name);
        s.source.volume *= volume;
        s.source.Play();
    }

    public void UpdateVolumes()
    {
        AudioListener.volume = GameManager.instance.volumeMaster;

        foreach (Sound s in sounds)
        {
            if (s.type == "sfx")
                s.source.volume = GameManager.instance.volumeSFX;
            else if (s.type == "music")
                s.source.volume = GameManager.instance.volumeMusic;
        }
    }
}