using System.ComponentModel.DataAnnotations;

namespace Disha.Web.Models.ViewModels
{
    public class OnboardingViewModel
    {
        [Required(ErrorMessage = "Please select your education level.")]
        [Display(Name = "Current Education Level")]
        public string EducationLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your prior computer exposure.")]
        [Display(Name = "Prior Computer Exposure")]
        public string PriorComputerExposure { get; set; } = "none";

        [Required(ErrorMessage = "Please select your state.")]
        [Display(Name = "State / Union Territory")]
        public string State { get; set; } = string.Empty;

        [Display(Name = "Preferred Language")]
        public string LanguagePref { get; set; } = "en";
    }
}
