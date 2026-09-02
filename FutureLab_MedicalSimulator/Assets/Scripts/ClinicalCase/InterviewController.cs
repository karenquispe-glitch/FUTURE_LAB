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
    // ALERTAS DE DESEMPEÑO
    // =====================================================

    [Header("Sistema de alertas")]
    [SerializeField] private HUDController hudController;

    // =====================================================
    // VARIABLES
    // =====================================================

    private int selectedQuestion = 0;

    private HashSet<int> savedQuestions =
        new HashSet<int>();

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
    // ¿Qué molestias son las que más le preocupan actualmente?
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
    // ¿Hay alguna situación en la que sus molestias
    // aparezcan o empeoren?
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
    // ¿Ha presentado visión borrosa, zumbidos,
    // hormigueos o ardor en manos o pies?
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
    // ¿Ha tenido hinchazón dolorosa de una pierna,
    // dolor en el pecho o algún episodio de trombosis?
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
    // ¿Fuma o está expuesto frecuentemente
    // a humo o combustión?
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
    // ¿Dónde vive habitualmente y desde hace cuánto tiempo?
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
    // ¿Ronca mucho, deja de respirar mientras duerme
    // o tiene mucha somnolencia durante el día?
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
    // ¿Utiliza testosterona, eritropoyetina,
    // esteroides anabólicos o tratamientos hormonales?
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
    // ¿Toma diuréticos o ha tenido deshidratación
    // importante recientemente?
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
    // ¿Ha presentado náuseas, acidez o molestias digestivas?
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
        // ---------------------------------------------
        // ERROR 1:
        // Intentar guardar sin seleccionar pregunta.
        // ---------------------------------------------

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

        // ---------------------------------------------
        // EVITAR INFORMACIÓN DUPLICADA
        // ---------------------------------------------

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

        // ---------------------------------------------
        // GUARDADO
        // ---------------------------------------------

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

        // PREGUNTA 1
        if (savedQuestions.Contains(1))
        {
            data +=
                "• Cefalea recurrente\n" +
                "• Mareos\n" +
                "• Prurito intenso\n";
        }

        // PREGUNTA 2
        if (savedQuestions.Contains(2))
        {
            data +=
                "• Prurito posterior al baño con agua tibia\n";
        }

        // PREGUNTA 3
        if (savedQuestions.Contains(3))
        {
            data +=
                "• Visión borrosa ocasional\n" +
                "• Ardor o calor en los pies\n";
        }

        // PREGUNTA 4
        if (savedQuestions.Contains(4))
        {
            data +=
                "• Sin antecedente trombótico conocido\n";
        }

        // PREGUNTA 5
        if (savedQuestions.Contains(5))
        {
            data +=
                "• Niega tabaquismo\n" +
                "• Niega exposición habitual a humo o combustión\n";
        }

        // PREGUNTA 6
        if (savedQuestions.Contains(6))
        {
            data +=
                "• Residencia habitual: La Paz\n";
        }

        // PREGUNTA 7
        if (savedQuestions.Contains(7))
        {
            data +=
                "• Sin datos claros de apnea del sueño\n";
        }

        // PREGUNTA 8
        if (savedQuestions.Contains(8))
        {
            data +=
                "• Niega testosterona, eritropoyetina o anabólicos\n";
        }

        // PREGUNTA 9
        if (savedQuestions.Contains(9))
        {
            data +=
                "• Niega diuréticos\n" +
                "• Niega deshidratación reciente\n";
        }

        // PREGUNTA 10 - DISTRACTORA
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
    // CONTINUAR A VALORACIÓN CLÍNICA
    // =====================================================

    public void ContinueToClinicalAssessment()
    {
        // ---------------------------------------------
        // ERROR:
        // Intentar continuar sin registrar información.
        // ---------------------------------------------

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

        // ---------------------------------------------
        // CONTINUAR CORRECTAMENTE
        // ---------------------------------------------

        Debug.Log(
            "CONTINUAR: pasando a Valoración Clínica / Signos Vitales."
        );

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

        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "InterviewController: falta asignar ClinicalAssessmentPanel."
            );
        }
    }
}