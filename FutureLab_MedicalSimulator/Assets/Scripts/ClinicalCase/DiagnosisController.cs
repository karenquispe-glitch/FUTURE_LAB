using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiagnosisController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject diagnosisPanel;

    [Header("Opciones de diagnóstico")]
    [SerializeField] private Button btnPolicitemiaVera;
    [SerializeField] private Button btnAnemiaFerropenica;
    [SerializeField] private Button btnLeucemiaMieloideCronica;
    [SerializeField] private Button btnTrombocitemiaEsencial;

    [Header("Botón confirmar")]
    [SerializeField] private Button btnConfirmDiagnosis;

    [Header("Colores de selección")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.cyan;

    // =====================================================
    // GAME OVER
    // =====================================================

    [Header("Game Over")]
    [SerializeField] private GameOverController gameOverController;

    private string selectedDiagnosis = "";

    private void Awake()
    {
        // Conectar automáticamente los botones.
        if (btnPolicitemiaVera != null)
        {
            btnPolicitemiaVera.onClick.AddListener(
                () => SelectDiagnosis("Policitemia vera")
            );
        }

        if (btnAnemiaFerropenica != null)
        {
            btnAnemiaFerropenica.onClick.AddListener(
                () => SelectDiagnosis("Anemia ferropénica")
            );
        }

        if (btnLeucemiaMieloideCronica != null)
        {
            btnLeucemiaMieloideCronica.onClick.AddListener(
                () => SelectDiagnosis("Leucemia mieloide crónica")
            );
        }

        if (btnTrombocitemiaEsencial != null)
        {
            btnTrombocitemiaEsencial.onClick.AddListener(
                () => SelectDiagnosis("Trombocitemia esencial")
            );
        }

        if (btnConfirmDiagnosis != null)
        {
            btnConfirmDiagnosis.onClick.AddListener(
                ConfirmDiagnosis
            );
        }

        ResetSelection();
    }

    // =====================================================
    // MOSTRAR VENTANA DE DIAGNÓSTICO
    // =====================================================

    public void OpenDiagnosis()
    {
        if (diagnosisPanel != null)
        {
            diagnosisPanel.SetActive(true);
        }

        ResetSelection();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("VENTANA DE DIAGNÓSTICO ABIERTA.");
    }

    // =====================================================
    // SELECCIONAR DIAGNÓSTICO
    // =====================================================

    private void SelectDiagnosis(string diagnosis)
    {
        selectedDiagnosis = diagnosis;

        ResetButtonColors();

        Button selectedButton = null;

        if (diagnosis == "Policitemia vera")
        {
            selectedButton = btnPolicitemiaVera;
        }
        else if (diagnosis == "Anemia ferropénica")
        {
            selectedButton = btnAnemiaFerropenica;
        }
        else if (diagnosis == "Leucemia mieloide crónica")
        {
            selectedButton = btnLeucemiaMieloideCronica;
        }
        else if (diagnosis == "Trombocitemia esencial")
        {
            selectedButton = btnTrombocitemiaEsencial;
        }

        if (selectedButton != null)
        {
            Image image =
                selectedButton.GetComponent<Image>();

            if (image != null)
            {
                image.color = selectedColor;
            }
        }

        Debug.Log(
            "DIAGNÓSTICO SELECCIONADO: " +
            selectedDiagnosis
        );
    }

    // =====================================================
    // CONFIRMAR DIAGNÓSTICO
    // =====================================================

    private void ConfirmDiagnosis()
    {
        if (string.IsNullOrEmpty(selectedDiagnosis))
        {
            Debug.LogWarning(
                "DEBES SELECCIONAR UN DIAGNÓSTICO."
            );

            return;
        }

        Debug.Log(
            "DIAGNÓSTICO CONFIRMADO: " +
            selectedDiagnosis
        );

        // ================================================
        // DIAGNÓSTICO CORRECTO
        // ================================================

        if (selectedDiagnosis == "Policitemia vera")
        {
            Debug.Log(
                "DIAGNÓSTICO CORRECTO."
            );

            if (diagnosisPanel != null)
            {
                diagnosisPanel.SetActive(false);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // AQUÍ posteriormente conectaremos
            // la siguiente fase del caso.
        }

        // ================================================
        // DIAGNÓSTICO INCORRECTO
        // ================================================

        else
        {
            Debug.Log(
                "DIAGNÓSTICO INCORRECTO."
            );

            // ============================================
            // GAME OVER
            // ============================================

            if (gameOverController != null)
            {
                gameOverController.GameOverDiagnosis();
            }
            else
            {
                Debug.LogError(
                    "DiagnosisController: falta asignar " +
                    "GameOverController."
                );
            }
        }
    }

    // =====================================================
    // RESTABLECER SELECCIÓN
    // =====================================================

    private void ResetSelection()
    {
        selectedDiagnosis = "";

        ResetButtonColors();
    }

    private void ResetButtonColors()
    {
        SetButtonColor(
            btnPolicitemiaVera,
            normalColor
        );

        SetButtonColor(
            btnAnemiaFerropenica,
            normalColor
        );

        SetButtonColor(
            btnLeucemiaMieloideCronica,
            normalColor
        );

        SetButtonColor(
            btnTrombocitemiaEsencial,
            normalColor
        );
    }

    private void SetButtonColor(
        Button button,
        Color color
    )
    {
        if (button == null)
        {
            return;
        }

        Image image =
            button.GetComponent<Image>();

        if (image != null)
        {
            image.color = color;
        }
    }

    // =====================================================
    // OBTENER DIAGNÓSTICO SELECCIONADO
    // =====================================================

    public string GetSelectedDiagnosis()
    {
        return selectedDiagnosis;
    }

    // =====================================================
    // RESTAURAR DIAGNÓSTICO GUARDADO
    // =====================================================

    public void RestoreDiagnosis(
        string diagnosis
    )
    {
        if (string.IsNullOrEmpty(diagnosis))
        {
            ResetSelection();
            return;
        }

        SelectDiagnosis(diagnosis);
    }
}