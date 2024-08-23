using System;
using System.Collections.Generic;
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
using MySql.Data.MySqlClient;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        int counter_main_image_timer;

        new private const int MinWidth = 910;
        new private const int MinHeight = 610;

        public string username = "user0";

        public MainWindow()
        {
            InitializeComponent();

            MusicPlayer.Initialize();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;
            counter_main_image_timer = 0;
            SetNextImage(counter_main_image_timer);
            timer.Start();
        }

        public MainWindow(bool previousWindowMaximized, double width, double height, double left, double top, string user)
        {
            InitializeComponent();

            username = user;

            if (previousWindowMaximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Width = width;
                Height = height;
                Left = left;
                Top = top;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;
            counter_main_image_timer = 0;
            SetNextImage(counter_main_image_timer);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            counter_main_image_timer++;
            SetNextImage(counter_main_image_timer);
        }

        private void SetNextImage(int counter_main_image_timer)
        {
            string imagePath;

            if (counter_main_image_timer % 2 == 0)
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\куб1.jpg";
                counter_main_image_timer = 2;
            }
            else
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\куб2.jpg";
            }

            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            main_image.Source = bitmap;
        } //смена изображения кубика

        private void Image_Click(object sender, MouseButtonEventArgs e) //переход в настройки
        {
            Hide();
            Window1 newForm = new Window1(WindowState == WindowState.Maximized, Width, Height, Left, Top, username);
            newForm.Show();
        }

        private void Cube_Click(object sender, MouseButtonEventArgs e) //начало игры
        {
            Hide();
            Window3 newForm = new Window3(WindowState == WindowState.Maximized, Width, Height, Left, Top, username);
            newForm.Show();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) //ограничение размера
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.NewSize.Width < MinWidth || sizeInfo.NewSize.Height < MinHeight)
            {
                Width = sizeInfo.NewSize.Width < MinWidth ? MinWidth : sizeInfo.NewSize.Width;
                Height = sizeInfo.NewSize.Height < MinHeight ? MinHeight : sizeInfo.NewSize.Height;
            }
        }

        public void How_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Что-бы пройти игру, нужно выбраться из комнаты. Собирайте предметы, используйте их, найдите ключ. Если непонятно - используйте подсказки");
        }
    }

    public static class MusicPlayer
    {
        public static MediaPlayer mediaPlayer { get; private set; }
        public static string username = "user0";

        public static void Initialize()
        {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            string filePath = @"D:\Рабочий стол\исходники для проекта\music.mp3"; // Путь к вашему музыкальному файлу
            mediaPlayer.Open(new Uri(filePath));

            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            connection.Open();
            bool music = false;

            MySqlCommand selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Музыка FROM Настройки WHERE Логин = @username";
            selectCommand.Parameters.AddWithValue("@username", username);

            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    music = reader.GetBoolean(0);
                }
            }

            if (music)
            {
                mediaPlayer.Play();
            }
            else
            {
                mediaPlayer.Stop();
            }
        }

        private static void MediaPlayer_MediaEnded(object sender, EventArgs e) // Возобновляем воспроизведение
        {
            mediaPlayer.Position = TimeSpan.Zero; // Устанавливаем позицию воспроизведения в начало
            mediaPlayer.Play();
        }
    }
}

