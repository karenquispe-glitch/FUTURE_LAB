using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterviewController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject patientInfoPanel;
    [SerializeField] private GameObject interviewPanel;
    [SerializeField] private GameObject registeredDataPanel;
    [SerializeField] private GameObject clinicalAssessmentPanel;

    [Header("Textos")]
    [SerializeField] private TMP_Text patientResponseText;
    [SerializeField] private TMP_Text registeredDataText;

    private int selectedQuestion = 0;
    private HashSet<int> savedQuestions = new HashSet<int>();

    private const string InitialMessage =
        "Seleccione una pregunta para conocer la respuesta del paciente.";

    private void Start()
    {
        // La entrevista empieza oculta.
        if (interviewPanel != null)
        {
            interviewPanel.SetActive(false);
        }

        // Los datos registrados empiezan ocultos.
        if (registeredDataPanel != null)
        {
            registeredDataPanel.SetActive(false);
        }

        // La valoración clínica empieza oculta.
        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        SetResponse(InitialMessage);
        UpdateRegisteredData();
    }

    // -----------------------------------------------------
    // ABRIR ENTREVISTA
    // -----------------------------------------------------

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

    // -----------------------------------------------------
    // PREGUNTAS Y RESPUESTAS
    // -----------------------------------------------------

    public void ShowAnswer1()
    {
        selectedQuestion = 1;

        SetResponse(
            "Desde hace un tiempo tengo dolor de cabeza, mareos y, " +
            "después de bañarme con agua tibia, me da mucha picazón " +
            "en todo el cuerpo."
        );
    }

    public void ShowAnswer2()
    {
        selectedQuestion = 2;

        SetResponse(
            "Las molestias comenzaron hace varios meses y últimamente " +
            "se han vuelto más frecuentes."
        );
    }

    public void ShowAnswer3()
    {
        selectedQuestion = 3;

        SetResponse(
            "La picazón aparece principalmente después de bañarme " +
            "con agua tibia y puede durar varios minutos."
        );
    }

    public void ShowAnswer4()
    {
        selectedQuestion = 4;

        SetResponse(
            "No, no he tenido dificultad para respirar."
        );
    }

    public void ShowAnswer5()
    {
        selectedQuestion = 5;

        SetResponse(
            "No he perdido el conocimiento, aunque algunas veces " +
            "el mareo ha sido bastante intenso."
        );
    }

    // -----------------------------------------------------
    // GUARDAR RESPUESTA
    // -----------------------------------------------------

    public void SaveCurrentResponse()
    {
        if (selectedQuestion == 0)
        {
            Debug.LogWarning(
                "Primero debe seleccionar una pregunta."
            );

            return;
        }

        // HashSet evita guardar la misma pregunta dos veces.
        savedQuestions.Add(selectedQuestion);

        UpdateRegisteredData();

        SetResponse(
            "Respuesta registrada. Seleccione otra pregunta para continuar."
        );

        selectedQuestion = 0;

        Debug.Log(
            "Información clínica registrada correctamente."
        );
    }

    // -----------------------------------------------------
    // ACTUALIZAR DATOS REGISTRADOS
    // -----------------------------------------------------

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

    // -----------------------------------------------------
    // CAMBIAR RESPUESTA DEL PACIENTE
    // -----------------------------------------------------

    private void SetResponse(string response)
    {
        if (patientResponseText != null)
        {
            patientResponseText.text = response;
        }
    }

    // -----------------------------------------------------
    // CONTINUAR A VALORACIÓN CLÍNICA
    // -----------------------------------------------------

    public void ContinueToClinicalAssessment()
    {
        Debug.Log(
            "CONTINUAR: pasando a Valoración Clínica / Signos Vitales."
        );

        // Ocultar la ficha anterior por seguridad.
        if (patientInfoPanel != null)
        {
            patientInfoPanel.SetActive(false);
        }

        // Ocultar la conversación.
        if (interviewPanel != null)
        {
            interviewPanel.SetActive(false);
        }

        // Ocultar datos registrados.
        if (registeredDataPanel != null)
        {
            registeredDataPanel.SetActive(false);
        }

        // Mostrar la siguiente etapa.
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