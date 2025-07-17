using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_SkillController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate=true;
    private bool isReturning;


    private float freezeTimeDuration;
    private float returnSpeed=12;

    [Header("Pierce info")]
    private float pierceAmount;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake()
    {

        anim = GetComponentInChildren<Animator>();
        rb= GetComponent<Rigidbody2D>();    
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir,float _gravityScale,Player _player,float _freezeTimeDuration,float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity= _dir;
        rb.gravityScale = _gravityScale;

        if(pierceAmount <= 0)
        anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    public void SetupBounce(bool _isBouncing,int _amountOfBouncs,float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBouncs;
        bounceSpeed = _bounceSpeed; 
        enemyTarget = new List<Transform>();    

    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning,float _maxTraveDistance,float _spinDuration,float _hitCoolDown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTraveDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCoolDown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;
        transform .parent=null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards
            (
                transform.position, player.transform.position, returnSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        BounceLogic();

        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x+spinDirection, transform.position.y), 1.5f * Time.deltaTime);//如果Spin不想剑穿过敌人，可删除这段代码

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            if (hit.GetComponent<Enemy>() != null)
                                SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }

        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) <  .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                //enemyTarget[targetIndex].GetComponent<Enemy>().DamageEffect();
                //enemyTarget[targetIndex].GetComponent<Enemy>().StartCoroutine("FreezeTimerFor", freezeTimeDuration);

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if (collision.GetComponent<Enemy>()!= null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }
        //collision.GetComponent<Enemy>()?.Damage();

        SetupTargetForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();//获取敌人的状态
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>());

        if (player.skill.sword.timeStopUnlocked)
            enemy.FreezeTimerFor(freezeTimeDuration);

        if (player.skill.sword.volnurableUnlocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);//敌人易伤

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);//获取护身符

        if (equipedAmulet != null)
            equipedAmulet.Effect(enemy.transform);//如果有护身符，就对敌人造成额外伤害
    }

    private void SetupTargetForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }
            
        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponentInChildren<ParticleSystem>().Play();// 播放粒子效果

        if (isBouncing && enemyTarget.Count>0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
