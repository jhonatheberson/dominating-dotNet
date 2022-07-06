using System;

namespace BaltaDataAcces.Models
{
  public class Student
  {
    public Guid Id { get; set; }
    public String Name { get; set; }
    public String Email { get; set; }
    public String Document { get; set; }
    public String Phone { get; set; }
    public DateTime Birthdate { get; set; }
    public DateTime CreateDate { get; set; }
  }

}