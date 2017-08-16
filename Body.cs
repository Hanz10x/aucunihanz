using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour 
{
	//変数の宣言と初期化
	public bool MoveOK = false; //true=前進+キー入力受付での旋回をしても良い
	public bool xOK = false; //true=縦方向に旋回可能
	public float GoSpeed = 0f; //速度
	public float SpeedUpperLimit = 0f; //速度上限
	public float SpeedLowerLimit = 0f; //速度下限
	public float SpeedPlus = 0f; //加速
	public float SpeedMinus = 0f; //減速
	public float UDlimit = 0f; //上下角度上限(180を超える=上限なし) 80以上にすると挙動がおかしくなる
	public float Startx = 0f;
	public float Starty = 0f; 
	public float Startz = 0f; //初期位置
	public float ChTurnx = 0f;
	public float ChTurny = 0f; //軸ごとの旋回量変動変数を初期化
	public float PoTurnx = 0f;
	public float PoTurny = 0f; //軸ごとの旋回量変化後の予測を格納する変数を初期化
	public float RChange = 0f; //方向キーが押された時の角度の変化倍率
	public float SChange = 0f; //速度変化量
	public Vector3 BodyPlace = Vector3.zero; //現在の機体の座標を格納する変数BodyPlaceを初期化
	public Quaternion Turn = Quaternion.identity; //旋回倍率を格納する変数Turnを初期化
	
	// Use this for initialization
	void Start ()
	{
		//変数の値を設定
		MoveOK = true; //最初のみ駆動を許可　今後は他のスクリプトでこれを制御
		SpeedUpperLimit = 55.0f;
		SpeedLowerLimit = 0.0f;
		GoSpeed = 0.0f;
		UDlimit = 30f;
		Startx = 0f;
		Starty = 0f; 
		Startz = 0f;
		RChange = 50f;
		SChange = 0.01f;
		//初期位置を設定
		BodyPlace = new Vector3(Startx, Starty, Startz);
		this.transform.position = BodyPlace;
		//初期角度を設定
		Turn = Quaternion.Euler(0f, 0f, 0f);
		this.transform.rotation = Turn; 
		//速度が範囲外だったら下限値に直す
		if(GoSpeed > SpeedUpperLimit || SpeedLowerLimit > GoSpeed)
		{
			GoSpeed = SpeedLowerLimit; 
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	if(MoveOK)
	{
		//基本的な動き
		//落下
		//直進
		this.transform.position += transform.forward * Time.deltaTime * GoSpeed;
		//現在地点を取得
		Vector3 BodyPlace = this.transform.position;

		//現在の旋回量(本体角度のオイラー角度表記でのデータ)を取得 +角度は0～180,-角度は180～360であらわされる
		float Turnx = transform.rotation.eulerAngles.x;
		float Turny = transform.rotation.eulerAngles.y;
		float Turnz = transform.rotation.eulerAngles.z;
		//実際の角度が負の値であれば変数値もそうなるように調整
		if (Turnx > 180)
			{Turnx -= 360;}
		if (Turny > 180)
			{Turny -= 360;}
		if (Turnz > 180)
			{Turnz -= 360;}
		//Debug.Log("現在角度:"+Turnx+","+Turny+","+Turnz);

		//上下旋回+左右旋回
		//Quaternion Turn = Quaternion.identity; //旋回倍率を格納する変数Turnを初期化
		//旋回した後の結果を予測
		ChTurnx = Input.GetAxis("Vertical") * Time.fixedDeltaTime * RChange;
		ChTurny = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * RChange;
		PoTurnx = -ChTurnx + Turnx;
		PoTurny = ChTurny + Turny;
		//Debug.Log("予測結果:"+PoTurnx+","+PoTurny+","+0);
		//予測内容の垂直方向の角度が許容範囲内もしくはこれまでと反対方向のものなら角度を変更
		xOK = false;
		//if (-LRlimit < PoTurny && PoTurny < LRlimit && -UDlimit < PoTurnx && PoTurnx < UDlimit )
		if (PoTurnx >= 0) //PoTurnx >= 0 下向き
		{
			if (PoTurnx <= UDlimit) 
			{
				xOK = true;
			}
		}
		else //PoTurnx < 0 上向き
		{
			if (-UDlimit <= PoTurnx)
			{
				xOK = true;				
			}
		}
		if (xOK)
		{
			//角度の変更
			Turn.eulerAngles = new Vector3(-ChTurnx, ChTurny, 0);
			this.transform.rotation *= Turn;
		}
		//常時z軸角度だけは元に戻ろうとする
		if (Turnz != 0.0f)
		{
			Turnx = transform.eulerAngles.x;
			Turny = transform.eulerAngles.y;
			//Turnz = transform.eulerAngles.z;
			this.transform.rotation = Quaternion.Euler(Turnx, Turny, 0);
		}

		//加速
		if (Input.GetKey(KeyCode.W))
		{
			SpeedMinus = 0;
			//速度が上限未満なら加速を許可
			if (GoSpeed < SpeedUpperLimit)
			{
				SpeedPlus += SChange;
				GoSpeed += SpeedPlus;
			}
			//速度が上限超過していたら直す
			if (GoSpeed > SpeedUpperLimit)
			{
				GoSpeed = SpeedUpperLimit;
				SpeedPlus = 0;
			}
		}

		//減速
		if (Input.GetKey(KeyCode.S))
		{
			SpeedPlus = 0;
			//速度が下限を超えていたら減速を許可
			if (GoSpeed > SpeedLowerLimit)
			{
				SpeedMinus += SChange;
				GoSpeed -= SpeedMinus;
			}
			//速度が下限を下回っていたら直す
			if (GoSpeed < SpeedLowerLimit)
			{
				GoSpeed = SpeedLowerLimit;
				SpeedMinus = 0;
			}
		}		
	}
	}
}
