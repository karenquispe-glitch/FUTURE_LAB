using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosisController : MonoBehaviour
{
    [Header("Panel de diagnóstico")]
    [SerializeField] private GameObject diagnosisPanel;

    [Header("Panel de victoria")]
    [SerializeField] private GameObject victoryPanel;

    [Header("Interfaz que debe ocultarse al finalizar")]
    [SerializeField] private GameObject hudPanel;

    [Header("Opciones de diagnóstico")]
    // Se conservan estos nombres para no perder referencias del Inspector.
    [SerializeField] private Button btnPolicitemiaVera;
    [SerializeField] private Button btnAnemiaFerropenica;
    [SerializeField] private Button btnLeucemiaMieloideCronica;
    [SerializeField] private Button btnTrombocitemiaEsencial;

    [Header("Botón confirmar")]
    [SerializeField] private Button btnConfirmDiagnosis;

    [Header("Colores de selección")]
    [SerializeField]
    private Color normalColor =
        new Color32(9, 42, 67, 255);

    [SerializeField]
    private Color selectedColor =
        new Color32(8, 127, 121, 255);

    [Header("Game Over")]
    [SerializeField] private GameOverController gameOverController;

    private const string CorrectDiagnosis =
        "Leucemia promielocítica aguda";

    private const string AcuteMyeloidLeukemia =
        "Leucemia mieloide aguda";

    private const string ChronicMyeloidLeukemia =
        "Leucemia mieloide crónica";

    private const string ImmuneThrombocytopenicPurpura =
        "Púrpura trombocitopénica inmune";

    private string selectedDiagnosis = "";

    private void Awake()
    {
        ConfigureButton(
            btnPolicitemiaVera,
            CorrectDiagnosis
        );

        ConfigureButton(
            btnAnemiaFerropenica,
            AcuteMyeloidLeukemia
        );

        ConfigureButton(
            btnLeucemiaMieloideCronica,
            ChronicMyeloidLeukemia
        );

        ConfigureButton(
            btnTrombocitemiaEsencial,
            ImmuneThrombocytopenicPurpura
        );

        if (btnConfirmDiagnosis != null)
        {
            btnConfirmDiagnosis.onClick.RemoveListener(
                ConfirmDiagnosis
            );

            btnConfirmDiagnosis.onClick.AddListener(
                ConfirmDiagnosis
            );
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        ResetSelection();
    }

    private void ConfigureButton(
        Button button,
        string diagnosis)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            () => SelectDiagnosis(diagnosis)
        );

        TMP_Text buttonText =
            button.GetComponentInChildren<TMP_Text>(true);

        if (buttonText != null)
        {
            buttonText.text = diagnosis;
        }
    }

    public void OpenDiagnosis()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (diagnosisPanel != null)
        {
            diagnosisPanel.SetActive(true);
            diagnosisPanel.transform.SetAsLastSibling();
        }

        ResetSelection();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("VENTANA DE DIAGNÓSTICO ABIERTA.");
    }

    private void SelectDiagnosis(string diagnosis)
    {
        selectedDiagnosis = diagnosis;

        ResetButtonColors();

        Button selectedButton =
            GetButtonForDiagnosis(diagnosis);

        if (selectedButton != null)
        {
            SetButtonColor(
                selectedButton,
                selectedColor
            );
        }

        Debug.Log(
            "DIAGNÓSTICO SELECCIONADO: " +
            selectedDiagnosis
        );
    }

    private Button GetButtonForDiagnosis(string diagnosis)
    {
        if (diagnosis == CorrectDiagnosis)
        {
            return btnPolicitemiaVera;
        }

        if (diagnosis == AcuteMyeloidLeukemia)
        {
            return btnAnemiaFerropenica;
        }

        if (diagnosis == ChronicMyeloidLeukemia)
        {
            return btnLeucemiaMieloideCronica;
        }

        if (diagnosis ==
            ImmuneThrombocytopenicPurpura)
        {
            return btnTrombocitemiaEsencial;
        }

        return null;
    }

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

        if (selectedDiagnosis == CorrectDiagnosis)
        {
            ShowVictory();
        }
        else
        {
            ShowDefeat();
        }
    }

    private void ShowVictory()
    {
        Debug.Log(
            "DIAGNÓSTICO CORRECTO: " +
            CorrectDiagnosis
        );

        if (diagnosisPanel != null)
        {
            diagnosisPanel.SetActive(false);
        }

        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            victoryPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError(
                "DiagnosisController: falta asignar VictoryPanel."
            );
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowDefeat()
    {
        Debug.Log(
            "DIAGNÓSTICO INCORRECTO. " +
            "La respuesta correcta era: " +
            CorrectDiagnosis
        );

        if (diagnosisPanel != null)
        {
            diagnosisPanel.SetActive(false);
        }

        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

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
        Color color)
    {
        if (button == null)
        {
            return;
        }

        Image image = button.GetComponent<Image>();

        if (image != null)
        {
            image.color = color;
        }
    }

    public string GetSelectedDiagnosis()
    {
        return selectedDiagnosis;
    }

    public void RestoreDiagnosis(string diagnosis)
    {
        if (string.IsNullOrEmpty(diagnosis))
        {
            ResetSelection();
            return;
        }

        SelectDiagnosis(diagnosis);
    }
}