using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour 
{
	//カメラのモードを決める為のスイッチ変数を宣言する
	public int CameraMode = 0; //0=俯瞰,1=主観
	//読み込んだデータを格納計算するための変数を宣言する
	public Vector3 RotationA = Vector3.zero; //機体の前回の角度を格納する変数
	public Vector3 RotationB = Vector3.zero; //機体の現在の角度を格納する変数
	public Vector3 RotationC = Vector3.zero; //機体の角度の変化量を格納する変数
	public Vector3 RotationD = Vector3.zero; //何らかの角度を格納しておく一時的な変数
	public Vector3 CameraPosition = Vector3.zero; //カメラの移動後の座標を格納する変数
	public Vector3 CameraRotation = Vector3.zero; //カメラの角度を格納する変数
	public Vector3 BodyPosition = Vector3.zero; //機体の現在の位置を格納する変数
    public Vector3 DistanceToBody = Vector3.zero; //機体との相対座標を格納する変数
	public float CameraPx = 0; //カメラ初期位置x
	public float CameraPy = 0; //カメラ初期位置y
	public float CameraPz = 0; //カメラ初期位置z
	public float CameraRx = 0; //カメラ初期角度x
	public float CameraRy = 0; //カメラ初期角度y
	public float BodyGoSpeed = 0.0f; //機体の速度データを読み込む変数
	public float xAdjust = 0.0f; //機体のx角を調整して格納する変数
	public float xTheta = 0.0f; //機体のx角を三角関数的に保存するための変数

	//Bodyに該当するオブジェクトとその特定コンポーネントへ関連付ける為の枠を作る。実際にはどちらも同じオブジェクトが入る
	public Transform BodyObjectTransform; //Transformは座標などを司るデフォルトのコンポーネント名。変更不可
	public Body BodyObjectMove; //Bodyは移動入力などを司るスクリプトのコンポーネント名(「Body.cs」の名前部分)。変更可

	// Use this for initialization
	void Start () 
	{
		CameraMode = 0;
		//カメラが対象を俯瞰できる初期位置と角度を格納する
		CameraPx = 0f;
		CameraPy = 3f;
		CameraPz = -5f;
		CameraRx = 20f;
		CameraRy = 0f;
		//起動時に機体の角度を取得しておく
		RotationA = BodyObjectTransform.rotation.eulerAngles;
		if (RotationA.x > 180)
			RotationA.x -= 360;
		if (RotationA.y > 180)
			RotationA.y -= 360;
		if (RotationA.z > 180)
			RotationA.z -= 360;
		//起動時に機体の座標を取得しておく
		BodyPosition = BodyObjectTransform.position;
		//カメラの位置を機体と同一にする
		GetComponent<Transform>().position = BodyPosition;

		Setup(RotationA);
		//決定された角度にする
		GetComponent<Transform>().rotation = Quaternion.Euler(CameraRotation);				
		//決定された位置にカメラを置く
		GetComponent<Transform>().position = CameraPosition;
	}

	void Setup(Vector3 RotationD) //引数として現在の機体の角度を扱う
	{
		//Debug.Log("現在機体角度;"+RotationD);
		//俯瞰時のカメラの角度を決定する
		CameraRotation = new Vector3(CameraRx, CameraRy, 0f); //絶対的な角度設定を取得
		CameraRotation.y += RotationD.y;
		Debug.Log("機体角度;"+RotationD+", カメラ角度;"+CameraRotation);

		//俯瞰時のカメラの位置を決定する
		/* カメラ位置に関するメモ
		機体のx角が0(北)を向いていて(3,4,5)にいた場合、カメラは(3+(0*1), 4+3, 5+(-5*1))にいてy角0を向いている必要がある
		機体のx角が90(東)を向いていて(3,4,5)にいた場合、カメラは(3+(-5*1), 4+3, 5+(0*-1))にいてy角90を向いている必要がある
		機体のx角が180(南)を向いていて(3,4,5)にいた場合、カメラは(3+(0*-1), 4+3, 5+(-5*-1))にいてy角180を向いている必要がある
		機体のx角が-90(西)を向いていて(3,4,5)にいた場合、カメラは(3+(-5*-1), 4+3, 5+(0*1))にいてy角-90を向いている必要がある

		機体のx角が0(北)を向いていて(0,0,0)にいた場合、カメラは(0+(0*1), 0+3, 0+(-5*1))にいてy角0を向いている必要がある
		機体のx角が90(東)を向いていて(0,0,0)にいた場合、カメラは(0+(-5*1), 0+3, 0+(0*-1))にいてy角90を向いている必要がある
		機体のx角が180(南)を向いていて(0,0,0)にいた場合、カメラは(0+(0*-1), 0+3, 0+(-5*-1))にいてy角180を向いている必要がある
		機体のx角が-90(西)を向いていて(0,0,0)にいた場合、カメラは(0+(-5*-1), 0+3, 0+(0*1))にいてy角-90を向いている必要がある

		機体x角が0; カメラxは相対xの値が正の値で乗算され相対zは0、zは相対zの値が正の値で乗算され相対xは0になる
		機体x角が90; カメラxは相対zの値が正の値で乗算され相対xは0、zは相対xの値が負の値で乗算され相対zは0になる
		機体x角が180; カメラxは相対xの値が負の値で乗算され相対zは0、zは相対zの値が負の値で乗算され相対xは0になる
		機体x角が270; カメラxは相対zの値が負の値で乗算され相対xは0、zは相対xの値が正の値で乗算され相対zは0になる
		機体x角をθとしたとき、カメラxは「機体座標x + cos(θ-90)*相対z + sin(θ-90)*相対x」,カメラzは「機体座標z - cos(θ-90)*相対x - sin(θ-90)*相対z」で求める

		Unityの三角関数の角度の単位はラジアンなので、機体のx角をラジアンに変換する 円周率はMathf.PIを用いる
		調整後角度0 = ラジアン0 ; Cos0π = 1, Sin0π = 0
		調整後角度1 = ラジアンπ/180 ;
		調整後角度90 = ラジアンπ/2 ; Cosπ/2 = 0, Sinπ/2 = 1
		調整後角度180 = ラジアンπ ; Cosπ = -1, Sinπ = 0
		調整後角度270 = ラジアン-π/2 ; Cos-π/2 = 0, Sin-π/2 = -1
		調整後角度θ = ラジアンπθ/180;
		*/
		
		//機体のy角を-179.9～180.0の範囲から0.0～359.9の範囲に直しつつ東向きを0度に規定
		xAdjust = RotationD.y + 270f;
		if (xAdjust >= 360f)
			{xAdjust -= 360f;} 
		xTheta = Mathf.Abs(xAdjust) * Mathf.PI / 180;
		//Debug.Log("調整後機体x角;"+xAdjust+", xTheta=ラジアン換算;"+xTheta+", Cos(xTheta);"+Mathf.Cos(xTheta)+", Sin(xTheta);"+Mathf.Sin(xTheta));

		//俯瞰時の自分自身とBodyとの相対距離を決定する
		DistanceToBody = new Vector3(1, CameraPy, 1); //相対的な機体との距離設定を取得 ここではy値のみ取得
		DistanceToBody.x = Mathf.Cos(xTheta) * CameraPz + Mathf.Sin(xTheta) * CameraPx;
		DistanceToBody.z = -1 * Mathf.Cos(xTheta) * CameraPx + -1 * Mathf.Sin(xTheta) * CameraPz;
		//Debug.Log("相対カメラ位置;"+DistanceToBody);
		
		//両者を組み合わせて最終的な座標を算出
		CameraPosition = BodyPosition + DistanceToBody; 
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Bodyの現在の角度を取得する
		RotationB = BodyObjectTransform.rotation.eulerAngles;
		if (RotationB.x > 180)
			{RotationB.x -= 360;}
		if (RotationB.y > 180)
			{RotationB.y -= 360;}
		if (RotationB.z > 180)
			{RotationB.z -= 360;}
		//Debug.Log("現在機体角度;"+RotationB);

		//Bodyの現在の座標を取得する
		BodyPosition = BodyObjectTransform.position;
		//Debug.Log("カメラ現在位置"+GetComponent<Transform>().position);

		//カメラの角度と座標を決める
		switch (CameraMode)
		{
		case 0: //俯瞰

			Setup(RotationB);
			//Setup()で決定されたカメラの角度と座標へカメラを移動させる

			//カメラの角度を決定
			RotationC = RotationA - RotationB; //Bodyの前回の角度と現在の角度の差を求める
			//Debug.Log("機体角度変化量;"+RotationC);
			//Bodyの角度変化に合わせてCameraの角度を調整 Bodyはxの値が-で上方へ、yの値が-で左へ行く
			CameraRotation.x += RotationC.x * -0.25f;

			//決定された角度にする
			GetComponent<Transform>().rotation = Quaternion.Euler(CameraRotation);
			//Debug.Log("カメラ角度"+CameraRotation);

			//決定された位置にカメラを置く
			GetComponent<Transform>().position = CameraPosition;
			//Debug.Log("カメラ最終位置"+CameraPosition);			
			break;

		case 1: //主観
			//Bodyの現在の角度に合わせてCameraの角度を変更
			CameraRotation.x = RotationB.x * 1;
			CameraRotation.y = RotationB.y * 1;
			CameraRotation.z = RotationB.z * 1;
			//カメラの角度を決定
			GetComponent<Transform>().rotation = Quaternion.Euler(CameraRotation);
			
			//カメラの位置を決定する
			CameraPosition.x = BodyPosition.x + CameraPx;
			CameraPosition.y = BodyPosition.y;
			CameraPosition.z = BodyPosition.z;
			//決定された位置にカメラを置く
			GetComponent<Transform>().position = CameraPosition;
			break;

		default: //上記以外
			CameraMode = 0;
			Setup(RotationB);
			
			//決定された角度にする
			GetComponent<Transform>().rotation = Quaternion.Euler(CameraRotation);
			//決定された位置にカメラを置く
			GetComponent<Transform>().position = CameraPosition;
			break; //Unityではswitch文でbreakしかジャンプ文が使えない？
		}
		
		//次回に持ち越すため今回の機体の角度を格納
		RotationA = RotationB;
	}

}