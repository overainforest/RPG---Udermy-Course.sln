using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void SetUp(int _damage,CharacterStats _targetStats)
    {
        this.damage = _damage;
        this.targetStats = _targetStats;
    }

    private void Update()
    {
        if (!targetStats)
            return;

        if (triggered)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right= transform.position - targetStats.transform.position;

        if(Vector2.Distance(transform.position,targetStats.transform.position)<.1f)
        {
            anim.transform.localPosition = new Vector3(0, .5f);
            anim.transform.localRotation=Quaternion.identity;

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);



            Invoke("DamageAndSelDestroy", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
        }
    }

    private void DamageAndSelDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(1);
        Destroy(gameObject, .4f);
    }
}
