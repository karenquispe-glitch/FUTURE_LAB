using System.Collections.Generic;
using UnityEngine;

public class ClinicalAssessmentController : MonoBehaviour
{
    public enum ClinicalAssessmentAction
    {
        Inspection,
        Pulse,
        BloodPressure,
        Temperature,
        OxygenSaturation,
        Respiration
    }

    [Header("Configuración")]
    [SerializeField] private bool logProgress = true;

    [Header("Cambio de modelos")]
    [Tooltip("Paciente utilizado durante la entrevista.")]
    [SerializeField] private GameObject normalPatientModel;

    [Tooltip("Conjunto nuevo con paciente, manos e instrumentos.")]
    [SerializeField] private GameObject clinicalAssessmentSetup;

    [Tooltip("Animator del conjunto nuevo.")]
    [SerializeField] private Animator clinicalAnimator;

    private readonly HashSet<ClinicalAssessmentAction> completedActions =
        new HashSet<ClinicalAssessmentAction>();

    private bool minimumRequirementsReported;
    private bool assessmentStarted;

    public bool CanRequestTests =>
        IsCompleted(ClinicalAssessmentAction.Inspection) &&
        IsCompleted(ClinicalAssessmentAction.Pulse) &&
        IsCompleted(ClinicalAssessmentAction.BloodPressure) &&
        IsCompleted(ClinicalAssessmentAction.Temperature);

    public int CompletedActionCount => completedActions.Count;

    public bool AssessmentStarted => assessmentStarted;

    private void Start()
    {
        // Al comenzar se utiliza el paciente original.
        SetClinicalAssessmentMode(false);
    }

    // =====================================================
    // INICIAR VALORACIÓN CLÍNICA
    // =====================================================

    public void StartClinicalAssessment()
    {
        assessmentStarted = true;

        // Ocultar paciente anterior y mostrar conjunto nuevo.
        SetClinicalAssessmentMode(true);

        // Reiniciar las animaciones en la postura inicial.
        if (clinicalAnimator != null)
        {
            clinicalAnimator.Rebind();
            clinicalAnimator.Update(0f);
        }
        else
        {
            Debug.LogWarning(
                "ClinicalAssessmentController: " +
                "Clinical Animator no está asignado."
            );
        }

        Debug.Log(
            "VALORACIÓN CLÍNICA INICIADA. " +
            "Conjunto clínico nuevo activado."
        );
    }

    // =====================================================
    // FINALIZAR VALORACIÓN CLÍNICA
    // =====================================================

    public void FinishClinicalAssessment()
    {
        assessmentStarted = false;

        // Ocultar conjunto nuevo y restaurar paciente anterior.
        SetClinicalAssessmentMode(false);

        Debug.Log(
            "VALORACIÓN CLÍNICA FINALIZADA."
        );
    }

    // =====================================================
    // CAMBIAR ENTRE LOS DOS MODELOS
    // =====================================================

    private void SetClinicalAssessmentMode(bool assessmentActive)
    {
        if (normalPatientModel != null)
        {
            normalPatientModel.SetActive(!assessmentActive);
        }

        if (clinicalAssessmentSetup != null)
        {
            clinicalAssessmentSetup.SetActive(assessmentActive);
        }
    }

    // =====================================================
    // REGISTRAR ACCIONES DE LA VALORACIÓN
    // =====================================================

    public void RegisterAssessment(ClinicalAssessmentAction action)
    {
        if (!System.Enum.IsDefined(
                typeof(ClinicalAssessmentAction),
                action))
        {
            Debug.LogWarning(
                "Acción de evaluación no válida."
            );

            return;
        }

        if (completedActions.Contains(action))
        {
            if (logProgress)
            {
                Debug.Log(
                    "Acción ya registrada: " + action
                );
            }

            return;
        }

        completedActions.Add(action);

        if (logProgress)
        {
            Debug.Log(
                "Evaluación registrada: " + action
            );

            Debug.Log(
                "Acciones completadas: " +
                CompletedActionCount + "/6"
            );
        }

        if (CanRequestTests &&
            !minimumRequirementsReported)
        {
            minimumRequirementsReported = true;

            Debug.Log(
                "Evaluación mínima completada. " +
                "Ya se pueden solicitar los exámenes iniciales."
            );
        }
    }

    // =====================================================
    // MÉTODOS PARA LOS BOTONES
    // =====================================================

    public void RegisterInspection()
    {
        RegisterAssessment(
            ClinicalAssessmentAction.Inspection
        );
    }

    public void RegisterPulse()
    {
        RegisterAssessment(
            ClinicalAssessmentAction.Pulse
        );
    }

    public void RegisterBloodPressure()
    {
        RegisterAssessment(
            ClinicalAssessmentAction.BloodPressure
        );
    }

    public void RegisterTemperature()
    {
        RegisterAssessment(
            ClinicalAssessmentAction.Temperature
        );
    }

    public void RegisterOxygenSaturation()
    {
        RegisterAssessment(
            ClinicalAssessmentAction.OxygenSaturation
        );
    }

    public void RegisterRespiration()
    {
        RegisterAssessment(
            ClinicalAssessmentAction.Respiration
        );
    }

    public bool IsCompleted(
        ClinicalAssessmentAction action)
    {
        return completedActions.Contains(action);
    }

    // =====================================================
    // REINICIAR VALORACIÓN
    // =====================================================

    public void ResetAssessment()
    {
        completedActions.Clear();
        minimumRequirementsReported = false;
        assessmentStarted = false;

        SetClinicalAssessmentMode(false);

        if (logProgress)
        {
            Debug.Log(
                "Evaluación clínica reiniciada."
            );
        }
    }
}