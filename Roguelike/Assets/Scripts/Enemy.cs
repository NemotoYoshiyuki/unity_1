using UnityEngine;
using System.Collections;
using System;

public class Enemy : MovingObject
{

    public int playerDamage;//プレイヤーへのダメージ量

    private Animator animator;
    private Transform target;//プレイヤーの位置情報
    private bool skipMove;//敵キャラが動くかどうかの判定

    //攻撃用の効果音を指定
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    //MovingObjectのStartメソッドを継承
    protected override void Start()
    {
        //GameManagerスクリプトのEnemyの配列に格納
        GameManager.instance.AddEnemyToList(this);
        //Animatorをキャッシュしておく
        animator = GetComponent<Animator>();
        //Playerの位置情報を所得
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //MovingObjectのStartメソッド呼び出し
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        //移動が終了したらtrueにする
        skipMove = true;
    }

    //敵キャラ移動用メソッド　GameManagerから呼ばれる
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        //同じカラム(x軸)にいる時
        //Mathf.Abs :絶対値をとる。-1なら1となる。
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //プレイヤーが上にいれば＋１、下にいればー１する
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            //プレイヤーが右にいれば＋１、左にいればー１する
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        //ジェネリック機能　攻撃対象はPlayerのみなので、型引数はPlayer
        AttemptMove<Player>(xDir, yDir);
    }

    //MovingObjectの抽象メソッドのため必ず必要
    protected override void OnCantMove<T>(T component)
    {
        //Playerクラスを取得
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");//攻撃アニメーションの実行
        //PlayerクラスのLoseFoodメソッドを呼び出す　引数はダメージ量
        hitPlayer.LoseFood(playerDamage);
        //攻撃用効果音をSoundManagerに渡し、ランダムで再生
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
    
}
