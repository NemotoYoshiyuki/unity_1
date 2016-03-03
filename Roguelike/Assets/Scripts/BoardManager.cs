using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    //カウント用のクラスを設定
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    //8*8のゲームボードを作るので縦の段を8、横の列を8
    public int columns = 8;
    public int rows = 8;
    //壁は５～９の間で出現
    public Count wallCount = new Count(5, 9);
    //アイテムは１～５の間で出現
    public Count foodCount = new Count(1, 5);
    //Exitは単体
    public GameObject exit;
    //床・内壁・アイテム・敵キャラ・外壁は複数あるため配列
    public GameObject[] floorTiles;
    public GameObject[] waillTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    //オブジェクトの位置情報を保存する変数
    private Transform boardHolder;
    //オブジェクトの配置できる範囲を表すリスト
    //Listは可変型の配列
    private List<Vector3> gridPosition = new List<Vector3>();

    //敵キャラ・アイテム・内壁・を配置できる範囲を決定
    void initialsetList()
    {
        //gridPositionをクリア
        gridPosition.Clear();
        //gridPositionにオブジェクト配置可能範囲を指定
        //x =1～６をループ
        for (int x = 1; x < columns - 1; x++)
        {
            //y = １～６をループ
            for (int y = 0; y < rows - 1; y++)
            {
                //6*6の範囲をgridPositionsに指定
                gridPosition.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //外壁、床を配置
    void BoardSetup ()
	{
		//Boardというオブジェクトを作成し、transform情報をboardHolderに保存
		boardHolder = new GameObject ("Board").transform;
		//x = -1〜8をループ
		for (int x = -1; x < columns + 1; x++) {
			//y = -1〜8をループ
			for (int y = -1; y < rows + 1; y++) {
				//床をランダムで選択
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				//左端or右端or最低部or最上部の時＝外壁を作る時
				if (x == -1 || x == columns || y == -1 || y == rows) {
					//floorTileの時と同じように外壁をランダムで選択し、上書きする
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}
				//床or外壁を生成し、instance変数に格納
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f),
					Quaternion.identity) as GameObject;
				//生成したinstanceをBoardオブジェクトの子オブジェクトとする
				instance.transform.SetParent(boardHolder);
			}
		}
	}

    Vector3 RandomPosition()
    {
        //０～３６からランダムで一つ決定し、位置情報を確定
        int randamIndex = Random.Range(0, gridPosition.Count);
        Vector3 randomPosition = gridPosition[randamIndex];
        //ランダムで決定した数値は削除
        gridPosition.RemoveAt(randamIndex);
        //確定した位置情報を返す
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //最低値〜最大値+1のランダム回数分だけループ
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            //gridPositionから位置情報を１つ取得
            Vector3 randomPosition = RandomPosition();
            //引数tileArrayからランダムで1つ選択
            GameObject tileChoise = tileArray[Random.Range(0, tileArray.Length)];
            //ランダムで決定した種類・位置でオブジェクトを生成
            Instantiate(tileChoise, randomPosition, Quaternion.identity);
        }
    }

    public void Start()
    {

    }
    //オブジェクトを配置していくメソッド
    //このクラス内唯一のpublicメッソド床を生成するタイミングでGameManagerから呼ばれる
    public void SetupScene(int level)
    {
        //床と外壁を配置し、
        BoardSetup();
        //敵キャラ・内壁・アイテムを配置できる位置を決定し、
        initialsetList();
        //内壁・アイテム・敵キャラをランダムで配置し
        LayoutObjectAtRandom(waillTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        //Mathf.Log　： 対数で計算。level=2なら4、level=3なら８
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        //Exitを7, 7の位置に配置する。
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);
    }
}