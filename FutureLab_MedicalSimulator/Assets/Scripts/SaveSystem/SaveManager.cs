using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string savePath;
    private string finalAttemptPath;

    // Límite real del caso: 20 minutos.
    private const float CaseTimeLimitSeconds = 1200f;

    private void Awake()
    {
        // Mantener un único SaveManager durante todo el juego.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Partida reanudable.
        savePath = Path.Combine(
            Application.persistentDataPath,
            "futurelab_save.json"
        );

        // Último intento finalizado.
        // Este archivo NO se utiliza para "Continuar partida".
        finalAttemptPath = Path.Combine(
            Application.persistentDataPath,
            "futurelab_last_attempt.json"
        );

        Debug.Log("SaveManager iniciado.");
        Debug.Log("Ruta de guardado reanudable: " + savePath);
        Debug.Log("Ruta de intento finalizado: " + finalAttemptPath);
    }

    // ==========================================
    // GUARDAR PARTIDA REANUDABLE
    // ==========================================

    public void SaveGame(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError(
                "SaveManager: no se puede guardar una partida nula."
            );

            return;
        }

        // SEGURIDAD:
        // un intento terminado no puede guardarse como reanudable.
        if (IsFinishedAttempt(data))
        {
            Debug.LogWarning(
                "SaveManager: el intento ya finalizó. " +
                "NO se guardará como partida reanudable."
            );

            // Conservar el resultado como intento finalizado.
            SaveFinalAttempt(data);

            // Si existía un guardado reanudable anterior,
            // eliminarlo para impedir que se continúe.
            DeleteSave();

            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(savePath, json);

            Debug.Log(
                "PARTIDA REANUDABLE GUARDADA CORRECTAMENTE."
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al guardar la partida. " +
                e.Message
            );
        }
    }

    // ==========================================
    // GUARDAR INTENTO FINALIZADO
    // ==========================================

    public void SaveFinalAttempt(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError(
                "SaveManager: no se puede guardar un intento final nulo."
            );

            return;
        }

        try
        {
            // Un intento almacenado aquí ya no es reanudable.
            data.caseCompleted = true;

            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(finalAttemptPath, json);

            Debug.Log(
                "INTENTO FINALIZADO GUARDADO CORRECTAMENTE."
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al guardar el intento finalizado. " +
                e.Message
            );
        }
    }

    // ==========================================
    // CARGAR PARTIDA REANUDABLE
    // ==========================================

    public SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log(
                "No existe ninguna partida reanudable guardada."
            );

            return null;
        }

        try
        {
            string json = File.ReadAllText(savePath);

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                Debug.LogWarning(
                    "SaveManager: el archivo de guardado no contiene datos válidos."
                );

                return null;
            }

            // Protección adicional para guardados antiguos:
            // nunca devolver un intento que ya haya finalizado.
            if (IsFinishedAttempt(data))
            {
                Debug.LogWarning(
                    "SaveManager: se encontró una partida finalizada. " +
                    "NO puede reanudarse."
                );

                SaveFinalAttempt(data);
                DeleteSave();

                return null;
            }

            Debug.Log(
                "PARTIDA REANUDABLE CARGADA CORRECTAMENTE."
            );

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al cargar la partida. " +
                e.Message
            );

            return null;
        }
    }

    // ==========================================
    // COMPROBAR SI EXISTE UNA PARTIDA REANUDABLE
    // ==========================================

    public bool HasSavedGame()
    {
        if (!File.Exists(savePath))
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(savePath);

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                return false;
            }

            // Si el archivo corresponde a un intento terminado,
            // se limpia y no se ofrece como "Continuar".
            if (IsFinishedAttempt(data))
            {
                SaveFinalAttempt(data);
                DeleteSave();

                return false;
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al comprobar la partida guardada. " +
                e.Message
            );

            return false;
        }
    }

    // ==========================================
    // ELIMINAR PARTIDA REANUDABLE
    // ==========================================

    public void DeleteSave()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log(
                "No existe partida reanudable para eliminar."
            );

            return;
        }

        try
        {
            File.Delete(savePath);

            Debug.Log(
                "PARTIDA REANUDABLE ELIMINADA."
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al eliminar la partida. " +
                e.Message
            );
        }
    }

    // ==========================================
    // CONSULTAR ÚLTIMO INTENTO FINALIZADO
    // ==========================================

    public bool HasFinalAttempt()
    {
        return File.Exists(finalAttemptPath);
    }

    public SaveData LoadFinalAttempt()
    {
        if (!File.Exists(finalAttemptPath))
        {
            Debug.Log(
                "No existe un intento finalizado guardado."
            );

            return null;
        }

        try
        {
            string json = File.ReadAllText(finalAttemptPath);

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al cargar el intento finalizado. " +
                e.Message
            );

            return null;
        }
    }

    // ==========================================
    // SABER SI UN INTENTO YA FINALIZÓ
    // ==========================================

    private bool IsFinishedAttempt(SaveData data)
    {
        if (data == null)
        {
            return false;
        }

        return
            data.caseCompleted ||
            data.elapsedTime >= CaseTimeLimitSeconds;
    }
}
