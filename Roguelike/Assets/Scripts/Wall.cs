using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    public AudioClip chopSound1;
    public AudioClip chopSound2;

    public Sprite dmgSprite;
    public int hp = 3;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        //SpriteRendererをキャッシュしておく
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //プレイヤーが内壁を攻撃した時に実行されるメソッド
    //PlayerクラスのOnCantMoveから呼び出し
    public void DamageWall(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

        //public変数で指定しておいた画像を表示
        spriteRenderer.sprite = dmgSprite;

        //体力を引数分だけ減らす
        hp -= loss;

        //体力が0以下になった時
        if (hp <= 0)
        {
            //内壁を無効にする
            gameObject.SetActive(false);
        }
    }
}
