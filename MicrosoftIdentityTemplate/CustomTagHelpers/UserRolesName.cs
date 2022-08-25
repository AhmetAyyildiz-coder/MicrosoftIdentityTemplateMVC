using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;


using System.Linq;
using MicrosoftIdentityTemplate.Models;

namespace MicrosoftIdentityTemplate.CustomTagHelpers
{
    /// <summary>
    /// Bu sınıf bizim user'larımızın rollerinin yazılmasını sağlayan bir tag kullanmamızı sağlıyor. 
    /// Bu sayede tag kullanıp içerisine değer olarak id gönderince bu tag otomatik olarak rolleri çıktı vericek.
    /// </summary>
    //td tagı içerisindeki user-roles ismindeki tagı yakala diyoruz burada.
    [HtmlTargetElement("td", Attributes = "user-roles")]
    public class UserRolesName : TagHelper
    {
        public UserManager<CustomIdentityUser> UserManager { get; set; }

        public UserRolesName(UserManager<CustomIdentityUser> userManager)
        {
            this.UserManager = userManager;
        }


        /// <summary>
        /// Tag = value ile verdiğimiz ifadedeki value'yi temsil eden değişkenimiz.
        /// </summary>
        [HtmlAttributeName("user-roles")] 
        public string UserId { get; set; }

        /// <summary>
        /// User-roles tagına verdiğimiz değeri alıp o tag içerisine değer olarak basmamızı sağlayan metotdur.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output">Tag içerisine yazılacak değeri içeren parametre </param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var tagname=  context.TagName;

            CustomIdentityUser user = await UserManager.FindByIdAsync(UserId); //öncelikle gelen userId'ye göre user'ımızı bulduk.

            IList<string> roles = await UserManager.GetRolesAsync(user); //bir kullanıcının birden fazla rolü olabilir.

            string html = string.Empty;

            roles.ToList().ForEach(x =>
            {
                html += $"<span class='badge badge-info'>  {x}  </span>";
            });

            output.Content.SetHtmlContent(html); //tag içerisine yazılacak değeri ayarladık.
        }
    }
}
