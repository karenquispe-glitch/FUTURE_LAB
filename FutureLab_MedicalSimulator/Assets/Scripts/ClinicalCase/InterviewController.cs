using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterviewController : MonoBehaviour
{
    // =====================================================
    // PANELES
    // =====================================================

    [Header("Paneles")]
    [SerializeField] private GameObject patientInfoPanel;
    [SerializeField] private GameObject interviewPanel;
    [SerializeField] private GameObject registeredDataPanel;
    [SerializeField] private GameObject clinicalAssessmentPanel;

    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("Textos")]
    [SerializeField] private TMP_Text patientResponseText;
    [SerializeField] private TMP_Text registeredDataText;

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
    // VARIABLES
    // =====================================================

    private int selectedQuestion = 0;

    private HashSet<int> savedQuestions =
        new HashSet<int>();

    // Indica si el participante ya terminó
    // la entrevista presionando CONTINUAR.
    private bool interviewCompleted = false;

    private const string InitialMessage =
        "Seleccione una pregunta para conocer la respuesta del paciente.";

    // =====================================================
    // INICIO
    // =====================================================

    private void Start()
    {
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

        SetResponse(InitialMessage);

        UpdateRegisteredData();
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

        selectedQuestion = 0;

        SetResponse(InitialMessage);

        UpdateRegisteredData();
    }

    // =====================================================
    // PREGUNTA 1
    // =====================================================

    public void ShowAnswer1()
    {
        selectedQuestion = 1;

        SetResponse(
            "Desde hace unas tres semanas y cada día me cuesta más caminar."
        );
    }

    // =====================================================
    // PREGUNTA 2
    // =====================================================

    public void ShowAnswer2()
    {
        selectedQuestion = 2;

        SetResponse(
            "Sí, principalmente por las noches."
        );
    }

    // =====================================================
    // PREGUNTA 3
    // =====================================================

    public void ShowAnswer3()
    {
        selectedQuestion = 3;

        SetResponse(
            "Me sangran las encías y dos veces me sangró la nariz."
        );
    }

    // =====================================================
    // PREGUNTA 4
    // =====================================================

    public void ShowAnswer4()
    {
        selectedQuestion = 4;

        SetResponse(
            "Sí, sin haberme golpeado."
        );
    }

    // =====================================================
    // PREGUNTA 5
    // =====================================================

    public void ShowAnswer5()
    {
        selectedQuestion = 5;

        SetResponse(
            "No tomo anticoagulantes."
        );
    }

    // =====================================================
    // PREGUNTA 6
    // =====================================================

    public void ShowAnswer6()
    {
        selectedQuestion = 6;

        SetResponse(
            "No."
        );
    }

    // =====================================================
    // PREGUNTA 7
    // =====================================================

    public void ShowAnswer7()
    {
        selectedQuestion = 7;

        SetResponse(
            "He bajado un poco de peso y como menos."
        );
    }

    // =====================================================
    // PREGUNTA 8
    // =====================================================

    public void ShowAnswer8()
    {
        selectedQuestion = 8;

        SetResponse(
            "No que yo sepa."
        );
    }

    // =====================================================
    // PREGUNTA 9 - DISTRACTORA
    // =====================================================

    public void ShowAnswer9()
    {
        selectedQuestion = 9;

        SetResponse(
            "Comida habitual."
        );
    }

    // =====================================================
    // GUARDAR RESPUESTA
    // =====================================================

    public void SaveCurrentResponse()
    {
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

        bool newInformation =
            savedQuestions.Add(selectedQuestion);

        if (!newInformation)
        {
            if (hudController != null)
            {
                hudController.ShowWarningAlert(
                    "Esta información ya fue registrada."
                );
            }

            SetResponse(
                "Esta respuesta ya se encuentra registrada."
            );

            selectedQuestion = 0;

            return;
        }

        UpdateRegisteredData();

        SetResponse(
            "Respuesta registrada. " +
            "Seleccione otra pregunta para continuar."
        );

        if (hudController != null)
        {
            hudController.ShowSuccessAlert(
                "Información clínica registrada."
            );
        }

        selectedQuestion = 0;

        Debug.Log(
            "Información clínica registrada."
        );
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

        if (savedQuestions.Count == 0)
        {
            registeredDataText.text =
                "Aún no se registraron datos.";

            return;
        }

        string data = "";

        if (savedQuestions.Contains(1))
        {
            data +=
                "• Cansancio de aproximadamente tres semanas\n" +
                "• Dificultad progresiva para caminar\n";
        }

        if (savedQuestions.Contains(2))
        {
            data +=
                "• Fiebre principalmente nocturna\n";
        }

        if (savedQuestions.Contains(3))
        {
            data +=
                "• Sangrado de encías\n" +
                "• Episodios de epistaxis\n";
        }

        if (savedQuestions.Contains(4))
        {
            data +=
                "• Moretones sin traumatismo previo\n" +
                "• Presencia de puntos rojos/petequias\n";
        }

        if (savedQuestions.Contains(5))
        {
            data +=
                "• Niega uso de anticoagulantes\n";
        }

        if (savedQuestions.Contains(6))
        {
            data +=
                "• Niega traumatismo reciente\n";
        }

        if (savedQuestions.Contains(7))
        {
            data +=
                "• Pérdida leve de peso\n" +
                "• Disminución del apetito\n";
        }

        if (savedQuestions.Contains(8))
        {
            data +=
                "• Sin antecedentes hematológicos conocidos\n";
        }

        if (savedQuestions.Contains(9))
        {
            data +=
                "• Alimentación habitual\n";
        }

        registeredDataText.text = data;
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
    // CONTINUAR A VALORACIÓN CLÍNICA
    // =====================================================

    public void ContinueToClinicalAssessment()
    {
        // Las preguntas 1, 2, 3 y 4 son obligatorias.
        bool hasRequiredQuestions =
            savedQuestions.Contains(1) &&
            savedQuestions.Contains(2) &&
            savedQuestions.Contains(3) &&
            savedQuestions.Contains(4);

        if (!hasRequiredQuestions)
        {
            Debug.LogWarning(
                "No puede continuar sin registrar las preguntas clínicas obligatorias."
            );

            if (hudController != null)
            {
                hudController.ShowErrorAlert(
                    "Debe registrar duración de síntomas, fiebre, sangrado y moretones/petequias."
                );
            }

            return;
        }

        // =================================================
        // LA ENTREVISTA HA TERMINADO
        // =================================================

        interviewCompleted = true;

        if (hudController != null)
        {
            hudController.SetCurrentPhase("Valoración");
        }

        if (caseFlowController != null)
        {
            caseFlowController.GoToAssessment();
        }

        // =================================================
        // CERRAR VENTANAS DE LA ENTREVISTA
        // =================================================

        if (patientInfoPanel != null)
        {
            patientInfoPanel.SetActive(false);
        }

        if (interviewPanel != null)
        {
            interviewPanel.SetActive(false);
        }

        if (registeredDataPanel != null)
        {
            registeredDataPanel.SetActive(false);
        }

        // =================================================
        // ABRIR VALORACIÓN CLÍNICA
        // =================================================

        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(true);

            Debug.Log(
                "ENTREVISTA FINALIZADA. PASANDO A VALORACIÓN CLÍNICA."
            );
        }
        else
        {
            Debug.LogError(
                "InterviewController: falta asignar ClinicalAssessmentPanel."
            );
        }
    }

    // =====================================================
    // GUARDADO DE PROGRESO
    // =====================================================

    public List<int> GetSavedQuestions()
    {
        return new List<int>(savedQuestions);
    }

    // =====================================================
    // SABER SI LA ENTREVISTA TERMINÓ
    // =====================================================

    public bool IsInterviewCompleted()
    {
        return interviewCompleted;
    }

    // =====================================================
    // RESTAURAR ENTREVISTA
    // =====================================================

    public void RestoreInterviewState(
        List<int> questions,
        bool completed
    )
    {
        savedQuestions.Clear();

        if (questions != null)
        {
            foreach (int question in questions)
            {
                savedQuestions.Add(question);
            }
        }

        interviewCompleted = completed;

        selectedQuestion = 0;

        UpdateRegisteredData();

        // =================================================
        // SI NO TERMINÓ LA ENTREVISTA
        // =================================================

        if (!interviewCompleted)
        {
            if (patientInfoPanel != null)
            {
                patientInfoPanel.SetActive(false);
            }

            if (clinicalAssessmentPanel != null)
            {
                clinicalAssessmentPanel.SetActive(false);
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
                hudController.SetUIInteractionMode(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log(
                "ENTREVISTA RESTAURADA. " +
                "LA ENTREVISTA AÚN NO HABÍA TERMINADO."
            );
        }
        else
        {
            if (hudController != null)
            {
                hudController.SetCurrentPhase("Valoración");
            }

            if (caseFlowController != null)
            {
                caseFlowController.GoToAssessment();
            }

            Debug.Log(
                "ENTREVISTA RESTAURADA COMO FINALIZADA. " +
                "FASE ACTUAL: VALORACIÓN CLÍNICA."
            );
        }
    }

    // =====================================================
    // COMPATIBILIDAD
    // =====================================================

    public void RestoreSavedQuestions(List<int> questions)
    {
        RestoreInterviewState(
            questions,
            interviewCompleted
        );
    }
}