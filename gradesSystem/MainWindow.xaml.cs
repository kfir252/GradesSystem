using gradesSystem.Models;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace gradesSystem
{
    public partial class MainWindow : Window
    {
        //UI Objects
        List<TextBox> gradeBoxes;
        List<Label> gradeTitleLable;
        
        //courent file and course that shown
        Course? courentCourse;
        string? fileName;
        List<string> fileNames;


        public MainWindow()
        {
            //Initializ Components
            InitializeComponent();

            //set UI-Objects in a lists
            gradeTitleLable = new List<Label>() { T0, T1, T2, T3, T4, T5, T6, T7, T8, T9 };
            gradeBoxes = new List<TextBox>() { G0, G1, G2, G3, G4, G5, G6, G7, G8, G9 };
            
            //clear the UI
            UserDataClear();
            UserGradeClear();
            FactorButton.Visibility = Visibility.Collapsed;
            saveFactorMessage.Visibility = Visibility.Collapsed;


            //Setup The DropDown for fileNames.json
            try
            {
                var f = File.ReadAllText("fileNames.json");
                fileNames = System.Text.Json.JsonSerializer.Deserialize<List<string>>(f)!;
            }
            catch 
            {
                fileNames = new List<string>();
                return;
            }
            
            foreach (var nv in fileNames)
            {
                //Abale to Factor
                FactorButton.Visibility = Visibility.Visible;

                //Add to DropDownList
                var item = new ComboBoxItem { Content = nv };
                DropDown.Items.Add(item);
                DropDown.SelectedItem = item;
            }
        }

        private void LoadNewCourseOnClick(object sender, RoutedEventArgs e)
        {
            //Open The FileDialog Asking For {.csv} File
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "[CSV files] (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                //Get The Needed Data About The File
                string filePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                //write the path, and change the title
                FilePathTextBox.Text = filePath;
                Title = fileName;

                //Convert The CSV File To New Json File
                courentCourse = Course.ReadCSV(FilePathTextBox.Text);
                string usersJsonText = System.Text.Json.JsonSerializer.Serialize<Course>(courentCourse);
                
                if (getDataplusFileName(fileName!) != "")
                    File.Delete(getDataplusFileName(fileName!));
                string date = DateTime.Now.ToString("dd-MM-yyyy-h-mm-ss-tt ");
                File.WriteAllText(date + fileName + ".json", usersJsonText);

                //Add the Course to the DropDown (and select it)
                if (fileNames.Contains(fileName))
                {
                    DropDown.SelectedValue = fileName;
                    UserGradesfill();
                    return;
                }

                var item = new ComboBoxItem { Content = fileName };
                DropDown.Items.Add(item);
                DropDown.SelectedItem = item;
                fileNames.Add(fileName);
                File.WriteAllText("fileNames.json", System.Text.Json.JsonSerializer.Serialize<List<string>>(fileNames));

                //abale to Factor
                FactorButton.Visibility = Visibility.Visible;

            }
        }

        private void DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get The Name Of The Selected Course
            fileName = DropDown.SelectedValue.ToString();
            Title = fileName;


            //Deserialize The Json File Of The Course
            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(getDataplusFileName(fileName!)))!;

            //Deserialize the titles of the asingments
            var Titles = new List<string>();
            var titlesTokens = ((JArray)jsonObject.tasksTitles).ToList();
            Titles.AddRange(titlesTokens.Select(x => x.ToObject<string>())!);

            //Deserialize the students
            var studentGroup = new List<CourseStudent>();
            foreach (var stude in jsonObject.students)
                studentGroup.Add(new CourseStudent((string)stude.FirstName, (string)stude.LastName, (string)stude.ID, (string)stude.Year, ((JArray)stude.Grades).Select(g => (string)g).ToList()!));

            //Deserialize the Course
            courentCourse = new Course((string)jsonObject.Name, Titles, studentGroup);

            //Update Students List
            StudentsList.ItemsSource = courentCourse.students;

            //Update The Class Grade
            calculateClassGrade();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If Nothing Is Selected Than Clear:
            if(StudentsList.SelectedIndex < 0)
            {
                UserDataClear();
                UserGradeClear();
                return;
            }
            UserDataUpdete();//write his data.
            UserGradesfill();//write his grades.
        }

        private void SaveJsonClick(object sender, RoutedEventArgs e)
        {
            //get the student we are updating his grades
            var s = courentCourse!.students[StudentsList.SelectedIndex];

            //get data from gradeboxs to courentCourse
            for (int i = 0; i < s.Grades.Count; i++)
            {
                if (!(int.TryParse(gradeBoxes![i].Text, out int n) && int.Parse(gradeBoxes![i].Text) <= 100 && int.Parse(gradeBoxes![i].Text) >= 0))
                    gradeBoxes![i].Text = "0";
                s.Grades[i] = gradeBoxes![i].Text;
            }

            //Save The New Data From courentCourse To Json
            string usersJsonText = System.Text.Json.JsonSerializer.Serialize<Course>(courentCourse);
            File.Delete(getDataplusFileName(fileName!));
            string date = DateTime.Now.ToString("dd-MM-yyyy-h-mm-ss-tt ");
            File.WriteAllText(date+fileName + ".json", usersJsonText);

            //Calculate Grades
            calculateClassGrade();
            calculateCourentStudentGrade();
            saveFactorMessage.Visibility = Visibility.Collapsed;
        }

        private void FactorOnClick(object sender, RoutedEventArgs e)
        {
            if (courentCourse != null)
            {
                factor factorwin = new factor(courentCourse!);
                factorwin.ShowDialog();
                UserGradesfill();

                //YOU HAVE TO PRESS THE SAVEJSON BUTTON TO MAKE SURE
                //YOU ARE REALLY READY TO CHANGE THE GRADES OF ALL THE STUDENTS 
                saveFactorMessage.Visibility = Visibility.Visible;
            }
        }


        #region User Sections UIs
        private void UserDataUpdete()
        {
            //get the selected student
            var s = courentCourse!.students[StudentsList.SelectedIndex];

            // write the his information
            UserFullName.Content = s.FirstName + " " + s.LastName;
            UserID.Content = "StudentID: " + s.ID.ToString();
            UserYear.Content = "StudentYear: " + s.Year.ToString();
        }
        private void UserGradesfill()
        {
            if (StudentsList.SelectedIndex < 0)
                return;
            //get the selected student
            var s = courentCourse!.students[StudentsList.SelectedIndex];
            
            //run on each gradeBoxes and fill it with 
            int i;
            for ( i = 0; i < s.Grades.Count; i++)
            {
                gradeTitleLable![i].Content = courentCourse.tasksTitles[i];
                gradeBoxes[i].Visibility = Visibility.Visible;
                gradeBoxes[i].Text = s.Grades[i];

                //Validation
                if (!(int.TryParse(gradeBoxes![i].Text, out int n) && int.Parse(gradeBoxes![i].Text) <= 100 && int.Parse(gradeBoxes![i].Text) >= 0))
                    gradeBoxes[i].Text = "0";
            }

            //Hide All The Leftovers
            while (i < 10)
            {
                gradeBoxes![i].Visibility = Visibility.Collapsed;
                gradeTitleLable![i].Content = "";
                i++;
            }

            //Show She SaveJson-Button
            SaveJson.Visibility = Visibility.Visible;

            //Show The Student Final Grade
            calculateCourentStudentGrade();
            StudentFinalGradeLable.Visibility = Visibility.Visible;
            StudentFinalGradeTextBox.Visibility = Visibility.Visible;
        }


        private void UserGradeClear()
        {
            //clear the grade section
            int i = 0;
            while (i < 10)
            {
                gradeBoxes[i].Visibility = Visibility.Collapsed;
                gradeTitleLable[i].Content = "";
                i++;
            }
            //Hide StudentFinalGrade Area
            StudentFinalGradeTextBox.Visibility = Visibility.Collapsed;
            StudentFinalGradeLable.Visibility = Visibility.Collapsed;
            SaveJson.Visibility = Visibility.Collapsed;
        }
        private void UserDataClear()
        {
            UserFullName.Content = "";
            UserID.Content = "";
            UserYear.Content = "";
        }
        private void calculateCourentStudentGrade()
        {
            StudentFinalGradeTextBox.Text = courentCourse!.getStudentsGrade(StudentsList.SelectedIndex).ToString();
        }
        private void calculateClassGrade()
        {
            //sum the grades of the class
            float sum = 0;
            for (int i = 0; i < courentCourse.students.Count; i++)
            {
                sum += courentCourse.getStudentsGrade(i);
            }

            //write the ClassGrade
            ClassGrade.Content = fileName + " (" + (sum / courentCourse.students.Count).ToString() + ")";
        }

        private string getDataplusFileName(string courseName)
        {
            string path = Directory.GetCurrentDirectory();
            var fileslist = Directory.GetFiles(path);
            foreach (var filee in fileslist)
            {
                if (filee.EndsWith(fileName + ".json"))
                {
                    return filee;
                }
            }
            return "";
        }
        #endregion


    }
}