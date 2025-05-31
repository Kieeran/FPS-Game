using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class Melee : NetworkBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;
    [SerializeField] AudioSource knifeAudio;

    void Start()
    {
        knifeAudio.spatialBlend = 1f;
        knifeAudio.maxDistance = 100f;
    }

    public void SetAttackFalse()
    {
        _animator.SetBool("Attack", false);
        _animator.Play("Attack", 0, 0f);
    }

    void Update()
    {
        if (_playerAssetsInputs.shoot == true)
        {
            _playerAssetsInputs.shoot = false;

            if (_animator.GetBool("Attack") == false)
            {
                PlayKnifeAudio_ServerRpc(transform.position);
                _animator.SetBool("Attack", true);
            }
        }
    }

    public void PlayKnifeAudio(Vector3 position)
    {
        knifeAudio.transform.position = position;
        knifeAudio.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayKnifeAudio_ServerRpc(Vector3 position)
    {
        PlayKnifeAudio(position);
        PlayKnifeAudio_ClientRpc(position);
    }

    [ClientRpc]
    public void PlayKnifeAudio_ClientRpc(Vector3 position)
    {
        PlayKnifeAudio(position);
    }
}