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

    private void Start()
    {
        // Mostrar la presentación inicial.
        if (caseBriefingPanel != null)
        {
            caseBriefingPanel.SetActive(true);
        }

        // Desactivar temporalmente el control del jugador.
        SetPlayerControl(false);

        // Mostrar el cursor para poder pulsar COMENZAR.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

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

        // Desactivar temporalmente las colisiones.
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Llevar al jugador frente al paciente.
        player.SetPositionAndRotation(
            patientApproachPoint.position,
            patientApproachPoint.rotation
        );

        // Reiniciar la inclinación vertical de la cámara.
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

        // Reactivar las colisiones.
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // Ocultar la presentación.
        if (caseBriefingPanel != null)
        {
            caseBriefingPanel.SetActive(false);
        }

        // Activar movimiento y cámara.
        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

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