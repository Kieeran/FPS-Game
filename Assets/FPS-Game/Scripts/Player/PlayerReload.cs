using System;
using Unity.Netcode;

public class PlayerReload : NetworkBehaviour, IInitAwake
{
    public PlayerRoot PlayerRoot { get; private set; }

    bool _isReloading;
    public bool GetIsReloading() { return _isReloading; }
    public void ResetIsReloading() { _isReloading = false; }

    public event EventHandler OnReload;
    public Action reload;

    // Awake
    public int PriorityAwake => 1000;
    public void InitializeAwake()
    {
        PlayerRoot = GetComponent<PlayerRoot>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (PlayerRoot.PlayerAssetsInputs.reload == true)
        {
            reload?.Invoke();
            PlayerRoot.PlayerAssetsInputs.reload = false;

            if (_isReloading != true)
            {
                _isReloading = true;
                OnReload.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
