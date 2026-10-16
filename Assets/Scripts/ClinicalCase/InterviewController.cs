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

    // Este panel se crear�/configurar� en la siguiente tarea.
    [SerializeField] private GameObject interviewSummaryPanel;

    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("Textos")]
    [SerializeField] private TMP_Text patientResponseText;
    [SerializeField] private TMP_Text registeredDataText;

    [Header("Textos del resumen")]
    [SerializeField] private TMP_Text summaryDataText;
    [SerializeField] private TMP_Text essentialProgressText;

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
    // el jugador NO haya registrado todav�a su hallazgo.
    //
    // Ejemplo:
    // answeredQuestions contiene 3
    // registeredFindings NO contiene 3
    //
    // Esto significa:
    // Pregunta 3 realizada = S�
    // Hallazgo 3 registrado = NO
    // -----------------------------------------------------

    private HashSet<int> answeredQuestions =
        new HashSet<int>();

    // -----------------------------------------------------
    // HALLAZGOS REGISTRADOS
    // -----------------------------------------------------
    //
    // Solo se agregan aqu� cuando el jugador presiona
    // expl�citamente "Registrar dato".
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
    // interviewCompleted solamente deber� convertirse
    // en true cuando posteriormente se confirme
    // el resumen de entrevista.
    //
    // "Finalizar entrevista" todav�a NO significa
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

        // El paciente quieto se utilizar� despu�s de la entrevista.
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
    // BLOQUEAR INTERACCIONES SI EL CASO YA FINALIZ�
    // =====================================================

    private bool BlockIfCaseEnded(string actionName)
    {
        if (hudController != null &&
            hudController.IsCaseEnded())
        {
            Debug.LogWarning(
                "ACCI�N CL�NICA BLOQUEADA: " +
                actionName +
                ". EL CASO YA FINALIZ�."
            );

            return true;
        }

        return false;
    }

    // =====================================================
    // ABRIR ENTREVISTA
    // =====================================================

    public void OpenInterview()
    {
        if (BlockIfCaseEnded("ABRIR ENTREVISTA"))
        {
            return;
        }

        // Si la entrevista ya fue confirmada, no se puede volver a abrir.
        if (interviewCompleted)
        {
            Debug.Log(
                "ENTREVISTA YA CONFIRMADA. " +
                "NO SE PUEDE REABRIR."
            );

            return;
        }

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
    // S�NTOMAS PRINCIPALES
    // ESENCIAL
    // =====================================================

    public void ShowAnswer1()
    {
        SelectQuestion(
            1,
            "Me siento muy d�bil, me mareo con frecuencia " +
            "y me falta el aire cuando hago esfuerzo."
        );
    }

    // =====================================================
    // PREGUNTA 2
    // TIEMPO DE EVOLUCI�N
    // ESENCIAL
    // =====================================================

    public void ShowAnswer2()
    {
        SelectQuestion(
            2,
            "Los s�ntomas comenzaron hace varios d�as " +
            "y �ltimamente han empeorado."
        );
    }

    // =====================================================
    // PREGUNTA 3
    // TRANSFUSI�N ANTERIOR
    // ESENCIAL
    // =====================================================

    public void ShowAnswer3()
    {
        SelectQuestion(
            3,
            "S�. Recib� una transfusi�n de sangre " +
            "durante una operaci�n anterior."
        );
    }

    // =====================================================
    // PREGUNTA 4
    // REACCI�N TRANSFUSIONAL PREVIA
    // ESENCIAL
    // =====================================================

    public void ShowAnswer4()
    {
        SelectQuestion(
            4,
            "S�. Durante aquella transfusi�n present� " +
            "fiebre y escalofr�os."
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
    // GRUPO SANGU�NEO REFERIDO
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
    // Estos m�todos permanecen temporalmente porque
    // actualmente pueden existir botones de Unity
    // conectados a ShowAnswer7/8/9.
    //
    // Cuando eliminemos esos botones del InterviewPanel,
    // estos m�todos tambi�n podr�n eliminarse.
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
        if (BlockIfCaseEnded("REALIZAR PREGUNTA"))
        {
            return;
        }

        // Guardamos qu� pregunta est� actualmente seleccionada.
        selectedQuestion = questionId;

        // IMPORTANTE:
        // La pregunta queda marcada como REALIZADA
        // solamente por haberla consultado.
        //
        // Todav�a NO significa que su hallazgo
        // haya sido registrado.
        answeredQuestions.Add(questionId);

        // Mostrar respuesta del paciente.
        SetResponse(patientAnswer);

        // Habilitar o deshabilitar Registrar dato.
        //
        // Si el hallazgo ya hab�a sido registrado,
        // el bot�n queda deshabilitado.
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
        if (BlockIfCaseEnded("REALIZAR PREGUNTA"))
        {
            return;
        }

        selectedQuestion = 0;

        SetResponse(
            "Esta pregunta ya no forma parte " +
            "de la entrevista cl�nica actual."
        );

        UpdateRegisterButtonState();

        if (hudController != null)
        {
            hudController.ShowWarningAlert(
                "Pregunta no disponible en esta versi�n del caso."
            );
        }
    }

    // =====================================================
    // REGISTRAR DATO
    // =====================================================

    public void SaveCurrentResponse()
    {
        if (BlockIfCaseEnded("REGISTRAR HALLAZGO"))
        {
            return;
        }

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
        // solo existen actualmente preguntas cl�nicas
        // v�lidas del 1 al 6.
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
                    "Esta informaci�n ya fue registrada."
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
                "Informaci�n cl�nica registrada."
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
    // ACTUALIZAR BOT�N REGISTRAR DATO
    // =====================================================

    private void UpdateRegisterButtonState()
    {
        if (saveResponseButton == null)
        {
            return;
        }

        if (hudController != null &&
            hudController.IsCaseEnded())
        {
            saveResponseButton.interactable = false;
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
    // ACTUALIZAR INFORMACI�N REGISTRADA
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
                "A�n no se registraron datos.\n\n" +
                "Informaci�n esencial registrada: 0/4";

            registeredDataText.text = data;

            return;
        }

        // -------------------------------------------------
        // HALLAZGO 1
        // S�NTOMAS PRINCIPALES
        // -------------------------------------------------

        if (registeredFindings.Contains(1))
        {
            data +=
                "� S�ntomas principales: debilidad, " +
                "mareos y disnea al esfuerzo.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 2
        // TIEMPO DE EVOLUCI�N
        // -------------------------------------------------

        if (registeredFindings.Contains(2))
        {
            data +=
                "� Tiempo de evoluci�n: s�ntomas de varios d�as " +
                "con empeoramiento actual.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 3
        // TRANSFUSI�N ANTERIOR
        // -------------------------------------------------

        if (registeredFindings.Contains(3))
        {
            data +=
                "� Transfusi�n anterior: recibi� sangre " +
                "durante una operaci�n.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 4
        // REACCI�N TRANSFUSIONAL PREVIA
        // -------------------------------------------------

        if (registeredFindings.Contains(4))
        {
            data +=
                "� Reacci�n previa: present� fiebre " +
                "y escalofr�os durante una transfusi�n.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 5
        // ALERGIAS
        // COMPLEMENTARIO
        // -------------------------------------------------

        if (registeredFindings.Contains(5))
        {
            data +=
                "� Alergias: no refiere alergias conocidas.\n";
        }

        // -------------------------------------------------
        // HALLAZGO 6
        // GRUPO SANGU�NEO REFERIDO
        // COMPLEMENTARIO
        // -------------------------------------------------

        if (registeredFindings.Contains(6))
        {
            data +=
                "� Grupo sangu�neo referido: A positivo, " +
                "pendiente de confirmaci�n.\n";
        }

        // -------------------------------------------------
        // PROGRESO ESENCIAL
        // -------------------------------------------------

        int essentialCount =
            GetEssentialFindingsCount();

        data +=
            "\nInformaci�n esencial registrada: " +
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
    // Este m�todo conserva el nombre antiguo
    // ContinueToClinicalAssessment para NO romper todav�a
    // el OnClick configurado en Unity.
    //
    // Sin embargo, YA NO cambia a Valoraci�n.
    //
    // En la siguiente tarea este bot�n abrir�
    // InterviewSummaryPanel.
    // =====================================================

    public void ContinueToClinicalAssessment()
    {
        FinishInterview();
    }

    // =====================================================
    // PREPARAR FINALIZACI�N DE ENTREVISTA
    // =====================================================

    public void FinishInterview()
    {
        if (BlockIfCaseEnded("FINALIZAR ENTREVISTA"))
        {
            return;
        }

        // -------------------------------------------------
        // NO MARCAR TODAV�A interviewCompleted = true
        // -------------------------------------------------
        //
        // Eso solamente ocurrir� cuando posteriormente
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
            // Al finalizar la entrevista solo actualizamos y mostramos el resumen.
            // El cambio de paciente se realiza recién al confirmar la entrevista.
            UpdateInterviewSummary();

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
                "LA FASE CONTIN�A SIENDO ENTREVISTA."
            );

            return;
        }

        // -------------------------------------------------
        // EL PANEL TODAV�A NO FUE CREADO
        // -------------------------------------------------

        Debug.Log(
            "FINALIZACI�N DE ENTREVISTA SOLICITADA. " +
            "InterviewSummaryPanel todav�a no est� asignado. " +
            "NO se cambia a valoraci�n cl�nica."
        );

        if (hudController != null)
        {
            hudController.ShowWarningAlert(
                "La entrevista est� lista para mostrar su resumen."
            );
        }
    }

    // =====================================================
    // PREPARAR PACIENTE PARA VALORACI�N CL�NICA
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
            "PACIENTE DE VALORACI�N ACTIVADO."
        );
    }

    // =====================================================
    // ACTUALIZAR RESUMEN DE ENTREVISTA
    // =====================================================

    private void UpdateInterviewSummary()
    {
        if (summaryDataText != null)
        {
            string summary = "";

            if (registeredFindings.Contains(1))
            {
                summary +=
                    "� S�ntomas principales: debilidad, mareos y disnea al esfuerzo.\n";
            }

            if (registeredFindings.Contains(2))
            {
                summary +=
                    "� Tiempo de evoluci�n: s�ntomas de varios d�as con empeoramiento actual.\n";
            }

            if (registeredFindings.Contains(3))
            {
                summary +=
                    "� Transfusi�n anterior: recibi� sangre durante una operaci�n.\n";
            }

            if (registeredFindings.Contains(4))
            {
                summary +=
                    "� Reacci�n previa: present� fiebre y escalofr�os durante una transfusi�n.\n";
            }

            if (registeredFindings.Contains(5))
            {
                summary +=
                    "� Alergias: no refiere alergias conocidas.\n";
            }

            if (registeredFindings.Contains(6))
            {
                summary +=
                    "� Grupo sangu�neo referido: A positivo, pendiente de confirmaci�n.\n";
            }

            if (string.IsNullOrEmpty(summary))
            {
                summary =
                    "A�n no hay hallazgos registrados.";
            }

            summaryDataText.text = summary;
        }

        if (essentialProgressText != null)
        {
            essentialProgressText.text =
                "Informaci�n esencial registrada: " +
                GetEssentialFindingsCount() +
                "/4";
        }
    }

    // =====================================================
    // VOLVER A LA ENTREVISTA DESDE EL RESUMEN
    // =====================================================

    public void BackToInterview()
    {
        if (BlockIfCaseEnded("VOLVER A LA ENTREVISTA"))
        {
            return;
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

        // Seguimos en ENTREVISTA.
        if (hudController != null)
        {
            hudController.SetCurrentPhase("Entrevista");
            hudController.SetUIInteractionMode(true);
        }

        if (caseFlowController != null)
        {
            caseFlowController.GoToInterview();
        }

        UpdateRegisteredData();

        SetResponse(
            "Puede continuar entrevistando al paciente."
        );

        Debug.Log(
            "VOLVIENDO A LA ENTREVISTA SIN PERDER LOS DATOS REGISTRADOS."
        );
    }

    // =====================================================
    // CERRAR ENTREVISTA Y PASAR A VALORACI�N
    // TAREA 9
    // =====================================================

    private void CompleteInterviewAndReturnToPatientView()
    {
        // Cambiar al paciente preparado para valoración únicamente
        // después de confirmar la entrevista.
        PreparePatientForClinicalAssessment();

        // Cerrar toda la interfaz de entrevista.
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

        if (interviewSummaryPanel != null)
        {
            interviewSummaryPanel.SetActive(false);
        }

        // No abrir autom�ticamente la antigua valoraci�n cl�nica.
        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        selectedQuestion = 0;
        UpdateRegisterButtonState();

        // Cambiar HUD a VALORACI�N y devolver el control de c�mara.
        if (hudController != null)
        {
            hudController.SetCurrentPhase("Valoracion");
            hudController.SetUIInteractionMode(false);
        }

        // Sincronizar la fase l�gica del caso.
        if (caseFlowController != null)
        {
            caseFlowController.GoToAssessment();
        }

        Debug.Log(
            "ENTREVISTA CERRADA. " +
            "FASE CAMBIADA A VALORACI�N. " +
            "NO SE ABRE NING�N PANEL DE VALORACI�N AUTOM�TICAMENTE."
        );
    }

    // =====================================================
    // CONFIRMAR RESUMEN Y CONTINUAR
    // =====================================================

    public void ConfirmInterviewSummary()
    {
        if (BlockIfCaseEnded("CONFIRMAR ENTREVISTA"))
        {
            return;
        }

        int essentialCount = GetEssentialFindingsCount();

        // 0/4 o 1/4: bloquear sin penalizaci�n.
        if (essentialCount <= 1)
        {
            if (hudController != null)
            {
                hudController.ShowWarningAlert(
                    "Informaci�n cl�nica insuficiente. " +
                    "Contin�e la entrevista antes de avanzar."
                );
            }

            Debug.LogWarning(
                "CONFIRMACI�N BLOQUEADA. " +
                "INFORMACI�N ESENCIAL: " +
                essentialCount +
                "/4"
            );

            return;
        }

        // 2/4: bloquear y aplicar +05:00 una sola vez.
        if (essentialCount == 2)
        {
            if (hudController != null)
            {
                hudController.ShowWarningAlert(
                    "Informaci�n cl�nica insuficiente. " +
                    "Contin�e la entrevista antes de avanzar."
                );

                hudController.ApplyTimePenalty(
                    "ENTREVISTA_INSUFICIENTE_2_DE_4"
                );
            }

            Debug.LogWarning(
                "CONFIRMACI�N BLOQUEADA CON 2/4."
            );

            return;
        }

        // 3/4: permitir con advertencia y futura
        // penalizaci�n de puntuaci�n.
        if (essentialCount == 3)
        {
            if (hudController != null)
            {
                hudController.ShowWarningAlert(
                    "La entrevista se continuar� con " +
                    "informaci�n cl�nica incompleta."
                );
            }

            Debug.LogWarning(
                "ENTREVISTA CONFIRMADA CON 3/4. " +
                "DEBER� APLICARSE PENALIZACI�N DE PUNTUACI�N."
            );

            interviewCompleted = true;

            CompleteInterviewAndReturnToPatientView();

            return;
        }

        // 4/4: confirmar normalmente.
        if (essentialCount == 4)
        {
            interviewCompleted = true;

            if (hudController != null)
            {
                hudController.ShowSuccessAlert(
                    "Entrevista completada correctamente."
                );
            }

            Debug.Log(
                "ENTREVISTA CONFIRMADA CON 4/4. " +
                "PUNTUACI�N COMPLETA."
            );

            CompleteInterviewAndReturnToPatientView();

            return;
        }
    }

    // =====================================================
    // OCULTAR INTERFAZ CL�NICA AL FINALIZAR EL CASO
    // =====================================================

    public void HideInterviewUIForCaseEnd()
    {
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

        if (interviewSummaryPanel != null)
        {
            interviewSummaryPanel.SetActive(false);
        }

        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        selectedQuestion = 0;

        if (saveResponseButton != null)
        {
            saveResponseButton.interactable = false;
        }

        UpdateRegisterButtonState();

        Debug.Log(
            "INTERFAZ CL�NICA OCULTADA POR FINALIZACI�N DEL CASO."
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
    // Conservamos ese m�todo, pero desde ahora representa
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
    // SABER SI LA ENTREVISTA TERMIN�
    // =====================================================

    public bool IsInterviewCompleted()
    {
        return interviewCompleted;
    }

    // =====================================================
    // ESTADO VISUAL ACTUAL DE LA ENTREVISTA
    // =====================================================

    public string GetInterviewViewState()
    {
        if (interviewSummaryPanel != null &&
            interviewSummaryPanel.activeSelf)
        {
            return "Summary";
        }

        if (interviewPanel != null &&
            interviewPanel.activeSelf)
        {
            return "Interview";
        }

        return "Hidden";
    }

    public int GetSelectedInterviewQuestion()
    {
        return selectedQuestion;
    }

    public string GetCurrentInterviewResponse()
    {
        if (patientResponseText == null)
        {
            return "";
        }

        return patientResponseText.text;
    }

    // =====================================================
    // RESTAURAR ESTADO NUEVO DE ENTREVISTA
    // =====================================================
    //
    // Este m�todo ya soporta por separado:
    // - preguntas realizadas
    // - hallazgos registrados
    //
    // Posteriormente SaveData y GameSaveLoader utilizar�n
    // esta versi�n.
    // =====================================================

    public void RestoreInterviewState(
        List<int> questions,
        List<int> findings,
        bool completed,
        string savedViewState,
        int savedSelectedQuestion,
        string savedResponse
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

        if (savedSelectedQuestion >= 1 &&
            savedSelectedQuestion <= 6)
        {
            selectedQuestion = savedSelectedQuestion;
        }
        else
        {
            selectedQuestion = 0;
        }

        UpdateRegisteredData();

        // -------------------------------------------------
        // ENTREVISTA A�N NO CONFIRMADA
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

            // Compatibilidad con guardados antiguos:
            // si no existe estado visual, volver a entrevista.
            string viewState =
                string.IsNullOrWhiteSpace(savedViewState)
                ? "Interview"
                : savedViewState;

            if (viewState == "Summary")
            {
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
                    UpdateInterviewSummary();
                    interviewSummaryPanel.SetActive(true);
                    interviewSummaryPanel.transform.SetAsLastSibling();
                }

                Debug.Log(
                    "ENTREVISTA RESTAURADA EN RESUMEN."
                );
            }
            else
            {
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

                if (!string.IsNullOrEmpty(savedResponse))
                {
                    SetResponse(savedResponse);
                }
                else
                {
                    SetResponse(InitialMessage);
                }

                Debug.Log(
                    "ENTREVISTA RESTAURADA EN PANEL DE ENTREVISTA."
                );
            }

            UpdateRegisterButtonState();

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
                "/4 | VISTA: " +
                viewState
            );
        }
        else
        {
            PreparePatientForClinicalAssessment();

            // La entrevista ya hab�a sido confirmada.
            // No se abre nuevamente la entrevista ni el resumen.

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
    // COMPATIBILIDAD CON LLAMADAS DE 3 PAR�METROS
    // =====================================================

    public void RestoreInterviewState(
        List<int> questions,
        List<int> findings,
        bool completed
    )
    {
        RestoreInterviewState(
            questions,
            findings,
            completed,
            "Interview",
            0,
            InitialMessage
        );
    }

    // =====================================================
    // COMPATIBILIDAD CON GUARDADOS ANTIGUOS
    // =====================================================
    //
    // Actualmente GameSaveLoader puede seguir utilizando
    // esta versi�n de dos par�metros.
    //
    // Los guardados anteriores usaban la misma lista
    // tanto para informaci�n preguntada como registrada.
    //
    // Por ello, temporalmente restauramos esos valores
    // en ambas colecciones.
    //
    // Esto se cambiar� cuando ampliemos SaveData.
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
