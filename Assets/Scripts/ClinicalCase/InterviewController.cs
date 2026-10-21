using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterviewController : MonoBehaviour
{
    // =====================================================
    // PANELES
    // =====================================================

    [Header("Paneles")]
    [SerializeField] private GameObject patientInfoPanel;
    [SerializeField] private GameObject interviewPanel;
    [SerializeField] private GameObject registeredDataPanel;

    // Se conserva temporalmente para no romper referencias
    // existentes en el Inspector.
    [SerializeField] private GameObject clinicalAssessmentPanel;

    // Este panel se creará/configurará en la siguiente tarea.
    [SerializeField] private GameObject interviewSummaryPanel;

    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("Textos")]
    [SerializeField] private TMP_Text patientResponseText;
    [SerializeField] private TMP_Text registeredDataText;

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("Botones")]
    [SerializeField] private Button saveResponseButton;

    // =====================================================
    // ALERTAS
    // =====================================================

    [Header("Sistema de alertas")]
    [SerializeField] private HUDController hudController;

    // =====================================================
    // FLUJO DEL CASO
    // =====================================================

    [Header("Flujo del caso")]
    [SerializeField] private ClinicalCaseFlowController caseFlowController;

    // =====================================================
    // PACIENTE
    // =====================================================

    [Header("Cambio de paciente")]
    [SerializeField] private GameObject pacienteAnimado;
    [SerializeField] private GameObject pacienteValoracion;

    // =====================================================
    // VARIABLES DE LA ENTREVISTA
    // =====================================================

    private int selectedQuestion = 0;

    // -----------------------------------------------------
    // PREGUNTAS REALIZADAS
    // -----------------------------------------------------
    //
    // Una pregunta puede haber sido realizada aunque
    // el jugador NO haya registrado todavía su hallazgo.
    //
    // Ejemplo:
    // answeredQuestions contiene 3
    // registeredFindings NO contiene 3
    //
    // Esto significa:
    // Pregunta 3 realizada = SÍ
    // Hallazgo 3 registrado = NO
    // -----------------------------------------------------

    private HashSet<int> answeredQuestions =
        new HashSet<int>();

    // -----------------------------------------------------
    // HALLAZGOS REGISTRADOS
    // -----------------------------------------------------
    //
    // Solo se agregan aquí cuando el jugador presiona
    // explícitamente "Registrar dato".
    // -----------------------------------------------------

    private HashSet<int> registeredFindings =
        new HashSet<int>();

    // =====================================================
    // HALLAZGOS ESENCIALES
    // =====================================================

    private readonly HashSet<int> essentialFindingIds =
        new HashSet<int>()
        {
            1,
            2,
            3,
            4
        };

    private const int TotalEssentialFindings = 4;

    // =====================================================
    // ESTADO DE LA ENTREVISTA
    // =====================================================

    // IMPORTANTE:
    // interviewCompleted solamente deberá convertirse
    // en true cuando posteriormente se confirme
    // el resumen de entrevista.
    //
    // "Finalizar entrevista" todavía NO significa
    // entrevista confirmada.
    private bool interviewCompleted = false;

    private const string InitialMessage =
        "Seleccione una pregunta para conocer la respuesta del paciente.";

    // =====================================================
    // INICIO
    // =====================================================

    private void Start()
    {
        // Al comenzar se muestra el paciente original animado.
        if (pacienteAnimado != null)
        {
            pacienteAnimado.SetActive(true);
        }

        // El paciente quieto se utilizará después de la entrevista.
        if (pacienteValoracion != null)
        {
            pacienteValoracion.SetActive(false);
        }

        if (interviewPanel != null)
        {
            interviewPanel.SetActive(false);
        }

        if (registeredDataPanel != null)
        {
            registeredDataPanel.SetActive(false);
        }

        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        if (interviewSummaryPanel != null)
        {
            interviewSummaryPanel.SetActive(false);
        }

        selectedQuestion = 0;

        SetResponse(InitialMessage);

        UpdateRegisteredData();

        UpdateRegisterButtonState();
    }

    // =====================================================
    // ABRIR ENTREVISTA
    // =====================================================

    public void OpenInterview()
    {
        if (patientInfoPanel != null)
        {
            patientInfoPanel.SetActive(false);
        }

        if (interviewPanel != null)
        {
            interviewPanel.SetActive(true);
        }

        if (registeredDataPanel != null)
        {
            registeredDataPanel.SetActive(true);
        }

        if (interviewSummaryPanel != null)
        {
            interviewSummaryPanel.SetActive(false);
        }

        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        selectedQuestion = 0;

        SetResponse(InitialMessage);

        UpdateRegisteredData();

        UpdateRegisterButtonState();

        // Mantener fase en ENTREVISTA.
        if (hudController != null)
        {
            hudController.SetCurrentPhase("Entrevista");
        }

        if (caseFlowController != null)
        {
            caseFlowController.GoToInterview();
        }
    }

    // =====================================================
    // PREGUNTA 1
    // SÍNTOMAS PRINCIPALES
    // ESENCIAL
    // =====================================================

    public void ShowAnswer1()
    {
        SelectQuestion(
            1,
            "Me siento muy débil, me mareo con frecuencia " +
            "y me falta el aire cuando hago esfuerzo."
        );
    }

    // =====================================================
    // PREGUNTA 2
    // TIEMPO DE EVOLUCIÓN
    // ESENCIAL
    // =====================================================

    public void ShowAnswer2()
    {
        SelectQuestion(
            2,
            "Los síntomas comenzaron hace varios días " +
            "y últimamente han empeorado."
        );
    }

    // =====================================================
    // PREGUNTA 3
    // TRANSFUSIÓN ANTERIOR
    // ESENCIAL
    // =====================================================

    public void ShowAnswer3()
    {
        SelectQuestion(
            3,
            "Sí. Recibí una transfusión de sangre " +
            "durante una operación anterior."
        );
    }

    // =====================================================
    // PREGUNTA 4
    // REACCIÓN TRANSFUSIONAL PREVIA
    // ESENCIAL
    // =====================================================

    public void ShowAnswer4()
    {
        SelectQuestion(
            4,
            "Sí. Durante aquella transfusión presenté " +
            "fiebre y escalofríos."
        );
    }

    // =====================================================
    // PREGUNTA 5
    // ALERGIAS
    // COMPLEMENTARIA
    // =====================================================

    public void ShowAnswer5()
    {
        SelectQuestion(
            5,
            "No refiero alergias conocidas."
        );
    }

    // =====================================================
    // PREGUNTA 6
    // GRUPO SANGUÍNEO REFERIDO
    // COMPLEMENTARIA
    // =====================================================

    public void ShowAnswer6()
    {
        SelectQuestion(
            6,
            "Me indicaron que soy A positivo, " +
            "pero entiendo que debe confirmarse."
        );
    }

    // =====================================================
    // COMPATIBILIDAD TEMPORAL
    // PREGUNTAS 7, 8 Y 9
    // =====================================================
    //
    // Estos métodos permanecen temporalmente porque
    // actualmente pueden existir botones de Unity
    // conectados a ShowAnswer7/8/9.
    //
    // Cuando eliminemos esos botones del InterviewPanel,
    // estos métodos también podrán eliminarse.
    // =====================================================

    public void ShowAnswer7()
    {
        ShowDeprecatedQuestionWarning();
    }

    public void ShowAnswer8()
    {
        ShowDeprecatedQuestionWarning();
    }

    public void ShowAnswer9()
    {
        ShowDeprecatedQuestionWarning();
    }

    // =====================================================
    // SELECCIONAR PREGUNTA
    // =====================================================

    private void SelectQuestion(
        int questionId,
        string patientAnswer
    )
    {
        // Guardamos qué pregunta está actualmente seleccionada.
        selectedQuestion = questionId;

        // IMPORTANTE:
        // La pregunta queda marcada como REALIZADA
        // solamente por haberla consultado.
        //
        // Todavía NO significa que su hallazgo
        // haya sido registrado.
        answeredQuestions.Add(questionId);

        // Mostrar respuesta del paciente.
        SetResponse(patientAnswer);

        // Habilitar o deshabilitar Registrar dato.
        //
        // Si el hallazgo ya había sido registrado,
        // el botón queda deshabilitado.
        UpdateRegisterButtonState();

        Debug.Log(
            "PREGUNTA REALIZADA: " +
            questionId +
            " | HALLAZGO REGISTRADO: " +
            registeredFindings.Contains(questionId)
        );
    }

    // =====================================================
    // PREGUNTA ANTIGUA / NO DISPONIBLE
    // =====================================================

    private void ShowDeprecatedQuestionWarning()
    {
        selectedQuestion = 0;

        SetResponse(
            "Esta pregunta ya no forma parte " +
            "de la entrevista clínica actual."
        );

        UpdateRegisterButtonState();

        if (hudController != null)
        {
            hudController.ShowWarningAlert(
                "Pregunta no disponible en esta versión del caso."
            );
        }
    }

    // =====================================================
    // REGISTRAR DATO
    // =====================================================

    public void SaveCurrentResponse()
    {
        // No existe una pregunta seleccionada.
        if (selectedQuestion == 0)
        {
            Debug.LogWarning(
                "Primero debe seleccionar una pregunta."
            );

            if (hudController != null)
            {
                hudController.ShowWarningAlert(
                    "Primero debe seleccionar una pregunta."
                );
            }

            return;
        }

        // Seguridad:
        // solo existen actualmente preguntas clínicas
        // válidas del 1 al 6.
        if (selectedQuestion < 1 ||
            selectedQuestion > 6)
        {
            Debug.LogWarning(
                "La pregunta seleccionada no posee " +
                "un hallazgo registrable."
            );

            selectedQuestion = 0;

            UpdateRegisterButtonState();

            return;
        }

        // -------------------------------------------------
        // REGISTRAR HALLAZGO
        // -------------------------------------------------

        bool newFinding =
            registeredFindings.Add(selectedQuestion);

        // -------------------------------------------------
        // EVITAR DUPLICADOS
        // -------------------------------------------------

        if (!newFinding)
        {
            if (hudController != null)
            {
                hudController.ShowWarningAlert(
                    "Esta información ya fue registrada."
                );
            }

            SetResponse(
                "Este hallazgo ya se encuentra registrado."
            );

            selectedQuestion = 0;

            UpdateRegisterButtonState();

            return;
        }

        // -------------------------------------------------
        // ACTUALIZAR EXPEDIENTE
        // -------------------------------------------------

        UpdateRegisteredData();

        // -------------------------------------------------
        // INFORMAR AL JUGADOR
        // -------------------------------------------------

        SetResponse(
            "Dato registrado correctamente. " +
            "Seleccione otra pregunta para continuar."
        );

        if (hudController != null)
        {
            hudController.ShowSuccessAlert(
                "Información clínica registrada."
            );
        }

        Debug.Log(
            "HALLAZGO REGISTRADO: " +
            selectedQuestion +
            " | PROGRESO ESENCIAL: " +
            GetEssentialFindingsCount() +
            "/" +
            TotalEssentialFindings
        );

        // Ya no tenemos una pregunta activa.
        selectedQuestion = 0;

        UpdateRegisterButtonState();
    }

    // =====================================================
    // ACTUALIZAR BOTÓN REGISTRAR DATO
    // =====================================================

    private void UpdateRegisterButtonState()
    {
        if (saveResponseButton == null)
        {
            return;
        }

        // Sin pregunta seleccionada:
        // no se puede registrar nada.
        if (selectedQuestion == 0)
        {
            saveResponseButton.interactable = false;

            return;
        }

        // Si el hallazgo ya fue registrado,
        // tampoco permitimos volver a registrarlo.
        bool alreadyRegistered =
            registeredFindings.Contains(selectedQuestion);

        saveResponseButton.interactable =
            !alreadyRegistered;
    }

    // =====================================================
    // ACTUALIZAR INFORMACIÓN REGISTRADA
    // =====================================================

    private void UpdateRegisteredData()
    {
        if (registeredDataText == null)
        {
            return;
        }

        string data = "";

        // -------------------------------------------------
        // SIN HALLAZGOS
        // -------------------------------------------------

        if (registeredFindings.Count == 0)
        {
            data =
                "Aún no se registraron datos.\n\n" +
                "Información esencial registrada: 0/4";

            registeredDataText.text = data;

            return;
        }

        // -------------------------------------------------
        // HALLAZGO 1
        // SÍNTOMAS PRINCIPALES
        // -------------------------------------------------

        if (registeredFindings.Contains(1))
        {
            data +=
                "• Síntomas principales: debilidad, " +
                "mareos y disnea al esfuerzo.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 2
        // TIEMPO DE EVOLUCIÓN
        // -------------------------------------------------

        if (registeredFindings.Contains(2))
        {
            data +=
                "• Tiempo de evolución: síntomas de varios días " +
                "con empeoramiento actual.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 3
        // TRANSFUSIÓN ANTERIOR
        // -------------------------------------------------

        if (registeredFindings.Contains(3))
        {
            data +=
                "• Transfusión anterior: recibió sangre " +
                "durante una operación.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 4
        // REACCIÓN TRANSFUSIONAL PREVIA
        // -------------------------------------------------

        if (registeredFindings.Contains(4))
        {
            data +=
                "• Reacción previa: presentó fiebre " +
                "y escalofríos durante una transfusión.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 5
        // ALERGIAS
        // COMPLEMENTARIO
        // -------------------------------------------------

        if (registeredFindings.Contains(5))
        {
            data +=
                "• Alergias: no refiere alergias conocidas.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 6
        // GRUPO SANGUÍNEO REFERIDO
        // COMPLEMENTARIO
        // -------------------------------------------------

        if (registeredFindings.Contains(6))
        {
            data +=
                "• Grupo sanguíneo referido: A positivo, " +
                "pendiente de confirmación.\n";
        }

        // -------------------------------------------------
        // PROGRESO ESENCIAL
        // -------------------------------------------------

        int essentialCount =
            GetEssentialFindingsCount();

        data +=
            "\nInformación esencial registrada: " +
            essentialCount +
            "/" +
            TotalEssentialFindings;

        registeredDataText.text = data;
    }

    // =====================================================
    // CONTAR HALLAZGOS ESENCIALES
    // =====================================================

    public int GetEssentialFindingsCount()
    {
        int count = 0;

        foreach (int findingId in essentialFindingIds)
        {
            if (registeredFindings.Contains(findingId))
            {
                count++;
            }
        }

        return count;
    }

    // =====================================================
    // MOSTRAR RESPUESTA DEL PACIENTE
    // =====================================================

    private void SetResponse(string response)
    {
        if (patientResponseText != null)
        {
            patientResponseText.text = response;
        }
    }

    // =====================================================
    // FINALIZAR ENTREVISTA
    // =====================================================
    //
    // IMPORTANTE:
    //
    // Este método conserva el nombre antiguo
    // ContinueToClinicalAssessment para NO romper todavía
    // el OnClick configurado en Unity.
    //
    // Sin embargo, YA NO cambia a Valoración.
    //
    // En la siguiente tarea este botón abrirá
    // InterviewSummaryPanel.
    // =====================================================

    public void ContinueToClinicalAssessment()
    {
        FinishInterview();
    }

    // =====================================================
    // PREPARAR FINALIZACIÓN DE ENTREVISTA
    // =====================================================

    public void FinishInterview()
    {
        // -------------------------------------------------
        // NO MARCAR TODAVÍA interviewCompleted = true
        // -------------------------------------------------
        //
        // Eso solamente ocurrirá cuando posteriormente
        // se confirme el resumen.
        // -------------------------------------------------

        interviewCompleted = false;

        // Mantener fase ENTREVISTA.
        if (hudController != null)
        {
            hudController.SetCurrentPhase("Entrevista");
        }

        if (caseFlowController != null)
        {
            caseFlowController.GoToInterview();
        }

        // -------------------------------------------------
        // SI YA EXISTE EL PANEL DE RESUMEN
        // -------------------------------------------------

        if (interviewSummaryPanel != null)
        {
            PreparePatientForClinicalAssessment();

            if (interviewPanel != null)
            {
                interviewPanel.SetActive(false);
            }

            if (registeredDataPanel != null)
            {
                registeredDataPanel.SetActive(false);
            }

            interviewSummaryPanel.SetActive(true);

            Debug.Log(
                "ENTREVISTA FINALIZADA. " +
                "ABRIENDO RESUMEN DE ENTREVISTA. " +
                "LA FASE CONTINÚA SIENDO ENTREVISTA."
            );

            return;
        }

        // -------------------------------------------------
        // EL PANEL TODAVÍA NO FUE CREADO
        // -------------------------------------------------

        Debug.Log(
            "FINALIZACIÓN DE ENTREVISTA SOLICITADA. " +
            "InterviewSummaryPanel todavía no está asignado. " +
            "NO se cambia a valoración clínica."
        );

        if (hudController != null)
        {
            hudController.ShowWarningAlert(
                "La entrevista está lista para mostrar su resumen."
            );
        }
    }

    // =====================================================
    // PREPARAR PACIENTE PARA VALORACIÓN CLÍNICA
    // =====================================================

    private void PreparePatientForClinicalAssessment()
    {
        if (pacienteAnimado != null)
        {
            pacienteAnimado.SetActive(false);
        }

        if (pacienteValoracion != null)
        {
            pacienteValoracion.SetActive(true);
        }

        Debug.Log(
            "PACIENTE DE VALORACIÓN ACTIVADO."
        );
    }

    // =====================================================
    // PREGUNTAS REALIZADAS
    // =====================================================

    public List<int> GetAnsweredQuestions()
    {
        return new List<int>(answeredQuestions);
    }

    // =====================================================
    // COMPATIBILIDAD CON EL GUARDADO ACTUAL
    // =====================================================
    //
    // HUDController actualmente llama GetSavedQuestions().
    //
    // Conservamos ese método, pero desde ahora representa
    // PREGUNTAS REALIZADAS.
    // =====================================================

    public List<int> GetSavedQuestions()
    {
        return GetAnsweredQuestions();
    }

    // =====================================================
    // HALLAZGOS REGISTRADOS
    // =====================================================

    public List<int> GetRegisteredFindings()
    {
        return new List<int>(registeredFindings);
    }

    // =====================================================
    // SABER SI UNA PREGUNTA FUE REALIZADA
    // =====================================================

    public bool WasQuestionAnswered(int questionId)
    {
        return answeredQuestions.Contains(questionId);
    }

    // =====================================================
    // SABER SI UN HALLAZGO FUE REGISTRADO
    // =====================================================

    public bool IsFindingRegistered(int findingId)
    {
        return registeredFindings.Contains(findingId);
    }

    // =====================================================
    // SABER SI LA ENTREVISTA TERMINÓ
    // =====================================================

    public bool IsInterviewCompleted()
    {
        return interviewCompleted;
    }

    // =====================================================
    // RESTAURAR ESTADO NUEVO DE ENTREVISTA
    // =====================================================
    //
    // Este método ya soporta por separado:
    // - preguntas realizadas
    // - hallazgos registrados
    //
    // Posteriormente SaveData y GameSaveLoader utilizarán
    // esta versión.
    // =====================================================

    public void RestoreInterviewState(
        List<int> questions,
        List<int> findings,
        bool completed
    )
    {
        answeredQuestions.Clear();
        registeredFindings.Clear();

        // -------------------------------------------------
        // RESTAURAR PREGUNTAS REALIZADAS
        // -------------------------------------------------

        if (questions != null)
        {
            foreach (int question in questions)
            {
                if (question >= 1 &&
                    question <= 6)
                {
                    answeredQuestions.Add(question);
                }
            }
        }

        // -------------------------------------------------
        // RESTAURAR HALLAZGOS REGISTRADOS
        // -------------------------------------------------

        if (findings != null)
        {
            foreach (int finding in findings)
            {
                if (finding >= 1 &&
                    finding <= 6)
                {
                    registeredFindings.Add(finding);
                }
            }
        }

        interviewCompleted = completed;

        selectedQuestion = 0;

        UpdateRegisteredData();

        UpdateRegisterButtonState();

        // -------------------------------------------------
        // ENTREVISTA AÚN NO CONFIRMADA
        // -------------------------------------------------

        if (!interviewCompleted)
        {
            if (pacienteAnimado != null)
            {
                pacienteAnimado.SetActive(true);
            }

            if (pacienteValoracion != null)
            {
                pacienteValoracion.SetActive(false);
            }

            if (patientInfoPanel != null)
            {
                patientInfoPanel.SetActive(false);
            }

            if (clinicalAssessmentPanel != null)
            {
                clinicalAssessmentPanel.SetActive(false);
            }

            if (interviewSummaryPanel != null)
            {
                interviewSummaryPanel.SetActive(false);
            }

            if (interviewPanel != null)
            {
                interviewPanel.SetActive(true);
            }

            if (registeredDataPanel != null)
            {
                registeredDataPanel.SetActive(true);
            }

            SetResponse(InitialMessage);

            if (hudController != null)
            {
                hudController.SetCurrentPhase("Entrevista");

                hudController.SetUIInteractionMode(true);
            }

            if (caseFlowController != null)
            {
                caseFlowController.GoToInterview();
            }

            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;

            Debug.Log(
                "ENTREVISTA RESTAURADA. " +
                "PREGUNTAS REALIZADAS: " +
                answeredQuestions.Count +
                " | HALLAZGOS REGISTRADOS: " +
                registeredFindings.Count +
                " | ESENCIALES: " +
                GetEssentialFindingsCount() +
                "/4"
            );
        }
        else
        {
            PreparePatientForClinicalAssessment();

            // La entrevista ya había sido confirmada.
            //
            // NO abrimos automáticamente valoración aquí.
            // La fase guardada será restaurada por
            // ClinicalCaseFlowController / GameSaveLoader.

            if (interviewPanel != null)
            {
                interviewPanel.SetActive(false);
            }

            if (registeredDataPanel != null)
            {
                registeredDataPanel.SetActive(false);
            }

            if (interviewSummaryPanel != null)
            {
                interviewSummaryPanel.SetActive(false);
            }

            Debug.Log(
                "ENTREVISTA RESTAURADA COMO CONFIRMADA."
            );
        }
    }

    // =====================================================
    // COMPATIBILIDAD CON GUARDADOS ANTIGUOS
    // =====================================================
    //
    // Actualmente GameSaveLoader puede seguir utilizando
    // esta versión de dos parámetros.
    //
    // Los guardados anteriores usaban la misma lista
    // tanto para información preguntada como registrada.
    //
    // Por ello, temporalmente restauramos esos valores
    // en ambas colecciones.
    //
    // Esto se cambiará cuando ampliemos SaveData.
    // =====================================================

    public void RestoreInterviewState(
        List<int> questions,
        bool completed
    )
    {
        RestoreInterviewState(
            questions,
            questions,
            completed
        );
    }

    // =====================================================
    // COMPATIBILIDAD ADICIONAL
    // =====================================================

    public void RestoreSavedQuestions(
        List<int> questions
    )
    {
        RestoreInterviewState(
            questions,
            questions,
            interviewCompleted
        );
    }
}
