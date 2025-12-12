using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class EventCameraController : MonoBehaviour
{
    public static EventCameraController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("VCams")]
    public CinemachineCamera normalCam;
    public CinemachineCamera eventCam;

    [Header("Target Group")]
    public CinemachineTargetGroup targetGroup;

    [Header("Player")]
    public Transform player;

    [Header("Ortho Zoom Settings")]
    public float padding = 1f;
    public float smoothSpeed = 5f;

    private Camera cam;

    private HashSet<Transform> activeEventObjects = new HashSet<Transform>();

    void Start()
    {
        cam = Camera.main;

        InitializeTargetGroup();
    }

    private void InitializeTargetGroup()
    {
        if (targetGroup.Targets == null)
            targetGroup.Targets = new List<CinemachineTargetGroup.Target>();

        targetGroup.Targets.Clear();

        targetGroup.Targets.Add(new CinemachineTargetGroup.Target
        {
            Object = player,
            Weight = 1f,
            Radius = 0.5f
        });
    }

    void LateUpdate()
    {
        if (eventCam.Priority <= normalCam.Priority || activeEventObjects.Count == 0)
            return;

        UpdateTargetGroup();
        UpdateOrthoSize();
    }

    private void UpdateTargetGroup()
    {
        targetGroup.Targets.RemoveRange(1, targetGroup.Targets.Count - 1);

        foreach (var obj in activeEventObjects)
        {
            targetGroup.Targets.Add(new CinemachineTargetGroup.Target
            {
                Object = obj,
                Weight = 1f,
                Radius = 0.5f
            });
        }
    }

    private void UpdateOrthoSize()
    {
        Vector3 min = player.position;
        Vector3 max = player.position;

        foreach (var obj in activeEventObjects)
        {
            min = Vector3.Min(min, obj.position);
            max = Vector3.Max(max, obj.position);
        }

        Vector3 size = (max - min) / 2f + Vector3.one * padding;
        float orthoSize = Mathf.Max(size.y, size.x / cam.aspect);

        eventCam.Lens.OrthographicSize = Mathf.Lerp(
            eventCam.Lens.OrthographicSize,
            orthoSize,
            Time.deltaTime * smoothSpeed
        );
    }

    public void StartEvent(Transform eventTarget, bool resetPrevious = false)
    {
        if (eventTarget == null) return;

        if (resetPrevious)
            ResetEvent();

        activeEventObjects.Add(eventTarget);

        eventCam.Priority = 20;
        normalCam.Priority = 10;
    }

    public void EndEvent(Transform eventTarget)
    {
        if (eventTarget == null) return;

        activeEventObjects.Remove(eventTarget);

        if (activeEventObjects.Count == 0)
        {
            eventCam.Priority = 10;
            normalCam.Priority = 20;
        }
    }

    public void ResetEvent()
    {
        activeEventObjects.Clear();
        InitializeTargetGroup();

        eventCam.Priority = 10;
        normalCam.Priority = 20;

        eventCam.Lens.OrthographicSize = normalCam.Lens.OrthographicSize;
    }
}
