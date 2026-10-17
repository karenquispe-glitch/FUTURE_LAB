using UnityEngine;

public class MenuPanelController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject configurationPanel;

    public void OpenConfiguration()
    {
        mainMenuPanel.SetActive(false);
        configurationPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        configurationPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}