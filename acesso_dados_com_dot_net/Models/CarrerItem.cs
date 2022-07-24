using System;
namespace BaltaDataAcces.Models
{
  public class CarrerItem
  {
    public Guid id { get; set; }
    public string Title { get; set; }

    public Course Course {get; set;}
  }
}