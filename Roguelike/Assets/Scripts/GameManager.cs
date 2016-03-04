using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public float levelstartDelay = 2f;
    public float turnDeplay = .1f;//Enemyの動作時間（0.1）
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100; //プレイヤーの体力
                                       //HideInInspector: public変数だけどInspectorで編集させない
                                       //プレイヤーの順番か判定
    [HideInInspector]
    public bool playersTurn = true;
    private Text levelText; //レベルテキスト
    private GameObject levelImage;//レベルイメージ
    private int level = 1;//レベルは１
    private bool doingSetup;//levelImageの表示等で活躍
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

    //UnityのAPIで、Sceneが呼ばれる度に実行されるメッソド
    public void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }

    void InitGame()
    {
        //trueの間、プレイヤーは身動きとれない
        doingSetup = true;
        //LevelImageオブジェクト・LevelTextオブジェクトの所得
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day" + level;//最新のレベルに更新
        levelImage.SetActive(true);//LebelImageをアクティブにし表示
        Invoke("HideLevelImage", levelstartDelay);//2秒後にメソッド呼び出し
       
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);//LevelImage非アクティブ化
        doingSetup = false;//プレイヤーが動けるようになる
    }

    public void GameOver()
    {
        //ゲームオーバーメッセージを表示
        levelText.text = "After" + level + "days, you starved.";
        levelImage.SetActive(true);
        //GameManagerを無効にする
        enabled = false;
    }

    void Update()
    {
        //プレイヤーのターンかEnemyが動いた後ならUpdateしない
        //doingSetup=trueの時はEnemyを動かさない
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
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