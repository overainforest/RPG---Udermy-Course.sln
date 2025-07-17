using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//2024.12.6
public class PlayerFX : EntityFX
{

    [Header("����Ч")]//Screen shake FX
    [SerializeField] private float shakeMultiplier;
    private CinemachineImpulseSource screenShake;
    public Vector3 shakeSwordImpact;//Ͷ����Ч������
    public Vector3 shakeHighDamage;//���˺���Ч������


    [Header("��Ӱ��Ч")]//After image FX
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;//��ɫ��ʧ��
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


    public void CreateAfterImage()//���ɲ�Ӱ
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;//������ȴʱ��

            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position + new Vector3(0, 0, 0), transform.rotation);//���ɲ�Ӱʵ��

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