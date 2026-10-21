using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicalHandsAnimationController : MonoBehaviour
{
    [Header("Animator de las manos")]
    [SerializeField] private Animator handsAnimator;

    [Header("Puntos de interacción")]
    [SerializeField] private Transform palpationPoint;
    [SerializeField] private Transform pulsePoint;
    [SerializeField] private Transform bloodPressurePoint;

    [Header("Panel de resultados")]
    [SerializeField] private GameObject inspectionResultsPanel;
    [SerializeField] private TMP_Text inspectionResultsText;

    [Header("Finalizar inspección")]
    [SerializeField] private Button finishInspectionButton;

    [Header("Transición al laboratorio")]
    [SerializeField] private GameObject clinicalAssessmentPanel;
    [SerializeField] private LaboratoryUIController laboratoryUIController;

    private bool palpationCompleted;
    private bool pulseCompleted;
    private bool bloodPressureCompleted;

    private void Start()
    {
        if (inspectionResultsPanel != null)
        {
            inspectionResultsPanel.SetActive(false);
        }

        if (finishInspectionButton != null)
        {
            finishInspectionButton.interactable = false;
        }
    }

    // =====================================================
    // PALPACIÓN ABDOMINAL
    // =====================================================

    public void PlayPalpation()
    {
        if (!PrepareProcedure(palpationPoint))
        {
            return;
        }

        ResetAllTriggers();
        handsAnimator.SetTrigger("Palpacion");

        palpationCompleted = true;
        ShowResults();
    }

    // =====================================================
    // TOMA DE PULSO
    // =====================================================

    public void PlayPulse()
    {
        if (!PrepareProcedure(pulsePoint))
        {
            return;
        }

        ResetAllTriggers();
        handsAnimator.SetTrigger("TomarPulso");

        pulseCompleted = true;
        ShowResults();
    }

    // =====================================================
    // MEDICIÓN DE PRESIÓN ARTERIAL
    // =====================================================

    public void PlayBloodPressure()
    {
        if (!PrepareProcedure(bloodPressurePoint))
        {
            return;
        }

        ResetAllTriggers();
        handsAnimator.SetTrigger("MedirPresion");

        bloodPressureCompleted = true;
        ShowResults();
    }

    // =====================================================
    // PREPARAR PROCEDIMIENTO
    // =====================================================

    private bool PrepareProcedure(Transform targetPoint)
    {
        if (handsAnimator == null)
        {
            Debug.LogError(
                "No se asignó el Animator de las manos."
            );

            return false;
        }

        if (targetPoint == null)
        {
            Debug.LogError(
                "No se asignó el punto de interacción."
            );

            return false;
        }

        // Cambiar únicamente la posición.
        // La orientación original se mantiene.
        transform.position = targetPoint.position;

        return true;
    }

    // =====================================================
    // MOSTRAR Y ACUMULAR RESULTADOS
    // =====================================================

    private void ShowResults()
    {
        if (inspectionResultsPanel == null)
        {
            Debug.LogError(
                "No se asignó Inspection Results Panel."
            );

            return;
        }

        if (inspectionResultsText == null)
        {
            Debug.LogError(
                "No se asignó Inspection Results Text."
            );

            return;
        }

        inspectionResultsPanel.SetActive(true);

        string results = "";

        if (palpationCompleted)
        {
            results +=
                "• PALPACIÓN ABDOMINAL\n" +
                "Abdomen blando, depresible y sin dolor.\n\n";
        }

        if (pulseCompleted)
        {
            results +=
                "• TOMA DE PULSO\n" +
                "Pulso regular. Frecuencia: 104 lpm.\n\n";
        }

        if (bloodPressureCompleted)
        {
            results +=
                "• PRESIÓN ARTERIAL\n" +
                "Presión arterial: 100/65 mmHg.\n\n";
        }

        inspectionResultsText.text = results;

        // Una inspección es suficiente para continuar.
        if (finishInspectionButton != null)
        {
            finishInspectionButton.interactable =
                palpationCompleted ||
                pulseCompleted ||
                bloodPressureCompleted;
        }
    }

    // =====================================================
    // FINALIZAR INSPECCIÓN Y ABRIR LABORATORIO
    // =====================================================

    public void FinishInspection()
    {
        bool performedAnyInspection =
            palpationCompleted ||
            pulseCompleted ||
            bloodPressureCompleted;

        if (!performedAnyInspection)
        {
            Debug.LogWarning(
                "Debes realizar al menos una inspección."
            );

            return;
        }

        if (laboratoryUIController == null)
        {
            Debug.LogError(
                "No se asignó Laboratory UI Controller."
            );

            return;
        }

        ResetAllTriggers();

        // Habilitar el botón de laboratorio existente.
        laboratoryUIController.EnableAfterAssessment();

        // Abrir directamente la selección de laboratorio.
        laboratoryUIController.OpenLaboratory();

        // Cerrar la interfaz de inspección.
        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        Debug.Log(
            "INSPECCIÓN FINALIZADA. LABORATORIO HABILITADO."
        );
    }

    // =====================================================
    // LIMPIAR TRIGGERS
    // =====================================================

    private void ResetAllTriggers()
    {
        if (handsAnimator == null)
        {
            return;
        }

        handsAnimator.ResetTrigger("Palpacion");
        handsAnimator.ResetTrigger("TomarPulso");
        handsAnimator.ResetTrigger("MedirPresion");
    }
}