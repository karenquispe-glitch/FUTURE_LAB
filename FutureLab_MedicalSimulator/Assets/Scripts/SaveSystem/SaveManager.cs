using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string savePath;

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

        savePath = Path.Combine(
            Application.persistentDataPath,
            "futurelab_save.json"
        );

        Debug.Log("SaveManager iniciado.");
        Debug.Log("Ruta de guardado: " + savePath);
    }

    // ==========================================
    // GUARDAR PARTIDA
    // ==========================================

    public void SaveGame(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("SaveManager: no se puede guardar una partida nula.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(savePath, json);

            Debug.Log("PARTIDA GUARDADA CORRECTAMENTE.");
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al guardar la partida. " + e.Message
            );
        }
    }

    // ==========================================
    // CARGAR PARTIDA
    // ==========================================

    public SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No existe ninguna partida guardada.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(savePath);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("PARTIDA CARGADA CORRECTAMENTE.");

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al cargar la partida. " + e.Message
            );

            return null;
        }
    }

    // ==========================================
    // COMPROBAR SI EXISTE UNA PARTIDA
    // ==========================================

    public bool HasSavedGame()
    {
        return File.Exists(savePath);
    }

    // ==========================================
    // ELIMINAR PARTIDA
    // ==========================================

    public void DeleteSave()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No existe partida para eliminar.");
            return;
        }

        try
        {
            File.Delete(savePath);

            Debug.Log("PARTIDA GUARDADA ELIMINADA.");
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "SaveManager: error al eliminar la partida. " + e.Message
            );
        }
    }
}