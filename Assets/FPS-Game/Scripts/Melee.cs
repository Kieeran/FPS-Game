using PlayerAssets;
using UnityEngine;

public class Melee : Weapon
{
    [SerializeField] Animator _animator;
    [SerializeField] PlayerAssetsInputs _playerAssetsInputs;

    public void SetAttackFalse()
    {
        _animator.SetBool("Attack", false);
        _animator.Play("Attack", 0, 0f);
    }

    private void Update()
    {
        if (_playerAssetsInputs.shoot == true)
        {
            _playerAssetsInputs.shoot = false;

            if (_animator.GetBool("Attack") == false)
            {
                _animator.SetBool("Attack", true);
            }
        }
    }
}