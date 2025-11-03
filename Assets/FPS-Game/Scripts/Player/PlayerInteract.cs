using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : PlayerBehaviour
{
    Interactable _currentInteractableObj;
    float _playerReach;

    // Start
    public override void InitializeStart()
    {
        base.InitializeStart();

        _playerReach = 3f;
    }

    void Update()
    {
        if (!IsOwner) return;

        CheckInteraction();

        if (PlayerRoot.PlayerAssetsInputs.interact == true)
        {
            PlayerRoot.PlayerAssetsInputs.interact = false;

            if (_currentInteractableObj != null)
            {
                Debug.Log("Interact with " + _currentInteractableObj.name);
                PlayerRoot.PlayerInventory.RefillAmmos();
            }

        }
    }

    void CheckInteraction()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _playerReach))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable newInteractableObj = hit.collider.GetComponent<Interactable>();

                if (_currentInteractableObj != null)
                {
                    if (newInteractableObj == _currentInteractableObj) return;

                    _currentInteractableObj.DisableOutline();
                }

                SetNewCurrentInteractable(newInteractableObj);
            }

            else
            {
                DisableCurrentInteractable();
            }
        }

        else
        {
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable interactable)
    {
        _currentInteractableObj = interactable;
        _currentInteractableObj.EnableOutline();
    }

    void DisableCurrentInteractable()
    {
        if (_currentInteractableObj != null)
        {
            _currentInteractableObj.DisableOutline();
            _currentInteractableObj = null;
        }
    }
}
