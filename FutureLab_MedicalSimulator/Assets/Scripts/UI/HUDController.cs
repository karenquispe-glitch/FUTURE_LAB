using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
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

    [Header("Tiempo agotado")]
    [SerializeField] private GameObject timeOutPanel;
    [SerializeField] private TMP_Text timeOutSummaryText;

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
    // ENTRADA A VALORACIÓN CLÍNICA
    // TAREA 10
    // =====================================================

    [Header("Entrada a valoración clínica")]
    [SerializeField] private GameObject clinicalAssessmentPrompt;

    [SerializeField]
    private UnityEvent onClinicalAssessmentRequested =
        new UnityEvent();

    private bool waitingForClinicalAssessmentInput = false;

    // =====================================================
    // VARIABLES DEL CONTADOR
    // =====================================================

    private float elapsedTime = 0f;

    // Límite máximo del caso: 20 minutos = 1200 segundos.
    private const float MaxCaseTime = 1200f;

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

    // Se activa cuando el caso llega al límite máximo de 20:00.
    private bool caseEnded = false;

    // =====================================================
    // SISTEMA CENTRAL DE PENALIZACIONES
    // =====================================================

    // Cada penalización de tiempo equivale a 5 minutos.
    private const float TimePenaltySeconds = 300f;

    // Guarda los IDs de errores que ya fueron penalizados.
    // HashSet impide duplicados automáticamente.
    private readonly HashSet<string> appliedPenaltyIds =
        new HashSet<string>();

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
        caseEnded = false;

        // Nuevo intento: comenzar sin penalizaciones previas.
        appliedPenaltyIds.Clear();

        // La fase se muestra normalmente mientras el caso está activo.
        if (currentPhaseText != null)
        {
            currentPhaseText.gameObject.SetActive(true);
        }

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


        // Ocultar pantalla de tiempo agotado al iniciar.
        if (timeOutPanel != null)
        {
            timeOutPanel.SetActive(false);
        }

        // El mensaje de VALORACIÓN solo aparece
        // después de confirmar la entrevista.
        waitingForClinicalAssessmentInput = false;

        if (clinicalAssessmentPrompt != null)
        {
            clinicalAssessmentPrompt.SetActive(false);
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
        // E = SOLICITAR INICIO DE VALORACIÓN CLÍNICA
        // -------------------------------------------------

        if (
            waitingForClinicalAssessmentInput &&
            !gamePaused &&
            !caseEnded &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame
        )
        {
            OnClinicalAssessmentRequested();
        }

        // -------------------------------------------------
        // CONTADOR
        // -------------------------------------------------

        if (!timerRunning || gamePaused)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        // El reloj nunca puede superar 20:00.
        if (elapsedTime >= MaxCaseTime)
        {
            elapsedTime = MaxCaseTime;

            UpdateTimerText();

            EndCaseByTime();

            return;
        }

        UpdateTimerText();
    }

    // =====================================================
    // CONTADOR
    // =====================================================

    public void StartTimer()
    {
        elapsedTime = 0f;

        // Inicio de un caso nuevo: limpiar penalizaciones del intento anterior.
        appliedPenaltyIds.Clear();

        caseEnded = false;
        timerRunning = true;
        caseStarted = true;

        if (currentPhaseText != null)
        {
            currentPhaseText.gameObject.SetActive(true);
        }

        if (timeOutPanel != null)
        {
            timeOutPanel.SetActive(false);
        }

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
        caseEnded = false;

        // Reiniciar también las penalizaciones si se reinicia el caso.
        appliedPenaltyIds.Clear();

        UpdateTimerText();

        Debug.Log("CONTADOR DEL CASO REINICIADO.");
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        // Asegurar también a nivel visual que nunca aparezca
        // un valor superior a 20:00.
        float displayTime =
            Mathf.Clamp(elapsedTime, 0f, MaxCaseTime);

        int minutes =
            Mathf.FloorToInt(displayTime / 60f);

        int seconds =
            Mathf.FloorToInt(displayTime % 60f);

        timerText.text = string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }

    // =====================================================
    // FINALIZAR CASO POR TIEMPO
    // =====================================================

    private void EndCaseByTime()
    {
        if (caseEnded)
        {
            return;
        }

        elapsedTime = MaxCaseTime;

        caseEnded = true;
        timerRunning = false;
        timerWasRunningBeforePause = false;
        caseStarted = false;
        gamePaused = false;

        Time.timeScale = 1f;

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (saveGamePanel != null)
        {
            saveGamePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

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

        waitingForClinicalAssessmentInput = false;

        if (clinicalAssessmentPrompt != null)
        {
            clinicalAssessmentPrompt.SetActive(false);
        }

        UpdateTimerText();

        // Mantener visible únicamente el contador.
        // La etiqueta de fase (por ejemplo, ENTREVISTA) se oculta.
        if (currentPhaseText != null)
        {
            currentPhaseText.gameObject.SetActive(false);
        }

        // Cerrar la interfaz clínica que pudiera quedar abierta:
        // entrevista, datos registrados, resumen, ficha del paciente, etc.
        if (interviewController != null)
        {
            interviewController.HideInterviewUIForCaseEnd();
        }

        // Cerrar cualquier alerta superpuesta.
        if (performanceAlertPanel != null)
        {
            performanceAlertPanel.SetActive(false);
        }

        // Mostrar la pantalla de tiempo agotado por encima del resto.
        if (timeOutPanel != null)
        {
            timeOutPanel.SetActive(true);
            timeOutPanel.transform.SetAsLastSibling();
        }

        if (timeOutSummaryText != null)
        {
            int askedQuestions = 0;
            int registeredFindingsCount = 0;
            int essentialFindingsCount = 0;

            if (interviewController != null)
            {
                askedQuestions =
                    interviewController.GetAnsweredQuestions().Count;

                registeredFindingsCount =
                    interviewController.GetRegisteredFindings().Count;

                essentialFindingsCount =
                    interviewController.GetEssentialFindingsCount();
            }

            timeOutSummaryText.text =
                "El tiempo máximo de 20:00 ha finalizado.\n\n" +
                "Preguntas realizadas: " +
                askedQuestions +
                "\nHallazgos registrados: " +
                registeredFindingsCount +
                "\nInformación esencial: " +
                essentialFindingsCount +
                "/4\n\n" +
                "El caso clínico ha sido cerrado.";
        }

        // Guardar automáticamente el intento como FINALIZADO.
        // Este resultado no debe quedar disponible como partida reanudable.
        if (SaveManager.Instance != null)
        {
            SaveData finalData =
                BuildCurrentSaveData();

            finalData.caseStarted = false;
            finalData.caseCompleted = true;
            finalData.elapsedTime = MaxCaseTime;

            SaveManager.Instance.SaveFinalAttempt(
                finalData
            );

            // Si existía una partida reanudable anterior,
            // eliminarla para impedir continuar un intento ya finalizado.
            SaveManager.Instance.DeleteSave();
        }
        else
        {
            Debug.LogWarning(
                "HUDController: no existe SaveManager activo. " +
                "No se pudo registrar el intento finalizado."
            );
        }

        Debug.Log(
            "TIEMPO AGOTADO. EL CASO HA FINALIZADO EN 20:00."
        );
    }

    // =====================================================
    // CONSULTAS DEL CONTADOR / ESTADO DEL CASO
    // =====================================================

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public bool IsCaseEnded()
    {
        return caseEnded;
    }


    // =====================================================
    // APLICAR PENALIZACIÓN DE TIEMPO
    // =====================================================

    public bool ApplyTimePenalty(string penaltyId)
    {
        // No aceptar IDs vacíos.
        if (string.IsNullOrWhiteSpace(penaltyId))
        {
            Debug.LogWarning(
                "HUDController: se intentó aplicar una penalización sin ID."
            );

            return false;
        }

        // Si el caso ya terminó, no se puede seguir penalizando.
        if (caseEnded)
        {
            Debug.LogWarning(
                "PENALIZACIÓN IGNORADA: el caso ya finalizó."
            );

            return false;
        }

        // Si este error ya fue penalizado, no volver a sumar tiempo.
        if (appliedPenaltyIds.Contains(penaltyId))
        {
            Debug.Log(
                "PENALIZACIÓN YA APLICADA: " +
                penaltyId +
                ". NO SE SUMA TIEMPO NUEVAMENTE."
            );

            return false;
        }

        // Registrar el ID antes de modificar el reloj.
        appliedPenaltyIds.Add(penaltyId);

        // Sumar 5 minutos.
        elapsedTime += TimePenaltySeconds;

        Debug.Log(
            "PENALIZACIÓN APLICADA: " +
            penaltyId +
            " | +05:00"
        );

        // Nunca permitir superar 20:00.
        if (elapsedTime >= MaxCaseTime)
        {
            elapsedTime = MaxCaseTime;

            UpdateTimerText();
            EndCaseByTime();

            return true;
        }

        // Reflejar la penalización inmediatamente en pantalla.
        UpdateTimerText();

        return true;
    }

    // =====================================================
    // CONSULTAR PENALIZACIONES APLICADAS
    // =====================================================

    public List<string> GetAppliedPenaltyIds()
    {
        return new List<string>(appliedPenaltyIds);
    }

    public bool HasTimePenalty(string penaltyId)
    {
        return appliedPenaltyIds.Contains(penaltyId);
    }

    // =====================================================
    // PREPARAR ENTRADA A VALORACIÓN CLÍNICA
    // TAREA 10
    // =====================================================

    public void PrepareClinicalAssessmentEntry()
    {
        if (caseEnded)
        {
            return;
        }

        waitingForClinicalAssessmentInput = true;

        if (clinicalAssessmentPrompt != null)
        {
            clinicalAssessmentPrompt.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "HUDController: Clinical Assessment Prompt " +
                "todavía no está asignado en el Inspector."
            );
        }

        Debug.Log(
            "VALORACIÓN PREPARADA. " +
            "PRESIONA E PARA INICIAR LA VALORACIÓN CLÍNICA."
        );
    }

    // =====================================================
    // PUNTO DE ENLACE PARA EL FUTURO SISTEMA DE VALORACIÓN
    // =====================================================

    public void OnClinicalAssessmentRequested()
    {
        if (
            !waitingForClinicalAssessmentInput ||
            caseEnded
        )
        {
            return;
        }

        waitingForClinicalAssessmentInput = false;

        if (clinicalAssessmentPrompt != null)
        {
            clinicalAssessmentPrompt.SetActive(false);
        }

        Debug.Log(
            "TECLA E DETECTADA. " +
            "SOLICITUD DE VALORACIÓN CLÍNICA ENVIADA."
        );

        // Aquí se conectará posteriormente el sistema
        // de procedimientos, signos vitales, animaciones, etc.
        if (onClinicalAssessmentRequested != null)
        {
            onClinicalAssessmentRequested.Invoke();
        }
    }

    // =====================================================
    // PAUSAR JUEGO
    // =====================================================

    public void PauseGame()
    {
        if (!caseStarted || gamePaused || caseEnded)
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
        if (!gamePaused || caseEnded)
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
    // REINTENTAR CASO
    // =====================================================

    public void RetryCase()
    {
        // Asegurar que el tiempo de Unity esté normal.
        Time.timeScale = 1f;

        // Limpiar estados generales.
        gamePaused = false;
        timerRunning = false;
        timerWasRunningBeforePause = false;
        caseStarted = false;
        caseEnded = false;

        // Liberar el cursor antes de recargar.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "REINTENTANDO CASO CLÍNICO."
        );

        // Recargar completamente la escena actual.
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
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
        caseEnded = false;

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
        if (caseEnded)
        {
            Debug.LogWarning(
                "No se puede continuar: el caso ya finalizó por tiempo."
            );

            return;
        }

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
    // CREAR DATOS ACTUALES DEL CASO
    // =====================================================

    private SaveData BuildCurrentSaveData()
    {
        SaveData data = new SaveData();

        data.sceneName = "Hospital_Base";
        data.caseID = caseID;

        data.caseStarted = caseStarted;

        // Persistir explícitamente si el intento ya terminó.
        data.caseCompleted = caseEnded;

        if (caseFlowController != null)
        {
            data.currentPhase =
                caseFlowController.GetCurrentPhase().ToString();
        }
        else
        {
            data.currentPhase = currentPhase;
        }

        data.elapsedTime = elapsedTime;

        data.appliedPenaltyIds =
            GetAppliedPenaltyIds();

        if (interviewController != null)
        {
            data.answeredQuestions =
                interviewController.GetSavedQuestions();

            data.registeredFindings =
                interviewController.GetRegisteredFindings();

            data.interviewCompleted =
                interviewController.IsInterviewCompleted();

            data.interviewViewState =
                interviewController.GetInterviewViewState();

            data.selectedInterviewQuestion =
                interviewController.GetSelectedInterviewQuestion();

            data.currentInterviewResponse =
                interviewController.GetCurrentInterviewResponse();
        }

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

        data.saveDate =
            System.DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss"
            );

        return data;
    }

    // =====================================================
    // GUARDAR Y SALIR
    // =====================================================

    public void SaveAndExit()
    {
        if (caseEnded)
        {
            Debug.LogWarning(
                "NO SE PUEDE GUARDAR COMO REANUDABLE: " +
                "EL CASO YA FINALIZÓ."
            );

            return;
        }

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
        // Crear datos actuales de la partida
        // ---------------------------------------------

        SaveData data =
            BuildCurrentSaveData();

        Debug.Log(
            "GUARDANDO FASE CLÍNICA: " +
            data.currentPhase
        );

        Debug.Log(
            "GUARDANDO PENALIZACIONES DE TIEMPO: " +
            data.appliedPenaltyIds.Count
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
            "[OK] ACCIÓN CORRECTA\n" + message,
            new Color(0.10f, 0.45f, 0.25f, 0.95f)
        );
    }

    public void ShowWarningAlert(string message)
    {
        ShowPerformanceAlert(
            "[!] ATENCIÓN\n" + message,
            new Color(0.85f, 0.50f, 0.10f, 0.95f)
        );
    }

    public void ShowErrorAlert(string message)
    {
        ShowPerformanceAlert(
            "[X] ACCIÓN INCOMPLETA\n" + message,
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

        // Restaurar tiempo sin permitir valores superiores
        // al máximo de 20:00.
        elapsedTime =
            Mathf.Clamp(data.elapsedTime, 0f, MaxCaseTime);

        // ---------------------------------------------
        // RESTAURAR PENALIZACIONES DE TIEMPO
        // ---------------------------------------------

        appliedPenaltyIds.Clear();

        if (data.appliedPenaltyIds != null)
        {
            foreach (string penaltyId in data.appliedPenaltyIds)
            {
                if (!string.IsNullOrWhiteSpace(penaltyId))
                {
                    appliedPenaltyIds.Add(penaltyId);
                }
            }
        }

        Debug.Log(
            "PENALIZACIONES DE TIEMPO RESTAURADAS: " +
            appliedPenaltyIds.Count
        );

        caseEnded =
            data.caseCompleted ||
            elapsedTime >= MaxCaseTime;

        // Restaurar estado del caso.
        caseStarted =
            data.caseStarted && !caseEnded;

        // Restaurar fase.
        currentPhase = data.currentPhase;
        UpdateCurrentPhaseText();

        // El caso continúa ejecutándose únicamente
        // si todavía no alcanzó el límite de 20:00.
        gamePaused = false;
        timerRunning = !caseEnded;

        // Mostrar botón PAUSA solo para partidas activas.
        if (pauseButton != null)
        {
            pauseButton.SetActive(!caseEnded);
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

        if (caseEnded)
        {
            if (currentPhaseText != null)
            {
                currentPhaseText.gameObject.SetActive(false);
            }

            if (interviewController != null)
            {
                interviewController.HideInterviewUIForCaseEnd();
            }

            if (performanceAlertPanel != null)
            {
                performanceAlertPanel.SetActive(false);
            }

            if (timeOutPanel != null)
            {
                timeOutPanel.SetActive(true);
                timeOutPanel.transform.SetAsLastSibling();
            }

            if (timeOutSummaryText != null)
            {
                timeOutSummaryText.text =
                    "El tiempo máximo de 20:00 ha finalizado.\n\n" +
                    "El caso clínico ha sido cerrado.";
            }

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

            Time.timeScale = 1f;

            Debug.Log(
                "PARTIDA RESTAURADA EN 20:00. " +
                "EL CASO PERMANECE FINALIZADO."
            );

            return;
        }

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

        // Si la partida fue guardada después de confirmar
        // la entrevista, volver a preparar la entrada con E.
        if (
            NormalizePhaseName(currentPhase) == "Valoracion" &&
            data.interviewCompleted
        )
        {
            PrepareClinicalAssessmentEntry();
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
        if (caseEnded)
        {
            Debug.LogWarning(
                "No se puede cambiar de fase: " +
                "el caso ya finalizó por tiempo."
            );

            return;
        }

        currentPhase = phase;
        UpdateCurrentPhaseText();

        string normalizedPhase =
            NormalizePhaseName(phase);

        // Al entrar en VALORACIÓN mostramos el mensaje
        // y dejamos preparada la tecla E.
        if (normalizedPhase == "Valoracion")
        {
            PrepareClinicalAssessmentEntry();
        }
        else
        {
            waitingForClinicalAssessmentInput = false;

            if (clinicalAssessmentPrompt != null)
            {
                clinicalAssessmentPrompt.SetActive(false);
            }
        }

        // =================================================
        // SINCRONIZAR CON EL CONTROLADOR CENTRAL
        // =================================================

        if (caseFlowController != null)
        {
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
