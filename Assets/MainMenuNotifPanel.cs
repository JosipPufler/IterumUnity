using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenuNotifPanel : MonoBehaviour
{
    public static readonly float ErrorMessageDuration = 2;

    public GameObject errorPanel;
    public TMP_Text errorLabel;

    private void Start()
    {
        errorPanel.SetActive(false);
    }

    public void SetErrorMessage(string message)
    {
        errorLabel.color = Color.red;
        errorLabel.text = message;
        errorPanel.SetActive(true);
        StartCoroutine(HideAfterTime(ErrorMessageDuration));
    }

    public void SetInfoMessage(string message)
    {
        errorLabel.color = Color.green;
        errorLabel.text = message;
        errorPanel.SetActive(true);
        StartCoroutine(HideAfterTime(ErrorMessageDuration));
    }

    public IEnumerator HideAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        errorPanel.SetActive(false);
    }
}
