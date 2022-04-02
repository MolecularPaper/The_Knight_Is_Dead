using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager sound;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("Sound")]
    [SerializeField] AudioClip buttonClickSound;
    [SerializeField] Slider bgmSilder;
    [SerializeField] Slider seSilder;

    void Awake()
    {
        sound = this;
        AddButtonSoundEffect();
    }

    void Start()
    {
        
    }

    public void PlaySE(AudioClip audioClip)
    {
        seSource.PlayOneShot(audioClip);
    }

    private void AddButtonSoundEffect()
    {
        EventTrigger[] allButtons = FindObjectsOfType<EventTrigger>();
        foreach (EventTrigger eventTrigger in allButtons) {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            entry.callback.AddListener((data) => { PlaySE(buttonClickSound); });
            eventTrigger.triggers.Add(entry);
        }
    }

    public void SetVolume(GameData gameData)
    {
        bgmSilder.value = gameData.bgmVolume;
        bgmSource.volume = gameData.bgmVolume;

        seSilder.value = gameData.seVolume;
        seSource.volume = gameData.seVolume;
    }

    public void ChangeBGMVolume(Slider slider)
    {
        GameManager.gm.gameData.bgmVolume = slider.value;
        bgmSource.volume = slider.value;
    }

    public void ChangeSEVolume(Slider slider)
    {
        GameManager.gm.gameData.seVolume = slider.value;
        seSource.volume = slider.value;
    }
}
