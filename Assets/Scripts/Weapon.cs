using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    // ���
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDely = 0.2f;

    //����ģʽ����
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // ǹе����������
    public float spreadIntensity;

    //�ӵ�����
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f; //�ӵ����ٶ�
    public float bulletPrefabLifeTime = 3f; //�ӵ�����������

    public GameObject muzzleEffect;
    internal Animator animator;

    // loading
    public float reloadTime; //��������ʱ��
    public int magazineSize, bulletsLeft; //��ϻ����, ʣ���ӵ�
    public bool isReloding; // ����Ƿ񻻵�

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;


    public enum WeaponModel
    {
        Pistol1911,
        M16
    }

    public WeaponModel thisWeaponModel;
   
    public enum ShootingMode //���ģʽ
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
    }

    void Update()
    {

        

        if (isActiveWeapon)
        {

            GetComponent<Outline>().enabled = false;

            // ��ϻ�յ�ʱ�򷢳�������
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }
            if (currentShootingMode == ShootingMode.Auto)
            {
                // ����(��ס������)
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single ||
                    currentShootingMode == ShootingMode.Burst)
            {
                // ����(�������)
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloding == false)
            {
                Reload();
            }

            // �Զ�����
            if (bulletsLeft <= 0 && isReloding == false && isShooting == false && readyToShoot)
            {
                Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (AmmoManager.Instance.ammoDisplay != null)
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
            } 
        }

    }

    private void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // ʵ�����ӵ�
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        //��׼�ӵ�������������
        bullet.transform.forward = shootingDirection;

        // �����ӵ�
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // �ӵ�����
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // ����Ƿ�������
        if (allowReset)
        {
            Invoke("ResetShot", shootingDely);
            allowReset = false;
        }

        // ����Ƿ�������ģʽ
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
        bulletsLeft = magazineSize;
        isReloding = false;

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // ����Ļ�м�������Լ������ָ������
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // ����ĳ����Ʒ
            targetPoint = hit.point;
        }
        else
        {
            // ���������
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //�������㷽�����ɢ
        return direction + new Vector3(x, y, 0);

    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
