using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApp1
{
    class Ball
    {
        private int diam;//ボールの直径
        private int x, y;//X座標、Y座標
        private int vx, vy;//ボールの速度
        private int pre_x, pre_y;//１つ前のX座標、Y座標
        private Bitmap canvas;
        private Brush color;
        private bool clicked;//クリック判定フラグ
        static public int ballcount = -1;//金ボールの総数、見本用に１つ多く生成するため-1からスタート
        static Random rnd = new System.Random();//乱数生成オブジェクト

        //コンストラクタ
        public Ball(int diam, int x, int y, int v, Bitmap canvas, Brush color)
        {
            this.x = x;
            this.y = y;
            this.vx = v;
            this.vy = v;
            this.diam = diam;
            this.color = color;
            this.canvas = canvas;
            clicked = false;
            if (color == Brushes.Gold) { ++ballcount; }//金のボールを生成したらカウントアップ
        }
        //パラメータ初期化
        public void Ini(int diam, int x, int y, int v, Bitmap canvas, Brush color)
        {
            this.x = x;
            this.y = y;
            this.vx = v;
            this.vy = v;
            this.diam = diam;
            this.color = color;
            this.canvas = canvas;
            clicked = false;
            if (color == Brushes.Gold) { ++ballcount; }//金のボールを生成したらカウントアップ
        }
        //ボールを描画
        public Bitmap DrawCircle()
        {
            if (!clicked)//まだクリックされてなければ
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.FillEllipse(color, x, y, diam, diam);//ボールを描画
                }
                pre_x = x;
                pre_y = y;
            }
            return canvas;
        }
        //ボールを消去
        public void EraseCircle()
        {
            if (!clicked)//まだクリックされていなければ
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.FillEllipse(Brushes.White, pre_x, pre_y, diam, diam);//白で塗ってボールを消去
                }
            }
        }
        //ボール移動
        public Bitmap Move()
        {
            x = x + vx;//座標更新
            y = y + vy;//座標更新
            ColBox();//ゲーム画面４辺の衝突処理
            EraseCircle();//過去座標のボールを消去
            return DrawCircle();//現在座標にボールを描画
        }
        //ゲーム画面４辺の衝突処理
        public void ColBox()
        {
            if (x >= canvas.Width - diam)//右辺の当たり判定
            {
                x = canvas.Width - diam - 1;//ｘ座標を右辺から直径分差し引いた値に設定
                vx *= -1;//速度反転
            }
            if (x <= 0)//左辺の当たり判定
            {
                x = 0;
                vx *= -1;
            }
            if (y >= canvas.Height - diam)//下辺の当たり判定
            {
                y = canvas.Height - diam - 1;//y座標を下辺から直径分差し引いた値に設定
                vy *= -1;
            }
            if (y <= 0)//上辺の当たり判定
            {
                y = 0;
                vy *= -1;
            }
        }
        //ボール同士の衝突処理
        public void ColBall(Ball otherball)
        {
            int[] self_entity;//自身の実体領域
            int[] other_entity;//相手の実体領域
            bool[] col = { false, false, false, false };
            self_entity = Entity(); //自身の実体を四角形とし、座標値をtop,under,left,rightの順に格納
            other_entity = otherball.Entity();//相手の実体を四角形とし、座標値をtop,under,left,rightの順に格納
            if (!clicked && !otherball.clicked)//自身も相手もまだクリックされていなければ
            {
                //上辺の当たり判定
                col[0] = self_entity[0] > other_entity[0] && self_entity[0] < other_entity[1];
                //下辺の当たり判定
                col[1] = self_entity[1] < other_entity[1] && self_entity[1] > other_entity[0];
                //左辺の当たり判定
                col[2] = self_entity[2] > other_entity[2] && self_entity[2] < other_entity[3];
                //右辺の当たり判定
                col[3] = self_entity[3] < other_entity[3] && self_entity[3] > other_entity[2];
                if ((col[0] || col[1]) && (col[2] || col[3]))//x方向とｙ方向どちらも衝突していたらボールが衝突
                {
                    ColisionX(otherball);//X方向の衝突処理
                    ColisionY(otherball);//Y方向の衝突処理
                }
            }
        }
        //自身の実体を四角形とし、座標値をtop,under,left,rightの順に格納
        private int[] Entity()
        {
            int[] entity;
            entity = new int[4];
            int pad;
            pad = diam / 10;           
            entity[0] = y + pad; //top            
            entity[1] = y + diam - pad;//under            
            entity[2] = x + pad;//left            
            entity[3] = x + diam - pad;//right
            return entity;
        }
        //衝突時の処理（Y軸）
        private void ColisionY(Ball otherball)
        {
            int pad;
            pad = 0;
            if (vy * otherball.vy < 0)//逆向きに進んでいた時に衝突したら
            {
                vy *= -1;
                otherball.vy *= -1;
                y = y + pad / 2 * vy;//速度とパッドの割合だけずらす
                otherball.y = otherball.y + pad * otherball.vy / 2 * otherball.vy;//速度とパッドの割合だけずらす
            }
            else
            {
                if (vy >= otherball.vy)//同じ向きに進んでいて、かつ、自身の速度の方が速い
                {
                    vy *= -1;
                    y = y + pad / 2 * vy;
                    otherball.y = otherball.y + pad / 2 * otherball.vy;//速度とパッドの割合だけずらす
                }
                else//同じ向きに進んでいて、かつ、自身の速度の方が遅い
                {
                    otherball.vy *= -1;
                    y = y + pad / 2 * vy;
                    otherball.y = otherball.y + pad / 2 * otherball.vy;//速度とパッドの割合だけずらす
                }
            }
        }
        //衝突時の処理（X軸)
        private void ColisionX(Ball otherball)
        {
            int pad;
            pad = 10;
            if (vx * otherball.vx < 0)//逆向きに進んでいた時に衝突したら
            {
                vx *= -1;
                otherball.vx *= -1;
                x = x + pad / 2 * vx;//速度とパッドの割合だけずらす
                otherball.x = otherball.x + pad / 2 * otherball.vx;//速度とパッドの割合だけずらす
            }
            else
            {
                if (vx >= otherball.vx)//同じ向きに進んでいて、かつ、自身の速度の方が速い
                {
                    vx *= -1;
                    x = x + diam / 2 * vx;
                    otherball.x = otherball.x + pad / 2 * otherball.vx;//速度とパッドの割合だけずらす
                }
                else//同じ向きに進んでいて、かつ、自身の速度の方が遅い
                {
                    otherball.vx *= -1;
                    x = x + diam / 2 * vx;
                    otherball.x = otherball.x + pad / 2 * otherball.vx;//速度とパッドの割合だけずらす
                }
            }
        }
        //クリック判定
        public void Check_Clicked(int x, int y)
        {
            int[] entity = Entity();//自身の実体を四角形とし、座標値をtop,under,left,rightの順に格納
            if (!clicked)//クリックされていなければ
            {
                if (entity[0] < y && entity[1] > y && entity[2] < x && entity[3] > x)//クリック座標が実体の内側にあれば
                {
                    EraseCircle();//ボールを消去
                    clicked = true;
                    if (color == Brushes.Gold) { --ballcount; }//金のボールであればカウントダウン
                }
            }
        }
    }
}
