using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{

    protected virtual void OnTriggerEnter2D(Collider2D collision)//碰撞处理
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

            EnemyStats ememyTarget = collision.GetComponent<EnemyStats>();//获取碰撞到敌人的数据

            playerStats.DoMagicalDamage(ememyTarget);//造成魔法伤害
        }
    }
}