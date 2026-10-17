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
        // RESTAURAR ENTREVISTA SOLO SI CORRESPONDE
        // =====================================================

        if (
            normalizedPhase == "Entrevista" &&
            interviewController != null
        )
        {
            interviewController.RestoreInterviewState(
                data.answeredQuestions,
                data.interviewCompleted
            );

            Debug.Log(
                "ESTADO DE ENTREVISTA RESTAURADO."
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
        // RESTAURAR FASE EXACTA DEL CASO
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
        // RESTAURAR HUD
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

        // =====================================================
        // FUTURAS RESTAURACIONES
        // =====================================================

        // Cuando estén terminados e integrados
        // los controladores de la Persona 2,
        // aquí se restaurarán:
        //
        // data.completedAssessments
        // data.requestedTests
        // data.reviewedResults
        // data.riskAlertTriggered
        // data.bleedingEventLevel
        // data.provisionalDiagnosis
        // data.score

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