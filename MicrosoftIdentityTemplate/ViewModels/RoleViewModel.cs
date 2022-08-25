using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MicrosoftIdentityTemplate.ViewModels
{
    public class RoleViewModel
    {
        [Display(Name = "Role ismi")]
        [Required(ErrorMessage = "Role ismi gereklidir")]
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
