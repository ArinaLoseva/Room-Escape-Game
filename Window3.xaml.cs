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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Media;
using System.Threading;


namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        string room1 = "D:\\Рабочий стол\\исходники для проекта\\room1.png";
        string room2 = "D:\\Рабочий стол\\исходники для проекта\\room2.png";
        string room3 = "D:\\Рабочий стол\\исходники для проекта\\room3.png";
        string room4 = "D:\\Рабочий стол\\исходники для проекта\\room4.png";
        string inventory = "D:\\Рабочий стол\\исходники для проекта\\inventary.png";

        new private const int MinWidth = 910;
        new private const int MinHeight = 610;
        public string room = "";
        SoundPlayer player;

        public string username = "user0";
        public bool sound = false;
        public bool hints = false;
        int stage = 0;

        public Window3(bool previousWindowMaximized, double width, double height, double left, double top, string user)
        {
            InitializeComponent();

            username = user;
            room = "room1";
            player = new SoundPlayer();
            textBox1.Visibility = Visibility.Hidden;

            WindowState = WindowState.Maximized;

            
            Check();
            CheckStage();
            CheckRooms();
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

        private void Left_Click(object sender, MouseButtonEventArgs e) //поворот налево
        {
            string imagePath = "";

            if (room == "room1")
            {
                imagePath = room4;
                room = "room4";
            }
            else if (room == "room2")
            {
                imagePath = room1;
                room = "room1";
            }
            else if (room == "room3")
            {
                imagePath = room2;
                room = "room2";
            }
            else if (room == "room4")
            {
                imagePath = room3;
                room = "room3";
            }
            else if (room == "room_box")
            {
                imagePath = room1;
                room = "room1";
                textBox1.Visibility = Visibility.Hidden;
            }

            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            game.Source = bitmap;

            if (sound == true)
            {
                string soundFilePath = @"D:\Рабочий стол\исходники для проекта\room.wav";
                player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                player.Play(); // Воспроизводим звук
            }
        }

        private void Right_Click(object sender, MouseButtonEventArgs e) //поворот направо
        {
            string imagePath = "";

            if (room == "room1")
            {
                imagePath = room2;
                room = "room2";
            }
            else if (room == "room2")
            {
                imagePath = room3;
                room = "room3";
            }
            else if (room == "room3")
            {
                imagePath = room4;
                room = "room4";
            }
            else if (room == "room4")
            {
                imagePath = room1;
                room = "room1";
            }
            else if (room == "room_box")
            {
                imagePath = room1;
                room = "room1";
                textBox1.Visibility = Visibility.Hidden;
            }

            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            game.Source = bitmap;

            if (sound == true)
            {
                string soundFilePath = @"D:\Рабочий стол\исходники для проекта\room.wav";
                player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                player.Play(); // Воспроизводим звук
            }
        }

        private void Settings_Click(object sender, MouseButtonEventArgs e) //настройки
        {
            WriteStage();
            WriteRooms();
            Hide();
            Window1 newForm = new Window1(WindowState == WindowState.Maximized, Width, Height, Left, Top, true, username);
            newForm.Show();
        }

        private void Hints_Click(object sender, MouseButtonEventArgs e) //подсказка
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            connection.Open();
            string help = "";
            
            MySqlCommand selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Подсказка FROM Подсказки WHERE Этап_прохождения = @user_stage";
            selectCommand.Parameters.AddWithValue("@user_stage", stage);

            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    help = reader.GetString(0);
                }
            }
            Hint_text.Content = help;
        }

        public void Check()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            connection.Open();

            MySqlCommand selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Звук, Подсказки FROM Настройки WHERE Логин = @username";
            selectCommand.Parameters.AddWithValue("@username", username);

            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    sound = reader.GetBoolean(0);
                    hints = reader.GetBoolean(1);
                }
            }

            if (hints == true)
            {
                Hints.Visibility = Visibility.Visible;
            }
            else
            {
                Hints.Visibility = Visibility.Hidden;
            }
        }//проверка включен ли звук и подсказки

        public void CheckStage()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            connection.Open();

            MySqlCommand selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Этап_прохождения FROM Пользователи WHERE Логин = @username";
            selectCommand.Parameters.AddWithValue("@username", username);

            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    stage = reader.GetInt32(0);
                }
            }
            
        }//проверка этапа прохождения

        public void CheckRooms()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            connection.Open();

            MySqlCommand selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Стена_1, Стена_2, Стена_3, Стена_4, Инвентарь FROM Стены WHERE Логин = @username";
            selectCommand.Parameters.AddWithValue("@username", username);

            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    room1 = reader.GetString(0);
                    room2 = reader.GetString(1);
                    room3 = reader.GetString(2);
                    room4 = reader.GetString(3);
                    inventory = reader.GetString(4);
                }
                inventary.Source = new BitmapImage(new Uri(inventory));
                game.Source = new BitmapImage(new Uri(room1));
            }

        }//проверка состояния комнат

        public void Vase_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room1" && stage == 0)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_with_vase.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room1.2.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_with_vase.png";
                room1 = @"D:\Рабочий стол\исходники для проекта\room1.2.png";
                stage = 1;
            }
            if (room == "room1" && stage == 3)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_seed_vase.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room1.2.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_seed_vase.png";
                room1 = @"D:\Рабочий стол\исходники для проекта\room1.2.png";
                stage = 4;
            }
            if (room == "room1" && stage == 8)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_with_vase.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room1.2.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_with_vase.png";
                room1 = @"D:\Рабочий стол\исходники для проекта\room1.2.png";
                stage = 11;
            }
        } //ваза

        public void Bed_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room4" && stage == 1)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_vase_seed.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_vase_seed.png";
                stage = 2;
            }
            if (room == "room4" && stage == 0)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_seed.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_seed.png";
                stage = 3;
            }
            if (room == "room4" && stage == 5)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_water_vase_seed.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_water_vase_seed.png";
                stage = 7;
            }
        } //семечко в кровати

        public void Washstand_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room4" && stage == 1)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png";
                stage = 5;
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\water.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }
            if (room == "room4" && stage == 2)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_water_vase_seed.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_water_vase_seed.png";
                stage = 7;
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\water.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }
            if (room == "room4" && stage == 4)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_seed_water_vase.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_seed_water_vase.png";
                stage = 6;
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\water.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }
            if (room == "room4" && (stage == 10 || stage == 11))
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png";
                stage = 9;
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\water.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }
        } //вода в раковине

        public void Pot_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room3" && stage == 12)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_flower.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room3.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_flower.png";
                room3 = @"D:\Рабочий стол\исходники для проекта\room3.png";
                stage = 13;
            }
            if (room == "room3" && stage == 9)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room3.2.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary.png";
                room3 = @"D:\Рабочий стол\исходники для проекта\room3.2.png";
                stage = 12;
            }
            if (room == "room3" && stage == 3)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary.png";
                stage = 8;
            }
            if (room == "room3" && stage == 4)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_with_vase.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_with_vase.png";
                stage = 10;
            }
            if (room == "room3" && stage == 6)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png";
                stage = 9;
            }
            if (room == "room3" && stage == 7)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_water_vase.png";
                stage = 9;
            }
        } //посадить цветок

        public void Hand_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room3" && stage == 16)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_key.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room3.4.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_key.png";
                room3 = @"D:\Рабочий стол\исходники для проекта\room3.4.png";
                stage = 17;
            }
            if (room == "room3" && stage == 15)
            {
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room3.5.png"));
                room3 = @"D:\Рабочий стол\исходники для проекта\room3.5.png";
                stage = 16;
            }
            if (room == "room3" && stage == 14)
            {
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room3.4.png"));
                room3 = @"D:\Рабочий стол\исходники для проекта\room3.4.png";
                stage = 15;
            }
            if (room == "room3" && stage == 13)
            {
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room3.3.png"));
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary.png";
                room3 = @"D:\Рабочий стол\исходники для проекта\room3.3.png";
                stage = 14;
            }
        } //отдать цветок руке

        public void Case_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room2" && stage == 18)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_bird.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room2.3.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_bird.png";
                room2 = @"D:\Рабочий стол\исходники для проекта\room2.3.png";
                stage = 19;
            }
            if (room == "room2" && stage == 17)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary.png"));
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room2.2.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary.png";
                room2 = @"D:\Рабочий стол\исходники для проекта\room2.2.png";
                stage = 18;
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\key.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }  
        } //отпереть шкаф

        public void Clock_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room1" && stage == 19)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary.png";
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\kukushka.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
                stage = 20;
            }
            if (room == "room1" && stage == 20)
            {
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\kukushka.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }
        } //часы и кукушка

        public void Box_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room1")
            {
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room_box.png"));
                textBox1.Visibility = Visibility.Visible;
                room = "room_box";
            }
        } //окно шкатулки

        public void Box_button_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room_box" && textBox1.Text == "10")
            {
                game.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\room_box3.png"));
                textBox1.Visibility = Visibility.Hidden;
                stage = 21;
                if (sound == true)
                {
                    string soundFilePath = @"D:\Рабочий стол\исходники для проекта\paper.wav";
                    player.SoundLocation = soundFilePath; // Устанавливаем путь к звуковому файлу
                    player.Play(); // Воспроизводим звук
                }
            }
        } //бумага в шкатулке

        public void Carpet_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room2")
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary_doorkey.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary_doorkey.png";
                stage = 22;
            }
        } //ключ под ковриком

        public void Door_Click(object sender, MouseButtonEventArgs e)
        {
            if (room == "room2" && stage == 22)
            {
                inventary.Source = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\inventary.png"));
                inventory = @"D:\Рабочий стол\исходники для проекта\inventary.png";
                stage = 23;
                MessageBox.Show("Поздравляем, вы прошли игру!");
                Hide();
                MainWindow newForm = new MainWindow(WindowState == WindowState.Maximized, Width, Height, Left, Top, username);
                newForm.Show();
            }
        } //конец игры

        public void WriteStage()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            try
            {
                connection.Open();

                MySqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Пользователи SET Этап_прохождения = @stage WHERE Логин = @username";
                updateCommand.Parameters.AddWithValue("@stage", stage.ToString());
                updateCommand.Parameters.AddWithValue("@username", username);

                int rowsAffected = updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при сохранении этапа прохождения: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        } //запомнить этап прорхождения

        public void WriteRooms()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            try
            {
                connection.Open();

                MySqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Стены SET Стена_1 = @room1, Стена_2 = @room2, Стена_3 = @room3, Стена_4 = @room4, Инвентарь = @inventory WHERE Логин = @username";
                updateCommand.Parameters.AddWithValue("@room1", room1);
                updateCommand.Parameters.AddWithValue("@room2", room2);
                updateCommand.Parameters.AddWithValue("@room3", room3);
                updateCommand.Parameters.AddWithValue("@room4", room4);
                updateCommand.Parameters.AddWithValue("@inventory", inventory);
                updateCommand.Parameters.AddWithValue("@username", username);

                int rowsAffected = updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при сохранении состояния элементов: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        } //запомнить состояние элементов
    }
}

