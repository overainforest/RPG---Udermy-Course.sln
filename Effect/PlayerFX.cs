using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//2024.12.6
public class PlayerFX : EntityFX
{

    [Header("震动特效")]//Screen shake FX
    [SerializeField] private float shakeMultiplier;
    private CinemachineImpulseSource screenShake;
    public Vector3 shakeSwordImpact;//投掷震动效果向量
    public Vector3 shakeHighDamage;//高伤害震动效果向量


    [Header("残影特效")]//After image FX
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;//颜色丢失率
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;

    [Space]
    [SerializeField] private ParticleSystem dustFx;

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }


    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }


    public void CreateAfterImage()//生成残影
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;//重置冷却时间

            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position + new Vector3(0, 0, 0), transform.rotation);//生成残影实例

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
        if (dustFx != null)
            dustFx.Play();
    }
}