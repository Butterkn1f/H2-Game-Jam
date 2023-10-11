using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This file contains a majority of the audio objects
namespace Audio
{
    [System.Serializable]
    public class AudioObject
    {
        // The unique indentifier of an audio clip
        public SoundUID AudioID;
        public AudioClip Clip; // The physical audio clip
        public AudioType Type;
    }

    // Audio Type
    public enum AudioType
    {
        MUSIC,
        SFX
    }

    // Action to be performed on the audio clip
    public enum AudioAction
    {
        START,
        STOP,
        RESTART
    }

    // Add audio type
    [System.Serializable]
    public class AudioTrack 
    {
        public AudioSource Source; // Source of audio to play
        public AudioObject[] Audio; // Audio to play
    }

    // Class to represent an action to be performed on an audio clip
    [System.Serializable]
    public class AudioJob
    {
        public AudioAction Action;
        public SoundUID AudioID;

        // Audio effects
        public bool Loop;
        public bool Fade;
        public float FadeDuration;
        public WaitForSeconds Delay;

        public AudioJob (AudioAction action, SoundUID audioID, bool loop = false, bool fade = false, float delay = 0f, float fadeDuration = 0f)
        {
            Action = action;
            AudioID = audioID;
            Loop = loop;
            Fade = fade;
            FadeDuration = fadeDuration;
            Delay = (delay > 0f) ? new WaitForSeconds(delay) : null;
        }
    }
}

