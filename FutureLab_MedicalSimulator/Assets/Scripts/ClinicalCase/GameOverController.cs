using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    [Header("Panel Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Textos")]
    [SerializeField] private TMP_Text gameOverTitle;
    [SerializeField] private TMP_Text gameOverMessage;

    [Header("Paneles que deben cerrarse")]
    [SerializeField]
    private List<GameObject> panelsToHide =
        new List<GameObject>();

    [Header("Jugador")]
    [SerializeField] private MonoBehaviour firstPersonController;

    [Header("Configuración")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool gameOverActive = false;

    // =====================================================
    // INICIO
    // =====================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =====================================================
    // ACTIVAR GAME OVER
    // =====================================================

    public void ShowGameOver(string reason)
    {
        if (gameOverActive)
        {
            return;
        }

        gameOverActive = true;

        // -------------------------------------------------
        // CERRAR LOS PANELES DE LA FASE ACTUAL
        // -------------------------------------------------

        foreach (GameObject panel in panelsToHide)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // -------------------------------------------------
        // MOSTRAR GAME OVER
        // -------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverTitle != null)
        {
            gameOverTitle.text = "GAME OVER";
        }

        if (gameOverMessage != null)
        {
            gameOverMessage.text = reason;
        }

        // -------------------------------------------------
        // DETENER AL JUGADOR
        // -------------------------------------------------

        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }

        // -------------------------------------------------
        // LIBERAR MOUSE
        // -------------------------------------------------

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // -------------------------------------------------
        // DETENER EL TIEMPO DEL CASO
        // -------------------------------------------------

        Time.timeScale = 0f;

        Debug.Log(
            "GAME OVER: " + reason
        );
    }

    // =====================================================
    // GAME OVER POR DIAGNÓSTICO INCORRECTO
    // =====================================================

    public void GameOverDiagnosis()
    {
        ShowGameOver(
            "El diagnóstico seleccionado no es correcto.\n\n" +
            "El caso clínico ha terminado."
        );
    }

    // =====================================================
    // VOLVER AL MENÚ
    // =====================================================

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        gameOverActive = false;

        SceneManager.LoadScene(mainMenuScene);
    }

    // =====================================================
    // REINICIAR CASO
    // =====================================================

    public void RestartCase()
    {
        Time.timeScale = 1f;

        gameOverActive = false;

        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.name
        );
    }

    // =====================================================
    // CONSULTAR ESTADO
    // =====================================================

    public bool IsGameOver()
    {
        return gameOverActive;
    }
}