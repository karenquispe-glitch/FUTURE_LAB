using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaboratoryUIController : MonoBehaviour
{
    [Header("Controladores existentes")]
    [SerializeField] private HUDController hudController;
    [SerializeField] private LabTestController labTestController;

    [Header("Paneles y textos")]
    [SerializeField] private GameObject labSelectionPanel;
    [SerializeField] private GameObject labResultsPanel;
    [SerializeField] private TMP_Text processingText;
    [SerializeField] private TMP_Text resultsText;

    [Header("Boton principal del HUD")]
    [SerializeField] private Button mainButton;
    [SerializeField] private Sprite requestLaboratorySprite;
    [SerializeField] private Sprite reviewResultsSprite;

    [Header("Seleccion de pruebas")]
    [SerializeField] private Toggle toggleHemogram;
    [SerializeField] private Toggle togglePeripheralSmear;
    [SerializeField] private Toggle togglePT;
    [SerializeField] private Toggle togglePTT;
    [SerializeField] private Toggle toggleFibrinogen;
    [SerializeField] private Toggle toggleDimerD;

    private readonly List<LabTestController.LabTest> acceptedTests =
        new List<LabTestController.LabTest>();

    private bool processing;
    private bool resultsReady;
    private bool resultsReviewed;

    private void Start()
    {
        if (labTestController == null)
            labTestController = GetComponent<LabTestController>();

        if (labSelectionPanel != null)
            labSelectionPanel.SetActive(false);

        if (labResultsPanel != null)
            labResultsPanel.SetActive(false);

        if (processingText != null)
            processingText.gameObject.SetActive(false);

        if (mainButton != null && requestLaboratorySprite != null)
            mainButton.image.sprite = requestLaboratorySprite;
    }

    // Mantiene la funcion On Click ya configurada en el boton del HUD.
    // Despues de procesar, el mismo boton abre los resultados.
    public void OpenLaboratory()
    {
        if (processing)
            return;

        if (resultsReady)
        {
            ShowLaboratoryResults();
            return;
        }

        if (labSelectionPanel == null)
        {
            Debug.LogError("Falta asignar LabSelectionPanel.");
            return;
        }

        labSelectionPanel.SetActive(true);
        SetUIMode(true);
    }

    public void CloseLaboratory()
    {
        if (labSelectionPanel != null)
            labSelectionPanel.SetActive(false);

        SetUIMode(false);
    }

    public void ConfirmRequest()
    {
        if (processing || resultsReady)
            return;

        if (labTestController == null || labSelectionPanel == null ||
            labResultsPanel == null || processingText == null ||
            resultsText == null || mainButton == null)
        {
            Debug.LogError("Faltan referencias del laboratorio en el Inspector.");
            return;
        }

        List<LabTestController.LabTest> selectedTests =
            new List<LabTestController.LabTest>();

        AddSelected(toggleHemogram, LabTestController.LabTest.Hemogram, selectedTests);
        AddSelected(togglePeripheralSmear, LabTestController.LabTest.PeripheralSmear, selectedTests);
        AddSelected(togglePT, LabTestController.LabTest.PT, selectedTests);
        AddSelected(togglePTT, LabTestController.LabTest.PTT, selectedTests);
        AddSelected(toggleFibrinogen, LabTestController.LabTest.Fibrinogen, selectedTests);
        AddSelected(toggleDimerD, LabTestController.LabTest.DimerD, selectedTests);

        if (selectedTests.Count == 0)
        {
            Debug.LogWarning("Selecciona al menos una prueba.");
            return;
        }

        acceptedTests.Clear();

        foreach (LabTestController.LabTest test in selectedTests)
        {
            // Evita solicitar de nuevo una prueba ya registrada.
            if (labTestController.IsTestRequested(test) ||
                labTestController.RequestTest(test))
            {
                acceptedTests.Add(test);
            }
        }

        if (acceptedTests.Count == 0)
        {
            Debug.LogWarning(
                "No se registraron pruebas. Comprueba que la valoracion clinica " +
                "se haya completado y que Assessment Controller este asignado."
            );
            return;
        }

        if (acceptedTests.Count < selectedTests.Count)
        {
            Debug.LogWarning(
                "Algunas pruebas fueron rechazadas; solo se procesaran las aceptadas."
            );
        }

        labSelectionPanel.SetActive(false);
        SetUIMode(false);
        StartCoroutine(ProcessLaboratoryTests());
    }

    private void AddSelected(
        Toggle toggle,
        LabTestController.LabTest test,
        List<LabTestController.LabTest> selectedTests)
    {
        if (toggle != null && toggle.isOn)
            selectedTests.Add(test);
    }

    private IEnumerator ProcessLaboratoryTests()
    {
        processing = true;
        processingText.text = "Procesando análisis...";
        processingText.gameObject.SetActive(true);
        mainButton.interactable = false;
        SetUIMode(true);

        // Tiempo del juego: tambien se detiene si el jugador pausa.
        yield return new WaitForSeconds(5f);

        processingText.gameObject.SetActive(false);
        processing = false;
        resultsReady = true;

        if (reviewResultsSprite != null)
            mainButton.image.sprite = reviewResultsSprite;
        else
            Debug.LogWarning("Falta asignar la imagen REVISAR RESULTADOS.");

        mainButton.interactable = true;
        Debug.Log("Laboratorio listo: puedes revisar los resultados.");
    }

    public void ShowLaboratoryResults()
    {
        if (!resultsReady || labResultsPanel == null || resultsText == null)
            return;

        if (!resultsReviewed)
        {
            // La primera apertura registra la revision en la logica existente.
            foreach (LabTestController.LabTest test in acceptedTests)
                ReviewRequestedTest(test);

            resultsReviewed = true;
        }

        StringBuilder content = new StringBuilder();
        content.AppendLine("RESULTADOS DE LABORATORIO");
        content.AppendLine();

        foreach (LabTestController.LabTest test in acceptedTests)
        {
            content.AppendLine(GetResultText(test));
            content.AppendLine();
        }

        resultsText.text = content.ToString();
        labResultsPanel.SetActive(true);
        SetUIMode(true);
    }

    public void CloseResults()
    {
        if (labResultsPanel != null)
            labResultsPanel.SetActive(false);

        SetUIMode(false);
    }

    // Llamar desde el fin de la valoracion clinica cuando este integrado.
    public void EnableAfterAssessment()
    {
        if (mainButton != null)
            mainButton.gameObject.SetActive(true);
    }

    private void ReviewRequestedTest(LabTestController.LabTest test)
    {
        switch (test)
        {
            case LabTestController.LabTest.Hemogram:
                labTestController.ReviewHemogram(); break;
            case LabTestController.LabTest.PeripheralSmear:
                labTestController.ReviewPeripheralSmear(); break;
            case LabTestController.LabTest.PT:
                labTestController.ReviewPT(); break;
            case LabTestController.LabTest.PTT:
                labTestController.ReviewPTT(); break;
            case LabTestController.LabTest.Fibrinogen:
                labTestController.ReviewFibrinogen(); break;
            case LabTestController.LabTest.DimerD:
                labTestController.ReviewDimerD(); break;
        }
    }

    private string GetResultText(LabTestController.LabTest test)
    {
        // Estos datos corresponden al LabTestController actual.
        // Deben validarse con el equipo antes de usar otro caso clinico.
        switch (test)
        {
            case LabTestController.LabTest.Hemogram:
                return "HEMOGRAMA: Hb 8.1 g/dL; Hto 25%; leucocitos 38,000/uL; plaquetas 28,000/uL.";
            case LabTestController.LabTest.PeripheralSmear:
                return "FROTIS PERIFERICO: promielocitos anormales, granulacion intensa y bastones de Auer.";
            case LabTestController.LabTest.PT:
                return "TIEMPO DE PROTROMBINA: prolongado.";
            case LabTestController.LabTest.PTT:
                return "TIEMPO PARCIAL DE TROMBOPLASTINA: prolongado.";
            case LabTestController.LabTest.Fibrinogen:
                return "FIBRINOGENO: disminuido.";
            case LabTestController.LabTest.DimerD:
                return "DIMERO D: elevado.";
            default:
                return "Resultado no disponible.";
        }
    }

    private void SetUIMode(bool enabled)
    {
        if (hudController != null)
            hudController.SetUIInteractionMode(enabled);
    }
}
