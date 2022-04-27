using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public class SoundManager : MonoBehaviour, IGameObserver
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

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.Subscribe(this);
    }


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        isPlayBgm = false;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.Subscribe(this);
    }

    private void ResetVolume(GameInfo gameInfo)
    {
        bgmSilder = GameObject.FindWithTag("BgmSlider").GetComponent<Slider>();
        bgmSilder.onValueChanged.AddListener(ChangeBGMVolume);

        seSilder = GameObject.FindWithTag("SeSlider").GetComponent<Slider>();
        seSilder.onValueChanged.AddListener(ChangeSEVolume);

        bgmSilder.value = gameInfo.bgmVolume;
        bgmSource.volume = gameInfo.bgmVolume;
        seSilder.value = gameInfo.seVolume;
        seSource.volume = gameInfo.seVolume;
    }

    public void PlaySE(AudioClip audioClip)
    {
        seSource.PlayOneShot(audioClip);
    }

    private void AddButtonSoundEffect()
    {
        EventTrigger[] allTriggers = FindObjectsOfType<EventTrigger>();
        foreach (EventTrigger eventTrigger in allTriggers) {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            entry.callback.AddListener((data) => { PlaySE(buttonClickSound); });
            eventTrigger.triggers.Add(entry);
        }

        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons) {
            button.onClick.AddListener(() => {
                PlaySE(buttonClickSound);
            });
        }
    }

    public void ChangeBGMVolume(float value)
    {
        bgmSource.volume = bgmSilder.value;
        GameManager.gm.bgmVolume = bgmSilder.value;
    }

    public void ChangeSEVolume(float value)
    {
        seSource.volume = seSilder.value;
        GameManager.gm.seVolume = seSilder.value;
    }

    public void GameUpdated(GameObservable gameInfo)
    {
        ResetVolume(gameInfo);
        gameInfo.Unsubscribe(this);
    }
}
