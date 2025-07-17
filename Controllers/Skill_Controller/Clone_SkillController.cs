using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_SkillController : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLosingSpeed;

    private float cloneTimer;
    private float attackMulitiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = 10;
    private int facingDir = 1;

    private bool canDuplicateClone;
    private float chanceToDuplicate;

    [Space]
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float closestEnemyCheckRaduis = 25;
    [SerializeField] private Transform closestEnemy;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        StartCoroutine(FacingClosestarget());
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer<0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }
    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack,Vector3 _offset,bool _canDuplicateClone,float _chanceToDuplicate,Player _player,float _attackMultiplier)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        attackMulitiplier = _attackMultiplier;
        player = _player;
        transform.position= _newTransform.position+_offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();//获取玩家的stats
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>(); //获取敌人的stats

                playerStats.CloneDoDamage(enemyStats, attackMulitiplier);//对敌人造成伤害

                if (player.skill.clone.canApplyOnHitEffect)//如果可以应用击中效果
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)//应用武器特殊效果
                        weaponData.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if(Random.Range(0,100)<chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f*facingDir, 0));
                    }
                }
            }
                
        }

    }

    private IEnumerator FacingClosestarget()
    {
        yield return null;

        FindClosesEnemy();

        if (closestEnemy != null)
        {
           if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
    private void FindClosesEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, closestEnemyCheckRaduis,whatIsEnemy);

        float closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
        {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
        }
    }
}
