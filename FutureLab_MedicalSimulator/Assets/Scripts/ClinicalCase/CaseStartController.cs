using UnityEngine;
using StarterAssets;

public class CaseStartController : MonoBehaviour
{
    [Header("Interfaz inicial")]
    [SerializeField] private GameObject caseBriefingPanel;

    [Header("Jugador y punto de llegada")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform patientApproachPoint;

    [Header("Componentes del jugador")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [Header("Controladores del caso")]
    [SerializeField] private HUDController hudController;
    [SerializeField] private ClinicalCaseFlowController caseFlowController;

    private void Start()
    {
        // =====================================================
        // FASE INICIAL
        // =====================================================

        if (caseFlowController != null)
        {
            caseFlowController.GoToStart();
        }

        if (hudController != null)
        {
            hudController.SetCurrentPhase("Inicio");
        }

        // =====================================================
        // MOSTRAR PRESENTACIÓN INICIAL
        // =====================================================

        if (caseBriefingPanel != null)
        {
            caseBriefingPanel.SetActive(true);
        }

        // =====================================================
        // BLOQUEAR CONTROL DEL JUGADOR
        // =====================================================

        SetPlayerControl(false);

        // =====================================================
        // LIBERAR CURSOR
        // =====================================================

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "CASO PREPARADO. FASE ACTUAL: INICIO."
        );
    }

    // =====================================================
    // INICIAR CASO
    // =====================================================

    public void StartCase()
    {
        if (player == null)
        {
            Debug.LogError(
                "CaseStartController: falta asignar PlayerCapsule."
            );

            return;
        }

        if (patientApproachPoint == null)
        {
            Debug.LogError(
                "CaseStartController: falta asignar PatientApproachPoint."
            );

            return;
        }

        // =====================================================
        // DESACTIVAR COLISIONES TEMPORALMENTE
        // =====================================================

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // =====================================================
        // TELETRANSPORTAR AL JUGADOR FRENTE AL PACIENTE
        // =====================================================

        player.SetPositionAndRotation(
            patientApproachPoint.position,
            patientApproachPoint.rotation
        );

        // =====================================================
        // REINICIAR INCLINACIÓN VERTICAL DE LA CÁMARA
        // =====================================================

        if (
            firstPersonController != null &&
            firstPersonController.CinemachineCameraTarget != null
        )
        {
            firstPersonController
                .CinemachineCameraTarget
                .transform
                .localRotation = Quaternion.identity;
        }

        // =====================================================
        // REACTIVAR COLISIONES
        // =====================================================

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // =====================================================
        // OCULTAR PRESENTACIÓN
        // =====================================================

        if (caseBriefingPanel != null)
        {
            caseBriefingPanel.SetActive(false);
        }

        // =====================================================
        // CAMBIAR A FASE DE ACERCAMIENTO
        // =====================================================

        if (caseFlowController != null)
        {
            caseFlowController.GoToApproach();
        }

        if (hudController != null)
        {
            hudController.SetCurrentPhase("Acercamiento");

            // Iniciar cronómetro del caso.
            hudController.StartTimer();
        }

        // =====================================================
        // ACTIVAR MOVIMIENTO Y CÁMARA
        // =====================================================

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "ATENCIÓN INICIADA. " +
            "JUGADOR TELETRANSPORTADO AL PACIENTE. " +
            "FASE ACTUAL: ACERCAMIENTO."
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