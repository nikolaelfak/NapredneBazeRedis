using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitAndFun.Models
{
    public class Comment
{
    public int CommentId { get; set; }
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}
}