using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.UserLayoutViewComponents
{
    public class _UserLayoutBreadcombAreaComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
