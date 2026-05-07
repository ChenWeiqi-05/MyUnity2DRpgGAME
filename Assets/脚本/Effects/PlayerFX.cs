using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [SerializeField] private ParticleSystem dustFX;
    [Header("After Image FX")]
    [SerializeField] private float afterImagecooldown;
    [SerializeField] private GameObject afterImagePerfab;
    [SerializeField] private float colorLooseRate;
    private float afterImageCooldownTimer;

    [Header("Screen shake FX")]
    private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;



    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }
  
    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }
    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImagecooldown;
            GameObject newAfterImage = Instantiate(afterImagePerfab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }

    }
    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }
    public void PlayDustFX()
    {
        if (dustFX != null)
            dustFX.Play();
    }

}
