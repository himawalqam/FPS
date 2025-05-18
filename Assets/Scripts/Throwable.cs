using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f; 
    [SerializeField] float damageRadius = 20f; 
    [SerializeField] float explosionForce = 1200f;

    float countdown;

    public bool hasExploded = false; // 已爆炸
    public bool hasBeenThrown = false; // 已投掷

    public enum ThrowableType
    {
        Green
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if(countdown < 0f && !hasExploded)
            {
                Exploded();
                hasExploded = true;
            }
        }
    }

    private void Exploded()
    {
        GetThrowableEffect();
        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
       switch(throwableType)
        {
            case ThrowableType.Green:
                GrenadeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        // 视觉效果
        GameObject explosionEffect = GobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // 物理效果
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach(Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }


        }
    }
}
