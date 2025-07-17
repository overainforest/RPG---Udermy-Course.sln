using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    //素材是精灵、原材料和替换材料
    protected SpriteRenderer sr;
    protected Player player;

    [Header("弹出文本")]//Pop Up Text
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailemnt color")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("攻击特效")]//Hit FX
    [SerializeField] private GameObject hitFX;
    [SerializeField] private GameObject criticalHitFx;

    protected virtual void Start()
    {
        
        sr = GetComponentInChildren<SpriteRenderer>();//精灵是从组件上获取的
        player = PlayerManager.instance.player;
        
        originalMat = sr.material;  //原材料是精灵上的材料

    }

    public void CreatePopUpText(string _text)//生成弹出文本
    {
        float randomx = Random.Range(-0.5f, 0.5f);
        float randomy = Random.Range(1.5f, 3);

        Vector3 positionOffset = new Vector3(randomx, randomy, 0);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;//原材料被改变
        Color currrentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);//改变的时间是0.2s

        sr.color=currrentColor;
        sr.material = originalMat;//然后切换成原材料
    }

    public void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white ;

        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }

    public void IgniteFxFor(float _seconds)
    {
        igniteFx.Play();
        InvokeRepeating("IgniteColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }   

    public void ChillFxFor(float _seconds)
    {
        chillFx.Play();
        InvokeRepeating("ChillColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        shockFx.Play();
        InvokeRepeating("ShockColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }    

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void CreateHitFX(Transform _target, bool _critical)//攻击特效
    {

        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFXRotation = new Vector3(0, 0, zRotation);//生成的旋转向量

        GameObject hitPrefab = hitFX;

        if (_critical)//如果暴击
        {
            hitPrefab = criticalHitFx;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)//如果玩家朝向左边
                yRotation = 180;

            hitFXRotation = new Vector3(0, yRotation, zRotation);

        }

        GameObject newHitFX = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity);//,_target);//生成特效实例

        newHitFX.transform.Rotate(hitFXRotation);//设置旋转角度

        Destroy(newHitFX, .5f);

    }
}
