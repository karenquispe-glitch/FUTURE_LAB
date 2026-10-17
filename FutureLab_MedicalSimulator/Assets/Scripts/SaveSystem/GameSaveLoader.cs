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
    [SerializeField] private ClinicalCaseFlowController caseFlowController;

    // =====================================================
    // INICIO
    // =====================================================

    private void Start()
    {
        StartCoroutine(RestoreSavedGame());
    }

    // =====================================================
    // RESTAURAR PARTIDA
    // =====================================================

    private IEnumerator RestoreSavedGame()
    {
        // Esperar un frame para permitir que los demás
        // controladores terminen su inicialización.
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
        // NORMALIZAR FASE GUARDADA
        // =====================================================

        string normalizedPhase =
            NormalizePhaseName(data.currentPhase);

        // =====================================================
        // 1. RESTAURAR PRIMERO LA FASE CENTRAL
        // =====================================================
        //
        // IMPORTANTE:
        // Antes se restauraba la entrevista y DESPUÉS la fase.
        // Eso podía volver a ocultar InterviewPanel.
        //
        // Ahora la fase se restaura primero.
        // =====================================================

        if (caseFlowController != null)
        {
            ClinicalCaseFlowController.ClinicalCasePhase savedPhase;

            bool validPhase = System.Enum.TryParse(
                normalizedPhase,
                true,
                out savedPhase
            );

            if (validPhase)
            {
                caseFlowController.RestorePhase(
                    savedPhase
                );

                Debug.Log(
                    "FASE DEL CASO RESTAURADA: " +
                    savedPhase
                );
            }
            else
            {
                Debug.LogWarning(
                    "GameSaveLoader: no se pudo restaurar " +
                    "la fase guardada: " +
                    data.currentPhase
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "GameSaveLoader: " +
                "ClinicalCaseFlowController no asignado."
            );
        }

        // =====================================================
        // 2. RESTAURAR HUD
        // =====================================================

        if (hudController != null)
        {
            hudController.RestoreLoadedGame(data);

            Debug.Log(
                "HUD RESTAURADO."
            );
        }
        else
        {
            Debug.LogWarning(
                "GameSaveLoader: HUDController no asignado."
            );
        }

        // Esperar un frame adicional para que los cambios
        // de fase/HUD terminen antes de abrir la interfaz
        // específica de la entrevista.
        yield return null;

        // =====================================================
        // 3. RESTAURAR ENTREVISTA AL FINAL
        // =====================================================
        //
        // La entrevista se restaura AL FINAL para asegurar que:
        // - InterviewPanel quede visible;
        // - RegisteredDataPanel quede visible;
        // - el mouse quede libre;
        // - se conserve answeredQuestions;
        // - se conserve registeredFindings;
        // - se recupere el progreso X/4.
        // =====================================================

        if (
            normalizedPhase == "Entrevista" &&
            interviewController != null
        )
        {
            interviewController.RestoreInterviewState(
                data.answeredQuestions,
                data.registeredFindings,
                data.interviewCompleted,
                data.interviewViewState,
                data.selectedInterviewQuestion,
                data.currentInterviewResponse
            );

            Debug.Log(
                "ESTADO DE ENTREVISTA RESTAURADO AL FINAL DEL PROCESO."
            );
        }
        else if (
            normalizedPhase == "Entrevista" &&
            interviewController == null
        )
        {
            Debug.LogWarning(
                "GameSaveLoader: InterviewController no asignado."
            );
        }
        else
        {
            Debug.Log(
                "LA PARTIDA NO ESTABA EN ENTREVISTA. " +
                "NO SE ABRIRÁ LA ENTREVISTA AUTOMÁTICAMENTE."
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

    // =====================================================
    // NORMALIZAR NOMBRE DE FASE
    // =====================================================

    private string NormalizePhaseName(string phase)
    {
        if (string.IsNullOrEmpty(phase))
        {
            return phase;
        }

        string normalized = phase.Trim();

        normalized = normalized
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("Á", "A")
            .Replace("É", "E")
            .Replace("Í", "I")
            .Replace("Ó", "O")
            .Replace("Ú", "U")
            .Replace(" ", "");

        return normalized;
    }
}
