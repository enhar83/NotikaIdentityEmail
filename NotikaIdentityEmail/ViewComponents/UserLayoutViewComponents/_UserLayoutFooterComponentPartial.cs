using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.UserLayoutViewComponents
{
    public class _UserLayoutFooterComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
