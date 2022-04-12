using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class SetInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private GameObject error;

    public void SetNickName()
    {
        string check = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
        System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(check);

        if (rex.IsMatch(nickNameInput.text)) {
            error.SetActive(true);
            return;
        }

        NoDeletedData noDeletedData = new NoDeletedData(nickNameInput.text);
        GameDataManager.dataManager.SaveNoDeletedData(noDeletedData);
        gameObject.SetActive(false);

        SceneManager.LoadScene(2);
    }
}
