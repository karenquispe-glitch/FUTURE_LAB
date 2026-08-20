using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ControlCinematicaAmbulancia : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string escenaSiguiente = "Hospital_Base";

    private bool cargandoEscena;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += AlFinalizarVideo;
            videoPlayer.errorReceived += AlOcurrirError;
        }
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= AlFinalizarVideo;
            videoPlayer.errorReceived -= AlOcurrirError;
        }
    }

    private void AlFinalizarVideo(VideoPlayer reproductor)
    {
        CargarEscenaSiguiente();
    }

    private void AlOcurrirError(VideoPlayer reproductor, string mensaje)
    {
        Debug.LogWarning("Error al reproducir la cinemática: " + mensaje);
        CargarEscenaSiguiente();
    }

    public void OmitirCinematica()
    {
        CargarEscenaSiguiente();
    }

    private void CargarEscenaSiguiente()
    {
        if (cargandoEscena)
        {
            return;
        }

        cargandoEscena = true;
        SceneManager.LoadScene(escenaSiguiente);
    }

}