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
using System.ComponentModel;
using System.Collections;

namespace Password_game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game Main { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            //необходимое
            Main = new Game();
            myGrid.DataContext = Main;
            dataGrid.ItemsSource = Main.LinesArrayList;
            //настраиваемое
            Main.GetLenght = "4";
            Generate_Button_Click(GenerateButton, new RoutedEventArgs());
        }
        
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Adding 1 to make the row count start at 1 instead of 0
            // as pointed out by daub815
            if (e.Row.GetIndex() < Main.LinesArrayList.Count)
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
        }

        private void Solve_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Think_about_candidates();
        }

        private void Make_Move_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MakeMoveTextBox.Text != "" && PassTextBox.Text != "")
            {
                if (Main.lenght < 1 || Main.lenght > 9 || MakeMoveTextBox.Text.Count() != Main.lenght )
                {
                    MessageBox.Show("Длина пароля некорректна");
                    return;
                }
                int[] temp_mas = L.Func_conv(MakeMoveTextBox.Text);
                if (temp_mas.GroupBy(v => v).Where(g => g.Count() > 1).Count() != 0)
                {
                    MessageBox.Show("Есть повторяющиеся значения, такое нельзя по правилам");
                    return;
                }
                if (Main.LinesArrayList.Any(pp => MakeMoveTextBox.Text == pp.TryNumbersArray1))
                {
                    MessageBox.Show("Уже было проверено, нет смысла добавлять");
                    return;
                }

                int[] p = Calc_RightNumbersValue(L.Func_conv(PassTextBox.Text), temp_mas);
                Main.Add(new Line(Main.lenght, temp_mas, p[0], p[1]));
            }
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            Main.LinesArrayList.Clear();
            string str = "";
            System.Collections.ArrayList used_numbers = new System.Collections.ArrayList();
            int i = 0;
            while (i < Main.lenght)
            {
                int temp = r.Next(10);
                if (!used_numbers.Contains(temp))
                {
                    used_numbers.Add(temp);
                    str += temp.ToString();
                    i++;
                }
            }
            Main.GetPassword = str;
            Main.GetCandidates = "";
        }

        public int[] Calc_RightNumbersValue(int[] passw, int[] trymas)
        {
            int r1, r2 = 0, r3 = 0;
            r1 = trymas.Count(s => passw.Contains(s));
            for (int i = 0; i < trymas.Count(); i++)
            {
                if (passw.Contains(trymas[i])) r3++;
                if (passw[i] == trymas[i]) r2++;
            }
            if (r1 != r3) throw new Exception();
            return new int[] { r1, r2 };
        }
        public void Think_about_candidates()
        {
            //граничные случаи
            foreach (var item in Main.LinesArrayList)
            {
                if (item.RightNumbersValue1 == 0)
                {
                    //гарантированное удаление кандидатов при нуле
                    foreach (var item2_cand_of_cell in Main.SetCandidates)
                    {
                        foreach (var item3 in item.TryNumbersArray)
                        {
                            //гарантированное удаление кандидатов при нуле
                            item2_cand_of_cell.Remove(item3);

                            //также, первый раздел - исключение цифр
                            Main.s.justNumbersArray.Remove(item3);
                        }
                    }
                }
                if (item.RightNumbersValue1 == Main.lenght)
                {
                    //точное попадание
                    if (item.RightNumbersAndPositionValue1 == Main.lenght)
                    {
                        for (int i = 0; i < Main.SetCandidates.Count(); i++)
                        {
                            Main.SetCandidates[i] = new List<int>() { item.TryNumbersArray[i] };
                        }
                        MessageBox.Show("Ура, решено!");
                    }
                    else
                    {
                        //удаление остальных кандидатов
                        for (int i = 0; i < Main.SetCandidates.Count(); i++)
                        {
                            int[] temp_mas = Main.SetCandidates[i].ToArray();
                            int[] temp_unioned = temp_mas.Intersect(item.TryNumbersArray).ToArray();
                            if (temp_mas != temp_unioned) Main.SetCandidates[i] = new List<int>(temp_unioned);
                        }
                    }
                    Main.s.justNumbersArray = new List<int>(item.TryNumbersArray);
                    Main.s.justNumbersArray.Sort();
                }

                //места
                if (item.RightNumbersAndPositionValue1 == 0)
                {
                    //гарантированное кандидатов на позициях при нуле
                    Main.SetCandidates = Main.SetCandidates.Select((p, i) => { List<int> lst = p; lst.Remove(item.TryNumbersArray[i]); return lst; }).ToArray();
                }
            }

            //более сложная логика
            //поиск полного покрытия: например, 1234 и 5678
            if (Main.lenght < 10 / 2) //2, 3, 4
            {
                //только для двух
                //и здесь ненужный двойной проход, можно оптимизировать
                foreach (var item in Main.LinesArrayList)
                {
                    foreach (var item2 in Main.LinesArrayList)
                    {
                        if (item != item2)
                        {
                            int t = item.RightNumbersValue1 + item2.RightNumbersValue1;
                            if (t == Main.lenght)
                            {
                                var unioned = item.TryNumbersArray.Union(item2.TryNumbersArray).ToArray();

                                //два непересекающиеся множества
                                if (unioned.Count() == 2*Main.lenght)
                                {
                                    var exc_mas = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.ToArray().Except(unioned).ToArray();
                                    //удаление всего неполного множества из кандидатов                    
                                    Main.SetCandidates = Main.SetCandidates.Select((p, i) => {
                                        List<int> lst = p;
                                        foreach (var i8 in exc_mas)
                                        {
                                            lst.Remove(i8);
                                        }
                                        return lst;
                                    }).ToArray();
                                    foreach (var i8 in exc_mas)
                                    {
                                        Main.s.justNumbersArray.Remove(i8);
                                    }
                                    Main.s.justNumbersArray.Sort();
                                    //MessageBox.Show("Два непересекающиеся множества дали результат");
                                }
                            }
                        }
                    }
                }
            }
            else //5, 6, 7, 8, 9
            //проверка остатка от одной линии - этого достаточно для этих случаев
            {
                foreach (var item in Main.LinesArrayList)
                {
                    //остаток
                    var exc_mas = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.ToArray().Except(item.TryNumbersArray).ToArray();

                    if (exc_mas.Count() == Main.lenght - item.RightNumbersValue1)
                    {
                        //TODO ThinkAbout
                        //случай: множество неполное, надо как-то отметить и использовать это!

                        //для полного множества
                        //удаление всего, кроме этого множества
                        //не проверено!
                        if (exc_mas.Count() == Main.lenght)
                        {
                            for (int i = 0; i < Main.SetCandidates.Count(); i++)
                            {
                                int[] temp_mas = Main.SetCandidates[i].ToArray();
                                int[] temp_unioned = temp_mas.Intersect(exc_mas).ToArray();
                                if (temp_mas != temp_unioned) Main.SetCandidates[i] = new List<int>(temp_unioned);
                            }
                            Main.s.justNumbersArray = new List<int>(item.TryNumbersArray);
                            Main.s.justNumbersArray.Sort();
                        }
                    }
                }
            }

            Main.OnPropertyChanged("GetCandidates");
            Generate_statistick_matrix();
        }

        public void Generate_statistick_matrix()
        {
            //сборка всех вариантов
            List<int[]> mas_of_all_vars = new List<int[]>();

            if (Main.SetCandidates.Count() > 1)
            {
                List<int> first_mult = Main.SetCandidates[0];
                List<int> second_mult = Main.SetCandidates[1];
                //первое перемножение первых двух матриц
                mas_of_all_vars = first_mult.SelectMany(p => second_mult.Where(u => u != p).Select(o => new int[] { p, o }).ToList()).ToList();
            }

            for (int i = 2; i < Main.SetCandidates.Count(); i++)
            {
                List<int> third_mult = Main.SetCandidates[i];
                //доперемножение матриц
                mas_of_all_vars = mas_of_all_vars.SelectMany(p => third_mult.Where(u => !p.Contains(u)).Select(o => p.Concat(new int[] { o }).ToArray()).ToList()).ToList();
            }

            //здесь можно повычислять еще статистику
            //есть ли в этом смысл?
            int[] numbers_stat = new int[10];
            foreach (var item in mas_of_all_vars)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (item.Contains(i)) numbers_stat[i]++;
                }
            }
            MessageBox.Show(String.Join(" ", numbers_stat));

            //заполнение матрицы статистики
            //есть ли в этом смысл?
            for (int i = 0; i < Main.SetCandidates.Count(); i++)
            {
                var item = Main.SetCandidates[i];
                for (int j = 0; j < 10; j++)
                {
                    Main.s.stat_matrix[i][j] = (double)mas_of_all_vars.Count(p => p[i] == j);
                        // (double)mas_of_all_vars.Count();
                }
            }
            MessageBox.Show(mas_of_all_vars.Count() + "\n" + String.Join("\n", Main.s.stat_matrix.Select(p => String.Join("  ", p))));
        }

        private void GenerateNewRandomTryPassButton_Click(object sender, RoutedEventArgs e)
        {
            string s = "";
            for (int i = 0; i < Main.lenght; i++)
            {
                s += i.ToString();
            }
            
            if (Main.LinesArrayList.Count == 0)
            {
                MakeMoveTextBox.Text = s;
            }

            //выдача того, что осталось от первого
            if (Main.LinesArrayList.Count == 1 && Main.lenght < 10 / 2)
            {
                string s2 = "";
                int j = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (Main.LinesArrayList.Any(k => !k.TryNumbersArray.Contains(i)) && j < Main.lenght)
                    {
                        s2 += i.ToString();
                        j++;
                    }
                }
                MakeMoveTextBox.Text = s2;
            }


            //TODO алгоритм подбора попытки по вероятностям
        }
    }
}
