using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace WPF贪吃蛇游戏设计
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        List<SnakePart> snakeParts = new List<SnakePart>();
        //定义网格颜色
        Boolean isOdd = false;
        //定义网格大小
        int squareSize = 40;
        //蛇头和蛇身的颜色
        SolidColorBrush snakeHeadColor = Brushes.Red;
        SolidColorBrush snakeBodyColor = Brushes.Green;
        //初始位置
        Point startPos = new Point(120, 120);
        //初始长度
        int snakeLength = 3;
        //初始方向
        SnakeDirection snakeDirection = SnakeDirection.right;
        SnakeDirection snakeDirectionPre= SnakeDirection.right;
        //定时器
        DispatcherTimer dispatcher = new DispatcherTimer();
        //

        double foodPosX;
        double foodPosY;
        //
        Ellipse food = null;
        //
        int score = 0;
        //
        Boolean isGameRuning = false;
        //高分榜
        List<HighScore> highScoreList = new List<HighScore>();
        //xml文件路径
        string xmlName = "HighScoreList.xml";
        //
        int speed = 500;



        public MainWindow()
        {
            InitializeComponent();
            dispatcher.Tick += Dispatcher_Tick;
        }



        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DrawGameArea();
            if (File.Exists(xmlName) == false)
            {
                CreateXMLFile();
            }
            else
            {
                StreamReader read = new StreamReader(xmlName);
                ReadXML(read);
            }


        }

        private void Dispatcher_Tick(object sender, EventArgs e)
        {
                DirectionCheck();
                MoveSnake();

        }

        private void DrawGameArea()
        {
            for (int y = 0; y < GameArea.ActualHeight; y += squareSize)
            {
                for (int x = 0; x < GameArea.ActualWidth; x += squareSize)
                {
                    Rectangle rectangle = new Rectangle() { Width = squareSize, Height = squareSize };
                    rectangle.Fill = isOdd ? Brushes.White : Brushes.WhiteSmoke;
                    GameArea.Children.Add(rectangle);
                    Canvas.SetLeft(rectangle, x);
                    Canvas.SetTop(rectangle, y);
                    isOdd = !isOdd;
                }
                isOdd = !isOdd;
            }
        }

        private void DrawSnake(Point startPos, int snakelength)
        {
            Boolean isHead = false;

            double nextX = startPos.X;
            double nextY = startPos.Y;
            for (int i = 0; i < snakelength; i++)
            {
                if (i < snakelength - 1)
                {
                    isHead = false;
                    DrawSnake_sub(isHead, nextX, nextY);
                    nextX += squareSize;
                }
                else
                {
                    isHead = true;
                    DrawSnake_sub(isHead, nextX, nextY);
                }
            }
        }
        private void DrawSnake_sub(Boolean isHead , double nextX,double nextY)
        {
            
                Rectangle rectangle = new Rectangle() { Width = squareSize, Height = squareSize };
                rectangle.Fill = isHead ? snakeHeadColor : snakeBodyColor;
                SnakePart snakepart = new SnakePart();
                snakepart.uIElement = rectangle;
                snakepart.point = new Point(nextX, nextY);
                snakeParts.Add(snakepart);
                GameArea.Children.Add(rectangle);
                Canvas.SetLeft(rectangle, nextX);
                Canvas.SetTop(rectangle, nextY);

           
        }
        private void MoveSnake()
        {

            int collisionSitu = CollisionCheck();

            if (isGameRuning==false)
            {
                return;
            }

            if (collisionSitu != 1)
            {
                //移去最后一节身子
                GameArea.Children.Remove(snakeParts[0].uIElement);
                snakeParts.Remove(snakeParts[0]);

            }
            //获取原先蛇头的位置，并重新画一个身子
            double exHeadPosX = snakeParts[snakeParts.Count-1].point.X;
            double exHeadPosY = snakeParts[snakeParts.Count - 1].point.Y;
            DrawSnake_sub(false, exHeadPosX, exHeadPosY);
            //移去原先的蛇头
            GameArea.Children.Remove(snakeParts[snakeParts.Count - 2].uIElement);
            snakeParts.Remove(snakeParts[snakeParts.Count - 2]);
            //重新画一个蛇头
            switch (snakeDirection)
            {
                case SnakeDirection.left:
                    DrawSnake_sub(true, exHeadPosX - squareSize, exHeadPosY);
                    break;
                case SnakeDirection.up:
                    DrawSnake_sub(true, exHeadPosX , exHeadPosY - squareSize);
                    break;
                case SnakeDirection.right:
                    DrawSnake_sub(true, exHeadPosX + squareSize, exHeadPosY);
                    break;
                case SnakeDirection.down:
                    DrawSnake_sub(true, exHeadPosX , exHeadPosY + squareSize);
                    break;
                default:
                    break;
            }

            snakeDirectionPre = snakeDirection;

           
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (snakeDirection!=SnakeDirection.right && snakeDirection != SnakeDirection.left)
                    {
                        snakeDirection = SnakeDirection.left;
                        
                    }
                    break;
                case Key.Up:
                    if (snakeDirection!=SnakeDirection.down && snakeDirection != SnakeDirection.up)
                    {
                        snakeDirection = SnakeDirection.up;
                       
                    }
                    break;
                case Key.Right:
                    if (snakeDirection != SnakeDirection.right && snakeDirection != SnakeDirection.left)
                    {
                        snakeDirection = SnakeDirection.right;
                        
                    }
                    break;
                case Key.Down:
                    if (snakeDirection != SnakeDirection.down && snakeDirection != SnakeDirection.up)
                    {
                        snakeDirection = SnakeDirection.down;
                       
                    }
                    break;

                case Key.S:
                    if (isGameRuning==false && bdr_AddHighScore.Visibility==Visibility.Collapsed)
                    {
                         StartGame();

                    }
                    break;

                default:
                    break;
            }

        }

        private void DirectionCheck()
        {
            switch (snakeDirection)
            {
                case SnakeDirection.left:
                    if (snakeDirectionPre==SnakeDirection.right)
                    {
                        snakeDirection = snakeDirectionPre;
                    }
                    break;
                case SnakeDirection.up:
                    if (snakeDirectionPre == SnakeDirection.down)
                    {
                        snakeDirection = snakeDirectionPre;
                    }
                    break;
                case SnakeDirection.right:
                    if (snakeDirectionPre == SnakeDirection.left)
                    {
                        snakeDirection = snakeDirectionPre;
                    }
                    break;
                case SnakeDirection.down:
                    if (snakeDirectionPre == SnakeDirection.up)
                    {
                        snakeDirection = snakeDirectionPre;
                    }
                    break;
                default:
                    break;
            }
        }

        private void GenerateFood()
        {
            start:
            Random random = new Random();
            foodPosX = random.Next(0, (int)GameArea.ActualWidth / squareSize)*squareSize;
            foodPosY = random.Next(0, (int)GameArea.ActualHeight / squareSize) * squareSize;
            foreach (var snakePart in snakeParts)
            {
                if (snakePart.point.X==foodPosX && snakePart.point.Y==foodPosY)
                {
                    goto start;
                }
            }
            food = new Ellipse() { Width = squareSize, Height = squareSize };
            food.Fill = Brushes.Chocolate;
            GameArea.Children.Add(food);
            Canvas.SetLeft(food, foodPosX);
            Canvas.SetTop(food, foodPosY);
        }

        private int CollisionCheck()
        {
            if (snakeParts[snakeParts.Count-1].point.X==foodPosX && snakeParts[snakeParts.Count - 1].point.Y==foodPosY)
            {
                GameArea.Children.Remove(food);
                GenerateFood();
                score++;
                speed = speed > 100 ? speed - 50 : 100;
                dispatcher.Interval = TimeSpan.FromMilliseconds(speed);
                StatusUpdate();
                return 1;
            }
            else 
            {
                foreach (var snakeBody in snakeParts.Take(snakeParts.Count - 1))
                {
                    if (snakeBody.point.X == snakeParts[snakeParts.Count - 1].point.X && snakeBody.point.Y == snakeParts[snakeParts.Count - 1].point.Y)
                    {
                        EndGame(); 
                    }
                }

                if (snakeParts[snakeParts.Count - 1].point.X >= GameArea.ActualWidth ||
                   snakeParts[snakeParts.Count - 1].point.X < 0 ||
                   snakeParts[snakeParts.Count - 1].point.Y >= GameArea.ActualHeight ||
                   snakeParts[snakeParts.Count - 1].point.Y < 0)
                {
                    EndGame();
                }
                return 0;
            }
        }

        private void EndGame()
        {
            //dispatcher.Stop();
            dispatcher.IsEnabled = false;
            dispatcher.Stop();
            
            txt_score.Text = score.ToString();
            bdr_EndGame.Visibility = Visibility.Visible;
            isGameRuning = false;
            ScreenManagement();
            //MessageBox.Show("游戏结束","WPF贪吃蛇游戏",MessageBoxButton.OK,MessageBoxImage.Warning);
        }

        private void StartGame()
        {
       

            //欢迎画面消失
            bdr_Welcome.Visibility = Visibility.Collapsed;
            //清除上一局游戏里的蛇身
            if (snakeParts.Count!=0)
            {
                int snakeLength = snakeParts.Count;
                for (int i = 0; i < snakeLength; i++)
                {
                    GameArea.Children.Remove(snakeParts[0].uIElement);
                    snakeParts.Remove(snakeParts[0]);
                }
          
            }
            //清除上一局游戏里的食物
            if (food!=null)
            {
                GameArea.Children.Remove(food);
                food = null;
            }

            bdr_EndGame.Visibility = Visibility.Collapsed;

            //初始方向l
            snakeDirection = SnakeDirection.right;
            snakeDirectionPre = SnakeDirection.right;
            //重新生成蛇身以及食物
            DrawSnake(startPos, snakeLength);
            GenerateFood();
            //开始计时
            //初始化速度
            speed = 500;
            //初始化分数
            score = 0;
            dispatcher.Interval = TimeSpan.FromMilliseconds(speed);
            dispatcher.Start();
            isGameRuning = true;
            ScreenManagement();

            StatusUpdate();

        }

        private void CreateXMLFile()
        {
            //创建名为xmlName的文件流
            Stream write = new FileStream(xmlName, FileMode.Create);
            //设定xml的内部格式
            GenerateXML(write);
        }

        private void AddInfoToXML(string name)
        {
            HighScore highScore = new HighScore() {Name = name,Score = score };
            highScoreList.Add(highScore);
            highScoreList = new List<HighScore>(SortByScore(highScoreList));
            Stream write= new FileStream(xmlName, FileMode.Open);
            GenerateXML(write);
        }

        private void btn_submit(object sender, RoutedEventArgs e)
        {
            AddInfoToXML(txt_PlayName.Text);

            StreamReader read = new StreamReader(xmlName);
            ReadXML(read);
            bdr_TopList.Visibility = Visibility.Visible;
            bdr_AddHighScore.Visibility = Visibility.Collapsed;
        }

        private void GenerateXML(Stream write)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<HighScore>));
            
            xmlSerializer.Serialize(write, highScoreList);
          
            write.Close();
        }

        private void ReadXML(StreamReader readStream)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<HighScore>));
            
            highScoreList = (List<HighScore>)xmlSerializer.Deserialize(readStream);
            highScoreList = new List<HighScore>(SortByScore(highScoreList));
            Item_TopList.ItemsSource = highScoreList.Take(5);
            readStream.Close();
        }

        private IOrderedEnumerable<HighScore> SortByScore(List<HighScore> highscorelist)
        {
           var   highScoreOrderList= highscorelist.OrderByDescending(x => x.Score);
            return highScoreOrderList;
        }

        private void ScreenManagement()
        {
            if (isGameRuning==false)
            {
                if (highScoreList.Count!=0 && score > highScoreList[4].Score)
                {
                    bdr_AddHighScore.Visibility = Visibility.Visible;
                }


            }
            else
            {
                bdr_TopList.Visibility = Visibility.Collapsed;
                bdr_AddHighScore.Visibility = Visibility.Collapsed;
            }
        }

        private void btn_QueryHighScore(object sender, RoutedEventArgs e)
        {
            bdr_TopList.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatusUpdate()
        {
            txt_current_score.Text = "得分：" + score.ToString();
            txt_current_speed.Text = "速度" + speed.ToString();
        }
    }
}
