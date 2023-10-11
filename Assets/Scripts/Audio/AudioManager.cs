using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.DesignPatterns;
using System;
using System.Linq;
using UniRx.Extention;
using UniRx;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource _musicPlayer;
    [SerializeField] private AudioSource _sfxPlayer;

    [SerializeField, Range(0,1)] public ReactiveProp<float> MasterVolume = new ReactiveProp<float>();
    [SerializeField, Range(0,1)] public ReactiveProp<float> MusicVolume = new ReactiveProp<float>();
    [SerializeField, Range(0,1)] public ReactiveProp<float> SfxVolume = new ReactiveProp<float>();

    [SerializeField] private AudioList _currentAudioList;

    private void Start()
    {
        MasterVolume.Value.Subscribe(newVol => AudioListener.volume = newVol);
        MusicVolume.Value.Subscribe(newVol => _musicPlayer.volume = newVol);
        SfxVolume.Value.Subscribe(newVol => _sfxPlayer.volume = newVol);

        // Initialise default values
        // TODO: initialise with settings page later
        ChangeMasterVolume(1f);
        ChangeMusicVolume(1f);
        ChangeSFXVolume(1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayAudio(SoundUID.TEST_AUDIO);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeMasterVolume(0.5f);
        }
    }

    public void PlayAudio(SoundUID soundUID)
    {
        // Get the audio track
        AudioTrack _audioTrackToPlay = _currentAudioList._tracks.Where(track => track.UniqueIdentifier == soundUID).FirstOrDefault();

        // Get the audio type
        AudioType _trackAudioType = _audioTrackToPlay.Type;

        // Play based on the specifications
        switch (_trackAudioType)
        {
            case AudioType.MUSIC:
                _musicPlayer.clip = _audioTrackToPlay.Clip;
                _musicPlayer.volume = MusicVolume.GetValue();
                _musicPlayer.loop = _audioTrackToPlay.Loop;
                _musicPlayer.Play();
                break;
            case AudioType.SFX:
                _sfxPlayer.volume = SfxVolume.GetValue();
                _sfxPlayer.PlayOneShot(_audioTrackToPlay.Clip);
                break;
        }
    }

    public void StopAudio(SoundUID soundUID)
    {
        AudioTrack _audioTrackToPlay = _currentAudioList._tracks.Where(track => track.UniqueIdentifier == soundUID).FirstOrDefault();
        AudioType _trackAudioType = _audioTrackToPlay.Type;
        // Play based on the specifications
        switch (_trackAudioType)
        {
            // Only music can be stopped lol..................
            case AudioType.MUSIC:
                _musicPlayer.Stop();
                break;
        }
    }

    public void StopAllAudio()
    {
        _musicPlayer.Stop();
        _sfxPlayer.Stop();
    }

    public void ChangeMasterVolume(float newMasterVolume)
    {
        MasterVolume.SetValue(newMasterVolume);

        // Also change the volumes of the Music and SFX (for a more visible effect ig)
        ChangeMusicVolume(MusicVolume.GetValue() * newMasterVolume);
        ChangeSFXVolume(SfxVolume.GetValue() * newMasterVolume);
    }

    public void ChangeMusicVolume(float newMusicVolume)
    {
        MusicVolume.SetValue(newMusicVolume);
    }
    public void ChangeSFXVolume(float newSFXVolume)
    {
        SfxVolume.SetValue(newSFXVolume);
    }


}


public enum SoundUID
{
    NONE,
    TEST_AUDIO
}


public enum AudioType
{
    MUSIC, // Note, there can only be one music track playing at a time
    SFX
}