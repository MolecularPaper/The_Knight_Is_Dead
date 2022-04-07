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
}
