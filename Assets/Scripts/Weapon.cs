using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    [Header("Shooting")]
    // 射击
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDely = 0.2f;

    [Header("Burst")]
    //连发模式属性
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]  
    // 枪械灵敏度属性
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("Bullet")]
    //子弹属性
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f; //子弹的速度
    public float bulletPrefabLifeTime = 3f; //子弹发射后的寿命

    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Loading")]
    // Loading
    public float reloadTime; //武器换弹时间
    public int magazineSize, bulletsLeft; //弹匣容量, 剩余子弹
    public bool isReloding; // 检查是否换弹

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    private bool isADS;

    public enum WeaponModel
    {
        Pistol1911,
        M16
    }

    public WeaponModel thisWeaponModel;
   
    public enum ShootingMode //射击模式
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;
    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;

    }

    void Update()
    {

        if (isActiveWeapon)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }

            GetComponent<Outline>().enabled = false;

            // 弹匣空的时候发出的声音
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }
            if (currentShootingMode == ShootingMode.Auto)
            {
                // 连发(按住鼠标设计)
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single ||
                    currentShootingMode == ShootingMode.Burst)
            {
                // 单发(单击鼠标)
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloding == false && WeaponManager.Instance.CheckAmmoLeftfor(thisWeaponModel) > 0)
            {
                Reload();
            }

            // 自动换弹
            if (bulletsLeft <= 0 && isReloding == false && isShooting == false && readyToShoot)
            {
                // Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            
        }

    }

   
    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUNDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }
    private void ExitADS()
    {

        animator.SetTrigger("exitADS");
        isADS = false;
        HUNDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }

    private void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        // 在ADS状态后不播放后坐力动画
        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");  
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }
        

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // 实例化子弹
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        //瞄准子弹以面对射击方向
        bullet.transform.forward = shootingDirection;

        // 发射子弹
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // 子弹销毁
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // 检查是否射击完毕
        if (allowReset)
        {
            Invoke("ResetShot", shootingDely);
            allowReset = false;
        }

        // 检查是否在连射模式
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDely);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadingSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");


        isReloding = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    void ReloadCompleted()
    {
        //if (WeaponManager.Instance.CheckAmmoLeftfor(thisWeaponModel) > magazineSize)
        //{
        //    bulletsLeft = magazineSize;
        //    WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        //}
        //else
        //{
        //    bulletsLeft = WeaponManager.Instance.CheckAmmoLeftfor(thisWeaponModel);
        //    WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        //}

        int ammoLeft = WeaponManager.Instance.CheckAmmoLeftfor(thisWeaponModel);
        int bulletsNeeded = magazineSize - bulletsLeft;

        int bulletsToReload = Mathf.Min(bulletsNeeded, ammoLeft);

        bulletsLeft += bulletsToReload;
        WeaponManager.Instance.DecreaseTotalAmmo(bulletsToReload, thisWeaponModel);


        isReloding = false;

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // 从屏幕中间射击，以检查我们指向哪里
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // 击中某个物品
            targetPoint = hit.point;
        }
        else
        {
            // 往空中射击
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //返回拍摄方向和扩散
        return direction + new Vector3(0, y, z);

    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
