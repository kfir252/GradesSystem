using gradesSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gradesSystem
{
    public partial class factor : Window
    {
        Course courentCourse;
        bool changed;

        public factor(Course c)
        {
            InitializeComponent();
            changed = false;
            courentCourse = c;
            assignmentsList.ItemsSource = courentCourse.tasksTitles;
            Title = "AddFactor - " + courentCourse.Name;
        }

        private void addFactor(object sender, RoutedEventArgs e)
        {
            //If Nothing Is Selected Than Clear:
            if (assignmentsList.SelectedIndex < 0)
                return;

            if (int.TryParse(bonusPointsTextBox.Text, out int n))
            {
                foreach (var stud in courentCourse.students)
                {
                    var was = stud.Grades[assignmentsList.SelectedIndex];
                    if (!(int.TryParse(was, out int v) && int.Parse(was) <= 100 && int.Parse(was) >= 0))
                        was = "0";
                    stud.Grades[assignmentsList.SelectedIndex] = (Math.Min(int.Parse(was) + int.Parse(bonusPointsTextBox.Text), 100)).ToString();
                }
                changed = true;
                Close();
            }
            else
                bonusPointsTextBox.Text = "EnterNumberHere";
        }

        public bool Changed
        {
            get { return changed; }
        }

    }
}
