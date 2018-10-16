using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormBallGame : Form
    {
        private Ball[] balls;//ボールインスタンスの配列を格納
        private Ball selectball;//セレクトpictureboxに描画するボール
        private Brush[] colors = {Brushes.Green, Brushes.Black, Brushes.Goldenrod, Brushes.Pink, Brushes.Red,Brushes.Gold };//ボールの色の種類
        Bitmap selectCanvas;
        Bitmap mainCanvas;
        int num_gold = 5;//金のボールの数
        int num = 30;//ボールの数
        int num_colors = 5;//カラーの総数（ゴールドを除く)
        int index_gold = 5;//colors配列におけるゴールドのindex
        int ballsize_min = 15, ballseize_max = 50;//作成するボールサイズの範囲
        int v_min= -10, v_max = 10;//作成するボール速度の範囲
        bool game_flag = false;//ゲームフラグ、ゲーム中はtrue
        Random rnd = new System.Random();//乱数生成オブジェクト

        //コンストラクタ
        public FormBallGame()    
        {
            InitializeComponent();
        }       
        //ゲームの準備
        private void FormBallGame_Load(object sender, EventArgs e)
        {
            balls = new Ball[num];        
            selectCanvas =  new Bitmap(selectPictureBox.Width, selectPictureBox.Height);
            mainCanvas =   new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            selectball = new Ball(selectPictureBox.Height, 0,0,0, selectCanvas, Brushes.Gold);//selectpictureboxに描画するボールを生成
            selectPictureBox.Image = selectball.DrawCircle();//selectpictureboxにボールを描画
            make_Main_Balls(0, num_gold, num_colors);//mainpictureboxに描画するボール（金）を生成
            make_Main_Balls(num_gold, num, 0);//mainpictureboxに描画するボールを生成
            label2.Text = Ball.ballcount.ToString();//金のボールの数を表示
        }
        //ゲームスタート
        private void restartButton_Click(object sender, EventArgs e)
        {
            if (game_flag == false)//ゲーム中でなければ
            {
                timer1.Start();//タイマー起動
                restartButton.Text = "STOP";
                game_flag = true;
            }
            else//ゲーム中であれば
            {
                reset_Game();//ゲーム停止、リセット
            }
        }
        //ゲーム終了、リセット  
        private void reset_Game()
        {
            int i;
            Ball.ballcount = 0;
            for (i = 0; i < num; ++i)//ボールの数だけループ
            {
                balls[i].EraseCircle();//ボール消去
            }
            mainCanvas.Dispose();
            mainCanvas = new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            ini_Main_Balls(0, num_gold, num_colors);//金のボール初期化
            ini_Main_Balls(num_gold, num, 0);//ボール初期化
            timer1.Stop();//タイマーストップ
            restartButton.Text = "START";
            game_flag = false;
            label2.Text = Ball.ballcount.ToString();//金のボールの数を表示
        }
        //クリックの当たり判定とゲーム終了処理
        private void mainPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            int i;
            if (game_flag)//ゲーム中であれば
            {
                for (i = 0; i < num; ++i)//ボールの数だけループ
                {
                    balls[i].Check_Clicked(e.X, e.Y);//クリックの当たり判定
                }
                label2.Text = Ball.ballcount.ToString();//金のボールの数を表示
                if(Ball.ballcount <= 0)//金のボール数が０になれば
                {
                    MessageBox.Show("ゲームクリア");//ゲームクリア
                    reset_Game();//ゲーム終了、リセット
                }
            }
        }
        //タイマー作動時10msecごとに呼び出される、ボールの移動、衝突処理
        private void timer1_Tick(object sender, EventArgs e)
        {
            int i,m;
            //全てのボール同士で衝突処理
            for (i = 0; i < num; ++i)//ボールの数だけループ
            {
                for (m = i; m < num; ++m)//相手のボールの数だけループ
                {
                    balls[i].ColBall(balls[m]);//衝突処理
                }
                mainPictureBox.Image = balls[i].Move();//ボール移動
            }
        }
        //ボール生成
        private void make_Main_Balls(int start_index,int end_index,int colornumber)
        {
            int x, y, v, diam;
            Brush color;
            int i;
            for (i = start_index ; i < end_index; ++i)
            {
                diam = rnd.Next(ballsize_min, ballseize_max);//ボールの直径をランダムに設定
                x = rnd.Next(0, mainPictureBox.Width);       // ボールの初期位置をランダムに配置
                if (x > mainPictureBox.Width - diam) { x = mainPictureBox.Width - diam; }//ゲーム画面外であればゲーム画面内に配置
                y = rnd.Next(0, mainPictureBox.Height);      //ボールの初期位置をランダムに配置
                if (y > mainPictureBox.Height - diam) { y = mainPictureBox.Height - diam; }//ゲーム画面外であればゲーム画面内に配置
                v = rnd.Next(v_min, v_max);//ボールの速度をランダムに設定
                color = colors[rnd.Next(num_colors)];//ボールの色をランダムに設定
                if (colornumber == index_gold) { color = colors[num_colors]; }//ゴールドナンバーだったら色をゴールに設定
                balls[i] = new Ball(diam, x, y, v, mainCanvas, color);//ボール生成
                mainPictureBox.Image = balls[i].DrawCircle();//ボール描画
            }
        }
        //ボール初期化、ボール生成とほぼ同じ処理、今あるボールを再設定
        private void ini_Main_Balls(int start_index,int end_index,int colornumber)
        {
            int x, y, v, diam;
            Brush color;
            int i;
            for (i = start_index; i < end_index; ++i)
            {
                diam = rnd.Next(ballsize_min, ballseize_max);
                x = rnd.Next(0, mainPictureBox.Width);         
                if (x > mainPictureBox.Width - diam) { x = mainPictureBox.Width - diam; }
                y = rnd.Next(0, mainPictureBox.Height);        
                if (y > mainPictureBox.Height - diam) { y = mainPictureBox.Height - diam; }
                v = rnd.Next(v_min, v_max);
                color = colors[rnd.Next(num_colors)];
                if (colornumber == index_gold) { color = colors[num_colors]; }
                balls[i].Ini(diam, x, y, v, mainCanvas, color);
                mainPictureBox.Image = balls[i].DrawCircle();
            }
        }
        
    }

    

}
