using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;//�Ӳ�ϵ��

    private float xPosition;
    private float length;

    private void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    private void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);//����Ӳ�����
        float distanceToMave = cam.transform.position.x * parallaxEffect;//����Ӳ�
        //��parallaxEffectΪ1���򱳾�����������ƶ�����С��1���򱳾������������
        transform.position = new Vector3(xPosition + distanceToMave, transform.position.y);

        if (distanceMoved > xPosition + length)//�Ӳ�����һ��������������ѳ�����������������λ�ø��£��ң�
            xPosition = xPosition + length * 2;
        else if (distanceMoved < xPosition - length)//�Ӳ�����һ�������������������λ�ø��£���
            xPosition = xPosition - length * 2;
    }
}
