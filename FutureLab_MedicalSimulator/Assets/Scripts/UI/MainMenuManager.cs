using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject clinicalCasesPanel;

    public void OpenClinicalCases()
    {
        mainMenuPanel.SetActive(false);
        clinicalCasesPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        clinicalCasesPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void StartSimulation()
    {
        SceneManager.LoadScene("CinematicaAmbulancia");
    }
}