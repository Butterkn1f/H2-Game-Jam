using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.DesignPatterns;
using System;
using System.Linq;
using UniRx.Extention;
using UniRx;
using DG.Tweening;

public class AudioManager : SingletonPersistent<AudioManager>
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
        

        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeMasterVolume(0.5f);
        }
    }

    public void PlayAudio(SoundUID soundUID, bool fade = true, float playAtTime = 0)
    {
        // Get the audio track
        AudioTrack _audioTrackToPlay = _currentAudioList._tracks.Where(track => track.UniqueIdentifier == soundUID).FirstOrDefault();

        // Get the audio type
        AudioType _trackAudioType = _audioTrackToPlay.Type;


        // Play based on the specifications
        switch (_trackAudioType)
        {
            case AudioType.MUSIC:
                Sequence fadeAudio = DOTween.Sequence();

                // Check if is currently playing anyt
                if (_musicPlayer.isPlaying)
                {
                    if (_musicPlayer.clip == _audioTrackToPlay.Clip)
                    {
                        return;
                    }

                    fadeAudio.Append(_musicPlayer.DOFade(0, 0.25f));
                    fadeAudio.AppendCallback(() => _musicPlayer.Stop());
                    fadeAudio.AppendCallback(() =>
                    {
                        _musicPlayer.clip = _audioTrackToPlay.Clip;
                        _musicPlayer.volume = MusicVolume.GetValue();
                        _musicPlayer.loop = _audioTrackToPlay.Loop;
                        _musicPlayer.time = playAtTime;
                        _musicPlayer.Play();
                    });
                }
                else
                {
                    _musicPlayer.clip = _audioTrackToPlay.Clip;
                    _musicPlayer.volume = MusicVolume.GetValue();
                    _musicPlayer.loop = _audioTrackToPlay.Loop;
                    _musicPlayer.time = playAtTime;
                    _musicPlayer.Play();
                }
                
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
        ChangeMusicVolume(MusicVolume.GetValue());
    }

    public void ChangeMusicVolume(float newMusicVolume)
    {
        MusicVolume.SetValue(newMusicVolume);
    }
    public void ChangeSFXVolume(float newSFXVolume)
    {
        SfxVolume.SetValue(newSFXVolume);
    }

    public float GetCurrentMusicTime()
    {
        return _musicPlayer.time;
    }

    public void PlayClickSound()
    {
        PlayAudio(SoundUID.CLICK_SOUND);
    }

    public void PlayBellSound()
    {
        PlayAudio(SoundUID.CLICK_SOUND);
    }

}


public enum SoundUID
{
    NONE,
    TEST_AUDIO,

    // Music
    CHAT_AUDIO,
    MAIN_GAME_AUDIO,
    FRENZY_AUDIO,
    RESULTS_AUDIO,

    MENU_AUDIO,
    MAP_SELECT_AUDIO,

    // SFX
    CORRECT_ORDER,
    WRONG_ORDER,
    POOF,
    CLICK_SOUND,
    METAL_SHUTTER,
    BELL
}


public enum AudioType
{
    MUSIC, // Note, there can only be one music track playing at a time
    SFX
}