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
    // =====================================================

    public void ShowAnswer1()
    {
        selectedQuestion = 1;

        SetResponse(
            "Desde hace un tiempo tengo dolor de cabeza, " +
            "mareos y, después de bañarme con agua tibia, " +
            "me da mucha picazón en todo el cuerpo."
        );
    }

    // =====================================================
    // PREGUNTA 2
    // =====================================================

    public void ShowAnswer2()
    {
        selectedQuestion = 2;

        SetResponse(
            "Las molestias comenzaron hace varios meses " +
            "y últimamente se han vuelto más frecuentes."
        );
    }

    // =====================================================
    // PREGUNTA 3
    // =====================================================

    public void ShowAnswer3()
    {
        selectedQuestion = 3;

        SetResponse(
            "La picazón aparece principalmente después " +
            "de bañarme con agua tibia y puede durar " +
            "varios minutos."
        );
    }

    // =====================================================
    // PREGUNTA 4
    // =====================================================

    public void ShowAnswer4()
    {
        selectedQuestion = 4;

        SetResponse(
            "No, no he tenido dificultad para respirar."
        );
    }

    // =====================================================
    // PREGUNTA 5
    // =====================================================

    public void ShowAnswer5()
    {
        selectedQuestion = 5;

        SetResponse(
            "No he perdido el conocimiento, aunque algunas " +
            "veces el mareo ha sido bastante intenso."
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
        // GUARDADO CORRECTO
        // ---------------------------------------------

        UpdateRegisteredData();

        SetResponse(
            "Respuesta registrada. " +
            "Seleccione otra pregunta para continuar."
        );

        if (hudController != null)
        {
            hudController.ShowSuccessAlert(
                "Información clínica registrada correctamente."
            );
        }

        selectedQuestion = 0;

        Debug.Log(
            "Información clínica registrada correctamente."
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
                "• Prurito posterior al baño\n";
        }

        if (savedQuestions.Contains(2))
        {
            data +=
                "• Evolución de varios meses\n";
        }

        if (savedQuestions.Contains(3))
        {
            data +=
                "• Prurito desencadenado por agua tibia\n";
        }

        if (savedQuestions.Contains(4))
        {
            data +=
                "• Niega dificultad respiratoria\n";
        }

        if (savedQuestions.Contains(5))
        {
            data +=
                "• Niega pérdida de conocimiento\n";
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
        // ERROR 2:
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