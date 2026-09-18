using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject clinicalCasesPanel;

    [Header("Botón Continuar")]
    public Button continueButton;

    // =====================================================
    // PARTIDA PENDIENTE DE CARGAR
    // =====================================================

    public static SaveData pendingSaveData;

    // =====================================================
    // INICIO
    // =====================================================

    private void Start()
    {
        UpdateContinueButton();
    }

    // =====================================================
    // ACTUALIZAR BOTÓN CONTINUAR
    // =====================================================

    private void UpdateContinueButton()
    {
        if (continueButton == null)
        {
            Debug.LogWarning(
                "MainMenuManager: no se asignó el botón CONTINUAR."
            );

            return;
        }

        bool hasSavedGame = false;

        if (SaveManager.Instance != null)
        {
            hasSavedGame = SaveManager.Instance.HasSavedGame();
        }

        continueButton.interactable = hasSavedGame;

        Debug.Log(
            "Botón CONTINUAR: " +
            (hasSavedGame ? "HABILITADO" : "DESHABILITADO")
        );
    }

    // =====================================================
    // MENÚ DE CASOS CLÍNICOS
    // =====================================================

    public void OpenClinicalCases()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (clinicalCasesPanel != null)
        {
            clinicalCasesPanel.SetActive(true);
        }
    }

    public void BackToMainMenu()
    {
        if (clinicalCasesPanel != null)
        {
            clinicalCasesPanel.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        UpdateContinueButton();
    }

    // =====================================================
    // INICIAR NUEVA SIMULACIÓN
    // =====================================================

    public void StartSimulation()
    {
        // Una partida nueva no debe utilizar datos anteriores.
        pendingSaveData = null;

        // Eliminar cualquier partida anterior.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();

            Debug.Log(
                "PARTIDA ANTERIOR ELIMINADA."
            );
        }
        else
        {
            Debug.LogWarning(
                "MainMenuManager: no existe SaveManager."
            );
        }

        Debug.Log(
            "INICIANDO NUEVA SIMULACIÓN."
        );

        SceneManager.LoadScene(
            "CinematicaAmbulancia"
        );
    }

    // =====================================================
    // CONTINUAR PARTIDA
    // =====================================================

    public void ContinueGame()
    {
        Debug.Log(
            "VERIFICANDO PARTIDA GUARDADA..."
        );

        // -------------------------------------------------
        // COMPROBAR SAVEMANAGER
        // -------------------------------------------------

        if (SaveManager.Instance == null)
        {
            Debug.LogError(
                "MainMenuManager: no existe un SaveManager activo."
            );

            return;
        }

        // -------------------------------------------------
        // COMPROBAR SI EXISTE PARTIDA
        // -------------------------------------------------

        if (!SaveManager.Instance.HasSavedGame())
        {
            Debug.LogWarning(
                "NO EXISTE NINGUNA PARTIDA GUARDADA."
            );

            if (continueButton != null)
            {
                continueButton.interactable = false;
            }

            return;
        }

        // -------------------------------------------------
        // CARGAR PARTIDA
        // -------------------------------------------------

        SaveData data =
            SaveManager.Instance.LoadGame();

        // -------------------------------------------------
        // COMPROBAR DATOS
        // -------------------------------------------------

        if (data == null)
        {
            Debug.LogError(
                "MainMenuManager: no se pudo cargar la partida."
            );

            if (continueButton != null)
            {
                continueButton.interactable = false;
            }

            return;
        }

        // -------------------------------------------------
        // GUARDAR DATOS TEMPORALMENTE
        // -------------------------------------------------

        pendingSaveData = data;

        Debug.Log(
            "PARTIDA PREPARADA PARA CONTINUAR."
        );

        Debug.Log(
            "Caso: " +
            data.caseID
        );

        Debug.Log(
            "Fase: " +
            data.currentPhase
        );

        Debug.Log(
            "Tiempo: " +
            data.elapsedTime
        );

        Debug.Log(
            "Preguntas respondidas: " +
            (
                data.answeredQuestions != null
                    ? data.answeredQuestions.Count
                    : 0
            )
        );

        // -------------------------------------------------
        // CARGAR ESCENARIO CLÍNICO
        // -------------------------------------------------

        SceneManager.LoadScene(
            "Hospital_Base"
        );
    }
}