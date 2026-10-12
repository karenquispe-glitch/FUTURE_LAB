using System.Collections.Generic;
using UnityEngine;

public class LabTestController : MonoBehaviour
{
public enum LabTest
{
Hemogram,
PeripheralSmear,
PT,
PTT,
Fibrinogen,
DimerD,
BoneMarrow,
Cytochemistry,
FlowCytometry,
Cytogenetics,
PMLRARA
}

[Header("Dependencias")]
[SerializeField]
private ClinicalAssessmentController assessmentController;

[SerializeField]
private ClinicalAlertController clinicalAlertController;

[Header("Configuracion")]
[SerializeField]
private bool logProgress = true;

private readonly HashSet<LabTest> requestedTests =
    new HashSet<LabTest>();

private readonly HashSet<LabTest> reviewedTests =
    new HashSet<LabTest>();

private bool confirmatoryTestsUnlocked;
private bool bleedingRiskAlertTriggered;

public bool IsPMLRARAPositive { get; private set; }

public bool ConfirmatoryTestsUnlocked =>
    confirmatoryTestsUnlocked;

public bool BleedingRiskAlertTriggered =>
    bleedingRiskAlertTriggered;

public int RequestedTestCount =>
    requestedTests.Count;

public int ReviewedTestCount =>
    reviewedTests.Count;

// =====================================================
// CONTAR PRUEBAS DE COAGULACION REVISADAS
// =====================================================

public int ReviewedCoagulationCount
{
    get
    {
        int count = 0;

        if (IsTestReviewed(LabTest.PT))
            count++;

        if (IsTestReviewed(LabTest.PTT))
            count++;

        if (IsTestReviewed(LabTest.Fibrinogen))
            count++;

        if (IsTestReviewed(LabTest.DimerD))
            count++;

        return count;
    }
}

// =====================================================
// VALIDAR PRUEBAS CONFIRMATORIAS
// =====================================================

public bool ConfirmatoryTestsCompleted
{
    get
    {
        return
            IsTestRequested(LabTest.FlowCytometry) &&
            IsTestReviewed(LabTest.FlowCytometry) &&

            IsTestRequested(LabTest.Cytogenetics) &&
            IsTestReviewed(LabTest.Cytogenetics) &&

            IsTestRequested(LabTest.PMLRARA) &&
            IsTestReviewed(LabTest.PMLRARA);
    }
}

// =====================================================
// CONFIGURAR DEPENDENCIAS
// =====================================================

public void InitializeDependencies(
    ClinicalAssessmentController assessment,
    ClinicalAlertController alerts)
{
    assessmentController = assessment;
    clinicalAlertController = alerts;

    if (assessmentController == null)
    {
        Debug.LogWarning(
            "LabTestController: ClinicalAssessmentController " +
            "no esta configurado."
        );
    }

    if (clinicalAlertController == null)
    {
        Debug.LogWarning(
            "LabTestController: ClinicalAlertController " +
            "no esta configurado."
        );
    }

    if (logProgress)
    {
        Debug.Log(
            "Dependencias del laboratorio configuradas."
        );
    }
}

// =====================================================
// SOLICITAR PRUEBAS
// =====================================================

public bool RequestTest(LabTest test)
{
    if (!System.Enum.IsDefined(typeof(LabTest), test))
    {
        Debug.LogWarning("Tipo de prueba no valido.");
        return false;
    }

    if (reviewedTests.Contains(test) ||
        requestedTests.Contains(test))
    {
        if (logProgress)
        {
            Debug.Log(
                "La prueba ya fue solicitada o revisada: " + test
            );
        }

        return false;
    }

    if (assessmentController == null)
    {
        Debug.LogWarning(
            "No se puede solicitar la prueba: falta configurar " +
            "ClinicalAssessmentController."
        );
        return false;
    }

    if (!assessmentController.CanRequestTests)
    {
        Debug.LogWarning(
            "Completa primero inspeccion, pulso, presion arterial " +
            "y temperatura antes de solicitar pruebas."
        );
        return false;
    }

    if (IsConfirmatoryTest(test) &&
        !confirmatoryTestsUnlocked)
    {
        Debug.LogWarning(
            "Prueba confirmatoria bloqueada. Revisa primero " +
            "el frotis y al menos dos resultados de coagulacion."
        );
        return false;
    }

    requestedTests.Add(test);

    if (logProgress)
    {
        Debug.Log("Prueba solicitada: " + test);
    }

    return true;
}

// =====================================================
// PRUEBAS INICIALES Y DE COAGULACION
// =====================================================

public void ReviewHemogram() =>
    ReviewTest(LabTest.Hemogram);

public void ReviewPeripheralSmear() =>
    ReviewTest(LabTest.PeripheralSmear);

public void ReviewPT() =>
    ReviewTest(LabTest.PT);

public void ReviewPTT() =>
    ReviewTest(LabTest.PTT);

public void ReviewFibrinogen() =>
    ReviewTest(LabTest.Fibrinogen);

public void ReviewDimerD() =>
    ReviewTest(LabTest.DimerD);

// =====================================================
// PRUEBAS CONFIRMATORIAS
// =====================================================

public void ReviewBoneMarrow() =>
    ReviewTest(LabTest.BoneMarrow);

public void ReviewCytochemistry() =>
    ReviewTest(LabTest.Cytochemistry);

public void ReviewFlowCytometry() =>
    ReviewTest(LabTest.FlowCytometry);

public void ReviewCytogenetics() =>
    ReviewTest(LabTest.Cytogenetics);

public void ReviewPMLRARA() =>
    ReviewTest(LabTest.PMLRARA);

// =====================================================
// LOGICA DE REVISION
// =====================================================

private void ReviewTest(LabTest test)
{
    if (reviewedTests.Contains(test))
    {
        if (logProgress)
        {
            Debug.Log(
                "Esta prueba ya fue revisada: " + test
            );
        }

        return;
    }

    if (!requestedTests.Contains(test))
    {
        Debug.LogWarning(
            "Debes solicitar la prueba antes de revisar " +
            "su resultado: " + test
        );
        return;
    }

    if (IsConfirmatoryTest(test) &&
        !confirmatoryTestsUnlocked)
    {
        Debug.LogWarning(
            "La prueba confirmatoria continua bloqueada: " + test
        );
        return;
    }

    reviewedTests.Add(test);

    if (logProgress)
    {
        Debug.Log("Resultado revisado: " + test);
    }

    ShowKnownResult(test);
    CheckClinicalConditions();
}

private bool IsConfirmatoryTest(LabTest test)
{
    return test == LabTest.BoneMarrow ||
           test == LabTest.Cytochemistry ||
           test == LabTest.FlowCytometry ||
           test == LabTest.Cytogenetics ||
           test == LabTest.PMLRARA;
}

// =====================================================
// DESBLOQUEO Y ALERTAS
// =====================================================

private void CheckClinicalConditions()
{
    bool smearReviewed =
        IsTestReviewed(LabTest.PeripheralSmear);

    bool coagulationRequirementMet =
        ReviewedCoagulationCount >= 2;

    if (smearReviewed &&
        coagulationRequirementMet &&
        !bleedingRiskAlertTriggered)
    {
        if (clinicalAlertController != null)
        {
            clinicalAlertController.EvaluateHemorrhagicRisk();
            bleedingRiskAlertTriggered = true;
        }
        else
        {
            Debug.LogWarning(
                "Riesgo hemorragico detectado por laboratorio, " +
                "pero ClinicalAlertController aun no esta " +
                "configurado. La alerta podra reintentarse."
            );
        }
    }

    if (smearReviewed &&
        coagulationRequirementMet &&
        !confirmatoryTestsUnlocked)
    {
        confirmatoryTestsUnlocked = true;

        if (logProgress)
        {
            Debug.Log(
                "Pruebas confirmatorias desbloqueadas."
            );
        }
    }
}

// =====================================================
// RESULTADOS DEL CASO CLINICO
// =====================================================

private void ShowKnownResult(LabTest test)
{
    switch (test)
    {
        case LabTest.Hemogram:
            Debug.Log(
                "HEMOGRAMA: Hb 8.1 g/dL, Hto 25%, " +
                "leucocitos 38,000/uL y plaquetas 28,000/uL."
            );
            break;

        case LabTest.PeripheralSmear:
            Debug.Log(
                "FROTIS: promielocitos anormales, " +
                "granulacion intensa y bastones de Auer."
            );
            break;

        case LabTest.PT:
            Debug.Log(
                "COAGULACION: tiempo de protrombina prolongado."
            );
            break;

        case LabTest.PTT:
            Debug.Log(
                "COAGULACION: tiempo parcial de tromboplastina " +
                "prolongado."
            );
            break;

        case LabTest.Fibrinogen:
            Debug.Log(
                "COAGULACION: fibrinogeno disminuido."
            );
            break;

        case LabTest.DimerD:
            Debug.Log(
                "COAGULACION: dimero D elevado."
            );
            break;

        case LabTest.BoneMarrow:
            Debug.Log(
                "ASPIRADO DE MEDULA OSEA revisado."
            );
            break;

        case LabTest.Cytochemistry:
            Debug.Log(
                "CITOQUIMICA: mieloperoxidasa (MPO) positiva."
            );
            break;

        case LabTest.FlowCytometry:
            Debug.Log(
                "CITOMETRIA DE FLUJO: resultado compatible " +
                "con leucemia promielocitica aguda."
            );
            break;

        case LabTest.Cytogenetics:
            Debug.Log(
                "CITOGENETICA: translocacion t(15;17)."
            );
            break;

        case LabTest.PMLRARA:
            IsPMLRARAPositive = true;

            Debug.Log(
                "PRUEBA MOLECULAR: PML-RARA detectado. " +
                "Resultado positivo registrado."
            );
            break;
    }
}

// =====================================================
// CONSULTAS PUBLICAS
// =====================================================

public bool IsTestRequested(LabTest test)
{
    return requestedTests.Contains(test);
}

public bool IsTestReviewed(LabTest test)
{
    return reviewedTests.Contains(test);
}

// =====================================================
// REINICIAR
// =====================================================

public void ResetLabTests()
{
    requestedTests.Clear();
    reviewedTests.Clear();

    confirmatoryTestsUnlocked = false;
    bleedingRiskAlertTriggered = false;
    IsPMLRARAPositive = false;

    if (logProgress)
    {
        Debug.Log(
            "Progreso de laboratorio reiniciado."
        );
    }
}


}
