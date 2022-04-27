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
        text.text = "���ӵ����͸� �����ϰԵǸ� ���� �ɷ�ġ�� �ҿ�, ũ����Ż ��� \n�ʱ�ȭ �˴ϴ�!\n������ �ʱ�ȭ �Ͻðڽ��ϱ�?";
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(GameDataManager.dataManager.DeleteSaveData);
        canvas.SetActive(true);
    }

    public void GameQuitCommit()
    {
        text.text = "������ ������\n�����Ͻðڽ��ϱ�?";
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { Application.Quit(); });
        canvas.SetActive(true);
    }
}
