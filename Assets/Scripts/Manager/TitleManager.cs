using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameDataManager dataManager;
    [SerializeField] private TitleData titleData;

    public bool IntroEnd {
        get => titleData.introEnd;
        set {
            titleData.introEnd = value;
            SaveData();
        }
    }

    void Awake()
    {
        try {
            titleData = dataManager.LoadTitleData();
        }
        catch { }
    }

    public void GameStart()
    {
        if (titleData.introEnd) {
            SceneManager.LoadScene(1);
        }
        else {
            SceneManager.LoadScene(2);
        }
    }

    public void SaveData()
    {
        dataManager.SaveTitleData(titleData);
    }
}
