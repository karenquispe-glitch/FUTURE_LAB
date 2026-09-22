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

    // ==============================
    // HALLAZGOS CLÍNICOS
    // ==============================

    public List<string> clinicalFindings =
        new List<string>();

    // ==============================
    // ESTUDIOS / PRUEBAS
    // ==============================

    public List<string> requestedTests =
        new List<string>();

    public List<string> testResults =
        new List<string>();

    // ==============================
    // DIAGNÓSTICO
    // ==============================

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