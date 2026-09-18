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
    // DIAGNÓSTICO
    // =====================================================

    [Header("Diagnóstico")]
    [SerializeField] private DiagnosisController diagnosisController;

    // =====================================================
    // VARIABLES
    // =====================================================

    private int selectedQuestion = 0;

    private HashSet<int> savedQuestions =
        new HashSet<int>();

    // NUEVO:
    // Indica si el participante ya terminó la entrevista
    // presionando el botón CONTINUAR.
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
            "Lo que más me preocupa son los dolores de cabeza, " +
            "los mareos y una picazón muy intensa que aparece algunas veces."
        );
    }

    // =====================================================
    // PREGUNTA 2
    // =====================================================

    public void ShowAnswer2()
    {
        selectedQuestion = 2;

        SetResponse(
            "Sí. La picazón se vuelve mucho más intensa " +
            "después de bañarme con agua tibia."
        );
    }

    // =====================================================
    // PREGUNTA 3
    // =====================================================

    public void ShowAnswer3()
    {
        selectedQuestion = 3;

        SetResponse(
            "A veces veo un poco borroso cuando tengo mucho mareo " +
            "y en algunas ocasiones siento calor o ardor en los pies."
        );
    }

    // =====================================================
    // PREGUNTA 4
    // =====================================================

    public void ShowAnswer4()
    {
        selectedQuestion = 4;

        SetResponse(
            "No. Nunca me han diagnosticado una trombosis " +
            "y tampoco he tenido hinchazón dolorosa en las piernas " +
            "ni dolor en el pecho."
        );
    }

    // =====================================================
    // PREGUNTA 5
    // =====================================================

    public void ShowAnswer5()
    {
        selectedQuestion = 5;

        SetResponse(
            "No fumo y tampoco estoy expuesto habitualmente " +
            "a humo, combustión o monóxido de carbono."
        );
    }

    // =====================================================
    // PREGUNTA 6
    // =====================================================

    public void ShowAnswer6()
    {
        selectedQuestion = 6;

        SetResponse(
            "Vivo habitualmente en La Paz desde hace varios años."
        );
    }

    // =====================================================
    // PREGUNTA 7
    // =====================================================

    public void ShowAnswer7()
    {
        selectedQuestion = 7;

        SetResponse(
            "No que yo sepa. Nadie me ha dicho que deje de respirar " +
            "mientras duermo y no suelo tener demasiada somnolencia " +
            "durante el día."
        );
    }

    // =====================================================
    // PREGUNTA 8
    // =====================================================

    public void ShowAnswer8()
    {
        selectedQuestion = 8;

        SetResponse(
            "No utilizo testosterona, eritropoyetina, " +
            "esteroides anabólicos ni tratamientos hormonales."
        );
    }

    // =====================================================
    // PREGUNTA 9
    // =====================================================

    public void ShowAnswer9()
    {
        selectedQuestion = 9;

        SetResponse(
            "No tomo diuréticos y tampoco he tenido vómitos, " +
            "diarrea o deshidratación importante recientemente."
        );
    }

    // =====================================================
    // PREGUNTA 10 - DISTRACTORA
    // =====================================================

    public void ShowAnswer10()
    {
        selectedQuestion = 10;

        SetResponse(
            "No. No he tenido náuseas, acidez ni molestias " +
            "digestivas importantes recientemente."
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
                "• Cefalea recurrente\n" +
                "• Mareos\n" +
                "• Prurito intenso\n";
        }

        if (savedQuestions.Contains(2))
        {
            data +=
                "• Prurito posterior al baño con agua tibia\n";
        }

        if (savedQuestions.Contains(3))
        {
            data +=
                "• Visión borrosa ocasional\n" +
                "• Ardor o calor en los pies\n";
        }

        if (savedQuestions.Contains(4))
        {
            data +=
                "• Sin antecedente trombótico conocido\n";
        }

        if (savedQuestions.Contains(5))
        {
            data +=
                "• Niega tabaquismo\n" +
                "• Niega exposición habitual a humo o combustión\n";
        }

        if (savedQuestions.Contains(6))
        {
            data +=
                "• Residencia habitual: La Paz\n";
        }

        if (savedQuestions.Contains(7))
        {
            data +=
                "• Sin datos claros de apnea del sueño\n";
        }

        if (savedQuestions.Contains(8))
        {
            data +=
                "• Niega testosterona, eritropoyetina o anabólicos\n";
        }

        if (savedQuestions.Contains(9))
        {
            data +=
                "• Niega diuréticos\n" +
                "• Niega deshidratación reciente\n";
        }

        if (savedQuestions.Contains(10))
        {
            data +=
                "• Niega síntomas digestivos relevantes\n";
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
    // CONTINUAR A DIAGNÓSTICO
    // =====================================================

    public void ContinueToClinicalAssessment()
    {
        if (savedQuestions.Count == 0)
        {
            Debug.LogWarning(
                "No puede continuar sin registrar información clínica."
            );

            if (hudController != null)
            {
                hudController.ShowErrorAlert(
                    "Registre información clínica antes de continuar."
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
            hudController.SetCurrentPhase("Diagnóstico");
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
        // ABRIR VENTANA DE DIAGNÓSTICO
        // =================================================

        if (diagnosisController != null)
        {
            diagnosisController.OpenDiagnosis();

            Debug.Log(
                "ENTREVISTA FINALIZADA. PASANDO A DIAGNÓSTICO."
            );
        }
        else
        {
            Debug.LogError(
                "InterviewController: falta asignar DiagnosisController."
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

            // IMPORTANTE:
            // La entrevista es una interfaz.
            // Liberamos el mouse para poder seleccionar
            // las preguntas nuevamente.
            if (hudController != null)
            {
                hudController.SetUIInteractionMode(true);
            }

            // Forzar nuevamente el estado de interfaz.
            // Esto evita que el cursor quede bloqueado
            // después de restaurar la partida.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


            Debug.Log(
                "ENTREVISTA RESTAURADA. " +
                "LA ENTREVISTA AÚN NO HABÍA TERMINADO."
            );
        }
        else
        {
            Debug.Log(
                "ENTREVISTA RESTAURADA COMO FINALIZADA."
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