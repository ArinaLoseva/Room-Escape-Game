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
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");

        private DispatcherTimer timer;
        int counter_exit_image_timer;
        bool registration;
        public string main_username = "user0";

        new private const int MinWidth = 910;
        new private const int MinHeight = 610;

        public Window2(bool previousWindowMaximized, double width, double height, double left, double top, bool reg)
        {
            InitializeComponent();

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
            counter_exit_image_timer = 0;
            SetNextImage(counter_exit_image_timer);
            timer.Start();

            registration = reg;
            if(registration == true)
            {
                textBox3.Visibility = Visibility.Visible;
                label3.Visibility = Visibility.Visible;
                pen.ToolTip = "регистрация";
            }
            else
            {
                textBox3.Visibility = Visibility.Hidden;
                label3.Visibility = Visibility.Hidden;
                pen.ToolTip = "войти в аккаунт";
            }
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

        private void Image_exit_Click(object sender, MouseButtonEventArgs e) //выход в настройки
        {
            Hide();
            Window1 newForm = new Window1(WindowState == WindowState.Maximized, Width, Height, Left, Top, main_username);
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

        private void Image_pen_Click(object sender, MouseButtonEventArgs e)//создание или вход в учетку
        {
            if(registration == true)//регистрация
            {
                if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "")
                {
                    if (textBox2.Text == textBox3.Text)
                    {
                        if(CheckPassword(textBox2.Text) == true)
                        {
                            if (CheckUsername(textBox1.Text) == true)
                            {
                                try
                                {
                                    MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");
                                    string username = textBox1.Text;
                                    string password = textBox2.Text;
                                    connection.Open();

                                    // Проверка наличия пользователей с таким же логином
                                    MySqlCommand checkCommand = connection.CreateCommand();
                                    checkCommand.CommandText = "SELECT COUNT(*) FROM Пользователи WHERE Логин = @username";
                                    checkCommand.Parameters.AddWithValue("@username", username);
                                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                                    if (count > 0)
                                    {
                                        MessageBox.Show("Пользователь с таким именем уже существует");
                                    }
                                    else
                                    {
                                        MySqlCommand insertCommand = connection.CreateCommand();
                                        insertCommand.CommandText = "INSERT INTO Пользователи (Логин, Пароль) VALUES (@username, @password)";
                                        insertCommand.Parameters.AddWithValue("@username", username);
                                        insertCommand.Parameters.AddWithValue("@password", password);

                                        int rowsAffected = insertCommand.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            // Добавление записи в таблицу "Настройки"
                                            MySqlCommand settingsCommand = connection.CreateCommand();
                                            settingsCommand.CommandText = "INSERT INTO Настройки (Логин) VALUES (@username)";
                                            settingsCommand.Parameters.AddWithValue("@username", username);
                                            settingsCommand.ExecuteNonQuery();

                                            // Добавление записи в таблицу "Достижения"
                                            MySqlCommand achievementsCommand = connection.CreateCommand();
                                            achievementsCommand.CommandText = "INSERT INTO Достижения (Логин) VALUES (@username)";
                                            achievementsCommand.Parameters.AddWithValue("@username", username);
                                            achievementsCommand.ExecuteNonQuery();

                                            // Добавление записи в таблицу "Стены"
                                            MySqlCommand roomsCommand = connection.CreateCommand();
                                            roomsCommand.CommandText = "INSERT INTO Стены (Логин) VALUES (@username)";
                                            roomsCommand.Parameters.AddWithValue("@username", username);
                                            roomsCommand.ExecuteNonQuery();

                                            MessageBox.Show("Регистрация успешно завершена");
                                            main_username = username;
                                            Hide();
                                            Window1 newForm = new Window1(WindowState == WindowState.Maximized, Width, Height, Left, Top, main_username);
                                            newForm.Show();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Не удалось добавить запись");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Произошла ошибка при добавлении записи: " + ex.Message);
                                }
                                finally
                                {
                                    connection.Close();
                                }
                            }
                            else
                                MessageBox.Show("Имя пользователя должно содержать не менее 4 символов, строчные и прописные латинские буквы");
                        }
                        else
                            MessageBox.Show("Пароль должен содержать не менее 8 символов, строчные и прописные латинские буквы, цифры и специальные символы");
                    }
                    else
                        MessageBox.Show("Пароли не совпадают");
                }
                else
                    MessageBox.Show("Для регистрации необходимо заполнить все поля");
            }
            else//авторизация
            {
                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    try
                    {
                        MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;database=game_DB;user=root;password=i hate mysql");

                        bool isAuthenticated = false;
                        //DataBase dataBase = new DataBase();
                        //dataBase.username = textBox1.Text;
                        //dataBase.password = textBox2.Text;
                        string username = textBox1.Text;
                        string password = textBox2.Text;
                        connection.Open();

                        string query = "SELECT COUNT(*) FROM Пользователи WHERE Логин = @Username AND Пароль = @Password";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", username);
                            command.Parameters.AddWithValue("@Password", password);

                            int count = Convert.ToInt32(command.ExecuteScalar());
                            isAuthenticated = count > 0;
                        }

                        if (isAuthenticated)
                        {
                            main_username = username;
                            MessageBox.Show("Вы вошли в аккаунт");
                            Hide();
                            Window1 newForm = new Window1(WindowState == WindowState.Maximized, Width, Height, Left, Top, main_username);
                            newForm.Show();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось войти в аккаунт");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка при поиске записи: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                else
                    MessageBox.Show("Для авторизации необходимо заполнить все поля");
            }
        }

        public bool CheckPassword(string input)
        {
            // Проверяем длину строки
            if (input.Length < 8)
            {
                return false;
            }
            // Проверяем наличие строчных и прописных букв, специальных символов и цифр
            if (!Regex.IsMatch(input, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()])(?=.*\d)"))
            {
                return false;
            }
            return true;
        }

        public bool CheckUsername(string input)
        {
            // Проверяем длину строки
            if (input.Length < 4)
            {
                return false;
            }
            // Проверяем наличие строчных и прописных букв
            if (!Regex.IsMatch(input, @"^(?=.*[a-z])(?=.*[A-Z])"))
            {
                return false;
            }
            return true;
        }

    }
}
