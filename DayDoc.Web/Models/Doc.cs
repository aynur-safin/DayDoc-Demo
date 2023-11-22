using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DayDoc.Web.Models
{
    public class Doc
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;

        public DocType DocType { get; set; } = DocType.Akt;

        [Required]
        public int OwnCompanyId { get; set; }
        public virtual Company? OwnCompany { get; set; }

        [Required]
        public int ContragentId { get; set; }
        public virtual Company? Contragent { get; set; }

        [Required]
        public string Num { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now.Date;

        [Precision(19, 4)] // sql server money type analog
        public decimal Sum { get; set; } = 0;

        public string? Dogovor_Num { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Dogovor_Date { get; set; }

        public string? WorkName { get; set; }

        public virtual List<XmlDoc> XmlDocs { get; set; } = new();
    }

    public class DocValidator : AbstractValidator<Doc>
    {
        public DocValidator()
        {
            RuleFor(x => x.OwnCompanyId)
                .NotNull()
                .NotEmpty(); ;

            RuleFor(x => x.ContragentId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Num)
                .NotEmpty();

            RuleFor(x => x.Date)
                .NotEmpty();

            RuleFor(x => x.Sum)
                .NotEmpty();

            //RuleFor(x => x.INN)
            //    .Length(10, 12).When(s => !string.IsNullOrEmpty(s.INN));
        }
    }
}