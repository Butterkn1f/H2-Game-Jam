using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    // Sliders for volume
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        _masterSlider.value = AudioManager.Instance.MasterVolume.GetValue();
        _musicSlider.value = AudioManager.Instance.MusicVolume.GetValue();
        _sfxSlider.value = AudioManager.Instance.SfxVolume.GetValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterSlider()
    {
        AudioManager.Instance.ChangeMasterVolume(_masterSlider.value);
    }

    public void SetMusicSlider()
    {
        AudioManager.Instance.ChangeMusicVolume(_musicSlider.value);
    }

    public void SetSFXSlider()
    {
        AudioManager.Instance.ChangeSFXVolume(_sfxSlider.value);
    }
}
