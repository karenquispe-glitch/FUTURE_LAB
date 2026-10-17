using UnityEngine;

public class ClinicalAlertController : MonoBehaviour
{
public enum BleedingEventLevel
{
None = 0,
FirstDelay = 1,
SecondDelay = 2,
CriticalEnding = 3
}

[Header("Estado de las alertas")]
[SerializeField]
private bool hemorrhagicRiskDetected;

[SerializeField]
private BleedingEventLevel bleedingEventLevel =
    BleedingEventLevel.None;

[Header("Estado clinico")]
[SerializeField]
private bool nonPriorityActionsBlocked;

[SerializeField]
private bool gameOverRequired;

[Header("Depuracion")]
[SerializeField]
private bool logProgress = true;

private LabTestController labTestController;
private ScoreController scoreController;

private int bleedingEventCounter;

public bool HemorrhagicRiskDetected =>
    hemorrhagicRiskDetected;

public BleedingEventLevel CurrentBleedingEventLevel =>
    bleedingEventLevel;

public bool NonPriorityActionsBlocked =>
    nonPriorityActionsBlocked;

public bool GameOverRequired =>
    gameOverRequired;

public int BleedingEventCounter =>
    bleedingEventCounter;

// =====================================================
// CONFIGURAR DEPENDENCIAS
// =====================================================

public void InitializeDependencies(
    LabTestController laboratory,
    ScoreController score)
{
    labTestController = laboratory;
    scoreController = score;

    if (labTestController == null)
    {
        Debug.LogWarning(
            "ClinicalAlertController: falta configurar " +
            "LabTestController."
        );
    }

    if (scoreController == null)
    {
        Debug.LogWarning(
            "ClinicalAlertController: falta configurar " +
            "ScoreController. Las penalizaciones no podran " +
            "registrarse hasta configurar esta dependencia."
        );
    }

    if (logProgress)
    {
        Debug.Log(
            "Dependencias de alertas clinicas configuradas."
        );
    }
}

// =====================================================
// EVALUAR RIESGO HEMORRAGICO
// =====================================================

public void EvaluateHemorrhagicRisk()
{
    if (hemorrhagicRiskDetected)
    {
        if (logProgress)
        {
            Debug.Log(
                "El riesgo hemorragico ya fue detectado."
            );
        }

        return;
    }

    if (labTestController == null)
    {
        Debug.LogWarning(
            "ClinicalAlertController: falta configurar " +
            "LabTestController."
        );
        return;
    }

    bool smearReviewed =
        labTestController.IsTestReviewed(
            LabTestController.LabTest.PeripheralSmear
        );

    int coagulationTestsReviewed =
        labTestController.ReviewedCoagulationCount;

    if (smearReviewed && coagulationTestsReviewed >= 2)
    {
        TriggerHemorrhagicRiskAlert();
    }
    else if (logProgress)
    {
        Debug.Log(
            "Riesgo hemorragico pendiente. Frotis revisado: " +
            smearReviewed +
            " | Pruebas de coagulacion revisadas: " +
            coagulationTestsReviewed + "/4"
        );
    }
}

public void TriggerHemorrhagicRiskAlert()
{
    if (hemorrhagicRiskDetected)
        return;

    hemorrhagicRiskDetected = true;

    Debug.LogWarning(
        "ALERTA CLINICA: riesgo hemorragico elevado. " +
        "Se requiere priorizar la atencion."
    );
}

// =====================================================
// AVANZAR EVENTO DE SANGRADO
// Cada llamada representa un avance explicito.
// No utiliza temporizadores ni animaciones.
// =====================================================

public void TriggerBleedingEvent()
{
    if (gameOverRequired)
    {
        Debug.LogWarning(
            "El evento ya alcanzo el estado critico."
        );
        return;
    }

    if (!hemorrhagicRiskDetected)
    {
        Debug.LogWarning(
            "Primero debe detectarse el riesgo hemorragico."
        );
        return;
    }

    bleedingEventCounter++;

    switch (bleedingEventCounter)
    {
        case 1:
            RegisterFirstDelay();
            break;

        case 2:
            RegisterSecondDelay();
            break;

        default:
            RegisterCriticalEnding();
            break;
    }
}

private void RegisterFirstDelay()
{
    bleedingEventLevel = BleedingEventLevel.FirstDelay;

    ApplyBleedingDelayPenaltyIfAvailable();

    Debug.LogWarning(
        "PRIMER RETRASO: riesgo hemorragico sin atender. " +
        "Se registro la primera penalizacion cuando " +
        "ScoreController esta configurado."
    );
}

private void RegisterSecondDelay()
{
    bleedingEventLevel = BleedingEventLevel.SecondDelay;
    nonPriorityActionsBlocked = true;

    ApplyBleedingDelayPenaltyIfAvailable();

    Debug.LogError(
        "SEGUNDO RETRASO: deterioro clinico. " +
        "Acciones no prioritarias bloqueadas."
    );
}

private void RegisterCriticalEnding()
{
    bleedingEventLevel = BleedingEventLevel.CriticalEnding;
    nonPriorityActionsBlocked = true;
    gameOverRequired = true;

    if (scoreController != null)
    {
        scoreController.ApplyCriticalErrorPenalty();
    }
    else
    {
        Debug.LogWarning(
            "No se pudo aplicar la penalizacion critica: " +
            "ScoreController no esta configurado."
        );
    }

    Debug.LogError(
        "GAME OVER CLINICO: persiste el retraso ante " +
        "el riesgo hemorragico."
    );
}

private void ApplyBleedingDelayPenaltyIfAvailable()
{
    if (scoreController != null)
    {
        scoreController.ApplyBleedingDelayPenalty();
    }
    else
    {
        Debug.LogWarning(
            "No se pudo aplicar la penalizacion por retraso: " +
            "ScoreController no esta configurado."
        );
    }
}

// =====================================================
// CONTROL DE ACCIONES
// =====================================================

public bool CanPerformNonPriorityAction()
{
    return !nonPriorityActionsBlocked &&
           !gameOverRequired;
}

public bool CanPerformPriorityAction()
{
    return !gameOverRequired;
}

public void UnblockNonPriorityActions()
{
    if (gameOverRequired)
    {
        Debug.LogWarning(
            "No se pueden desbloquear acciones " +
            "despues del estado critico."
        );
        return;
    }

    if (!nonPriorityActionsBlocked)
    {
        if (logProgress)
        {
            Debug.Log(
                "Las acciones no prioritarias ya estan habilitadas."
            );
        }

        return;
    }

    nonPriorityActionsBlocked = false;

    if (logProgress)
    {
        Debug.Log(
            "Acciones no prioritarias habilitadas nuevamente."
        );
    }
}

// =====================================================
// CONSULTAS
// =====================================================

public int GetBleedingEventLevel()
{
    return (int)bleedingEventLevel;
}

public bool IsCriticalEnding()
{
    return gameOverRequired;
}

// =====================================================
// REINICIAR ESTADO
// =====================================================

public void ResetClinicalAlerts()
{
    hemorrhagicRiskDetected = false;

    bleedingEventLevel = BleedingEventLevel.None;

    nonPriorityActionsBlocked = false;
    gameOverRequired = false;

    bleedingEventCounter = 0;

    if (logProgress)
    {
        Debug.Log(
            "Estado de alertas clinicas reiniciado."
        );
    }
}

}
