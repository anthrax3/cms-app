using System;
using System.ComponentModel.DataAnnotations;

// created by Charles Drews
namespace CMS_App.Models
{
    public class SelectedOption
    {
        [Required(ErrorMessage = "Please select a rating")]
        public Guid ResponseOptionId { get; set; }

        public SelectedOption()
        {
            ResponseOptionId = Guid.Empty;
        }

        public SelectedOption(Guid responseOptionId)
        {
            ResponseOptionId = responseOptionId;
        }
    }
}