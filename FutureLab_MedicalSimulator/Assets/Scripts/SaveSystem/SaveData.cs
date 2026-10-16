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
    // true = la entrevista ya fue confirmada.
    public bool interviewCompleted;

    // Vista exacta en la que quedó la entrevista al guardar.
    // Valores actuales:
    // "Interview" = panel de entrevista
    // "Summary"   = resumen de entrevista
    // "Hidden"    = interfaz de entrevista cerrada
    public string interviewViewState = "Interview";

    // Pregunta que estaba seleccionada al momento de guardar.
    // 0 = ninguna pregunta seleccionada.
    public int selectedInterviewQuestion;

    // Texto visible en la respuesta del paciente al guardar.
    public string currentInterviewResponse;

    // ==============================
    // TIEMPO DEL CASO
    // ==============================

    public float elapsedTime;

    // ==============================
    // PENALIZACIONES DE TIEMPO
    // ==============================

    // Guarda los identificadores únicos de penalizaciones
    // que ya fueron aplicadas durante este intento.
    //
    // Ejemplo:
    // ENTREVISTA_INSUFICIENTE_2_DE_4
    //
    // Esto permite restaurar la partida sin aplicar
    // nuevamente +05:00 por el mismo error.
    public List<string> appliedPenaltyIds =
        new List<string>();

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

    // Hallazgos que el participante registró explícitamente
    // mediante el botón "Registrar dato".
    //
    // Se guardan por separado de answeredQuestions porque
    // preguntar algo no significa necesariamente registrarlo.
    public List<int> registeredFindings =
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
