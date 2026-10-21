using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicalHandsAnimationController : MonoBehaviour
{
    [Header("Animator del conjunto clínico")]
    [SerializeField] private Animator handsAnimator;

    [Header("Controlador de valoración")]
    [SerializeField]
    private ClinicalAssessmentController clinicalAssessmentController;

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

    private const string PalpationState =
        "Base Layer.PalpacionAbdominal";

    private const string PulseState =
        "Base Layer.TomarPulso";

    private const string BloodPressureState =
        "Base Layer.MedirPresion";

    private void Awake()
    {
        if (handsAnimator != null)
        {
            handsAnimator.applyRootMotion = false;
        }
    }

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
        if (!PlayAnimation(PalpationState))
        {
            return;
        }

        palpationCompleted = true;

        if (clinicalAssessmentController != null)
        {
            clinicalAssessmentController.RegisterInspection();
        }

        ShowResults();

        Debug.Log("ANIMACIÓN DE PALPACIÓN EJECUTADA.");
    }

    // =====================================================
    // TOMA DE PULSO
    // =====================================================

    public void PlayPulse()
    {
        if (!PlayAnimation(PulseState))
        {
            return;
        }

        pulseCompleted = true;

        if (clinicalAssessmentController != null)
        {
            clinicalAssessmentController.RegisterPulse();
        }

        ShowResults();

        Debug.Log("ANIMACIÓN DE PULSO EJECUTADA.");
    }

    // =====================================================
    // MEDICIÓN DE PRESIÓN ARTERIAL
    // =====================================================

    public void PlayBloodPressure()
    {
        if (!PlayAnimation(BloodPressureState))
        {
            return;
        }

        bloodPressureCompleted = true;

        if (clinicalAssessmentController != null)
        {
            clinicalAssessmentController.RegisterBloodPressure();
        }

        ShowResults();

        Debug.Log("ANIMACIÓN DE PRESIÓN ARTERIAL EJECUTADA.");
    }

    // =====================================================
    // REPRODUCIR ESTADO DIRECTAMENTE
    // =====================================================

    private bool PlayAnimation(string stateName)
    {
        if (handsAnimator == null)
        {
            Debug.LogError(
                "ClinicalHandsAnimationController: " +
                "no se asignó Hands Animator."
            );

            return false;
        }

        if (!handsAnimator.gameObject.activeInHierarchy)
        {
            Debug.LogError(
                "ClinicalHandsAnimationController: " +
                "ClinicalInspectionHands_NEW está desactivado."
            );

            return false;
        }

        if (handsAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError(
                "ClinicalHandsAnimationController: " +
                "el Animator nuevo no tiene Animator Controller."
            );

            return false;
        }

        int stateHash = Animator.StringToHash(stateName);

        if (!handsAnimator.HasState(0, stateHash))
        {
            Debug.LogError(
                "No se encontró el estado '" +
                stateName +
                "' en la capa 0 del Animator."
            );

            return false;
        }

        handsAnimator.enabled = true;
        handsAnimator.applyRootMotion = false;

        // Reproduce el estado desde el primer fotograma.
        handsAnimator.Play(stateHash, 0, 0f);
        handsAnimator.Update(0f);

        Debug.Log(
            "Estado enviado al Animator: " +
            stateName
        );

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

        if (clinicalAssessmentController != null)
        {
            clinicalAssessmentController.FinishClinicalAssessment();
        }

        if (laboratoryUIController == null)
        {
            Debug.LogError(
                "No se asignó Laboratory UI Controller."
            );

            return;
        }

        laboratoryUIController.EnableAfterAssessment();
        laboratoryUIController.OpenLaboratory();

        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        Debug.Log(
            "INSPECCIÓN FINALIZADA. LABORATORIO HABILITADO."
        );
    }
}