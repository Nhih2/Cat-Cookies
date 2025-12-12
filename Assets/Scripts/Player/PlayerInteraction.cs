using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [HideInInspector] public Task targetTask;
    [SerializeField] private KeyCode interactButton = KeyCode.E;

    private float tapTime = 0;

    void Update()
    {
        HandleInput();
        HandleTimer();
    }

    private bool isDisable = false;
    public void DisableMovement()
    {
        isDisable = true;
    }
    public void EnableMovement()
    {
        isDisable = false;
    }

    private void HandleTimer()
    {
        if (isDisable) return;
        if (Input.GetKey(interactButton)) tapTime += Time.deltaTime;
    }

    private void HandleInput()
    {
        if (isDisable) return;
        if (!targetTask) return;
        if (Input.GetKeyDown(interactButton)) tapTime = 0;
        if (Input.GetKey(interactButton))
        {
            targetTask.Interact(tapTime);
        }
    }
}
