using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public float turnDeplay = .1f;//Enemyの動作時間（0.1）
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100; //プレイヤーの体力
                                       //HideInInspector: public変数だけどInspectorで編集させない
                                       //プレイヤーの順番か判定
    [HideInInspector]
    public bool playersTurn = true;
    private int level = 3;
    private List<Enemy> enemies;//Enemyクラスの配列
    private bool enemiesMoving;//Enemyのターン中true

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        //Enemyを格納する配列の作成
        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }
    public void GameOver()
    {
        //GameManagerを無効にする
        enabled = false;
    }

    void Update()
    {
        //プレイヤーのターンがEnemyが動いた後ならUpdateしない
        if (playersTurn || enemiesMoving)
        {
            return;
        }

        
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDeplay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDeplay);
        }
        //Enemyの数だけEnemyスクリプトのMoveEnemyを実行
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}