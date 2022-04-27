using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FloatingUI : MonoBehaviour
{
    public Canvas commitUI;
    public TextMeshProUGUI commitText;
    public Button commitButton;

    [Space(10)]
    public Canvas infoUI;
    public TextMeshProUGUI infoText;

    public void SetInfoUI(string text)
    {
        infoText.text = text;
        infoUI.enabled = true;
    }

    public void GameDataDeleteCommit()
    {
        commitText.text = "게임데이터를 삭제하게되면 유저 능력치와 소울, 크리스탈 모두 \n초기화 됩니다!\n정말로 초기화 하시겠습니까?";
        commitButton.onClick.RemoveAllListeners();
        commitButton.onClick.AddListener(GameDataManager.dataManager.DeleteSaveData);
        commitUI.enabled = true;
    }

    public void GameQuitCommit()
    {
        commitText.text = "정말로 게임을\n종료하시겠습니까?";
        commitButton.onClick.RemoveAllListeners();
        commitButton.onClick.AddListener(() => { Application.Quit(); });
        commitUI.enabled = true;
    }
}
