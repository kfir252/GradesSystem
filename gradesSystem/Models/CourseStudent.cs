using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gradesSystem.Models
{
    public class CourseStudent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ID { get; set; }
        public int Year{ get; set; }
        public List<string> Grades { get; set; }



        //what i use to convert from database
        public CourseStudent(string firstName, string lastName, string id, string year, List<string> grades)
        {
            this.ID = Convert.ToInt32(id);
            this.Year = Convert.ToInt32(year);
            this.FirstName = firstName; 
            this.LastName = lastName;
            this.Grades = grades;
        }

        //what i use to convert from database
        public CourseStudent(string FirstName, string LastName, int ID, int Year, List<string> Grades)
        {
            this.ID = ID;
            this.Year = Year;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Grades = Grades;
        }

        public override string ToString(){ 
            return FirstName + " " + LastName;
        }
    }
}
