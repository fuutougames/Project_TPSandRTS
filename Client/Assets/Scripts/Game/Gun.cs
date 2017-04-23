using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The weapon base in the game 
/// </summary>
public class Gun : MonoBase
{
    public enum FireMode { Auto, Burst, Single }
    public FireMode fireMode;

    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = 3;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMovSettleTime = .1f;
    public float recoilRotSettleTime = .1f;

    //Effects
    private Transform[] projectTileSpawner;
    private Projectile projectile;
    private Shell shellPrefab;
    private Transform shellEjection;
    private MuzzleFlash muzzleflash;
    private float newShotTime;
    [Header("Effects")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    private bool triggerReleasedSinceLastShot;
    private int shotsRemainingInBurst;
    private int projectilsRemainingInMag;
    private bool isReloading;

    private Vector3 recoilSmoothDamVelocity;
    private float recoilRotSmoothDamVelocity;
    private float recoilAngle;

    protected override void OnAwake()
    {
        base.OnAwake();
        projectTileSpawner = transform.FindChild("Muzzle").GetComponentsInChildren<Transform>();
        shellEjection = transform.FindChild("ShellEjectionPoint");
        muzzleflash = transform.GetComponent<MuzzleFlash>();
        projectile = ResourceManager.Instance.LoadResource<Projectile>("Prefabs/Bullet");
        shellPrefab = ResourceManager.Instance.LoadResource<Shell>("Prefabs/Shell");
        shotsRemainingInBurst = burstCount;
        projectilsRemainingInMag = projectilesPerMag;
    }

    protected override void OnStart()
    {
        base.OnStart();
        //print(this.gameObject.name);
        //shootAudio = ResourceManager.Instance.LoadResource<AudioClip>("Audios/Guns/" + this.gameObject.name + "_shoot");
        //reloadAudio = ResourceManager.Instance.LoadResource<AudioClip>("Audios/Guns/" + this.gameObject.name + "_reload");
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        // animation recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDamVelocity, recoilMovSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDamVelocity, recoilRotSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && projectilsRemainingInMag == 0)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (isReloading) return;
        if(Time.time > newShotTime && projectilsRemainingInMag > 0)
        {
            if(fireMode == FireMode.Burst)
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single)
            {
                if(!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for(int i = 0; i < projectTileSpawner.Length; i++)
            {
                if (projectilsRemainingInMag == 0) break;
                projectilsRemainingInMag--;
                newShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectTileSpawner[i].position, projectTileSpawner[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shellPrefab, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.Instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload()
    {
        if(!isReloading && projectilsRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.Instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;

        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
            yield return null;
        }

        isReloading = false;
        projectilsRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (isReloading) return;
        transform.LookAt(aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
