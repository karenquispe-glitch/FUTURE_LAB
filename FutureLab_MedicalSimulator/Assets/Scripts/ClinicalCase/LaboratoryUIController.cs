using UnityEngine;

public class LaboratoryUIController : MonoBehaviour
{
    [Header("Paneles del laboratorio")]
    [SerializeField] private GameObject labSelectionPanel;

    [Header("Controlador del HUD")]
    [SerializeField] private HUDController hudController;

    // Al iniciar el videojuego, ocultamos el laboratorio.
    private void Start()
    {
        if (labSelectionPanel != null)
        {
            labSelectionPanel.SetActive(false);
        }
    }

    // Abre el panel de selección de pruebas.
    public void OpenLaboratory()
    {
        if (labSelectionPanel == null)
        {
            Debug.LogError("No se asignó LabSelectionPanel.");
            return;
        }

        labSelectionPanel.SetActive(true);

        // Liberar el cursor para usar la interfaz.
        if (hudController != null)
        {
            hudController.SetUIInteractionMode(true);
        }

        Debug.Log("Laboratorio abierto correctamente.");
    }

    // Cierra el panel de selección.
    public void CloseLaboratory()
    {
        if (labSelectionPanel != null)
        {
            labSelectionPanel.SetActive(false);
        }

        // Regresar al modo normal del videojuego.
        if (hudController != null)
        {
            hudController.SetUIInteractionMode(false);
        }

        Debug.Log("Laboratorio cerrado correctamente.");
    }
}