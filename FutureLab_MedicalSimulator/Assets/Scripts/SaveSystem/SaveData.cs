using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // ==============================
    // INFORMACIÓN GENERAL DEL CASO
    // ==============================

    public string sceneName;
    public int caseID;

    // ==============================
    // ESTADO DEL CASO
    // ==============================

    public bool caseStarted;
    public string currentPhase;

    // ==============================
    // ESTADO DE LA ENTREVISTA
    // ==============================

    // false = la entrevista todavía no terminó.
    // true = el participante pulsó CONTINUAR
    // y pasó a Valoración Clínica.
    public bool interviewCompleted;

    // ==============================
    // TIEMPO DEL CASO
    // ==============================

    public float elapsedTime;

    // ==============================
    // POSICIÓN DEL JUGADOR
    // ==============================

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public float playerRotX;
    public float playerRotY;
    public float playerRotZ;

    // ==============================
    // ENTREVISTA
    // ==============================

    public List<int> answeredQuestions =
        new List<int>();

    // ==============================
    // RESPUESTAS
    // ==============================

    public List<string> answers =
        new List<string>();

    // ==============================
    // SIGNOS VITALES
    // ==============================

    public string bloodPressure;
    public string oxygenSaturation;
    public string heartRate;
    public string temperature;

    // Frecuencia respiratoria.
    public string respiratoryRate;

    // ==============================
    // HALLAZGOS CLÍNICOS
    // ==============================

    public List<string> clinicalFindings =
        new List<string>();

    // ==============================
    // VALORACIÓN CLÍNICA
    // ==============================

    // Acciones clínicas ya completadas.
    // Ejemplos:
    // Inspeccion
    // Pulso
    // Presion
    // Temperatura
    // Saturacion
    // Respiracion
    public List<string> completedAssessments =
        new List<string>();

    // ==============================
    // ESTUDIOS / PRUEBAS
    // ==============================

    public List<string> requestedTests =
        new List<string>();

    public List<string> testResults =
        new List<string>();

    // Resultados que el jugador
    // ya abrió o revisó.
    public List<string> reviewedResults =
        new List<string>();

    // ==============================
    // ALERTAS CLÍNICAS
    // ==============================

    // Indica si ya se activó
    // la alerta de riesgo hemorrágico.
    public bool riskAlertTriggered;

    // Nivel del evento dinámico de sangrado.
    //
    // 0 = sin evento
    // 1 = primera advertencia
    // 2 = deterioro clínico
    public int bleedingEventLevel;

    // ==============================
    // DIAGNÓSTICO
    // ==============================

    // Hipótesis o diagnóstico provisional.
    public string provisionalDiagnosis;

    // Diagnóstico final seleccionado.
    public string selectedDiagnosis;

    // ==============================
    // DECISIÓN CLÍNICA
    // ==============================

    public string clinicalDecision;

    // ==============================
    // EVALUACIÓN
    // ==============================

    public int score;

    public bool caseCompleted;

    // ==============================
    // FECHA DEL GUARDADO
    // ==============================

    public string saveDate;
}