using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Here to control the Gun
/// </summary>
public class GunController : MonoBase
{
    private Transform weaponHold;
    //private Gun startingGun;
    private Gun equippedGun;

    protected override void OnAwake()
    {
        base.OnAwake();
        weaponHold = transform.FindChild("WeaponHold");
        //startingGun = ResourceManager.Instance.LoadResource<Gun>("Prefabs/Gun");
    } 

    protected override void OnStart()
    {
        base.OnStart();
        //if(startingGun != null)
        //{
        //    EquipGun(startingGun);
        //}
    }

    public void EquipGun(int weaponIndex)
    {
        Gun weapon = ResourceManager.Instance.LoadResource<Gun>("Prefabs/Weapons/Gun0" + weaponIndex);
        if(weapon != null)
        {
            EquipGun(weapon, "Gun0" + weaponIndex);
        }
    }

    public void EquipGun(Gun gunToEquip, string gunName)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.SetParent(weaponHold);
        equippedGun.gameObject.name = gunName;
    }

    public void OnTriggerHold()
    {
        if(equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }
}