using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{

    protected virtual void OnTriggerEnter2D(Collider2D collision)//��ײ����
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

            EnemyStats ememyTarget = collision.GetComponent<EnemyStats>();//��ȡ��ײ�����˵�����

            playerStats.DoMagicalDamage(ememyTarget);//���ħ���˺�
        }
    }
}