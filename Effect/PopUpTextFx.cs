using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PopUpTextFx : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float speed;//�����ٶ�
    [SerializeField] private float disappearanceSpeed;//�ı���ʧʱ�����ٶȵ�˥��
    [SerializeField] private float colorDisappearanceSpeed;//�ı���ʧʱ��ɫ��˥��

    [SerializeField] private float lifeTime;

    private float textTimer;//��ʱ�ı�����������

    void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }


    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);// �ı���Y�᷽������


        textTimer -= Time.deltaTime;// ��ʱ���ݼ�

        if (textTimer < 0)
        {
            float alpha = myText.color.a - colorDisappearanceSpeed * Time.deltaTime;// �ı���͸�����𽥼���
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);


            if (myText.color.a < 50)// ��͸����С��50ʱ�������ı��������ٶ�
                speed = disappearanceSpeed;

            if (myText.color.a <= 0)
            {
                Destroy(gameObject); // �ݻٵ�ǰGameObject
            }
        }
    }
}