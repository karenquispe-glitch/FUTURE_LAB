using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using StarterAssets;

public class HUDController : MonoBehaviour
{
    // =====================================================
    // CONTADOR
    // =====================================================

    [Header("Contador")]
    [SerializeField] private TMP_Text timerText;

    [Header("Fase actual")]
    [SerializeField] private TMP_Text currentPhaseText;

    // =====================================================
    // ALERTAS DE DESEMPEÑO
    // =====================================================

    [Header("Alertas de desempeño")]
    [SerializeField] private GameObject performanceAlertPanel;
    [SerializeField] private TMP_Text performanceAlertText;
    [SerializeField] private Image performanceAlertBackground;

    // =====================================================
    // MENÚ DE PAUSA
    // =====================================================

    [Header("Menú de pausa")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject saveGamePanel;
    [SerializeField] private GameObject pauseMainMenu;
    [SerializeField] private GameObject settingsPanel;

    // =====================================================
    // CONTROL DEL JUGADOR
    // =====================================================

    [Header("Control del jugador")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    // =====================================================
    // GUARDADO
    // =====================================================

    [Header("Guardado de partida")]
    [SerializeField] private int caseID = 1;

    [SerializeField] private string currentPhase = "Inicio";

    [SerializeField] private Transform playerTransform;

    [SerializeField] private InterviewController interviewController;

    [SerializeField] private ClinicalCaseFlowController caseFlowController;

    // =====================================================
    // VARIABLES DEL CONTADOR
    // =====================================================

    private float elapsedTime = 0f;

    private bool timerRunning = false;
    private bool timerWasRunningBeforePause = false;

    // =====================================================
    // VARIABLES DE ALERTAS
    // =====================================================

    private Coroutine alertCoroutine;

    // =====================================================
    // ESTADO GENERAL
    // =====================================================

    private bool caseStarted = false;
    private bool gamePaused = false;

    // =====================================================
    // ESTADO ANTERIOR DEL JUGADOR
    // =====================================================

    private bool firstPersonWasEnabled;

    private bool cursorWasVisible;
    private CursorLockMode cursorLockBeforePause;

    private bool starterCursorLockedBeforePause;
    private bool starterCursorInputBeforePause;

    // =====================================================
    // INICIO
    // =====================================================

    private void Start()
    {
        elapsedTime = 0f;

        timerRunning = false;
        timerWasRunningBeforePause = false;

        caseStarted = false;
        gamePaused = false;

        // Si no asignamos manualmente el jugador,
        // usamos el transform del FirstPersonController.
        if (playerTransform == null &&
            firstPersonController != null)
        {
            playerTransform = firstPersonController.transform;
        }

        // El botón PAUSA no debe verse
        // antes de comenzar el caso.
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

        // Ocultar menú de pausa al iniciar.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Ocultar ventana de guardado al iniciar.
        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        // Preparar menú principal de pausa.
        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(true);
        }

        // Ocultar configuración al iniciar.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Ocultar alertas de desempeño al iniciar.
        if (performanceAlertPanel != null)
        {
            performanceAlertPanel.SetActive(false);
        }

        // Asegurarnos de que el juego no empiece pausado.
        Time.timeScale = 1f;

        UpdateTimerText();
        UpdateCurrentPhaseText();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // -------------------------------------------------
        // ESC = PAUSAR / REANUDAR
        // -------------------------------------------------

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (gamePaused)
            {
                // Si estamos dentro de configuración,
                // ESC vuelve primero al menú de pausa.
                if (settingsPanel != null &&
                    settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    ResumeGame();
                }
            }
            else if (caseStarted)
            {
                PauseGame();
            }
        }

        // -------------------------------------------------
        // CONTADOR
        // -------------------------------------------------

        if (!timerRunning || gamePaused)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        UpdateTimerText();
    }

    // =====================================================
    // CONTADOR
    // =====================================================

    public void StartTimer()
    {
        elapsedTime = 0f;

        timerRunning = true;
        caseStarted = true;

        // Una vez iniciado el caso,
        // PAUSA ya tiene sentido.
        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        UpdateTimerText();

        Debug.Log("CONTADOR DEL CASO INICIADO.");
    }

    public void StopTimer()
    {
        timerRunning = false;

        Debug.Log("CONTADOR DEL CASO DETENIDO.");
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;

        UpdateTimerText();

        Debug.Log("CONTADOR DEL CASO REINICIADO.");
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes =
            Mathf.FloorToInt(elapsedTime / 60f);

        int seconds =
            Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }

    // =====================================================
    // PAUSAR JUEGO
    // =====================================================

    public void PauseGame()
    {
        if (!caseStarted || gamePaused)
        {
            return;
        }

        gamePaused = true;

        // Guardar estado del contador.
        timerWasRunningBeforePause = timerRunning;
        timerRunning = false;

        // Guardar estado del controlador.
        firstPersonWasEnabled =
            firstPersonController != null &&
            firstPersonController.enabled;

        // Guardar estado del cursor.
        cursorWasVisible = Cursor.visible;
        cursorLockBeforePause = Cursor.lockState;

        // Guardar estado de Starter Assets.
        if (starterAssetsInputs != null)
        {
            starterCursorLockedBeforePause =
                starterAssetsInputs.cursorLocked;

            starterCursorInputBeforePause =
                starterAssetsInputs.cursorInputForLook;
        }

        // Mostrar menú pausa.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(true);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Cerrar ventana guardar.
        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        // Desactivar jugador.
        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.move = Vector2.zero;
            starterAssetsInputs.look = Vector2.zero;

            starterAssetsInputs.jump = false;
            starterAssetsInputs.sprint = false;

            starterAssetsInputs.cursorLocked = false;
            starterAssetsInputs.cursorInputForLook = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        Debug.Log("JUEGO EN PAUSA.");
    }

    // =====================================================
    // REANUDAR JUEGO
    // =====================================================

    public void ResumeGame()
    {
        if (!gamePaused)
        {
            return;
        }

        // Restaurar tiempo.
        Time.timeScale = 1f;

        gamePaused = false;

        // Cerrar configuración.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Cerrar ventana de guardado.
        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        // Preparar nuevamente el menú principal.
        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(true);
        }

        // Cerrar menú de pausa completo.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Continuar contador donde estaba.
        timerRunning = timerWasRunningBeforePause;

        // Restaurar controlador.
        if (firstPersonController != null)
        {
            firstPersonController.enabled =
                firstPersonWasEnabled;
        }

        // Restaurar Starter Assets.
        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.move = Vector2.zero;
            starterAssetsInputs.look = Vector2.zero;

            starterAssetsInputs.jump = false;
            starterAssetsInputs.sprint = false;

            starterAssetsInputs.cursorLocked =
                starterCursorLockedBeforePause;

            starterAssetsInputs.cursorInputForLook =
                starterCursorInputBeforePause;
        }

        // Restaurar cursor.
        Cursor.lockState = cursorLockBeforePause;
        Cursor.visible = cursorWasVisible;

        Debug.Log("JUEGO REANUDADO.");
    }

    // =====================================================
    // CONFIGURACIÓN
    // =====================================================

    public void OpenSettings()
    {
        // Ocultar botones principales de pausa.
        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(false);
        }

        // Mostrar configuración.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "HUDController: falta asignar SettingsPanel."
            );
        }

        Debug.Log("CONFIGURACIÓN ABIERTA.");
    }

    public void CloseSettings()
    {
        // Ocultar configuración.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Volver a mostrar botones principales.
        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(true);
        }

        Debug.Log("CONFIGURACIÓN CERRADA.");
    }

    // =====================================================
    // VOLUMEN
    // =====================================================

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;

        Debug.Log(
            "VOLUMEN GENERAL: " +
            Mathf.RoundToInt(value * 100f) +
            "%"
        );
    }

    // =====================================================
    // VOLVER AL MENÚ PRINCIPAL
    // =====================================================

    public void GoToMainMenu()
    {
        // Restaurar tiempo.
        Time.timeScale = 1f;

        gamePaused = false;
        timerRunning = false;
        caseStarted = false;

        // Ocultar PAUSA.
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

        // Cerrar configuración.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Cerrar ventana de guardado.
        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        // Cerrar menú principal de pausa.
        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(false);
        }

        // Cerrar menú de pausa.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Liberar cursor.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "VOLVIENDO AL MENÚ PRINCIPAL."
        );

        SceneManager.LoadScene("MainMenu");
    }

    // =====================================================
    // SALIR DEL SIMULADOR
    // =====================================================

    public void ExitGame()
    {
        // Abrir ventana de guardado.
        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(true);
        }

        // Ocultar menú de pausa.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Debug.Log("ABRIENDO VENTANA DE GUARDADO.");
    }

    // =====================================================
    // CONTINUAR DESDE VENTANA DE GUARDADO
    // =====================================================

    public void ContinueGame()
    {
        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        gamePaused = false;
        timerRunning = true;

        // Restaurar jugador.
        if (firstPersonController != null)
        {
            firstPersonController.enabled = true;
        }

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.cursorLocked = true;
            starterAssetsInputs.cursorInputForLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("CONTINUANDO CASO CLÍNICO.");
    }

    // =====================================================
    // SALIR SIN GUARDAR
    // =====================================================

    public void ExitWithoutSave()
    {
        Debug.Log("SALIENDO SIN GUARDAR.");

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    // =====================================================
    // GUARDAR Y SALIR
    // =====================================================

    public void SaveAndExit()
    {
        Debug.Log("GUARDANDO PARTIDA...");

        // ---------------------------------------------
        // Comprobar SaveManager
        // ---------------------------------------------

        if (SaveManager.Instance == null)
        {
            Debug.LogError(
                "HUDController: no existe un SaveManager activo."
            );

            return;
        }

        // ---------------------------------------------
        // Crear datos de la partida
        // ---------------------------------------------

        SaveData data = new SaveData();

        // Información general.
        data.sceneName = "Hospital_Base";
        data.caseID = caseID;

        // ---------------------------------------------
        // ESTADO DEL CASO
        // ---------------------------------------------

        data.caseStarted = caseStarted;

        // Guardar la fase real desde el controlador
        // central del nuevo caso clínico.
        if (caseFlowController != null)
        {
            data.currentPhase =
                caseFlowController.GetCurrentPhase().ToString();

            Debug.Log(
                "GUARDANDO FASE CLÍNICA: " +
                data.currentPhase
            );
        }
        else
        {
            // Si todavía no está conectado en Inspector,
            // utilizamos la fase almacenada en el HUD.
            data.currentPhase = currentPhase;

            Debug.LogWarning(
                "HUDController: ClinicalCaseFlowController " +
                "no asignado. Se utilizará la fase del HUD: " +
                currentPhase
            );
        }

        // ---------------------------------------------
        // TIEMPO
        // ---------------------------------------------

        data.elapsedTime = elapsedTime;

        // ---------------------------------------------
        // GUARDAR PROGRESO DE LA ENTREVISTA
        // ---------------------------------------------

        if (interviewController != null)
        {
            data.answeredQuestions =
                interviewController.GetSavedQuestions();

            // Guardar si la entrevista
            // ya fue finalizada.
            data.interviewCompleted =
                interviewController.IsInterviewCompleted();
        }
        else
        {
            Debug.LogWarning(
                "HUDController: InterviewController no asignado."
            );
        }

        // ---------------------------------------------
        // GUARDAR POSICIÓN DEL JUGADOR
        // ---------------------------------------------

        if (playerTransform != null)
        {
            data.playerPosX =
                playerTransform.position.x;

            data.playerPosY =
                playerTransform.position.y;

            data.playerPosZ =
                playerTransform.position.z;

            data.playerRotX =
                playerTransform.eulerAngles.x;

            data.playerRotY =
                playerTransform.eulerAngles.y;

            data.playerRotZ =
                playerTransform.eulerAngles.z;
        }
        else
        {
            Debug.LogWarning(
                "HUDController: Player Transform no asignado."
            );
        }

        // ---------------------------------------------
        // DATOS QUE SE INTEGRARÁN POSTERIORMENTE
        // ---------------------------------------------

        // Los siguientes campos ya existen en SaveData,
        // pero se llenarán cuando estén disponibles
        // los controladores correspondientes:
        //
        // data.completedAssessments
        // data.requestedTests
        // data.reviewedResults
        // data.riskAlertTriggered
        // data.bleedingEventLevel
        // data.provisionalDiagnosis
        // data.score

        // ---------------------------------------------
        // FECHA DEL GUARDADO
        // ---------------------------------------------

        data.saveDate =
            System.DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss"
            );

        // ---------------------------------------------
        // GUARDAR PARTIDA
        // ---------------------------------------------

        SaveManager.Instance.SaveGame(data);

        // ---------------------------------------------
        // SALIR AL MENÚ
        // ---------------------------------------------

        Time.timeScale = 1f;

        gamePaused = false;
        timerRunning = false;

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "PARTIDA GUARDADA. VOLVIENDO AL MENÚ PRINCIPAL."
        );

        SceneManager.LoadScene("MainMenu");
    }

    // =====================================================
    // ALERTAS DE DESEMPEÑO
    // =====================================================

    public void ShowSuccessAlert(string message)
    {
        ShowPerformanceAlert(
            "✓ ACCIÓN CORRECTA\n" + message,
            new Color(0.10f, 0.45f, 0.25f, 0.95f)
        );
    }

    public void ShowWarningAlert(string message)
    {
        ShowPerformanceAlert(
            "⚠ ATENCIÓN\n" + message,
            new Color(0.85f, 0.50f, 0.10f, 0.95f)
        );
    }

    public void ShowErrorAlert(string message)
    {
        ShowPerformanceAlert(
            "✕ ACCIÓN INCOMPLETA\n" + message,
            new Color(0.65f, 0.15f, 0.15f, 0.95f)
        );
    }

    private void ShowPerformanceAlert(
        string message,
        Color backgroundColor
    )
    {
        if (performanceAlertPanel == null ||
            performanceAlertText == null)
        {
            Debug.LogWarning(
                "HUDController: faltan referencias de alerta."
            );

            return;
        }

        // Cambiar texto.
        performanceAlertText.text = message;

        // Cambiar color según el tipo de alerta.
        if (performanceAlertBackground != null)
        {
            performanceAlertBackground.color =
                backgroundColor;
        }

        // Mostrar panel.
        performanceAlertPanel.SetActive(true);

        // Si ya había una alerta activa,
        // reiniciar su tiempo.
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
        }

        // Ocultarla automáticamente.
        alertCoroutine = StartCoroutine(
            HidePerformanceAlertAfterDelay()
        );
    }

    private IEnumerator HidePerformanceAlertAfterDelay()
    {
        // Esperar 3 segundos reales.
        yield return new WaitForSecondsRealtime(3f);

        if (performanceAlertPanel != null)
        {
            performanceAlertPanel.SetActive(false);
        }

        alertCoroutine = null;
    }

    // =====================================================
    // SEGURIDAD
    // =====================================================

    private void OnDestroy()
    {
        // Evita que Unity quede congelado
        // al cambiar de escena o detener Play.
        Time.timeScale = 1f;
    }

    // =====================================================
    // RESTAURAR PARTIDA CARGADA
    // =====================================================

    public void RestoreLoadedGame(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning(
                "HUDController: no hay datos para restaurar."
            );

            return;
        }

        // Restaurar tiempo.
        elapsedTime = data.elapsedTime;

        // Restaurar estado del caso.
        caseStarted = data.caseStarted;

        // Restaurar fase.
        currentPhase = data.currentPhase;
        UpdateCurrentPhaseText();

        // El caso continúa ejecutándose.
        gamePaused = false;
        timerRunning = true;

        // Mostrar botón PAUSA.
        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        // Cerrar menús.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Restaurar contador visual.
        UpdateTimerText();

        // =====================================================
        // RESTAURAR CONTROL DEL JUGADOR
        // =====================================================

        if (firstPersonController != null)
        {
            firstPersonController.enabled = true;
        }

        // =====================================================
        // RESTAURAR MODO SEGÚN LA FASE GUARDADA
        // =====================================================

        if (currentPhase == "Entrevista" &&
            data.interviewCompleted == false)
        {
            // La partida fue guardada durante la entrevista.
            // El jugador necesita el mouse libre para continuar
            // seleccionando las respuestas.

            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorLocked = false;
                starterAssetsInputs.cursorInputForLook = false;

                starterAssetsInputs.move = Vector2.zero;
                starterAssetsInputs.look = Vector2.zero;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log(
                "PARTIDA RESTAURADA EN ENTREVISTA. MOUSE LIBRE."
            );
        }
        else
        {
            // La partida está en una fase normal de simulación.

            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorLocked = true;
                starterAssetsInputs.cursorInputForLook = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log(
                "PARTIDA RESTAURADA EN MODO SIMULACIÓN. MOUSE BLOQUEADO."
            );
        }

        // Asegurar que el juego esté activo.
        Time.timeScale = 1f;

        Debug.Log(
            "HUD RESTAURADO. TIEMPO: " +
            elapsedTime +
            " | FASE: " +
            currentPhase
        );
    }

    // =====================================================
    // ACTUALIZAR FASE DEL CASO
    // =====================================================

    public void SetCurrentPhase(string phase)
    {
        currentPhase = phase;
        UpdateCurrentPhaseText();

        // =================================================
        // SINCRONIZAR CON EL CONTROLADOR CENTRAL
        // =================================================

        if (caseFlowController != null)
        {
            string normalizedPhase = NormalizePhaseName(phase);

            ClinicalCaseFlowController.ClinicalCasePhase parsedPhase;

            bool validPhase = System.Enum.TryParse(
                normalizedPhase,
                true,
                out parsedPhase
            );

            if (validPhase)
            {
                caseFlowController.SetPhase(parsedPhase);
            }
            else
            {
                Debug.LogWarning(
                    "HUDController: la fase '" +
                    phase +
                    "' no coincide con una fase del " +
                    "ClinicalCaseFlowController."
                );
            }
        }

        Debug.Log(
            "FASE ACTUALIZADA: " + currentPhase
        );
    }

    // =====================================================
    // ACTUALIZAR TEXTO VISUAL DE LA FASE
    // =====================================================

    private void UpdateCurrentPhaseText()
    {
        if (currentPhaseText == null)
        {
            return;
        }

        string normalizedPhase = NormalizePhaseName(currentPhase);

        switch (normalizedPhase)
        {
            case "Inicio":
                currentPhaseText.text = "INICIO";
                break;

            case "Acercamiento":
                currentPhaseText.text = "ACERCAMIENTO";
                break;

            case "Entrevista":
                currentPhaseText.text = "ENTREVISTA";
                break;

            case "Valoracion":
                currentPhaseText.text = "VALORACIÓN";
                break;

            case "PruebasIniciales":
                currentPhaseText.text = "PRUEBAS INICIALES";
                break;

            case "Hipotesis":
                currentPhaseText.text = "HIPÓTESIS";
                break;

            case "Confirmacion":
                currentPhaseText.text = "CONFIRMACIÓN";
                break;

            case "Diagnostico":
                currentPhaseText.text = "DIAGNÓSTICO";
                break;

            case "Cierre":
                currentPhaseText.text = "CIERRE";
                break;

            default:
                currentPhaseText.text =
                    string.IsNullOrWhiteSpace(currentPhase)
                    ? "INICIO"
                    : currentPhase.ToUpperInvariant();
                break;
        }
    }

    // =====================================================
    // NORMALIZAR NOMBRE DE FASE
    // =====================================================

    private string NormalizePhaseName(string phase)
    {
        if (string.IsNullOrEmpty(phase))
        {
            return phase;
        }

        string normalized = phase.Trim();

        normalized = normalized
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("Á", "A")
            .Replace("É", "E")
            .Replace("Í", "I")
            .Replace("Ó", "O")
            .Replace("Ú", "U")
            .Replace(" ", "");

        return normalized;
    }

    // =====================================================
    // MODO INTERFAZ
    // =====================================================

    public void SetUIInteractionMode(bool enabled)
    {
        if (enabled)
        {
            // Liberar mouse para poder interactuar
            // con los botones de la interfaz.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorLocked = false;
                starterAssetsInputs.cursorInputForLook = false;

                starterAssetsInputs.move = Vector2.zero;
                starterAssetsInputs.look = Vector2.zero;
            }

            Debug.Log(
                "MODO INTERFAZ ACTIVADO. MOUSE LIBRE."
            );
        }
        else
        {
            // Volver al modo normal de simulación.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorLocked = true;
                starterAssetsInputs.cursorInputForLook = true;
            }

            Debug.Log(
                "MODO SIMULACIÓN ACTIVADO. MOUSE BLOQUEADO."
            );
        }
    }
}