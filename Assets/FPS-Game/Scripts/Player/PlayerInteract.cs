using PlayerAssets;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    PlayerAssetsInputs _playerAssetsInputs;
    PlayerInventory _playerInventory;
    Interactable _currentInteractableObj;
    float _playerReach;

    void Start()
    {
        _playerAssetsInputs = GetComponent<PlayerAssetsInputs>();
        _playerInventory = GetComponent<PlayerInventory>();

        _playerReach = 3f;
    }

    void Update()
    {
        CheckInteraction();

        if (_playerAssetsInputs.interact == true)
        {
            _playerAssetsInputs.interact = false;

            if (_currentInteractableObj != null)
            {
                Debug.Log("Interact with " + _currentInteractableObj.name);
                _playerInventory.RefillAmmos();
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
