using UnityEngine;

public class ClinicalCaseFlowController : MonoBehaviour
{
    // =====================================================
    // FASES DEL CASO CLÍNICO
    // =====================================================

    public enum ClinicalCasePhase
    {
        Inicio,
        Acercamiento,
        Entrevista,
        Valoracion,
        PruebasIniciales,
        Hipotesis,
        Confirmacion,
        Diagnostico,
        Cierre
    }

    // =====================================================
    // ESTADO ACTUAL
    // =====================================================

    [Header("Estado del caso")]
    [SerializeField]
    private ClinicalCasePhase currentPhase =
        ClinicalCasePhase.Inicio;

    // =====================================================
    // CONSULTAR FASE ACTUAL
    // =====================================================

    public ClinicalCasePhase GetCurrentPhase()
    {
        return currentPhase;
    }

    // =====================================================
    // CAMBIAR FASE
    // =====================================================

    public void SetPhase(ClinicalCasePhase newPhase)
    {
        currentPhase = newPhase;

        Debug.Log(
            "FASE CLÍNICA ACTUAL: " +
            currentPhase
        );
    }

    // =====================================================
    // CAMBIOS DE FASE ESPECÍFICOS
    // =====================================================

    public void GoToStart()
    {
        SetPhase(
            ClinicalCasePhase.Inicio
        );
    }

    public void GoToApproach()
    {
        SetPhase(
            ClinicalCasePhase.Acercamiento
        );
    }

    public void GoToInterview()
    {
        SetPhase(
            ClinicalCasePhase.Entrevista
        );
    }

    public void GoToAssessment()
    {
        SetPhase(
            ClinicalCasePhase.Valoracion
        );
    }

    public void GoToInitialTests()
    {
        SetPhase(
            ClinicalCasePhase.PruebasIniciales
        );
    }

    public void GoToHypothesis()
    {
        SetPhase(
            ClinicalCasePhase.Hipotesis
        );
    }

    public void GoToConfirmation()
    {
        SetPhase(
            ClinicalCasePhase.Confirmacion
        );
    }

    public void GoToDiagnosis()
    {
        SetPhase(
            ClinicalCasePhase.Diagnostico
        );
    }

    public void GoToClosure()
    {
        SetPhase(
            ClinicalCasePhase.Cierre
        );
    }

    // =====================================================
    // RESTAURAR FASE DESDE GUARDADO
    // =====================================================

    public void RestorePhase(
        ClinicalCasePhase savedPhase
    )
    {
        currentPhase = savedPhase;

        Debug.Log(
            "FASE CLÍNICA RESTAURADA: " +
            currentPhase
        );
    }
}