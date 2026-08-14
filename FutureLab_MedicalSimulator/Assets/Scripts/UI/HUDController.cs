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
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject pauseMainMenu;
    [SerializeField] private GameObject settingsPanel;

    // =====================================================
    // CONTROL DEL JUGADOR
    // =====================================================

    [Header("Control del jugador")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

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

        // Ocultar menú de pausa al iniciar.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
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

        // Mostrar menú de pausa.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Mostrar botones principales.
        if (pauseMainMenu != null)
        {
            pauseMainMenu.SetActive(true);
        }

        // Configuración debe empezar cerrada.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Desactivar movimiento del jugador.
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

        // Liberar mouse.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Congelar simulación.
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

        // Cerrar configuración.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
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
        // Restaurar el tiempo por seguridad.
        Time.timeScale = 1f;

        Debug.Log("SALIENDO DEL SIMULADOR.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
}