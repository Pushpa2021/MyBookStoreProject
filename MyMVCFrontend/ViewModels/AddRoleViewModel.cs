using System.ComponentModel.DataAnnotations;

namespace MyMVCFrontend.ViewModels
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
