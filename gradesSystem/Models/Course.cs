using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gradesSystem.Models
{
    public class Course
    {
        public string Name { get; set; }
        public List<string> tasksTitles{ get; set; }
        public List<CourseStudent> students{ get; set; }


        public Course(string Name, List<string> tasksTitles, List<CourseStudent> students)
        {
            this.Name = Name;
            this.tasksTitles = tasksTitles; 
            this.students = students.OrderBy(o => o.FirstName).ToList();   
        }

        public float getStudentsGrade(int index)
        {
            CourseStudent s = students[index];

            int grade = 0;
            int gradeindex = 0;
            foreach (var title in tasksTitles)
            {
                string weightStr = title.Split(" - ")[1];
                int weight = int.Parse(weightStr.Remove(weightStr.Length - 1));
                if (int.TryParse(s.Grades[gradeindex], out int n))
                {
                    int g = int.Parse(s.Grades[gradeindex]);
                    if (g > 0 && g <= 100)
                        grade += weight * int.Parse(s.Grades[gradeindex]);
                }

                gradeindex += 1;
            }
            return (float)grade /100;
        }

        public static Course ReadCSV(string filePath)
        {
            //get all the lines of the CSV Table
            string[] linesArr = File.ReadAllLines(System.IO.Path.ChangeExtension(filePath, ".csv"));
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            List<string> lines = new List<string>(linesArr);


            //seperate the first line of the table
            string DataTitles = lines[0];
            List<string> titles = new List<string>(DataTitles.Split(','));
            titles.RemoveAt(0); titles.RemoveAt(0); titles.RemoveAt(0); titles.RemoveAt(0);
            lines.RemoveAt(0);


            //create list of student classes.
            List<CourseStudent> studs = new List<CourseStudent>();
            foreach (string line in lines)
            {
                List<string> data = new List<string>(line.Split(','));
                string firstName = data[0]; data.RemoveAt(0);
                string lastName = data[0]; data.RemoveAt(0);
                string id = data[0]; data.RemoveAt(0);
                string year = data[0]; data.RemoveAt(0);
                studs.Add(new CourseStudent(firstName, lastName, id, year, data));
            }

            //create/edit json file
            return (new Course(fileName, titles, studs));
        }
    }
}
