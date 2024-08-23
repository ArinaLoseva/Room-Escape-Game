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
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace WpfApp3
{
    public partial class Window1 : Window
    {
        private DispatcherTimer timer;
        int counter_exit_image_timer;

        new private const int MinWidth = 910;
        new private const int MinHeight = 610;

        bool flag_sound = true;
        bool flag_music = true;
        bool flag_hint = true;
        //bool account = false;
        bool registration = false;
        bool game = false;

        public string username = "user0";

        public Window1(bool previousWindowMaximized, double width, double height, double left, double top, string user)
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

            if(game == false)
            {
                Return_game.Visibility = Visibility.Hidden;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;
            counter_exit_image_timer = 0;
            SetNextImage(counter_exit_image_timer);
            timer.Start();

            Check(username);
        }

        public Window1(bool previousWindowMaximized, double width, double height, double left, double top, bool game, string user)
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

            if(game == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(@"D:\Рабочий стол\исходники для проекта\бумажка2.png"));
                paper1_image.Source = bitmap;

                Label1.Visibility = Visibility.Hidden;
                Label2.Visibility = Visibility.Hidden;
                Return_game.Visibility = Visibility.Visible;
            }
            else
            {
                Return_game.Visibility = Visibility.Hidden;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;
            counter_exit_image_timer = 0;
            SetNextImage(counter_exit_image_timer);
            timer.Start();

            Check(username);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            counter_exit_image_timer++;
            SetNextImage(counter_exit_image_timer);
        }//таймер

        private void SetNextImage(int counter_exit_image_timer)
        {
            string imagePath;

            if (counter_exit_image_timer % 2 == 0)
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\exit10.png";
                counter_exit_image_timer = 2;
            }
            else
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\exit20.png";
            }

            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            exit.Source = bitmap;
        } //смена изображения exit

        private void Image1_Click(object sender, MouseButtonEventArgs e) //изменение звука
        {
            string imagePath;

            if (flag_sound)
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\звук выкл.jpg";
                flag_sound = false;
                sound_image.ToolTip = "Включить звук";
                UpdateSoundSetting("user0", false);
            }
            else
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\звук вкл.jpg";
                flag_sound = true;
                sound_image.ToolTip = "Выключить звук";
                UpdateSoundSetting("user0", true);
            }
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            sound_image.Source = bitmap;
        }

        private void Image2_Click(object sender, MouseButtonEventArgs e) //изменение музыки
        {
            string imagePath;

            if (flag_music)
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\музыка выкл.jpg";
                flag_music = false;
                MusicPlayer.mediaPlayer.Stop();
                music_image.ToolTip = "Включить музыку";
                UpdateMusicSetting("user0", false);
            }
            else
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\музыка вкл.jpg";
                flag_music = true;
                MusicPlayer.mediaPlayer.Play();
                music_image.ToolTip = "Выключить музыку";
                UpdateMusicSetting("user0", true);
            }
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            music_image.Source = bitmap;
        }

        private void Image3_Click(object sender, MouseButtonEventArgs e) //изменение подсказок
        {
            string imagePath;

            if (flag_hint)
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\подсказки выкл.jpg";
                flag_hint = false;
                hint_image.ToolTip = "Включить подсказки";
                UpdateHintSetting("user0", false);
            }
            else
            {
                imagePath = @"D:\Рабочий стол\исходники для проекта\подсказки вкл.jpg";
                flag_hint = true;
                hint_image.ToolTip = "Выключить подсказки";
                UpdateHintSetting("user0", true);
            }
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            hint_image.Source = bitmap;
        }

        private void Label1_Click(object sender, MouseButtonEventArgs e) //войти в аккаунт
        {
            registration = false;
            Hide();
            Window2 newForm = new Window2(WindowState == WindowState.Maximized, Width, Height, Left, Top, registration);
            newForm.Show();
        }

        private void Label2_Click(object sender, MouseButtonEventArgs e) //регистрация
        {
            registration = true;
            Hide();
            Window2 newForm = new Window2(WindowState == WindowState.Maximized, Width, Height, Left, Top, registration);
            newForm.Show();
        }

        private void Image_exit_Click(object sender, MouseButtonEventArgs e) //выход на главную страницу
        {
            Hide();
            MainWindow newForm = new MainWindow(WindowState == WindowState.Maximized, Width, Height, Left, Top, username);
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

        private void UpdateMusicSetting(string username, bool musicEnabled)
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            try
            {
                connection.Open();

                MySqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Настройки SET Музыка = @musicEnabled WHERE Логин = @username";
                updateCommand.Parameters.AddWithValue("@musicEnabled", musicEnabled);
                updateCommand.Parameters.AddWithValue("@username", username);

                int rowsAffected = updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении настроек музыки: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        } //сохранение настроек музыки

        private void UpdateSoundSetting(string username, bool soundEnabled)
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            try
            {
                connection.Open();

                MySqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Настройки SET Звук = @soundEnabled WHERE Логин = @username";
                updateCommand.Parameters.AddWithValue("@soundEnabled", soundEnabled);
                updateCommand.Parameters.AddWithValue("@username", username);

                int rowsAffected = updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении настроек звука: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        } //сохранение настроек звука

        private void UpdateHintSetting(string username, bool hintEnabled)
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            try
            {
                connection.Open();

                MySqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Настройки SET Подсказки = @hintEnabled WHERE Логин = @username";
                updateCommand.Parameters.AddWithValue("@hintEnabled", hintEnabled);
                updateCommand.Parameters.AddWithValue("@username", username);

                int rowsAffected = updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении настроек подсказок: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        } //сохранение настроек подсказок

        public void Check(string username)
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
            connection.Open();
            bool music = false;
            bool sound = false;
            bool hint = false;

            MySqlCommand selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Музыка, Звук, Подсказки FROM Настройки WHERE Логин = @username";
            selectCommand.Parameters.AddWithValue("@username", username);

            using (MySqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    music = reader.GetBoolean(0);
                    sound = reader.GetBoolean(1);
                    hint = reader.GetBoolean(2);
                }
            }

            if (music == true || music == false)
            {
                string imagePath = "";
                if (music == true)
                {
                    imagePath = @"D:\Рабочий стол\исходники для проекта\музыка вкл.jpg";
                    flag_music = true;
                    music_image.ToolTip = "Выключить музыку";
                }
                if (music == false)
                {
                    imagePath = @"D:\Рабочий стол\исходники для проекта\музыка выкл.jpg";
                    flag_music = false;
                    music_image.ToolTip = "Включить музыку";
                }
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                music_image.Source = bitmap;
            }

            if (sound == true || sound == false)
            {
                string imagePath = "";
                if (sound == true)
                {
                    imagePath = @"D:\Рабочий стол\исходники для проекта\звук вкл.jpg";
                    flag_sound = true;
                    sound_image.ToolTip = "Выключить звук";
                }
                if (sound == false)
                {
                    imagePath = @"D:\Рабочий стол\исходники для проекта\звук выкл.jpg";
                    flag_sound = false;
                    sound_image.ToolTip = "Включить звук";
                }
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                sound_image.Source = bitmap;
            }

            if (hint == true || hint == false)
            {
                string imagePath = "";
                if (hint == true)
                {
                    imagePath = @"D:\Рабочий стол\исходники для проекта\подсказки вкл.jpg";
                    flag_hint = true;
                    hint_image.ToolTip = "Выключить подсказки";
                }
                if (hint == false)
                {
                    imagePath = @"D:\Рабочий стол\исходники для проекта\подсказки выкл.jpg";
                    flag_hint = false;
                    hint_image.ToolTip = "Включить подсказки";
                }
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                hint_image.Source = bitmap;
            }
        } //соответствующая картинка

        private void Return_Click(object sender, MouseButtonEventArgs e) //вернуться в игру
        {
            Hide();
            Window3 newForm = new Window3(WindowState == WindowState.Maximized, Width, Height, Left, Top, username);
            newForm.Show();
        }

        public void Reset_Click(object sender, MouseButtonEventArgs e)
        {
            string room1 = "D:\\Рабочий стол\\исходники для проекта\\room1.png";
            string room2 = "D:\\Рабочий стол\\исходники для проекта\\room2.png";
            string room3 = "D:\\Рабочий стол\\исходники для проекта\\room3.png";
            string room4 = "D:\\Рабочий стол\\исходники для проекта\\room4.png";
            string inventory = "D:\\Рабочий стол\\исходники для проекта\\inventary.png";
            string stage = "0";
           
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

            try
            {
                connection.Open();

                MySqlCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Пользователи SET Этап_прохождения = @stage WHERE Логин = @username";
                updateCommand.Parameters.AddWithValue("@stage", stage);
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
                MessageBox.Show("Прогресс игры сброшен");
            }
        } //сброс прогресса 
    }
}
