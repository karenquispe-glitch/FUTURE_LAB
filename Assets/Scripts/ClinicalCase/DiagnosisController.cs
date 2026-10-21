using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosisController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject diagnosisPanel;

    [Header("Opciones de diagnóstico")]
    // Se conservan estos nombres para no perder las referencias del Inspector.
    [SerializeField] private Button btnPolicitemiaVera;
    [SerializeField] private Button btnAnemiaFerropenica;
    [SerializeField] private Button btnLeucemiaMieloideCronica;
    [SerializeField] private Button btnTrombocitemiaEsencial;

    [Header("Botón confirmar")]
    [SerializeField] private Button btnConfirmDiagnosis;

    [Header("Colores de selección")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.cyan;

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
            btnConfirmDiagnosis.onClick.RemoveListener(ConfirmDiagnosis);
            btnConfirmDiagnosis.onClick.AddListener(ConfirmDiagnosis);
        }

        ResetSelection();
    }

    private void ConfigureButton(Button button, string diagnosis)
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
        if (diagnosisPanel != null)
        {
            diagnosisPanel.SetActive(true);
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

        Button selectedButton = GetButtonForDiagnosis(diagnosis);

        if (selectedButton != null)
        {
            Image image = selectedButton.GetComponent<Image>();

            if (image != null)
            {
                image.color = selectedColor;
            }
        }

        Debug.Log(
            "DIAGNÓSTICO SELECCIONADO: " + selectedDiagnosis
        );
    }

    private Button GetButtonForDiagnosis(string diagnosis)
    {
        if (diagnosis == CorrectDiagnosis)
            return btnPolicitemiaVera;

        if (diagnosis == AcuteMyeloidLeukemia)
            return btnAnemiaFerropenica;

        if (diagnosis == ChronicMyeloidLeukemia)
            return btnLeucemiaMieloideCronica;

        if (diagnosis == ImmuneThrombocytopenicPurpura)
            return btnTrombocitemiaEsencial;

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
            "DIAGNÓSTICO CONFIRMADO: " + selectedDiagnosis
        );

        if (selectedDiagnosis == CorrectDiagnosis)
        {
            Debug.Log(
                "DIAGNÓSTICO CORRECTO: " + CorrectDiagnosis
            );

            if (diagnosisPanel != null)
            {
                diagnosisPanel.SetActive(false);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // En el siguiente paso se conectará aquí la victoria.
        }
        else
        {
            Debug.Log(
                "DIAGNÓSTICO INCORRECTO. La respuesta correcta era: " +
                CorrectDiagnosis
            );

            if (gameOverController != null)
            {
                gameOverController.GameOverDiagnosis();
            }
            else
            {
                Debug.LogError(
                    "DiagnosisController: falta asignar GameOverController."
                );
            }
        }
    }

    private void ResetSelection()
    {
        selectedDiagnosis = "";
        ResetButtonColors();
    }

    private void ResetButtonColors()
    {
        SetButtonColor(btnPolicitemiaVera, normalColor);
        SetButtonColor(btnAnemiaFerropenica, normalColor);
        SetButtonColor(btnLeucemiaMieloideCronica, normalColor);
        SetButtonColor(btnTrombocitemiaEsencial, normalColor);
    }

    private void SetButtonColor(Button button, Color color)
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
