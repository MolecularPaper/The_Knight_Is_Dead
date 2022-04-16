using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public class SoundManager : MonoBehaviour
{
    public static SoundManager sound;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("Sound")]
    [SerializeField] AudioClip buttonClickSound;
    [SerializeField] Slider bgmSilder;
    [SerializeField] Slider seSilder;

    public bool isPlayBgm;

    void Awake()
    {
        if(sound == null) {
            sound = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this.gameObject);
        }

        AddButtonSoundEffect();

        if (isPlayBgm) {
            bgmSource.Play();
        }
    }

    private void Start()
    {
        ResetVolume();
    }

    private void OnApplicationQuit()
    {
        isPlayBgm = false;
    }

    private void ResetVolume()
    {
        bgmSilder.value = GameManager.gm.bgmVolume;
        bgmSource.volume = GameManager.gm.bgmVolume;
        seSilder.value = GameManager.gm.seVolume;
        seSource.volume = GameManager.gm.seVolume;
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

    public void ChangeBGMVolume()
    {
        bgmSource.volume = bgmSilder.value;
        GameManager.gm.bgmVolume = bgmSilder.value;
    }

    public void ChangeSEVolume()
    {
        seSource.volume = seSilder.value;
        GameManager.gm.seVolume = seSilder.value;
    }
}
