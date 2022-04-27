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
        text.text = "게임데이터를 삭제하게되면 유저 능력치와 소울, 크리스탈 모두 \n초기화 됩니다!\n정말로 초기화 하시겠습니까?";
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
