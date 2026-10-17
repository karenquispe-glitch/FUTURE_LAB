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

    [Header("Confirmación de nueva partida")]
    public GameObject confirmNewGamePanel;

    // =====================================================
    // PARTIDA PENDIENTE DE CARGAR
    // =====================================================

    public static SaveData pendingSaveData;

    // =====================================================
    // INICIO
    // =====================================================

    private void Start()
    {
        // La confirmación debe comenzar cerrada.
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }

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
    // SOLICITAR NUEVA SIMULACIÓN
    // =====================================================

    public void StartSimulation()
    {
        // Si existe una partida guardada, pedir confirmación
        // antes de eliminarla.
        if (
            SaveManager.Instance != null &&
            SaveManager.Instance.HasSavedGame()
        )
        {
            if (confirmNewGamePanel != null)
            {
                confirmNewGamePanel.SetActive(true);

                Debug.Log(
                    "SOLICITANDO CONFIRMACIÓN PARA REEMPLAZAR LA PARTIDA."
                );
            }
            else
            {
                Debug.LogError(
                    "MainMenuManager: falta asignar Confirm New Game Panel."
                );
            }

            return;
        }

        // Si no existe una partida guardada,
        // iniciar directamente.
        ConfirmStartNewGame();
    }

    // =====================================================
    // CONFIRMAR NUEVA SIMULACIÓN
    // =====================================================

    public void ConfirmStartNewGame()
    {
        // No utilizar información cargada anteriormente.
        pendingSaveData = null;

        // Eliminar la partida anterior.
        if (SaveManager.Instance != null)
        {
            if (SaveManager.Instance.HasSavedGame())
            {
                SaveManager.Instance.DeleteSave();

                Debug.Log(
                    "PARTIDA ANTERIOR ELIMINADA."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "MainMenuManager: no existe SaveManager."
            );
        }

        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }

        Debug.Log(
            "INICIANDO NUEVA SIMULACIÓN."
        );

        SceneManager.LoadScene(
            "CinematicaAmbulancia"
        );
    }

    // =====================================================
    // CANCELAR NUEVA SIMULACIÓN
    // =====================================================

    public void CancelStartNewGame()
    {
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }

        Debug.Log(
            "NUEVA SIMULACIÓN CANCELADA."
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

        if (SaveManager.Instance == null)
        {
            Debug.LogError(
                "MainMenuManager: no existe un SaveManager activo."
            );

            return;
        }

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

        SaveData data =
            SaveManager.Instance.LoadGame();

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

        SceneManager.LoadScene(
            "Hospital_Base"
        );
    }

    // =====================================================
    // SALIR DEL JUEGO
    // =====================================================

    public void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
