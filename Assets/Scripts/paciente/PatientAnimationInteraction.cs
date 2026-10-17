using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PatientAnimationInteraction : MonoBehaviour
{
    [SerializeField] private Animator patientAnimator;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 8f;

    private static readonly int Reaccionar =
        Animator.StringToHash("Reaccionar");

    private void Awake()
    {
        if (patientAnimator == null)
            patientAnimator = GetComponentInChildren<Animator>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit[] hits =
            Physics.RaycastAll(ray, interactionDistance);

        Array.Sort(hits, (a, b) =>
            a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
            {
                patientAnimator.ResetTrigger(Reaccionar);
                patientAnimator.SetTrigger(Reaccionar);
                return;
            }
        }
    }
}