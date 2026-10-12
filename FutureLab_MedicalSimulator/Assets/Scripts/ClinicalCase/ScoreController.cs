using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
public enum ScoreCategory
{
Interview,
Assessment,
InitialTests,
Interpretation,
Confirmation,
FinalDiagnosis
}

public enum FinalGrade
{
    Reprobado,
    Aprobado,
    Bien,
    MuyBien,
    Excelente
}

[Header("Puntuacion final")]
[SerializeField]
private int totalScore;

[Header("Puntos maximos por categoria")]
[SerializeField]
private int interviewMaxPoints = 20;

[SerializeField]
private int assessmentMaxPoints = 15;

[SerializeField]
private int initialTestsMaxPoints = 20;

[SerializeField]
private int interpretationMaxPoints = 20;

[SerializeField]
private int confirmationMaxPoints = 15;

[SerializeField]
private int finalDiagnosisMaxPoints = 10;

private int bonusPoints;
private int penaltyPoints;

private readonly Dictionary<ScoreCategory, int> categoryScores =
    new Dictionary<ScoreCategory, int>();

private readonly HashSet<ScoreCategory> completedCategories =
    new HashSet<ScoreCategory>();

public int TotalScore => totalScore;

public int BonusPoints => bonusPoints;

public int PenaltyPoints => penaltyPoints;

public FinalGrade Grade => GetFinalGrade();

// =====================================================
// INICIALIZACION
// =====================================================

private void Awake()
{
    InitializeCategories();
    RecalculateTotalScore();
}

private void InitializeCategories()
{
    categoryScores.Clear();

    foreach (ScoreCategory category in
             System.Enum.GetValues(typeof(ScoreCategory)))
    {
        categoryScores[category] = 0;
    }
}

// =====================================================
// REGISTRAR PUNTOS POR CATEGORIA
// =====================================================

public void RegisterCategoryScore(
    ScoreCategory category,
    int points)
{
    if (!System.Enum.IsDefined(
            typeof(ScoreCategory), category))
    {
        Debug.LogWarning(
            "Categoria de puntuacion no valida."
        );
        return;
    }

    if (completedCategories.Contains(category))
    {
        Debug.LogWarning(
            "La categoria ya fue evaluada: " + category
        );
        return;
    }

    int maxPoints = GetCategoryMaxPoints(category);

    if (maxPoints < 0)
    {
        Debug.LogError(
            "El maximo de puntos de la categoria no puede ser negativo."
        );
        return;
    }

    int awardedPoints = Mathf.Clamp(
        points, 0, maxPoints
    );

    categoryScores[category] = awardedPoints;
    completedCategories.Add(category);

    RecalculateTotalScore();

    Debug.Log(
        "Categoria: " + category +
        " | Puntos: " + awardedPoints + "/" + maxPoints +
        " | Penalizaciones acumuladas: -" + penaltyPoints +
        " | Total: " + totalScore + "/100"
    );
}

public void RegisterInterviewScore(int points)
{
    RegisterCategoryScore(
        ScoreCategory.Interview, points);
}

public void RegisterAssessmentScore(int points)
{
    RegisterCategoryScore(
        ScoreCategory.Assessment, points);
}

public void RegisterInitialTestsScore(int points)
{
    RegisterCategoryScore(
        ScoreCategory.InitialTests, points);
}

public void RegisterInterpretationScore(int points)
{
    RegisterCategoryScore(
        ScoreCategory.Interpretation, points);
}

public void RegisterConfirmationScore(int points)
{
    RegisterCategoryScore(
        ScoreCategory.Confirmation, points);
}

public void RegisterFinalDiagnosisScore(int points)
{
    RegisterCategoryScore(
        ScoreCategory.FinalDiagnosis, points);
}

// =====================================================
// PUNTOS ADICIONALES
// =====================================================

public void AddPoints(int points)
{
    if (points <= 0)
    {
        Debug.LogWarning(
            "AddPoints requiere una cantidad positiva."
        );
        return;
    }

    // Limitar los puntos adicionales a un maximo acumulado
    // de 100 puntos de bonificacion.
    int remainingBonusCapacity = 100 - bonusPoints;

    if (remainingBonusCapacity <= 0)
    {
        Debug.LogWarning(
            "Se alcanzo el limite de puntos adicionales."
        );
        return;
    }

    int acceptedPoints = Mathf.Min(
        points, remainingBonusCapacity
    );

    bonusPoints += acceptedPoints;

    RecalculateTotalScore();

    Debug.Log(
        "Puntos adicionales aceptados: +" + acceptedPoints +
        " | Total de bonificacion: +" + bonusPoints +
        " | Puntuacion: " + totalScore + "/100"
    );
}

// =====================================================
// PENALIZACIONES
// =====================================================

public void SubtractPoints(int points)
{
    if (points <= 0)
    {
        Debug.LogWarning(
            "SubtractPoints requiere una cantidad positiva."
        );
        return;
    }

    penaltyPoints += points;

    RecalculateTotalScore();

    Debug.LogWarning(
        "Penalizacion aplicada: -" + points +
        " | Penalizaciones acumuladas: -" + penaltyPoints +
        " | Total: " + totalScore + "/100"
    );
}

public void ApplyClinicalErrorPenalty()
{
    SubtractPoints(10);
}

public void ApplyBleedingDelayPenalty()
{
    SubtractPoints(10);
}

public void ApplyCriticalErrorPenalty()
{
    SubtractPoints(20);
}

// =====================================================
// CALCULAR PUNTUACION TOTAL
// =====================================================

private void RecalculateTotalScore()
{
    int categoryTotal = 0;

    foreach (int points in categoryScores.Values)
    {
        categoryTotal += points;
    }

    int calculatedScore =
        categoryTotal + bonusPoints - penaltyPoints;

    totalScore = Mathf.Clamp(
        calculatedScore, 0, 100
    );
}

// =====================================================
// MAXIMO DE CADA CATEGORIA
// =====================================================

private int GetCategoryMaxPoints(
    ScoreCategory category)
{
    switch (category)
    {
        case ScoreCategory.Interview:
            return interviewMaxPoints;

        case ScoreCategory.Assessment:
            return assessmentMaxPoints;

        case ScoreCategory.InitialTests:
            return initialTestsMaxPoints;

        case ScoreCategory.Interpretation:
            return interpretationMaxPoints;

        case ScoreCategory.Confirmation:
            return confirmationMaxPoints;

        case ScoreCategory.FinalDiagnosis:
            return finalDiagnosisMaxPoints;

        default:
            return 0;
    }
}

// =====================================================
// CLASIFICACION FINAL
// =====================================================

public FinalGrade GetFinalGrade()
{
    if (totalScore >= 91)
        return FinalGrade.Excelente;

    if (totalScore >= 81)
        return FinalGrade.MuyBien;

    if (totalScore >= 76)
        return FinalGrade.Bien;

    if (totalScore >= 71)
        return FinalGrade.Aprobado;

    return FinalGrade.Reprobado;
}

public string GetFinalGradeName()
{
    switch (GetFinalGrade())
    {
        case FinalGrade.Excelente:
            return "Excelente";

        case FinalGrade.MuyBien:
            return "Muy bien";

        case FinalGrade.Bien:
            return "Bien";

        case FinalGrade.Aprobado:
            return "Aprobado";

        default:
            return "Reprobado";
    }
}

// =====================================================
// CONSULTAS DE CATEGORIAS
// =====================================================

public bool IsCategoryCompleted(
    ScoreCategory category)
{
    return completedCategories.Contains(category);
}

public int GetCategoryScore(
    ScoreCategory category)
{
    return categoryScores.TryGetValue(
        category, out int points)
        ? points
        : 0;
}

// =====================================================
// REINICIAR EVALUACION
// =====================================================

public void ResetScore()
{
    InitializeCategories();
    completedCategories.Clear();

    bonusPoints = 0;
    penaltyPoints = 0;
    totalScore = 0;

    Debug.Log(
        "Puntuacion reiniciada."
    );
}

// =====================================================
// RESUMEN FINAL
// =====================================================

public string GetFinalSummary()
{
    return
        "Puntuacion final: " + totalScore + "/100\n" +
        "Penalizaciones: -" + penaltyPoints + "\n" +
        "Clasificacion: " + GetFinalGradeName();
}

}
