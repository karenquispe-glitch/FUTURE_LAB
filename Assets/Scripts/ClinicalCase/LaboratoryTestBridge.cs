using UnityEngine;

public class LaboratoryTestBridge : MonoBehaviour
{
    [Header("Controladores")]
    [SerializeField]
    private ClinicalAssessmentController assessmentController;

    [SerializeField]
    private LaboratoryUIController laboratoryUI;

    [SerializeField]
    private HUDController hudController;

    [Header("Panel de valoración")]
    [SerializeField]
    private GameObject clinicalAssessmentPanel;

    [ContextMenu("SIMULAR FIN DE VALORACION")]
    public void SimularFinValoracion()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Ejecuta Play antes de realizar la prueba.");
            return;
        }

        if (assessmentController == null ||
            laboratoryUI == null ||
            hudController == null)
        {
            Debug.LogError("Faltan referencias.");
            return;
        }

        // Simulamos que el jugador realizó
        // los cuatro procedimientos necesarios.

        assessmentController.RegisterInspection();
        assessmentController.RegisterPulse();
        assessmentController.RegisterBloodPressure();
        assessmentController.RegisterTemperature();

        if (!assessmentController.CanRequestTests)
        {
            Debug.LogWarning("Valoración incompleta.");
            return;
        }

        // Cerramos la valoración clínica.
        if (clinicalAssessmentPanel != null)
        {
            clinicalAssessmentPanel.SetActive(false);
        }

        // Actualizamos la fase.
        hudController.SetCurrentPhase("PruebasIniciales");

        // Mostramos el botón del laboratorio.
        laboratoryUI.EnableAfterAssessment();

        // Liberamos temporalmente el cursor para la prueba.
        hudController.SetUIInteractionMode(true);

        Debug.Log("LABORATORIO HABILITADO");
    }
}