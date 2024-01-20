using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitAndFun.Models
{
    public class Routine
    {
        [Key]
    public int RoutineId { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public int Duration { get; set; }
    public DateTime Date { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Location { get; set; }
    public string ActivityType { get; set; }
    public string AdditionalDescription { get; set; }
}
}
