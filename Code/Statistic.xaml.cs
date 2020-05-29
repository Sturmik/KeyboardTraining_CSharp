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

namespace StyleAndTemplates
{
    /// <summary>
    /// Логика взаимодействия для Statistic.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        List<UserStatistic> workWith;

        public Statistic(List<UserStatistic> toShow)
        {
            workWith = toShow;
            InitializeComponent();

            dataGridUserStatistic.ItemsSource = workWith;
        }
    }
}
