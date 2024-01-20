using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitAndFun.Models
{
    public class Nagrade
    {
        [Key]
    public int NagradeId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Value { get; set; }
    }
}
