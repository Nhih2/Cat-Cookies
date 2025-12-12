using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScan : MonoBehaviour
{
    private const float SCAN_INTERVAL = 0.1f;

    [HideInInspector] public Task targetTask { get; private set; }
    [HideInInspector] public bool needUpdate = false;

    [SerializeField] private Transform arrow;
    [SerializeField] private float interactRadius = 0.3f;
    [Header("x = sideway (+ right) / y = forward (+)")]
    [SerializeField] private Vector2 interactOffset = new Vector2(0, 0.5f);

    private LayerMask interactableMask;
    private float interval = 0;
    private bool isInteractable = false;

    void Awake()
    {
        interval = SCAN_INTERVAL;
        interactableMask = LayerMask.GetMask(GameSetting.INTERACTABLE_LAYER);
    }

    void Update()
    {
        HandleScan();
        HandleAvailibilityChange();
    }

    private void HandleAvailibilityChange()
    {
        if (!targetTask) return;
        if (isInteractable != targetTask.CheckAvailibility())
        {
            isInteractable = targetTask.CheckAvailibility();
            needUpdate = true;
        }
    }

    private void HandleScan()
    {
        interval -= Time.deltaTime;
        if (interval <= 0) Scan();
    }

    private void Scan()
    {
        List<Collider2D> hits = Physics2D.OverlapCircleAll(GetInteractPoint(), interactRadius, interactableMask).ToList();
        if (hits.Count == 0)
        {
            if (targetTask)
            {
                targetTask = null;
                needUpdate = true;
            }
            return;
        }

        Vector3 center = GetInteractPoint();
        Collider2D closest = new();
        while (hits.Count > 0)
        {
            closest = hits
                .OrderBy(h => Vector2.Distance(center, h.transform.position))
                .First();
            if (closest.GetComponent<Task>().CheckAvailibility()) break;
            hits.Remove(closest);
        }
        if (hits.Count == 0)
        {
            targetTask = null;
            needUpdate = true;
            return;
        }
        Task obj = closest.GetComponent<Task>();
        if (!obj) return;
        if (obj == targetTask) return;

        targetTask = obj;
        isInteractable = targetTask.CheckAvailibility();
        needUpdate = true;
    }

    private void OnDrawGizmos()
    {
        if (!arrow) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetInteractPoint(), interactRadius);
    }

    private Vector3 GetInteractPoint()
    {
        return arrow.TransformPoint(interactOffset);
    }
}
