using UnityEngine;

public class DifferentialDiagnosisController : MonoBehaviour
{
public enum DifferentialDiagnosis
{
None,
IronDeficiencyAnemia,
ImmuneThrombocytopenia,
AplasticAnemia,
AcuteMyeloidLeukemia,
AcutePromyelocyticLeukemia
}

[Header("Dependencias")]
[SerializeField]
private LabTestController labTestController;

[SerializeField]
private ScoreController scoreController;

[Header("Estado del diagnostico")]
[SerializeField]
private DifferentialDiagnosis provisionalDiagnosis =
    DifferentialDiagnosis.None;

[SerializeField]
private DifferentialDiagnosis confirmedDiagnosis =
    DifferentialDiagnosis.None;

private bool hasProvisionalDiagnosis;
private bool hasConfirmedDiagnosis;

public DifferentialDiagnosis ProvisionalDiagnosis =>
    provisionalDiagnosis;

public DifferentialDiagnosis ConfirmedDiagnosis =>
    confirmedDiagnosis;

public bool HasProvisionalDiagnosis =>
    hasProvisionalDiagnosis;

public bool HasConfirmedDiagnosis =>
    hasConfirmedDiagnosis;

// =====================================================
// CONFIGURAR DEPENDENCIAS
// =====================================================

public void InitializeDependencies(
    LabTestController laboratory)
{
    labTestController = laboratory;

    if (labTestController == null)
    {
        Debug.LogWarning(
            "DifferentialDiagnosisController: falta " +
            "configurar LabTestController."
        );
        return;
    }

    Debug.Log(
        "Dependencia del laboratorio configurada."
    );
}

public void InitializeScoreController(
    ScoreController score)
{
    scoreController = score;

    if (scoreController == null)
    {
        Debug.LogWarning(
            "DifferentialDiagnosisController: falta " +
            "configurar ScoreController."
        );
        return;
    }

    Debug.Log(
        "Dependencia de puntuacion configurada."
    );
}

// =====================================================
// DIAGNOSTICO PROVISIONAL
// =====================================================

public void SetProvisionalDiagnosis(
    DifferentialDiagnosis diagnosis)
{
    if (hasConfirmedDiagnosis)
    {
        Debug.LogWarning(
            "El diagnostico final ya fue confirmado."
        );
        return;
    }

    if (!System.Enum.IsDefined(
            typeof(DifferentialDiagnosis), diagnosis) ||
        diagnosis == DifferentialDiagnosis.None)
    {
        Debug.LogWarning(
            "Debe seleccionar un diagnostico valido."
        );
        return;
    }

    provisionalDiagnosis = diagnosis;
    hasProvisionalDiagnosis = true;

    Debug.Log(
        "Diagnostico provisional: " +
        GetDiagnosisName(provisionalDiagnosis)
    );

    if (diagnosis ==
        DifferentialDiagnosis.AcutePromyelocyticLeukemia)
    {
        Debug.Log(
            "La LPA permanece provisional hasta revisar " +
            "la evidencia confirmatoria, incluido PML-RARA."
        );
    }
}

// =====================================================
// OPCIONES DE DIAGNOSTICO DIFERENCIAL
// =====================================================

public void SetIronDeficiencyAnemia()
{
    SetProvisionalDiagnosis(
        DifferentialDiagnosis.IronDeficiencyAnemia);
}

public void SetImmuneThrombocytopenia()
{
    SetProvisionalDiagnosis(
        DifferentialDiagnosis.ImmuneThrombocytopenia);
}

public void SetAplasticAnemia()
{
    SetProvisionalDiagnosis(
        DifferentialDiagnosis.AplasticAnemia);
}

public void SetAcuteMyeloidLeukemia()
{
    SetProvisionalDiagnosis(
        DifferentialDiagnosis.AcuteMyeloidLeukemia);
}

public void SetAcutePromyelocyticLeukemia()
{
    SetProvisionalDiagnosis(
        DifferentialDiagnosis.AcutePromyelocyticLeukemia);
}

// =====================================================
// VALIDAR EVIDENCIA CLINICA
// =====================================================

private bool HasRequiredClinicalEvidence()
{
    if (labTestController == null)
    {
        Debug.LogError(
            "No se puede validar la evidencia: " +
            "LabTestController no esta configurado."
        );
        return false;
    }

    bool smearReviewed =
        labTestController.IsTestReviewed(
            LabTestController.LabTest.PeripheralSmear);

    if (!smearReviewed)
    {
        Debug.LogWarning(
            "Debes revisar el frotis periferico."
        );
        return false;
    }

    if (labTestController.ReviewedCoagulationCount < 2)
    {
        Debug.LogWarning(
            "Debes revisar al menos dos pruebas de coagulacion."
        );
        return false;
    }

    if (!labTestController.IsTestReviewed(
            LabTestController.LabTest.PMLRARA))
    {
        Debug.LogWarning(
            "La prueba molecular PML-RARA aun no fue revisada."
        );
        return false;
    }

    if (!labTestController.IsPMLRARAPositive)
    {
        Debug.LogWarning(
            "PML-RARA no tiene un resultado positivo registrado."
        );
        return false;
    }

    return true;
}

// =====================================================
// EVALUAR PUNTUACION DE CONFIRMACION
// =====================================================

public bool EvaluateConfirmatoryScore()
{
    if (labTestController == null)
    {
        Debug.LogWarning(
            "No se puede evaluar la confirmacion: " +
            "LabTestController no esta configurado."
        );
        return false;
    }

    if (scoreController == null)
    {
        Debug.LogWarning(
            "No se puede evaluar la confirmacion: " +
            "ScoreController no esta configurado."
        );
        return false;
    }

    if (!labTestController.ConfirmatoryTestsCompleted)
    {
        Debug.LogWarning(
            "Para completar la confirmacion debes solicitar " +
            "y revisar citometria de flujo, citogenetica " +
            "y PML-RARA."
        );
        return false;
    }

    // No volver a puntuar una categoria ya evaluada.
    if (scoreController.IsCategoryCompleted(
            ScoreController.ScoreCategory.Confirmation))
    {
        Debug.Log(
            "La categoria Confirmacion ya fue evaluada."
        );

        return scoreController.GetCategoryScore(
            ScoreController.ScoreCategory.Confirmation) == 15;
    }

    scoreController.RegisterConfirmationScore(15);

    Debug.Log(
        "Categoria Confirmacion evaluada: 15/15 puntos."
    );

    return true;
}

// =====================================================
// CONFIRMAR DIAGNOSTICO FINAL
// =====================================================

public bool ConfirmFinalDiagnosis()
{
    if (hasConfirmedDiagnosis)
    {
        Debug.LogWarning(
            "El diagnostico final ya esta confirmado."
        );
        return false;
    }

    if (!hasProvisionalDiagnosis)
    {
        Debug.LogWarning(
            "Primero debe establecer un diagnostico provisional."
        );
        return false;
    }

    if (provisionalDiagnosis !=
        DifferentialDiagnosis.AcutePromyelocyticLeukemia)
    {
        Debug.LogWarning(
            "El diagnostico provisional no corresponde " +
            "al diagnostico final esperado en este caso."
        );
        return false;
    }

    if (!HasRequiredClinicalEvidence())
        return false;

    if (labTestController == null ||
        !labTestController.ConfirmatoryTestsCompleted)
    {
        Debug.LogWarning(
            "Debes completar y revisar las tres pruebas " +
            "confirmatorias requeridas."
        );
        return false;
    }

    if (scoreController == null)
    {
        Debug.LogError(
            "No se puede confirmar el diagnostico: " +
            "ScoreController no esta configurado."
        );
        return false;
    }

    // Evaluar primero la categoria Confirmacion.
    if (!EvaluateConfirmatoryScore())
    {
        Debug.LogWarning(
            "No se pudo completar la puntuacion de confirmacion."
        );
        return false;
    }

    // Evitar puntuar el diagnostico final dos veces.
    if (scoreController.IsCategoryCompleted(
            ScoreController.ScoreCategory.FinalDiagnosis))
    {
        Debug.LogWarning(
            "La categoria Diagnostico final ya fue evaluada."
        );
        return false;
    }

    confirmedDiagnosis =
        DifferentialDiagnosis.AcutePromyelocyticLeukemia;

    hasConfirmedDiagnosis = true;

    scoreController.RegisterFinalDiagnosisScore(10);

    Debug.Log(
        "DIAGNOSTICO FINAL CONFIRMADO: " +
        "Leucemia promielocitica aguda (LPA). " +
        "Evidencia molecular PML-RARA positiva."
    );

    Debug.Log(
        "Puntuacion registrada: Confirmacion 15/15 " +
        "y Diagnostico final 10/10."
    );

    return true;
}

// =====================================================
// CONSULTAS PUBLICAS
// =====================================================

public bool IsDiagnosisConfirmed(
    DifferentialDiagnosis diagnosis)
{
    return hasConfirmedDiagnosis &&
           confirmedDiagnosis == diagnosis;
}

public string GetProvisionalDiagnosisName()
{
    return hasProvisionalDiagnosis
        ? GetDiagnosisName(provisionalDiagnosis)
        : "Sin diagnostico provisional";
}

public string GetConfirmedDiagnosisName()
{
    return hasConfirmedDiagnosis
        ? GetDiagnosisName(confirmedDiagnosis)
        : "Diagnostico no confirmado";
}

private string GetDiagnosisName(
    DifferentialDiagnosis diagnosis)
{
    switch (diagnosis)
    {
        case DifferentialDiagnosis.IronDeficiencyAnemia:
            return "Anemia ferropenica";

        case DifferentialDiagnosis.ImmuneThrombocytopenia:
            return "Trombocitopenia inmune";

        case DifferentialDiagnosis.AplasticAnemia:
            return "Anemia aplasica";

        case DifferentialDiagnosis.AcuteMyeloidLeukemia:
            return "Leucemia mieloide aguda";

        case DifferentialDiagnosis.AcutePromyelocyticLeukemia:
            return "Leucemia promielocitica aguda (LPA)";

        default:
            return "Sin diagnostico";
    }
}

// =====================================================
// REINICIAR
// =====================================================

public void ResetDiagnosis()
{
    provisionalDiagnosis = DifferentialDiagnosis.None;
    confirmedDiagnosis = DifferentialDiagnosis.None;

    hasProvisionalDiagnosis = false;
    hasConfirmedDiagnosis = false;

    Debug.Log(
        "Estado del diagnostico reiniciado."
    );
}


}
