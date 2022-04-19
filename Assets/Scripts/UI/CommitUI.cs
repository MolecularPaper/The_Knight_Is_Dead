using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CommitUI : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI text;
    public Button yesButton;

    public void GameDataDeleteCommit()
    {
        text.text = "정말로 게임 데이터를\n삭제하시겠습니까?";
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(GameDataManager.dataManager.DeleteSaveData);
        canvas.SetActive(true);
    }

    public void GameQuitCommit()
    {
        text.text = "정말로 게임을\n종료하시겠습니까?";
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { Application.Quit(); });
        canvas.SetActive(true);
    }
}
