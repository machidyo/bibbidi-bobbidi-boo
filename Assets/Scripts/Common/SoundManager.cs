using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const float DEFAULT_VOLUME = 0.5f;

    public enum SoundTypes
    {
        SE,
    }

    public enum SoundNames
    {
        BibbidiBobbidi,
        Boo
    }

    private Dictionary<SoundTypes, AudioSource> audioSources = new();

    private List<Sound> sounds = new()
    {
        new Sound(SoundNames.BibbidiBobbidi, SoundTypes.SE, "Sounds/きらきら輝く1"),
        new Sound(SoundNames.Boo, SoundTypes.SE, "Sounds/magic-turning-spells-casting_MJtx4SEu"),
    };  

    // PlaySound が実行される前に AudioSource を設定するために Awake
    void Awake()
    {
        foreach (var soundType in Enum.GetValues(typeof(SoundTypes)))
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = DEFAULT_VOLUME;
            audioSources.Add((SoundTypes)soundType, audioSource);
        }
    }

    public void PlaySound(SoundNames soundName)
    {
        if (sounds.Any(s => s.Name == soundName))
        {
            var sound = sounds.First(s => s.Name == soundName);
            if (!sound.HasCash)
            {
                sound.Clip = Resources.Load<AudioClip>(sound.Path);
            }

            audioSources[sound.Type].PlayOneShot(sound.Clip);
        }
        else
        {
            Debug.Log($"{soundName} is not found.");
        }
    }

    public void PlaySound(int typeNum)
    {
        PlaySound((SoundNames)typeNum);
    }

    public void PlaySound(AudioClip audioClip, SoundTypes soundTypes)
    {
        audioSources[soundTypes].PlayOneShot(audioClip);
    }

    public void StopSound(SoundTypes soundType)
    {
        audioSources[soundType].Stop();
    }

    public void PauseSound(SoundTypes soundType)
    {
        audioSources[soundType].Pause();
    }

    public void UnpauseSound(SoundTypes soundType)
    {
        audioSources[soundType].UnPause();
    }

    private class Sound
    {
        public SoundNames Name { get; }
        public SoundTypes Type { get; }
        public string Path { get; }
        public AudioClip Clip;

        public bool HasCash => Clip != null;

        public Sound(SoundNames soundName, SoundTypes soundType, string audioPath)
        {
            Name = soundName;
            Type = soundType;
            Path = audioPath;
        }
    }
}