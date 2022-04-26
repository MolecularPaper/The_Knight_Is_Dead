using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private SetInfoUI setInfoUI;
    [SerializeField] private TextMeshProUGUI version;
    
    private bool noData;


    void Start()
    {
        try {
            GameDataManager.dataManager.LoadGameData();
        }
        catch(System.IO.DirectoryNotFoundException) {
            noData = true;
        }

        version.text = Application.version;
    }

    public void GameStart()
    {
        if (noData) {
            setInfoUI.gameObject.SetActive(true);
        }
        else {
            SceneManager.LoadScene(2);
        }
    }
}
