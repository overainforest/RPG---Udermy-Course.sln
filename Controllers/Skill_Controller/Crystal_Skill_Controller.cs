using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim=>GetComponent<Animator>();
    private CircleCollider2D cd=>GetComponent<CircleCollider2D>();
    private Player player;

    private float crystalExistTimer;

    private bool canExpolde;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    [SerializeField] private float growSpeed=5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget,Player _player)
    {
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExpolde = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;
        if(crystalExistTimer<0)
        {
            FinishCrystal();
        }

        if(canMove)
        {
            if (closestTarget == null)
                return;

            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 0.5f)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if(canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());

                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);//获取护身符

                if (equipedAmulet != null)
                    equipedAmulet.Effect(hit.transform);//如果有护身符，就对敌人造成额外伤害
            }
               

        }
    }

    public void FinishCrystal()
    {
        if (canExpolde)
        {
            canGrow = true;
            anim.SetTrigger("Explode");     
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy() => Destroy(gameObject);
}
