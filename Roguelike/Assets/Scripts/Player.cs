using UnityEngine;
using System.Collections;
//MovingObjectクラスを継承する
public class Player : MovingObject {
	
	public int wallDamage = 1; //壁へのダメージ量
	public int pointsPerFood = 10; //フードの回復量
	public int pointsPerSoda = 20; //ソーダの回復量
	public float restartlevelDelay = 1f; //次レベルへ行く時の時間差
	
	private Animator animator; //PlayerChop, PlayerHit用
	private int food; //プレイヤーの体力

	//MovingObjectのStartメソッドを継承　baseで呼び出し
	protected override void Start () {
		//Animatorをキャッシュしておく
		animator = GetComponent<Animator>();
		//シングルトンであるGameManagerのplayerFoodPointsを使うことに
		//よって、レベルを跨いでも値を保持しておける
		food = GameManager.instance.playerFoodPoints;
		//MovingObjectのStartメソッド呼び出し
		base.Start();
	}
	//Playerスクリプトが無効になる前に、体力をGameManagerへ保存
	//UnityのAPIメソッド(Unityに標準で用意された機能)
	private void OnDisable ()
	{
		GameManager.instance.playerFoodPoints = food;
	}

	void Update ()
	{
		//プレイヤーの順番じゃない時Updateは実行しない
		if (!GameManager.instance.playersTurn)
			return;
		
		int horizontal = 0; //-1: 左移動, 1: 右移動
		int vertical = 0; //-1: 下移動, 1: 上移動
		
		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");
		//上下もしくは左右に移動を制限
		if (horizontal != 0) {
			vertical = 0;
		}
		//上下左右どれかに移動する時
		if (horizontal != 0 || vertical != 0) {
			//Wall: ジェネリックパラメーター<T>に渡す型引数
			//Playerの場合はWall以外判定する必要はない
			AttemptMove<Wall>(horizontal, vertical);
		}
	}
	
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		//移動1回につき1ポイント失う
		food--;
		//MovingObjectのAttemptMove呼び出し
		base.AttemptMove <T> (xDir, yDir);
		
		RaycastHit2D hit;
		
		CheckIfGameOver();
		//プレイヤーの順番終了
		GameManager.instance.playersTurn = false;
	}
	
	//MovingObjectの抽象メソッドのため必ず必要
	protected override void OnCantMove <T> (T component)
	{
		//Wall型を定義 Wallスクリプトを表す
		Wall hitWall = component as Wall;
		//WallスクリプトのDamageWallメソッド呼び出し
		hitWall.DamageWall(wallDamage);
		//Wallに攻撃するアニメーションを実行
		animator.SetTrigger("PlayerChop");
	}
	
	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Exit") {
			//Invoke: 引数分遅れてメソッドを実行する
			Invoke ("Restart", restartlevelDelay);
			enabled = false; //Playerを無効にする
		} else if (other.tag == "Food") {
			//体力を回復しotherオブジェクトを削除
			food += pointsPerFood;
			other.gameObject.SetActive(false);
		} else if (other.tag == "Soda") {
			//体力を回復しotherオブジェクトを削除
			food += pointsPerSoda;
			other.gameObject.SetActive(false);
		}
	}
	
	private void Restart ()
	{
		//同じシーンを読み込む
		Application.LoadLevel(Application.loadedLevel);
	}
	//敵キャラがプレイヤーを攻撃した時のメソッド
	public void LoseFood (int loss)
	{
		animator.SetTrigger("PlayerHit");
		food -= loss;
		CheckIfGameOver();
	}
	
	private void CheckIfGameOver ()
	{
		if (food <= 0) {
			//GameManagerのGameOverメソッド実行
			//public staticな変数なのでこのような簡単な形でメソッドを呼び出せる
			GameManager.instance.GameOver();
		}
	}
}