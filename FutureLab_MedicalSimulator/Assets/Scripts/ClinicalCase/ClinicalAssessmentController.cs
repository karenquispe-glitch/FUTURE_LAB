
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

    [Header("Configuracion")]
    [SerializeField] private bool logProgress = true;

    private readonly HashSet<ClinicalAssessmentAction> completedActions =
        new HashSet<ClinicalAssessmentAction>();

    private bool minimumRequirementsReported;

    public bool CanRequestTests =>
        IsCompleted(ClinicalAssessmentAction.Inspection) &&
        IsCompleted(ClinicalAssessmentAction.Pulse) &&
        IsCompleted(ClinicalAssessmentAction.BloodPressure) &&
        IsCompleted(ClinicalAssessmentAction.Temperature);

    public int CompletedActionCount => completedActions.Count;

    public void RegisterAssessment(ClinicalAssessmentAction action)
    {
        if (!System.Enum.IsDefined(
            typeof(ClinicalAssessmentAction), action))
        {
            Debug.LogWarning("Accion de evaluacion no valida.");
            return;
        }

        if (completedActions.Contains(action))
        {
            if (logProgress)
                Debug.Log("Accion ya registrada: " + action);

            return;
        }

        completedActions.Add(action);

        if (logProgress)
        {
            Debug.Log("Evaluacion registrada: " + action);
            Debug.Log("Acciones completadas: " +
                      CompletedActionCount + "/6");
        }

        if (CanRequestTests && !minimumRequirementsReported)
        {
            minimumRequirementsReported = true;

            Debug.Log(
                "Evaluacion minima completada. " +
                "Ya se pueden solicitar los examenes iniciales."
            );
        }
    }

    // Metodos publicos disponibles para futuras conexiones.

    public void RegisterInspection() =>
        RegisterAssessment(ClinicalAssessmentAction.Inspection);

    public void RegisterPulse() =>
        RegisterAssessment(ClinicalAssessmentAction.Pulse);

    public void RegisterBloodPressure() =>
        RegisterAssessment(ClinicalAssessmentAction.BloodPressure);

    public void RegisterTemperature() =>
        RegisterAssessment(ClinicalAssessmentAction.Temperature);

    public void RegisterOxygenSaturation() =>
        RegisterAssessment(ClinicalAssessmentAction.OxygenSaturation);

    public void RegisterRespiration() =>
        RegisterAssessment(ClinicalAssessmentAction.Respiration);

    public bool IsCompleted(ClinicalAssessmentAction action)
    {
        return completedActions.Contains(action);
    }

    public void ResetAssessment()
    {
        completedActions.Clear();
        minimumRequirementsReported = false;

        if (logProgress)
            Debug.Log("Evaluacion clinica reiniciada.");
    }
}