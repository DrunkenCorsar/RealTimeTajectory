using System;
using System.IO;
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

namespace CosmosWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int k; // Количество сохраненных экспериментов
        string[] mass = new string[1]; // Список названий экспериментов
        double h = 0, m = 0, s = 0; // Часы, минуты и секунды времени текущего эксперимента
        bool going = false; // Идет ли время на дисплее canvas
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(); // Таймер для расчетов
        System.Windows.Threading.DispatcherTimer painter = new System.Windows.Threading.DispatcherTimer(); // Таймер для рисования
        const int f = 200; // Константа, определяющая силу притяжения между телами (местная G)
        double r0 = 1; // Радиус точки траектории
        double end1 = 1e9, end2 = -1e9; // Границы области, за пределами которой планеты считаются несуществующими
        double m1, m2, m3, m4, m5; // Массы планет
        double r1, r2, r3, r4, r5; // Радиусы планет
        double x1, x2, x3, x4, x5; // Координаты планет по горизонтали
        double y1, y2, y3, y4, y5; // Координаты планет по вертикали
        double vx1, vx2, vx3, vx4, vx5; //Ггоризонтальная составляющая скорости планет
        double vy1, vy2, vy3, vy4, vy5; // Вертикальная составляющая скорости планет
        bool is1 = false, is2 = false, is3 = false, is4 = false, is5 = false; // Существуют ли планеты
        bool ts1 = false, ts2 = false, ts3 = false, ts4 = false, ts5 = false; // Нужно ли чертить траекторию у планет
        bool so1 = false, so2 = false, so3 = false, so4 = false, so5 = false; // Являются ли планеты системами отсчета
        Ellipse e1, e2, e3, e4, e5; // Эллипсы для рисования планет на дисплее canvas
        Brush c1 = Brushes.DarkRed, c01 = Brushes.Red; // Цвет первой планеты и ее траектории
        Brush c2 = Brushes.DarkGreen, c02 = Brushes.Green; // Цвет второй планеты и ее траектории
        Brush c3 = Brushes.DarkBlue, c03 = Brushes.Blue; // Цвет третьей планеты и ее траектории
        Brush c4 = Brushes.DarkOrange, c04 = Brushes.Orange; // Цвет четвертой планеты и ее траектории
        Brush c5 = Brushes.DarkViolet, c05 = Brushes.Violet; // Цвет пятой планеты и ее траектории

        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            painter.Tick += new EventHandler(painterTick);
            painter.Interval = new TimeSpan(0, 0, 0, 0, 1);
            StreamReader r = new StreamReader("cosf");
            string s = r.ReadLine();
            r.Close();
            string s1 = "";
            for (int i = 0; i < s.Length; i++)
            {
                s1 += (char)((int)s[i] - 200);
            }
            k = Convert.ToInt32(s1);
            if (k != 0)
            {
                mass = new string[k];
                for (int i = 0; i < k; i++)
                {
                    r = new StreamReader("fdm" + Convert.ToString(i + 1));
                    string[] str0 = r.ReadLine().Split(' ');
                    r.Close();
                    s1 = "";
                    for (int j = 0; j < str0[42].Length; j++)
                    {
                        s1 += (char)((int)str0[42][j] - 200);
                    }
                    mass[i] = s1;
                }
                combobox_2.Items.Clear();
                for (int i = 0; i < k; i++)
                {
                    TextBlock t1 = new TextBlock();
                    t1.Text = mass[i];
                    combobox_2.Items.Add(t1);
                }
            }
        }

        private void button_Delete_Click(object sender, RoutedEventArgs e)
        {
            button_Delete.Visibility = Visibility.Hidden;
            string s = textBox_File_delete.Text;
            int num = -1;
            if (k != 0)
            {
                for (int i = 0; i < k; i++)
                {
                    if (mass[i] == s) num = i;
                }
            }
            if (num == -1) MessageBox.Show("Файл не найден!");
            else
            {
                textBox_File_delete.Text = "";
                for (int i = num; i < k - 1; i++)
                {
                    StreamReader r = new StreamReader("fdm" + Convert.ToString(i + 2));
                    string str2 = r.ReadLine();
                    r.Close();
                    StreamWriter w = new StreamWriter("fdm" + Convert.ToString(i + 1));
                    w.WriteLine(str2);
                    w.Close();
                }
                k--;
                string str = Convert.ToString(k);
                string str1 = "";
                for (int i = 0; i < str.Length; i++)
                {
                    str1 += (char)((int)str[i] + 200);
                }
                StreamWriter r1 = new StreamWriter("cosf");
                r1.WriteLine(str1);
                r1.Close();
                string s1;
                if (k != 0)
                {
                    mass = new string[k];
                    for (int i = 0; i < k; i++)
                    {
                        StreamReader r = new StreamReader("fdm" + Convert.ToString(i + 1));
                        string[] str0 = r.ReadLine().Split(' ');
                        r.Close();
                        s1 = "";
                        for (int j = 0; j < str0[42].Length; j++)
                        {
                            s1 += (char)((int)str0[42][j] - 200);
                        }
                        mass[i] = s1;
                    }
                    combobox_2.Items.Clear();
                    for (int i = 0; i < k; i++)
                    {
                        TextBlock t1 = new TextBlock();
                        t1.Text = mass[i];
                        combobox_2.Items.Add(t1);
                    }
                }
                else combobox_2.Items.Clear();
            }
            button_Delete.Visibility = Visibility.Visible;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (combobox_2.SelectedIndex != -1)
            {
                button_Load.Visibility = Visibility.Hidden;
                int index = combobox_2.SelectedIndex;
                combobox_2.SelectedIndex = -1;
                StreamReader r = new StreamReader("fdm" + Convert.ToString(index + 1));
                string[] str0 = r.ReadLine().Split(' ');
                r.Close();
                string[] str = new string[str0.Length];
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = "";
                    if (str0[i].Length != 0)
                        for (int j = 0; j < str0[i].Length; j++)
                        {
                            str[i] += (char)((int)str0[i][j] - 200);
                        }
                }
                r_m1.Text = str[0]; r_x1.Text = str[1]; r_vx1.Text = str[2];
                r_r1.Text = str[3]; r_y1.Text = str[4]; r_vy1.Text = str[5];
                r_m2.Text = str[6]; r_x2.Text = str[7]; r_vx2.Text = str[8];
                r_r2.Text = str[9]; r_y2.Text = str[10]; r_vy2.Text = str[11];
                r_m3.Text = str[12]; r_x3.Text = str[13]; r_vx3.Text = str[14];
                r_r3.Text = str[15]; r_y3.Text = str[16]; r_vy3.Text = str[17];
                r_m4.Text = str[18]; r_x4.Text = str[19]; r_vx4.Text = str[20];
                r_r4.Text = str[21]; r_y4.Text = str[22]; r_vy4.Text = str[23];
                r_m5.Text = str[24]; r_x5.Text = str[25]; r_vx5.Text = str[26];
                r_r5.Text = str[27]; r_y5.Text = str[28]; r_vy5.Text = str[29];
                combobox_1.SelectedIndex = Convert.ToInt32(str[30]);
                if (str[31] == "0") c_ts1.IsChecked = false; else c_ts1.IsChecked = true;
                if (str[32] == "0") c_ts2.IsChecked = false; else c_ts2.IsChecked = true;
                if (str[33] == "0") c_ts3.IsChecked = false; else c_ts3.IsChecked = true;
                if (str[34] == "0") c_ts4.IsChecked = false; else c_ts4.IsChecked = true;
                if (str[35] == "0") c_ts5.IsChecked = false; else c_ts5.IsChecked = true;
                if (str[36] == "0") c_so1.IsChecked = false; else c_so1.IsChecked = true;
                if (str[37] == "0") c_so2.IsChecked = false; else c_so2.IsChecked = true;
                if (str[38] == "0") c_so3.IsChecked = false; else c_so3.IsChecked = true;
                if (str[39] == "0") c_so4.IsChecked = false; else c_so4.IsChecked = true;
                if (str[40] == "0") c_so5.IsChecked = false; else c_so5.IsChecked = true;
                if (str[41] == "0") c_so0.IsChecked = false; else c_so0.IsChecked = true;
                button_Load.Visibility = Visibility.Visible;
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = textBox_file.Text;
                bool err = false;
                if (str.Length == 0) err = true;
                else
                for (int i = 0; i < str.Length; i++)
                {
                    if (((int)str[i] < 28) || ((int)str[i] > 126)) err = true;
                }
                if (err) MessageBox.Show("Неправильное название файла!");
                else
                {
                    bool again = false;
                    if (k != 0)
                        for (int i = 0; i < mass.Length; i++)
                        {
                            if (mass[i] == str) again = true;
                        }
                    if (again == true) MessageBox.Show("Файл с таким именем уже существует!");
                    else
                    {
                        button_Save.Visibility = Visibility.Hidden;
                        StreamWriter r = new StreamWriter("fdm" + Convert.ToString(k + 1));
                        string str1 = "";
                        string b = "";
                        b = r_m1.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_x1.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vx1.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_r1.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_y1.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vy1.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_m2.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_x2.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vx2.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_r2.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_y2.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vy2.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_m3.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_x3.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vx3.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_r3.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_y3.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vy3.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_m4.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_x4.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vx4.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_r4.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_y4.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vy4.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_m5.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_x5.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vx5.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_r5.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_y5.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = r_vy5.Text;
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        b = Convert.ToString(combobox_1.SelectedIndex);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " ";
                        int n = 0;
                        if (c_ts1.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_ts2.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_ts3.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_ts4.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_ts5.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_so1.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_so2.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_so3.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_so4.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_so5.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        if (c_so0.IsChecked == true) n = 1;
                        b = Convert.ToString(n);
                        if (b.Length > 0)
                            for (int i = 0; i < b.Length; i++)
                                str1 += (char)((int)b[i] + 200);
                        str1 += " "; n = 0;
                        string st = "";
                        for (int i = 0; i < textBox_file.Text.Length; i++)
                        {
                            st += (char)((int)textBox_file.Text[i] + 200);
                        }
                        str1 += st;
                        r.WriteLine(str1);
                        r.Close();
                        k++;
                        str = Convert.ToString(k);
                        str1 = "";
                        for (int i = 0; i < str.Length; i++)
                        {
                            str1 += (char)((int)str[i] + 200);
                        }
                        r = new StreamWriter("cosf");
                        r.WriteLine(str1);
                        r.Close();
                        string s1;
                        mass = new string[k];
                        StreamReader r1;
                        for (int i = 0; i < k; i++)
                        {
                            r1 = new StreamReader("fdm" + Convert.ToString(i + 1));
                            string[] str0 = r1.ReadLine().Split(' ');
                            r.Close();
                            s1 = "";
                            for (int j = 0; j < str0[42].Length; j++)
                            {
                                s1 += (char)((int)str0[42][j] - 200);
                            }
                            mass[i] = s1;
                        }
                        combobox_2.Items.Clear();
                        for (int i = 0; i < k; i++)
                        {
                            TextBlock t1 = new TextBlock();
                            t1.Text = mass[i];
                            combobox_2.Items.Add(t1);
                        }
                        textBox_file.Text = "";
                        button_Save.Visibility = Visibility.Visible;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }

        private void timerTick(object sender, EventArgs e)
        {
            s+= 0.01;
            if (s >= 60) { s -= 60; m++; }
            if (m >= 60) { m -= 60; h++; }

            if (is1 && !so1)
            {
                if (is2)
                {
                    double S = Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
                    if (S < r1 + r2)
                    {
                        if (so2)
                        {
                            r2 = Math.Sqrt((r1 * r1) + (r2 * r2));
                            m2 += m1;
                            if (is3) { vx3 -= vx1 * (m1 / m2); vy3 -= vy1 * (m1 / m2); }
                            if (is4) { vx4 -= vx1 * (m1 / m2); vy4 -= vy1 * (m1 / m2); }
                            if (is5) { vx5 -= vx1 * (m1 / m2); vy5 -= vy1 * (m1 / m2); }
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                        else
                        {
                            r1 = Math.Sqrt((r1 * r1) + (r2 * r2));
                            vx1 = ((vx1 * m1) + (vx2 * m2)) / (m1 + m2);
                            vy1 = ((vy1 * m1) + (vy2 * m2)) / (m1 + m2);
                            m1 += m2;
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                    }
                    else
                    {
                        double v;
                        if (so2) v = (f * (m2 + m1)) / (S * S);
                        else v = (f * m2) / (S * S);
                        vx1 += ((x2 - x1) / S) * v;
                        vy1 += ((y2 - y1) / S) * v;
                    }
                }

                if (is3 && is1)
                {
                    double S = Math.Sqrt(((x1 - x3) * (x1 - x3)) + ((y1 - y3) * (y1 - y3)));
                    if (S < r1 + r3)
                    {
                        if (so3)
                        {
                            r3 = Math.Sqrt((r1 * r1) + (r3 * r3));
                            m3 += m1;
                            if (is2) { vx2 -= vx1 * (m1 / m3); vy2 -= vy1 * (m1 / m3); }
                            if (is4) { vx4 -= vx1 * (m1 / m3); vy4 -= vy1 * (m1 / m3); }
                            if (is5) { vx5 -= vx1 * (m1 / m3); vy5 -= vy1 * (m1 / m3); }
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                        else
                        {
                            r1 = Math.Sqrt((r1 * r1) + (r3 * r3));
                            vx1 = ((vx1 * m1) + (vx3 * m3)) / (m1 + m3);
                            vy1 = ((vy1 * m1) + (vy3 * m3)) / (m1 + m3);
                            m1 += m3;
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                    }
                    else
                    {
                        double v;
                        if (so3) v = (f * (m3 + m1)) / (S * S);
                        else v = (f * m3) / (S * S);
                        vx1 += ((x3 - x1) / S) * v;
                        vy1 += ((y3 - y1) / S) * v;
                    }
                }

                if (is4 && is1)
                {
                    double S = Math.Sqrt(((x1 - x4) * (x1 - x4)) + ((y1 - y4) * (y1 - y4)));
                    if (S < r1 + r4)
                    {
                        if (so4)
                        {
                            r4 = Math.Sqrt((r1 * r1) + (r4 * r4));
                            m4 += m1;
                            if (is2) { vx2 -= vx1 * (m1 / m4); vy2 -= vy1 * (m1 / m4); }
                            if (is3) { vx3 -= vx1 * (m1 / m4); vy3 -= vy1 * (m1 / m4); }
                            if (is5) { vx5 -= vx1 * (m1 / m4); vy5 -= vy1 * (m1 / m4); }
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                        else
                        {
                            r1 = Math.Sqrt((r1 * r1) + (r4 * r4));
                            vx1 = ((vx1 * m1) + (vx4 * m4)) / (m1 + m4);
                            vy1 = ((vy1 * m1) + (vy4 * m4)) / (m1 + m4);
                            m1 += m4;
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                    }
                    else
                    {
                        double v;
                        if (so4) v = (f * (m4 + m1)) / (S * S);
                        else v = (f * m4) / (S * S);
                        vx1 += ((x4 - x1) / S) * v;
                        vy1 += ((y4 - y1) / S) * v;
                    }
                }

                if (is5 && is1)
                {
                    double S = Math.Sqrt(((x1 - x5) * (x1 - x5)) + ((y1 - y5) * (y1 - y5)));
                    if (S < r1 + r5)
                    {
                        if (so5)
                        {
                            r5 = Math.Sqrt((r1 * r1) + (r5 * r5));
                            m5 += m1;
                            if (is2) { vx2 -= vx1 * (m1 / m5); vy2 -= vy1 * (m1 / m5); }
                            if (is3) { vx3 -= vx1 * (m1 / m5); vy3 -= vy1 * (m1 / m5); }
                            if (is4) { vx4 -= vx1 * (m1 / m5); vy4 -= vy1 * (m1 / m5); }
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                        else
                        {
                            r1 = Math.Sqrt((r1 * r1) + (r5 * r5));
                            vx1 = ((vx1 * m1) + (vx5 * m5)) / (m1 + m5);
                            vy1 = ((vy1 * m1) + (vy5 * m5)) / (m1 + m5);
                            m1 += m5;
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                    }
                    else
                    {
                        double v;
                        if (so5) v = (f * (m5 + m1)) / (S * S);
                        else v = (f * m5) / (S * S);
                        vx1 += ((x5 - x1) / S) * v;
                        vy1 += ((y5 - y1) / S) * v;
                    }
                }
            }
            if (is2 && !so2)
            {
                if (is1)
                {
                    double S = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
                    if (S < r2 + r1)
                    {
                        if (so1)
                        {
                            r1 = Math.Sqrt((r2 * r2) + (r1 * r1));
                            m1 += m2;
                            if (is3) { vx3 -= vx1 * (m2 / m1); vy3 -= vy1 * (m2 / m1); }
                            if (is4) { vx4 -= vx1 * (m2 / m1); vy4 -= vy1 * (m2 / m1); }
                            if (is5) { vx5 -= vx1 * (m2 / m1); vy5 -= vy1 * (m2 / m1); }
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                        else
                        {
                            r2 = Math.Sqrt((r2 * r2) + (r1 * r1));
                            vx2 = ((vx2 * m2) + (vx1 * m1)) / (m2 + m1);
                            vy2 = ((vy2 * m2) + (vy1 * m1)) / (m2 + m1);
                            m2 += m1;
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                    }
                    else
                    {
                        double v;
                        if (so1) v = (f * (m1 + m2)) / (S * S);
                        else v = (f * m1) / (S * S);
                        vx2 += ((x1 - x2) / S) * v;
                        vy2 += ((y1 - y2) / S) * v;
                    }
                }

                if (is3 && is2)
                {
                    double S = Math.Sqrt(((x2 - x3) * (x2 - x3)) + ((y2 - y3) * (y2 - y3)));
                    if (S < r2 + r3)
                    {
                        if (so3)
                        {
                            r3 = Math.Sqrt((r2 * r2) + (r3 * r3));
                            m3 += m2;
                            if (is1) { vx1 -= vx3 * (m2 / m3); vy1 -= vy3 * (m2 / m3); }
                            if (is4) { vx4 -= vx3 * (m2 / m3); vy4 -= vy3 * (m2 / m3); }
                            if (is5) { vx5 -= vx3 * (m2 / m3); vy5 -= vy3 * (m2 / m3); }
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                        else
                        {
                            r2 = Math.Sqrt((r2 * r2) + (r3 * r3));
                            vx2 = ((vx2 * m2) + (vx3 * m3)) / (m2 + m3);
                            vy2 = ((vy2 * m2) + (vy3 * m3)) / (m2 + m3);
                            m2 += m3;
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                    }
                    else
                    {
                        double v;
                        if (so3) v = (f * (m3 + m2)) / (S * S);
                        else v = (f * m3) / (S * S);
                        vx2 += ((x3 - x2) / S) * v;
                        vy2 += ((y3 - y2) / S) * v;
                    }
                }

                if (is4 && is2)
                {
                    double S = Math.Sqrt(((x2 - x4) * (x2 - x4)) + ((y2 - y4) * (y2 - y4)));
                    if (S < r2 + r4)
                    {
                        if (so4)
                        {
                            r4 = Math.Sqrt((r2 * r2) + (r4 * r4));
                            m4 += m2;
                            if (is1) { vx1 -= vx4 * (m2 / m4); vy1 -= vy4 * (m2 / m4); }
                            if (is3) { vx3 -= vx4 * (m2 / m4); vy3 -= vy4 * (m2 / m4); }
                            if (is5) { vx5 -= vx4 * (m2 / m4); vy5 -= vy4 * (m2 / m4); }
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                        else
                        {
                            r2 = Math.Sqrt((r2 * r2) + (r4 * r4));
                            vx2 = ((vx2 * m2) + (vx4 * m4)) / (m2 + m4);
                            vy2 = ((vy2 * m2) + (vy4 * m4)) / (m2 + m4);
                            m2 += m4;
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                    }
                    else
                    {
                        double v;
                        if (so4) v = (f * (m4 + m2)) / (S * S);
                        else v = (f * m4) / (S * S);
                        vx2 += ((x4 - x2) / S) * v;
                        vy2 += ((y4 - y2) / S) * v;
                    }
                }

                if (is5 && is2)
                {
                    double S = Math.Sqrt(((x2 - x5) * (x2 - x5)) + ((y2 - y5) * (y2 - y5)));
                    if (S < r2 + r5)
                    {
                        if (so5)
                        {
                            r5 = Math.Sqrt((r2 * r2) + (r5 * r5));
                            m5 += m2;
                            if (is1) { vx1 -= vx5 * (m2 / m5); vy1 -= vy5 * (m2 / m5); }
                            if (is3) { vx3 -= vx5 * (m2 / m5); vy3 -= vy5 * (m2 / m5); }
                            if (is4) { vx4 -= vx5 * (m2 / m5); vy4 -= vy5 * (m2 / m5); }
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                        else
                        {
                            r2 = Math.Sqrt((r2 * r2) + (r5 * r5));
                            vx2 = ((vx2 * m2) + (vx5 * m5)) / (m2 + m5);
                            vy2 = ((vy2 * m2) + (vy5 * m5)) / (m2 + m5);
                            m2 += m5;
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                    }
                    else
                    {
                        double v;
                        if (so5) v = (f * (m5 + m2)) / (S * S);
                        else v = (f * m5) / (S * S);
                        vx2 += ((x5 - x2) / S) * v;
                        vy2 += ((y5 - y2) / S) * v;
                    }
                }
            }
            if (is3 && !so3)
            {
                if (is1)
                {
                    double S = Math.Sqrt(((x3 - x1) * (x3 - x1)) + ((y3 - y1) * (y3 - y1)));
                    if (S < r3 + r1)
                    {
                        if (so1)
                        {
                            r1 = Math.Sqrt((r3 * r3) + (r1 * r1));
                            m1 += m3;
                            if (is2) { vx2 -= vx3 * (m3 / m1); vy2 -= vy3 * (m3 / m1); }
                            if (is4) { vx4 -= vx3 * (m3 / m1); vy4 -= vy3 * (m3 / m1); }
                            if (is5) { vx5 -= vx3 * (m3 / m1); vy5 -= vy3 * (m3 / m1); }
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                        else
                        {
                            r3 = Math.Sqrt((r3 * r3) + (r1 * r1));
                            vx3 = ((vx3 * m3) + (vx1 * m1)) / (m3 + m1);
                            vy3 = ((vy3 * m3) + (vy1 * m1)) / (m3 + m1);
                            m3 += m1;
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                    }
                    else
                    {
                        double v;
                        if (so1) v = (f * (m1 + m3)) / (S * S);
                        else v = (f * m1) / (S * S);
                        vx3 += ((x1 - x3) / S) * v;
                        vy3 += ((y1 - y3) / S) * v;
                    }
                }

                if (is2 && is3)
                {
                    double S = Math.Sqrt(((x3 - x2) * (x3 - x2)) + ((y3 - y2) * (y3 - y2)));
                    if (S < r3 + r2)
                    {
                        if (so2)
                        {
                            r2 = Math.Sqrt((r3 * r3) + (r2 * r2));
                            m2 += m3;
                            if (is1) { vx1 -= vx3 * (m3 / m2); vy1 -= vy3 * (m3 / m2); }
                            if (is4) { vx4 -= vx3 * (m3 / m2); vy4 -= vy3 * (m3 / m2); }
                            if (is5) { vx5 -= vx3 * (m3 / m2); vy5 -= vy3 * (m3 / m2); }
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                        else
                        {
                            r3 = Math.Sqrt((r3 * r3) + (r2 * r2));
                            vx3 = ((vx3 * m3) + (vx2 * m2)) / (m3 + m2);
                            vy3 = ((vy3 * m3) + (vy2 * m2)) / (m3 + m2);
                            m3 += m2;
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                    }
                    else
                    {
                        double v;
                        if (so2) v = (f * (m2 + m3)) / (S * S);
                        else v = (f * m2) / (S * S);
                        vx3 += ((x2 - x3) / S) * v;
                        vy3 += ((y2 - y3) / S) * v;
                    }
                }

                if (is4 && is3)
                {
                    double S = Math.Sqrt(((x3 - x4) * (x3 - x4)) + ((y3 - y4) * (y3 - y4)));
                    if (S < r3 + r4)
                    {
                        if (so4)
                        {
                            r4 = Math.Sqrt((r3 * r3) + (r4 * r4));
                            m4 += m3;
                            if (is2) { vx2 -= vx3 * (m3 / m4); vy2 -= vy3 * (m3 / m4); }
                            if (is1) { vx1 -= vx3 * (m3 / m4); vy1 -= vy3 * (m3 / m4); }
                            if (is5) { vx5 -= vx3 * (m3 / m4); vy5 -= vy3 * (m3 / m4); }
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                        else
                        {
                            r3 = Math.Sqrt((r3 * r3) + (r4 * r4));
                            vx3 = ((vx3 * m3) + (vx4 * m4)) / (m3 + m4);
                            vy3 = ((vy3 * m3) + (vy4 * m4)) / (m3 + m4);
                            m3 += m4;
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                    }
                    else
                    {
                        double v;
                        if (so4) v = (f * (m4 + m3)) / (S * S);
                        else v = (f * m4) / (S * S);
                        vx3 += ((x4 - x3) / S) * v;
                        vy3 += ((y4 - y3) / S) * v;
                    }
                }

                if (is5 && is3)
                {
                    double S = Math.Sqrt(((x3 - x5) * (x3 - x5)) + ((y3 - y5) * (y3 - y5)));
                    if (S < r3 + r5)
                    {
                        if (so5)
                        {
                            r5 = Math.Sqrt((r3 * r3) + (r5 * r5));
                            m5 += m3;
                            if (is2) { vx2 -= vx3 * (m3 / m5); vy2 -= vy3 * (m3 / m5); }
                            if (is4) { vx4 -= vx3 * (m3 / m5); vy4 -= vy3 * (m3 / m5); }
                            if (is1) { vx1 -= vx3 * (m3 / m5); vy1 -= vy3 * (m3 / m5); }
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                        else
                        {
                            r3 = Math.Sqrt((r3 * r3) + (r5 * r5));
                            vx3 = ((vx3 * m3) + (vx5 * m5)) / (m3 + m5);
                            vy3 = ((vy3 * m3) + (vy5 * m5)) / (m3 + m5);
                            m3 += m5;
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                    }
                    else
                    {
                        double v;
                        if (so5) v = (f * (m5 + m3)) / (S * S);
                        else v = (f * m5) / (S * S);
                        vx3 += ((x5 - x3) / S) * v;
                        vy3 += ((y5 - y3) / S) * v;
                    }
                }
            }
            if (is4 && !so4)
            {
                if (is1)
                {
                    double S = Math.Sqrt(((x4 - x1) * (x4 - x1)) + ((y4 - y1) * (y4 - y1)));
                    if (S < r4 + r1)
                    {
                        if (so1)
                        {
                            r1 = Math.Sqrt((r4 * r4) + (r1 * r1));
                            m1 += m4;
                            if (is2) { vx2 -= vx4 * (m4 / m1); vy2 -= vy4 * (m4 / m1); }
                            if (is3) { vx3 -= vx4 * (m4 / m1); vy3 -= vy4 * (m4 / m1); }
                            if (is5) { vx5 -= vx4 * (m4 / m1); vy5 -= vy4 * (m4 / m1); }
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                        else
                        {
                            r4 = Math.Sqrt((r4 * r4) + (r1 * r1));
                            vx4 = ((vx4 * m4) + (vx1 * m1)) / (m4 + m1);
                            vy4 = ((vy4 * m4) + (vy1 * m1)) / (m4 + m1);
                            m4 += m1;
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                    }
                    else
                    {
                        double v;
                        if (so1) v = (f * (m1 + m4)) / (S * S);
                        else v = (f * m1) / (S * S);
                        vx4 += ((x1 - x4) / S) * v;
                        vy4 += ((y1 - y4) / S) * v;
                    }
                }

                if (is2 && is4)
                {
                    double S = Math.Sqrt(((x4 - x2) * (x4 - x2)) + ((y4 - y2) * (y4 - y2)));
                    if (S < r4 + r2)
                    {
                        if (so2)
                        {
                            r2 = Math.Sqrt((r4 * r4) + (r2 * r2));
                            m2 += m4;
                            if (is1) { vx1 -= vx4 * (m4 / m2); vy1 -= vy4 * (m4 / m2); }
                            if (is3) { vx3 -= vx4 * (m4 / m2); vy3 -= vy4 * (m4 / m2); }
                            if (is5) { vx5 -= vx4 * (m4 / m2); vy5 -= vy4 * (m4 / m2); }
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                        else
                        {
                            r4 = Math.Sqrt((r4 * r4) + (r2 * r2));
                            vx4 = ((vx4 * m4) + (vx2 * m2)) / (m4 + m2);
                            vy4 = ((vy4 * m4) + (vy2 * m2)) / (m4 + m2);
                            m4 += m2;
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                    }
                    else
                    {
                        double v;
                        if (so2) v = (f * (m2 + m4)) / (S * S);
                        else v = (f * m2) / (S * S);
                        vx4 += ((x2 - x4) / S) * v;
                        vy4 += ((y2 - y4) / S) * v;
                    }
                }

                if (is3 && is4)
                {
                    double S = Math.Sqrt(((x4 - x3) * (x4 - x3)) + ((y4 - y3) * (y4 - y3)));
                    if (S < r4 + r3)
                    {
                        if (so3)
                        {
                            r3 = Math.Sqrt((r4 * r4) + (r3 * r3));
                            m3 += m4;
                            if (is2) { vx2 -= vx4 * (m4 / m3); vy2 -= vy4 * (m4 / m3); }
                            if (is1) { vx1 -= vx4 * (m4 / m3); vy1 -= vy4 * (m4 / m3); }
                            if (is5) { vx5 -= vx4 * (m4 / m3); vy5 -= vy4 * (m4 / m3); }
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                        else
                        {
                            r4 = Math.Sqrt((r4 * r4) + (r3 * r3));
                            vx4 = ((vx4 * m4) + (vx3 * m3)) / (m4 + m3);
                            vy4 = ((vy4 * m4) + (vy3 * m3)) / (m4 + m3);
                            m4 += m3;
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                    }
                    else
                    {
                        double v;
                        if (so3) v = (f * (m3 + m4)) / (S * S);
                        else v = (f * m3) / (S * S);
                        vx4 += ((x3 - x4) / S) * v;
                        vy4 += ((y3 - y4) / S) * v;
                    }
                }

                if (is5 && is4)
                {
                    double S = Math.Sqrt(((x4 - x5) * (x4 - x5)) + ((y4 - y5) * (y4 - y5)));
                    if (S < r4 + r5)
                    {
                        if (so5)
                        {
                            r5 = Math.Sqrt((r4 * r4) + (r5 * r5));
                            m5 += m4;
                            if (is2) { vx2 -= vx4 * (m4 / m5); vy2 -= vy4 * (m4 / m5); }
                            if (is1) { vx1 -= vx4 * (m4 / m5); vy1 -= vy4 * (m4 / m5); }
                            if (is3) { vx3 -= vx4 * (m4 / m5); vy3 -= vy4 * (m4 / m5); }
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                        else
                        {
                            r4 = Math.Sqrt((r4 * r4) + (r5 * r5));
                            vx4 = ((vx4 * m4) + (vx5 * m5)) / (m4 + m5);
                            vy4 = ((vy4 * m4) + (vy5 * m5)) / (m4 + m5);
                            m4 += m5;
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                    }
                    else
                    {
                        double v;
                        if (so5) v = (f * (m5 + m4)) / (S * S);
                        else v = (f * m5) / (S * S);
                        vx4 += ((x5 - x4) / S) * v;
                        vy4 += ((y5 - y4) / S) * v;
                    }
                }
            }
            if (is5 && !so5)
            {
                if (is1)
                {
                    double S = Math.Sqrt(((x5 - x1) * (x5 - x1)) + ((y5 - y1) * (y5 - y1)));
                    if (S < r5 + r1)
                    {
                        if (so1)
                        {
                            r1 = Math.Sqrt((r5 * r5) + (r1 * r1));
                            m1 += m5;
                            if (is2) { vx2 -= vx5 * (m5 / m1); vy2 -= vy5 * (m5 / m1); }
                            if (is4) { vx4 -= vx5 * (m5 / m1); vy4 -= vy5 * (m5 / m1); }
                            if (is3) { vx3 -= vx5 * (m5 / m1); vy3 -= vy5 * (m5 / m1); }
                            e1.Width = 2 * r1;
                            e1.Height = 2 * r1;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                        else
                        {
                            r5 = Math.Sqrt((r5 * r5) + (r1 * r1));
                            vx5 = ((vx5 * m5) + (vx1 * m1)) / (m5 + m1);
                            vy5 = ((vy5 * m5) + (vy1 * m1)) / (m5 + m1);
                            m5 += m1;
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is1 = false;
                            canvas_main.Children.Remove(e1);
                        }
                    }
                    else
                    {
                        double v;
                        if (so1) v = (f * (m1 + m5)) / (S * S);
                        else v = (f * m1) / (S * S);
                        vx5 += ((x1 - x5) / S) * v;
                        vy5 += ((y1 - y5) / S) * v;
                    }
                }

                if (is2 && is5)
                {
                    double S = Math.Sqrt(((x5 - x2) * (x5 - x2)) + ((y5 - y2) * (y5 - y2)));
                    if (S < r5 + r2)
                    {
                        if (so2)
                        {
                            r2 = Math.Sqrt((r5 * r5) + (r2 * r2));
                            m2 += m5;
                            if (is4) { vx4 -= vx5 * (m5 / m2); vy4 -= vy5 * (m5 / m2); }
                            if (is1) { vx1 -= vx5 * (m5 / m2); vy1 -= vy5 * (m5 / m2); }
                            if (is3) { vx3 -= vx5 * (m5 / m2); vy3 -= vy5 * (m5 / m2); }
                            e2.Width = 2 * r2;
                            e2.Height = 2 * r2;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                        else
                        {
                            r5 = Math.Sqrt((r5 * r5) + (r2 * r2));
                            vx5 = ((vx5 * m5) + (vx2 * m2)) / (m5 + m2);
                            vy5 = ((vy5 * m5) + (vy2 * m2)) / (m5 + m2);
                            m5 += m2;
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is2 = false;
                            canvas_main.Children.Remove(e2);
                        }
                    }
                    else
                    {
                        double v;
                        if (so2) v = (f * (m2 + m5)) / (S * S);
                        else v = (f * m2) / (S * S);
                        vx5 += ((x2 - x5) / S) * v;
                        vy5 += ((y2 - y5) / S) * v;
                    }
                }

                if (is3 && is5)
                {
                    double S = Math.Sqrt(((x5 - x3) * (x5 - x3)) + ((y5 - y3) * (y5 - y3)));
                    if (S < r5 + r3)
                    {
                        if (so3)
                        {
                            r3 = Math.Sqrt((r5 * r5) + (r3 * r3));
                            m3 += m5;
                            if (is2) { vx2 -= vx5 * (m5 / m3); vy2 -= vy5 * (m5 / m3); }
                            if (is1) { vx1 -= vx5 * (m5 / m3); vy1 -= vy5 * (m5 / m3); }
                            if (is4) { vx4 -= vx5 * (m5 / m3); vy4 -= vy5 * (m5 / m3); }
                            e3.Width = 2 * r3;
                            e3.Height = 2 * r3;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                        else
                        {
                            r5 = Math.Sqrt((r5 * r5) + (r3 * r3));
                            vx5 = ((vx5 * m5) + (vx3 * m3)) / (m5 + m3);
                            vy5 = ((vy5 * m5) + (vy3 * m3)) / (m5 + m3);
                            m5 += m3;
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is3 = false;
                            canvas_main.Children.Remove(e3);
                        }
                    }
                    else
                    {
                        double v;
                        if (so3) v = (f * (m3 + m5)) / (S * S);
                        else v = (f * m3) / (S * S);
                        vx5 += ((x3 - x5) / S) * v;
                        vy5 += ((y3 - y5) / S) * v;
                    }
                }

                if (is4 && is5)
                {
                    double S = Math.Sqrt(((x5 - x4) * (x5 - x4)) + ((y5 - y4) * (y5 - y4)));
                    if (S < r5 + r4)
                    {
                        if (so4)
                        {
                            r4 = Math.Sqrt((r5 * r5) + (r4 * r4));
                            m4 += m5;
                            if (is2) { vx2 -= vx5 * (m5 / m4); vy2 -= vy5 * (m5 / m4); }
                            if (is1) { vx1 -= vx5 * (m5 / m4); vy1 -= vy5 * (m5 / m4); }
                            if (is3) { vx3 -= vx5 * (m5 / m4); vy3 -= vy5 * (m5 / m4); }
                            e4.Width = 2 * r4;
                            e4.Height = 2 * r4;
                            is5 = false;
                            canvas_main.Children.Remove(e5);
                        }
                        else
                        {
                            r5 = Math.Sqrt((r5 * r5) + (r4 * r4));
                            vx5 = ((vx5 * m5) + (vx4 * m4)) / (m5 + m4);
                            vy5 = ((vy5 * m5) + (vy4 * m4)) / (m5 + m4);
                            m5 += m4;
                            e5.Width = 2 * r5;
                            e5.Height = 2 * r5;
                            is4 = false;
                            canvas_main.Children.Remove(e4);
                        }
                    }
                    else
                    {
                        double v;
                        if (so4) v = (f * (m4 + m5)) / (S * S);
                        else v = (f * m4) / (S * S);
                        vx5 += ((x4 - x5) / S) * v;
                        vy5 += ((y4 - y5) / S) * v;
                    }
                }
            }
            
            if (is1 && !so1)
            {
                x1 += vx1;
                y1 += vy1;
                if ((x1 > end1) || (x1 < end2) || (y1 > end1) || (y1 < end2)) is1 = false;
            }
            if (is2 && !so2)
            {
                x2 += vx2;
                y2 += vy2;
                if ((x2 > end1) || (x2 < end2) || (y2 > end1) || (y2 < end2)) is2 = false;
            }
            if (is3 && !so3)
            {
                x3 += vx3;
                y3 += vy3;
                if ((x3 > end1) || (x3 < end2) || (y3 > end1) || (y3 < end2)) is3 = false;
            }
            if (is4 && !so4)
            {
                x4 += vx4;
                y4 += vy4;
                if ((x4 > end1) || (x4 < end2) || (y4 > end1) || (y4 < end2)) is4 = false;
            }
            if (is5 && !so5)
            {
                x5 += vx5;
                y5 += vy5;
                if ((x5 > end1) || (x5 < end2) || (y5 > end1) || (y5 < end2)) is5 = false;
            }
        }

        private void painterTick(object sender, EventArgs e)
        {
            label_time.Content = String.Format("{0:00}:{1:00}:{2:00.00}", h, m, s);

            if (is1)
            {
                e1.SetValue(Canvas.TopProperty, y1 - r1);
                e1.SetValue(Canvas.LeftProperty, x1 - r1);
                if (ts1)
                {
                    Ellipse e0 = new Ellipse();
                    e0.Stroke = c01;
                    e0.Fill = c01;
                    e0.StrokeThickness = 0;
                    e0.Width = 2 * r0;
                    e0.Height = 2 * r0;
                    e0.SetValue(Canvas.TopProperty, y1 - r0);
                    e0.SetValue(Canvas.LeftProperty, x1 - r0);
                    canvas_main.Children.Add(e0);
                }
            }

            if (is2)
            {
                e2.SetValue(Canvas.TopProperty, y2 - r2);
                e2.SetValue(Canvas.LeftProperty, x2 - r2);
                if (ts2)
                {
                    Ellipse e0 = new Ellipse();
                    e0.Stroke = c02;
                    e0.Fill = c02;
                    e0.StrokeThickness = 0;
                    e0.Width = 2 * r0;
                    e0.Height = 2 * r0;
                    e0.SetValue(Canvas.TopProperty, y2 - r0);
                    e0.SetValue(Canvas.LeftProperty, x2 - r0);
                    canvas_main.Children.Add(e0);
                }
            }

            if (is3)
            {
                e3.SetValue(Canvas.TopProperty, y3 - r3);
                e3.SetValue(Canvas.LeftProperty, x3 - r3);
                if (ts3)
                {
                    Ellipse e0 = new Ellipse();
                    e0.Stroke = c03;
                    e0.Fill = c03;
                    e0.StrokeThickness = 0;
                    e0.Width = 2 * r0;
                    e0.Height = 2 * r0;
                    e0.SetValue(Canvas.TopProperty, y3 - r0);
                    e0.SetValue(Canvas.LeftProperty, x3 - r0);
                    canvas_main.Children.Add(e0);
                }
            }

            if (is4)
            {
                e4.SetValue(Canvas.TopProperty, y4 - r4);
                e4.SetValue(Canvas.LeftProperty, x4 - r4);
                if (ts4)
                {
                    Ellipse e0 = new Ellipse();
                    e0.Stroke = c04;
                    e0.Fill = c04;
                    e0.StrokeThickness = 0;
                    e0.Width = 2 * r0;
                    e0.Height = 2 * r0;
                    e0.SetValue(Canvas.TopProperty, y4 - r0);
                    e0.SetValue(Canvas.LeftProperty, x4 - r0);
                    canvas_main.Children.Add(e0);
                }
            }

            if (is5)
            {
                e5.SetValue(Canvas.TopProperty, y5 - r5);
                e5.SetValue(Canvas.LeftProperty, x5 - r5);
                if (ts5)
                {
                    Ellipse e0 = new Ellipse();
                    e0.Stroke = c05;
                    e0.Fill = c05;
                    e0.StrokeThickness = 0;
                    e0.Width = 2 * r0;
                    e0.Height = 2 * r0;
                    e0.SetValue(Canvas.TopProperty, y5 - r0);
                    e0.SetValue(Canvas.LeftProperty, x5 - r0);
                    canvas_main.Children.Add(e0);
                }
            }
        }

        private void button_stop_Click(object sender, RoutedEventArgs e)
        {
            if (!going)
            {
                going = true;
                timer.Start();
                painter.Start();
                button_clear.Visibility = Visibility.Hidden;
                button_start.Visibility = Visibility.Hidden;
                button_stop.Content = "Стоп";
            }
            else
            {
                going = false;
                timer.Stop();
                painter.Stop();
                button_clear.Visibility = Visibility.Visible;
                button_start.Visibility = Visibility.Visible;
                button_start.Content = "Заново";
                button_stop.Content = "Продолжить";
            }
        }

        private void button_clear_Click(object sender, RoutedEventArgs e)
        {
            canvas_main.Children.Clear();
            if (is1)
            {
                e1 = new Ellipse();
                e1.Stroke = c1;
                e1.Fill = c1;
                e1.StrokeThickness = 0;
                e1.Width = 2 * r1;
                e1.Height = 2 * r1;
                e1.SetValue(Canvas.TopProperty, y1 - r1);
                e1.SetValue(Canvas.LeftProperty, x1 - r1);
                canvas_main.Children.Add(e1);
            }
            if (is2)
            {
                e2 = new Ellipse();
                e2.Stroke = c2;
                e2.Fill = c2;
                e2.StrokeThickness = 0;
                e2.Width = 2 * r2;
                e2.Height = 2 * r2;
                e2.SetValue(Canvas.TopProperty, y2 - r2);
                e2.SetValue(Canvas.LeftProperty, x2 - r2);
                canvas_main.Children.Add(e2);
            }
            if (is3)
            {
                e3 = new Ellipse();
                e3.Stroke = c3;
                e3.Fill = c3;
                e3.StrokeThickness = 0;
                e3.Width = 2 * r3;
                e3.Height = 2 * r3;
                e3.SetValue(Canvas.TopProperty, y3 - r3);
                e3.SetValue(Canvas.LeftProperty, x3 - r3);
                canvas_main.Children.Add(e3);
            }
            if (is4)
            {
                e4 = new Ellipse();
                e4.Stroke = c4;
                e4.Fill = c4;
                e4.StrokeThickness = 0;
                e4.Width = 2 * r4;
                e4.Height = 2 * r4;
                e4.SetValue(Canvas.TopProperty, y4 - r4);
                e4.SetValue(Canvas.LeftProperty, x4 - r4);
                canvas_main.Children.Add(e4);
            }
            if (is5)
            {
                e5 = new Ellipse();
                e5.Stroke = c5;
                e5.Fill = c5;
                e5.StrokeThickness = 0;
                e5.Width = 2 * r5;
                e5.Height = 2 * r5;
                e5.SetValue(Canvas.TopProperty, y5 - r5);
                e5.SetValue(Canvas.LeftProperty, x5 - r5);
                canvas_main.Children.Add(e5);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            canvas_main.Children.Clear();
            h = 0; m = 0; s = 0;

            switch (combobox_1.SelectedIndex + 1)
            {
                case 1:
                    {
                        is1 = true;
                        is2 = false;
                        is3 = false;
                        is4 = false;
                        is5 = false;
                    }
                    break;
                case 2:
                    {
                        is1 = true;
                        is2 = true;
                        is3 = false;
                        is4 = false;
                        is5 = false;
                    }
                    break;
                case 3:
                    {
                        is1 = true;
                        is2 = true;
                        is3 = true;
                        is4 = false;
                        is5 = false;
                    }
                    break;
                case 4:
                    {
                        is1 = true;
                        is2 = true;
                        is3 = true;
                        is4 = true;
                        is5 = false;
                    }
                    break;
                case 5:
                    {
                        is1 = true;
                        is2 = true;
                        is3 = true;
                        is4 = true;
                        is5 = true;
                    }
                    break;
            }

            if (c_ts1.IsChecked == true) ts1 = true; else ts1 = false;
            if (c_ts2.IsChecked == true) ts2 = true; else ts2 = false;
            if (c_ts3.IsChecked == true) ts3 = true; else ts3 = false;
            if (c_ts4.IsChecked == true) ts4 = true; else ts4 = false;
            if (c_ts5.IsChecked == true) ts5 = true; else ts5 = false;

            if (c_so1.IsChecked == true) so1 = true; else so1 = false;
            if (c_so2.IsChecked == true) so2 = true; else so2 = false;
            if (c_so3.IsChecked == true) so3 = true; else so3 = false;
            if (c_so4.IsChecked == true) so4 = true; else so4 = false;
            if (c_so5.IsChecked == true) so5 = true; else so5 = false;

            if (is1)
            {
                r1 = Convert.ToDouble(r_r1.Text);
                x1 = Convert.ToDouble(r_x1.Text);
                y1 = Convert.ToDouble(r_y1.Text);
                vx1 = Convert.ToDouble(r_vx1.Text) / 100;
                vy1 = Convert.ToDouble(r_vy1.Text) / 100;
                m1 = Convert.ToDouble(r_m1.Text);
                e1 = new Ellipse();
                e1.Stroke = c1;
                e1.Fill = c1;
                e1.StrokeThickness = 0;
                e1.Width = 2 * r1;
                e1.Height = 2 * r1;
                e1.SetValue(Canvas.TopProperty, y1 - r1);
                e1.SetValue(Canvas.LeftProperty, x1 - r1);
                canvas_main.Children.Add(e1);
            }
            if (is2)
            {
                r2 = Convert.ToDouble(r_r2.Text);
                x2 = Convert.ToDouble(r_x2.Text);
                y2 = Convert.ToDouble(r_y2.Text);
                vx2 = Convert.ToDouble(r_vx2.Text) / 100;
                vy2 = Convert.ToDouble(r_vy2.Text) / 100;
                m2 = Convert.ToDouble(r_m2.Text);
                e2 = new Ellipse();
                e2.Stroke = c2;
                e2.Fill = c2;
                e2.StrokeThickness = 0;
                e2.Width = 2 * r2;
                e2.Height = 2 * r2;
                e2.SetValue(Canvas.TopProperty, y2 - r2);
                e2.SetValue(Canvas.LeftProperty, x2 - r2);
                canvas_main.Children.Add(e2);
            }
            if (is3)
            {
                r3 = Convert.ToDouble(r_r3.Text);
                x3 = Convert.ToDouble(r_x3.Text);
                y3 = Convert.ToDouble(r_y3.Text);
                vx3 = Convert.ToDouble(r_vx3.Text) / 100;
                vy3 = Convert.ToDouble(r_vy3.Text) / 100;
                m3 = Convert.ToDouble(r_m3.Text);
                e3 = new Ellipse();
                e3.Stroke = c3;
                e3.Fill = c3;
                e3.StrokeThickness = 0;
                e3.Width = 2 * r3;
                e3.Height = 2 * r3;
                e3.SetValue(Canvas.TopProperty, y3 - r3);
                e3.SetValue(Canvas.LeftProperty, x3 - r3);
                canvas_main.Children.Add(e3);
            }
            if (is4)
            {
                r4 = Convert.ToDouble(r_r4.Text);
                x4 = Convert.ToDouble(r_x4.Text);
                y4 = Convert.ToDouble(r_y4.Text);
                vx4 = Convert.ToDouble(r_vx4.Text) / 100;
                vy4 = Convert.ToDouble(r_vy4.Text) / 100;
                m4 = Convert.ToDouble(r_m4.Text);
                e4 = new Ellipse();
                e4.Stroke = c4;
                e4.Fill = c4;
                e4.StrokeThickness = 0;
                e4.Width = 2 * r4;
                e4.Height = 2 * r4;
                e4.SetValue(Canvas.TopProperty, y4 - r4);
                e4.SetValue(Canvas.LeftProperty, x4 - r4);
                canvas_main.Children.Add(e4);
            }
            if (is5)
            {
                r5 = Convert.ToDouble(r_r5.Text);
                x5 = Convert.ToDouble(r_x5.Text);
                y5 = Convert.ToDouble(r_y5.Text);
                vx5 = Convert.ToDouble(r_vx5.Text) / 100;
                vy5 = Convert.ToDouble(r_vy5.Text) / 100;
                m5 = Convert.ToDouble(r_m5.Text);
                e5 = new Ellipse();
                e5.Stroke = c5;
                e5.Fill = c5;
                e5.StrokeThickness = 0;
                e5.Width = 2 * r5;
                e5.Height = 2 * r5;
                e5.SetValue(Canvas.TopProperty, y5 - r5);
                e5.SetValue(Canvas.LeftProperty, x5 - r5);
                canvas_main.Children.Add(e5);
            }

            button_start.Visibility = Visibility.Hidden;
            button_clear.Visibility = Visibility.Hidden;
            button_stop.Content = "Старт!";
            button_stop.Visibility = Visibility.Visible;
        }
    }
}
