﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBunker : Enemy
{
    private float currentTime;
    private float attackUpdate;
    public GameObject prefabExplode; //死亡爆炸预制件
    public AudioSource TankBunkerShootSE;
    public AudioSource[] BeAttackedByBulletSE;
    private Vector3 attackPos; //攻击目标
    public GameObject prefabBulletTankBunker;

    // Use this for initialization
    void Start()
    {
        currentTime = Time.time;
        attackUpdate = Time.time;
        init();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.time;
        this.target = GameObject.FindGameObjectWithTag("Player1");
        enemyAttack();
    }

    void init()
    {
        this.lifeValue = GameData.TankBunkerLifeValue;
        this.attackRate = GameData.TankBunkerAttackRate;
        this.target = GameObject.FindGameObjectWithTag("Player1");
    }

    public override void enemyAttack()
    {
        if (currentTime - attackUpdate > this.attackRate && GameObject.FindGameObjectWithTag("Player1") != null)
        {
            //Debug.Log("TankBunkerAttack");
            BunkerHeadRotate();
            TankBunkerShootSE.Play();

            attackPos = target.transform.position;
            Invoke("attackDetail", 0.1f);
            Invoke("attackDetail", 0.3f);
            Invoke("attackDetail", 0.5f);
            attackUpdate = Time.time;
        }
    }

    void attackDetail()
    {
        attackAnimation();
        var bulletPrefab = Instantiate(prefabBulletTankBunker, transform.position, Quaternion.Euler(Vector3.zero));
        bulletPrefab.GetComponent<Bullet>().Shotter = transform.gameObject; //通过脚本获取物体
        bulletPrefab.GetComponent<Bullet>().attackPos = this.attackPos;
    }

    public override void attackAnimation()
    {
        //炮塔开火摇摆
        //Debug.Log("TankBunkerAttackAnimation");

    }

    void BunkerHeadRotate() //炮塔转至目标
    {
        //方案1：三角函数（因为诸多缺点，已经被迭代）
        //参考：https://jingyan.baidu.com/album/73c3ce280bb9f4e50343d9d1.html?picindex=1
        //缺点：角度计算不精确、冗长

        /*
        var thisPos = transform.position;
        var targetPos = target.transform.position;

        var distance_x = thisPos.x - targetPos.x; //对边
        var distance = Vector2.Distance(thisPos, targetPos); //斜边

        var rotationZ = targetPos.y > 0 ? System.Math.Asin(distance_x / distance) / System.Math.PI * 180 + 180 : -System.Math.Asin(distance_x / distance) / System.Math.PI * 180;

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, (float)rotationZ);

        //Debug.Log("炮塔Quatertion: " + transform.rotation);
        */

        //方案2：
        //API来源：https://docs.unity3d.com/ScriptReference/Mathf.Atan2.html
        //社区来源：https://forum.unity.com/threads/2d-lookat.99708/ 最后一个回答

        Vector3 relative = transform.InverseTransformPoint(target.transform.position);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, 180 - angle);
    }

    void OnTriggerEnter2D(Collider2D other) //碰撞检测
    {
        switch (other.tag)
        {
            case "BulletMachinGun":
                {
                    //只针对角色的【机枪】进行损血处理：

                    lifeValue -= GameData.bulletCharacMachinGunDemage;

                    if (lifeValue <= 0)
                    {
                        DiedBomb();
                    }
                    else
                    {
                        //音效随机
                        var randValue = Random.Range(0, 3);
                        BeAttackedByBulletSE[randValue].Play();
                    }

                    break;
                }

            case "Grenade":
                {
                    DiedBomb();
                    break;
                }

            case "BulletCharacMissile":
                {
                    DiedBomb();
                    break;
                }

        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        switch (collisionInfo.gameObject.tag)
        {
            case "Player1":
                {
                    DiedBomb();
                    break;
                }
        }
    }

    void DiedBomb()
    {
        GameObject.Destroy(gameObject); //销毁Jackal
        Instantiate(prefabExplode, transform.position, Quaternion.Euler(Vector3.zero));
    }

}