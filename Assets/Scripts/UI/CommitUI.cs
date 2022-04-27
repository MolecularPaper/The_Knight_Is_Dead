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
        commitText.text = "���ӵ����͸� �����ϰԵǸ� ���� �ɷ�ġ�� �ҿ�, ũ����Ż ��� \n�ʱ�ȭ �˴ϴ�!\n������ �ʱ�ȭ �Ͻðڽ��ϱ�?";
        commitButton.onClick.RemoveAllListeners();
        commitButton.onClick.AddListener(GameDataManager.dataManager.DeleteSaveData);
        commitUI.enabled = true;
    }

    public void GameQuitCommit()
    {
        commitText.text = "������ ������\n�����Ͻðڽ��ϱ�?";
        commitButton.onClick.RemoveAllListeners();
        commitButton.onClick.AddListener(() => { Application.Quit(); });
        commitUI.enabled = true;
    }
}
