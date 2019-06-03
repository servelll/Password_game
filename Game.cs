using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.ObjectModel;

namespace Password_game
{
    public class Game : INotifyPropertyChanged
    {
        ObservableCollection<Line> linesArrayList;
        public Solve s;
        string password;
        public int lenght;

        public event PropertyChangedEventHandler PropertyChanged;
        // usual OnPropertyChanged implementation
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Game()
        {
            s = new Solve();
            LinesArrayList = new ObservableCollection<Line>();
            lenght = -1;
        }
        public void Add(Line l)
        {
            if (l.TryNumbersArray1 == "")
            {
                System.Windows.MessageBox.Show("Строка неправильная");
                return;
            }
            LinesArrayList.Add(l);
            if (l.Lenght != lenght && lenght != -1)
            {
                throw new Exception("Разная длина");
            }
        }
        public void Print()
        {

        }
        public string GetLenght
        {
            get
            {
                if (lenght == 0)
                {
                    return "";
                }
                else
                {
                    return lenght.ToString();
                }
            }
            set
            {
                if (value.Count() <= 1 && value.All(s => char.IsDigit(s)) && value != "")
                {
                    lenght = Convert.ToInt32(value);
                    s.Fill_Clear_candidates(Convert.ToInt32(value));
                    OnPropertyChanged("GetLenght");
                }
                if (value == "")
                {
                    lenght = 0;
                    OnPropertyChanged("GetLenght");
                }
            }
        }
        public string GetCandidates
        {
            get
            {
                string str = "";
                foreach (var item in s.candidates)
                {
                    str += String.Join("", item.ToArray()) + " ";
                }
                return str + "| " + String.Join("", s.justNumbersArray.ToArray());
            }
            set
            {
                if (value == "")
                {
                    s.Fill_Clear_candidates(lenght);
                    OnPropertyChanged("GetCandidates");
                }
            }
        }

        public List<int>[] SetCandidates
        {
            get
            {
                return s.candidates;
            }
            set
            {
                s.candidates = value;
                OnPropertyChanged("SetCandidates");
            }
        }
        public string GetPassword {
            get
            {
                return password;
            }
            set
            {
                if (value.Count() <= lenght && value.All(s => char.IsDigit(s)))
                {
                    password = value;
                    s.Fill_Clear_candidates(lenght);
                    OnPropertyChanged("GetCandidates");
                    OnPropertyChanged("GetPassword");

                }
            }
        }

        public ObservableCollection<Line> LinesArrayList { get => linesArrayList; set { linesArrayList = value; OnPropertyChanged("LinesArrayList"); } }
    }

    public class Line
    {
        public int Lenght;
        public int[] TryNumbersArray;
        int RightNumbersValue;
        int RightNumbersAndPositionValue;
        public Line()
        {
            Lenght = -1;
            TryNumbersArray = new int[0];
            RightNumbersValue = -1;
            RightNumbersAndPositionValue = -1;
        }
        public Line(int l, int[] v, int rnm, int rnapv)
        {
            Lenght = l;
            TryNumbersArray = v;
            RightNumbersValue = rnm;
            RightNumbersAndPositionValue = rnapv;
        }

        public string Lenght1
        {
            get => Lenght.ToString(); set
            {
                Lenght = Convert.ToInt32(value);
            }
        }
        public string TryNumbersArray1
        {
            get
            {
                return String.Join("", TryNumbersArray);
            }
            set
            {
                TryNumbersArray = L.Func_conv(value);
            }
        }
        public int RightNumbersValue1
        {
            get => RightNumbersValue; set
            {
                RightNumbersValue = value;
            }
        }
        public int RightNumbersAndPositionValue1
        {
            get => RightNumbersAndPositionValue;
            set
            {
                RightNumbersAndPositionValue = value;
            }
        }

        public override string ToString()
        {
            return String.Join(" ", TryNumbersArray) + " | " + RightNumbersValue + " | " + RightNumbersAndPositionValue;
        }

    }

    public class Solve
    {
        public int[] numbers_pool;
        public List<int>[] candidates;
        public List<int> justNumbersArray;
        public double[][] stat_matrix;
        public Solve()
        {
            numbers_pool = new int[] { };
        }
        public void Fill_Clear_candidates(int l)
        {
            //полный массив кандидатов
            List<int> full_cand_mas = new List<int>();
            for (int j = 0; j < 10; j++)
            {
                full_cand_mas.Add(j);
            }

            //массивы массивов
            candidates = new List<int> [l];
            for (int i = 0; i < l; i++)
            {
                candidates[i] = new List<int> (full_cand_mas);
            }

            //массив кандидатов чисел
            justNumbersArray = full_cand_mas;

            //матрица статистики
            stat_matrix = new double[l][];
            for (int i = 0; i < l; i++)
            {
                stat_matrix[i] = new double[10];
            }
        }

    }
    public static class L {

        public static int[] Func_conv(string value)
        {
            int[] out_array;
            if (value.All(char.IsDigit))
            {
                if (value.Contains(' ') && value != "")
                {
                    out_array = value.Split(' ').Select(
                    x => (int.Parse(x) >= 0 && int.Parse(x) < 10) ? int.Parse(x) : -1
                    ).ToArray();
                }
                else
                {
                    out_array = value.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();
                }
            }
            else
            {
                out_array = new int[] { };
            }
            return out_array;
        }
    }

}
