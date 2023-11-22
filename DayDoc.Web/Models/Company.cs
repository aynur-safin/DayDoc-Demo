using System.ComponentModel.DataAnnotations;

namespace DayDoc.Web.Models
{
    public class Company
    {
        [Required]
        [Display(Name = "№")]
        public int Id { get; set; }

        [Required]
        public CompType CompType { get; set; } = CompType.Customer;

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; } = "";

        public string? Description { get; set; } = null;

        //[MaxLength(12)]
        [StringLength(12, MinimumLength = 10)]
        [Display(Name = "ИНН")]
        public string? INN { get; set; } = null;

        //[MaxLength(9)]
        [StringLength(9, MinimumLength = 9)]
        [Display(Name = "КПП")]
        public string? KPP { get; set; } = null;

        //[MaxLength(15)]
        [StringLength(15, MinimumLength = 13)]
        [Display(Name = "ОГРН №")]
        public string? OGRN { get; set; }

        [Display(Name = "ОГРН Дата")]
        [DataType(DataType.Date)]
        public DateTime? OGRN_Date { get; set; }

        [Display(Name = "ЭДО ID")]
        public string? EdoId { get; set; }

        [Display(Name = "Оператор ЭДО")]
        public int? EdoCompanyId { get; set; }
        public virtual Company? EdoCompany { get; set; }

        /* Подписант (Signatory) */
        [Display(Name = "Должность подписанта")]
        public string? Signatory_Position { get; set; }
        [Display(Name = "Основания полномочий подписанта")]
        public string? Signatory_Basis { get; set; } = "Должностные обязанности";
        [Display(Name = "Имя")]
        public string? Signatory_FirstName { get; set; }
        [Display(Name = "Отчество")]
        public string? Signatory_MiddleName { get; set; }
        [Display(Name = "Фамилия")]
        public string? Signatory_LastName { get; set; }

        [Display(Name = "Адрес")]
        public string? Address { get; set; }
    }

    public class CompanyValidator : AbstractValidator<Company>
    {
        public CompanyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.INN)
                .Length(10, 12).When(s => !string.IsNullOrEmpty(s.INN));

            RuleFor(x => x.KPP)
                .Length(9, 9).When(s => !string.IsNullOrEmpty(s.KPP));

            RuleFor(x => x.OGRN)
                .Length(13, 15).When(s => !string.IsNullOrEmpty(s.OGRN));
        }
    }
}
