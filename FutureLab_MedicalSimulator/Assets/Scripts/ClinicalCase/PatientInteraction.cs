using System;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class PatientInteraction : MonoBehaviour
{
    [Header("Interfaz del paciente")]
    [SerializeField] private GameObject patientInfoPanel;

    [Header("Detección con clic derecho")]
    [SerializeField] private Camera playerCamera;

    [SerializeField]
    [Min(1f)]
    private float interactionDistance = 8f;

    [SerializeField]
    [Min(0.05f)]
    private float detectionRadius = 0.35f;

    [Header("Control del participante")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [Header("Controladores del caso")]
    [SerializeField] private HUDController hudController;
    [SerializeField] private ClinicalCaseFlowController caseFlowController;

    private bool interactionEnabled;
    private bool panelOpen;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (patientInfoPanel != null)
        {
            patientInfoPanel.SetActive(false);
        }

        interactionEnabled = false;
        panelOpen = false;
    }

    private void Update()
    {
        if (!interactionEnabled || panelOpen)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        // Detectar el clic derecho.
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Debug.Log("CLIC DERECHO DETECTADO.");

        if (playerCamera == null)
        {
            Debug.LogError(
                "PatientInteraction: falta asignar Player Camera."
            );

            return;
        }

        // Crear un área invisible desde el centro de la cámara.
        RaycastHit[] hits = Physics.SphereCastAll(
            playerCamera.transform.position,
            detectionRadius,
            playerCamera.transform.forward,
            interactionDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        // Revisar primero los objetos más cercanos.
        Array.Sort(
            hits,
            (hitA, hitB) => hitA.distance.CompareTo(hitB.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(
                "OBJETO DETECTADO: " + hit.collider.gameObject.name
            );

            PatientInteraction detectedPatient =
                hit.collider.GetComponentInParent<PatientInteraction>();

            if (detectedPatient == this)
            {
                Debug.Log(
                    "PACIENTE DETECTADO CORRECTAMENTE."
                );

                OpenPatientPanel();

                return;
            }
        }

        Debug.LogWarning(
            "El clic derecho funcionó, pero no se detectó al paciente."
        );
    }

    // =====================================================
    // HABILITAR INTERACCIÓN
    // =====================================================

    public void EnableInteraction()
    {
        interactionEnabled = true;

        Debug.Log(
            "INTERACCIÓN CON EL PACIENTE HABILITADA."
        );
    }

    // =====================================================
    // ABRIR PANEL DEL PACIENTE
    // =====================================================

    public void OpenPatientPanel()
    {
        if (patientInfoPanel == null)
        {
            Debug.LogError(
                "PatientInteraction: falta asignar PatientInfoPanel."
            );

            return;
        }

        panelOpen = true;

        patientInfoPanel.SetActive(true);

        // =================================================
        // CAMBIAR FASE A ENTREVISTA
        // =================================================

        if (caseFlowController != null)
        {
            caseFlowController.GoToInterview();
        }

        if (hudController != null)
        {
            hudController.SetCurrentPhase("Entrevista");
        }

        // =================================================
        // BLOQUEAR CONTROL DEL JUGADOR
        // =================================================

        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "PANEL DEL PACIENTE ABIERTO. " +
            "FASE ACTUAL: ENTREVISTA."
        );
    }

    // =====================================================
    // CERRAR PANEL DEL PACIENTE
    // =====================================================

    public void ClosePatientPanel()
    {
        panelOpen = false;

        if (patientInfoPanel != null)
        {
            patientInfoPanel.SetActive(false);
        }

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "PANEL DEL PACIENTE CERRADO."
        );
    }

    // =====================================================
    // CONTROL DEL JUGADOR
    // =====================================================

    private void SetPlayerControl(bool enabled)
    {
        if (firstPersonController != null)
        {
            firstPersonController.enabled = enabled;
        }

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.move = Vector2.zero;
            starterAssetsInputs.look = Vector2.zero;
            starterAssetsInputs.jump = false;
            starterAssetsInputs.sprint = false;

            starterAssetsInputs.cursorLocked = enabled;
            starterAssetsInputs.cursorInputForLook = enabled;
        }
    }
}