using UnityEngine;
using UnityEngine.UI;

public class DiagnosisUIController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject diagnosisPanel;
    [SerializeField] private GameObject confirmationPanel;

    [Header("Opciones de diagnóstico")]
    [SerializeField] private Toggle[] diagnosisOptions;

    [Header("Controlador del HUD")]
    [SerializeField] private HUDController hudController;

    private int selectedDiagnosis = -1;

    private void Start()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    // Abrir el panel de diagnóstico
    public void OpenDiagnosis()
    {
        if (diagnosisPanel != null)
            diagnosisPanel.SetActive(true);

        if (hudController != null)
            hudController.SetUIInteractionMode(true);
    }

    // Cerrar el diagnóstico
    public void CloseDiagnosis()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        if (diagnosisPanel != null)
            diagnosisPanel.SetActive(false);

        if (hudController != null)
            hudController.SetUIInteractionMode(false);
    }

    // Solicitar confirmación
    public void RequestConfirmation()
    {
        selectedDiagnosis = -1;

        for (int i = 0; i < diagnosisOptions.Length; i++)
        {
            if (diagnosisOptions[i] != null &&
                diagnosisOptions[i].isOn)
            {
                selectedDiagnosis = i;
                break;
            }
        }

        if (selectedDiagnosis == -1)
        {
            Debug.LogWarning(
                "Debes seleccionar un diagnóstico."
            );
            return;
        }

        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
    }

    // Volver sin confirmar
    public void CancelConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    public int GetSelectedDiagnosis()
    {
        return selectedDiagnosis;
    }
}