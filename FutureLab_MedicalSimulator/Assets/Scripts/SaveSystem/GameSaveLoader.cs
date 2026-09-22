using System.Collections;
using UnityEngine;

public class GameSaveLoader : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController characterController;

    [Header("Interfaz")]
    [SerializeField] private GameObject caseBriefingPanel;

    [Header("Controladores")]
    [SerializeField] private HUDController hudController;
    [SerializeField] private InterviewController interviewController;

    private void Start()
    {
        StartCoroutine(RestoreSavedGame());
    }

    private IEnumerator RestoreSavedGame()
    {
        // Esperar un frame para que los demás controladores
        // terminen su inicialización.
        yield return null;

        // =====================================================
        // OBTENER PARTIDA PENDIENTE
        // =====================================================

        SaveData data = MainMenuManager.pendingSaveData;

        if (data == null)
        {
            Debug.Log(
                "No hay una partida pendiente de restaurar."
            );

            yield break;
        }

        Debug.Log(
            "INICIANDO RESTAURACIÓN DE PARTIDA..."
        );

        // =====================================================
        // RESTAURAR POSICIÓN DEL JUGADOR
        // =====================================================

        if (playerTransform != null)
        {
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            playerTransform.position = new Vector3(
                data.playerPosX,
                data.playerPosY,
                data.playerPosZ
            );

            playerTransform.rotation = Quaternion.Euler(
                data.playerRotX,
                data.playerRotY,
                data.playerRotZ
            );

            if (characterController != null)
            {
                characterController.enabled = true;
            }

            Debug.Log(
                "POSICIÓN DEL JUGADOR RESTAURADA."
            );
        }
        else
        {
            Debug.LogWarning(
                "GameSaveLoader: Player Transform no asignado."
            );
        }

        // =====================================================
        // CERRAR PRESENTACIÓN INICIAL
        // =====================================================

        if (caseBriefingPanel != null)
        {
            caseBriefingPanel.SetActive(false);
        }

        // =====================================================
        // RESTAURAR HUD
        // =====================================================

        if (hudController != null)
        {
            hudController.RestoreLoadedGame(data);
        }

        // =====================================================
        // RESTAURAR ENTREVISTA
        // =====================================================

        if (interviewController != null)
        {
            interviewController.RestoreInterviewState(
                data.answeredQuestions,
                data.interviewCompleted
            );
        }
        else
        {
            Debug.LogWarning(
                "GameSaveLoader: InterviewController no asignado."
            );
        }

        // =====================================================
        // LIMPIAR PARTIDA PENDIENTE
        // =====================================================

        MainMenuManager.pendingSaveData = null;

        Debug.Log(
            "PARTIDA RESTAURADA CORRECTAMENTE."
        );
    }
}