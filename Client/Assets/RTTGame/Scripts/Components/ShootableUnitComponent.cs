using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableUnitComponent : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = this.transform.GetComponentInChildren<Animator>();
    }

    public IEnumerator Shoot()
    {
        _animator.SetBool("Shoot_b", true);
        yield return new WaitForSeconds(0.5f / 1.5f);
        _animator.SetBool("Shoot_b", false);
    }
}
