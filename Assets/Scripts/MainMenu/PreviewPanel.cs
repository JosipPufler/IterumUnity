using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviewPanel : MonoBehaviour
{
    public GameObject pnlJournals;
    public GameObject pnlPreview;
    public TMP_Text content;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            pnlJournals.SetActive(true);
            pnlPreview.SetActive(false);
        }
    }
}
