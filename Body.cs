using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour 
{
	//変数の宣言
	public float GoSpeed = 1.0f; //現在速度
	public float LRlimit = 180.0f; //左右角度上限
	public float UDlimit = 90.0f; //上下角度上限
	public float SpeedPlus = 0.0f; //加速
	public float SpeedMinus = 0.0f; //減速
	public float SpeedLimit = 50.0f; //速度上限
	//public	float UseTime = 0.0f; //処理時間

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		//キー入力判定の初期化
		public bool NoKey = true;

		//基本的な動き
		//落下
		//直進
		this.transform.Translate(Vector3.forward * GoSpeed * 1 * Time.deltaTime);

		//旋回量変動値を初期化
		public float ChTurnx = 0.0f;
		public float ChTurny = 0.0f;
		public float ChTurnz = 0.0f;
		//現在の旋回量(本体のオイラー角度)を取得 +角度は0～180,-角度は180～360であらわされる
		public float Turnx = transform.eulerAngles.x;
		public float Turny = transform.eulerAngles.y;
		public float Turnz = transform.eulerAngles.z;
		//実際の角度が負の値であれば変数値もそうなるように調整
		if (Turnx > 180)
			Turnx -= 360;
		if (Turny > 180)
			Turny -= 360;
		if (Turnz > 180)
			Turnz -= 360;
		//Debug.Log("角度:"+Turnx+","+Turny+","+Turnz);
		//現在地点を取得
		Vector3 bodyplace = this.transform.position;		
		//右旋回
		if (Input.GetKey(KeyCode.RightArrow))
		{
			NoKey = false;
			if (Turny < LRlimit)
			{
				Vector3 axis = Vector3.up; //new Vector3(0.0f, 1.0f, 0.0f);
				ChTurnx = 2.0f;
				this.transform.RotateAround(bodyplace, axis, ChTurnx);
			}
		}
		//左旋回
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			NoKey = false;
			if (Turny > -LRlimit)
			{
				Vector3 axis = Vector3.up; //new Vector3(0.0f, 1.0f, 0.0f);
				ChTurnx = -2.0f;
				this.transform.RotateAround(bodyplace, axis, ChTurnx);
			}
		}
		//上旋回
		if (Input.GetKey(KeyCode.UpArrow))
		{
			NoKey = false;
			if (Turnx > -UDlimit)
			{
				Vector3 axis = Vector3.right; //new Vector3(1.0f, 0.0f, 0.0f);
				ChTurny = -2.0f;
				this.transform.RotateAround(bodyplace, axis, ChTurny);
			}
		}
		//下旋回
		if (Input.GetKey(KeyCode.DownArrow))
		{
			NoKey = false;
			if (Turnx < UDlimit)
			{
				Vector3 axis = Vector3.right; //new Vector3(1.0f, 0.0f, 0.0f);
				ChTurny = 2.0f;
				this.transform.RotateAround(bodyplace, axis, ChTurny);
			}
		}
		//キーが押されなければ元に戻ろうとする
		if (NoKey)
		{
/*			if (Turnx != 0.0f)
			{
				Vector3 axis = Vector3.right; //new Vector3(1.0f, 0.0f, 0.0f);
				ChTurny = Mathf.Sign(Turnx) * Mathf.Abs(Turnx) * 0.05f;
				this.transform.RotateAround(bodyplace, axis, ChTurny);
			}
*/
			if (Turnz != 0.0f)
			{
				/*
				Turnx = transform.eulerAngles.x;
				Turny = transform.eulerAngles.y;
				Turnz = transform.eulerAngles.z;
				ChTurnz = Turnz + Mathf.Sign(Turnz) * 0.1f;
				Vector3 NowT = new Vector3(Turnx, Turny, Turnz);
				Vector3 PostT = new Vector3(Turnx, Turny, ChTurnz);
				Quaternion.Euler QTurnz = Quaternion.Euler(NowT);
				Quaternion.Euler QChTurnz = Quaternion.Euler(PostT);
				if (UseTime < 1)
				{
					UseTime += Time.deltaTime;
					this.transform.rotation = Quaternion.Slerp(QTurnz, QChTurnz, UseTime);
				}
				*/
				Turnx = transform.eulerAngles.x;
				Turny = transform.eulerAngles.y;
				transform.rotation = Quaternion.Euler(Turnx, Turny, 0);
			}
			else
			{
				UseTime = 0.0f;
			}
		}

		//加速
		if (Input.GetKey(KeyCode.W))
		{
			SpeedPlus += 1;
			SpeedMinus = 0;
			GoSpeed += SpeedPlus;
			if (GoSpeed > SpeedLimit)
			{
				GoSpeed = SpeedLimit;
			}
		}

		//減速
		if (Input.GetKey(KeyCode.S))
		{
			SpeedMinus += 1;
			SpeedPlus = 0;
			GoSpeed -= SpeedMinus;
			if (GoSpeed < 0.0f)
			{
				GoSpeed = 0.0f;
			}
		}		
	}
}
