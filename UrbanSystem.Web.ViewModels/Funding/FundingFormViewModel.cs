using System.ComponentModel.DataAnnotations;
using static UrbanSystem.Common.ValidationStrings.Funding;

namespace UrbanSystem.Web.ViewModels.Funding
{
    public class FundingFormViewModel
    {
        [Required]
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; } = null!;

        public decimal FundsNeeded { get; set; }

        public decimal FundsRaised { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = AmountGreaterThanZero)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = ConfirmationDisplayName)]
        public bool IsConfirmed { get; set; }
    }
}
