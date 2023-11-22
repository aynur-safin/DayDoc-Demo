using System.ComponentModel.DataAnnotations;

namespace DayDoc.Web.Models
{
    public class Setting
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        [Display(Name = "Своя компания")]
        public int OwnCompanyId { get; set; }
        public virtual Company? OwnCompany { get; set; }

        [Display(Name = "Работа")]
        public string? WorkName { get; set; }
    }

    public class SettingValidator : AbstractValidator<Setting>
    {
        public SettingValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.OwnCompanyId)
                .NotNull()
                .NotEmpty();
        }
    }
}
