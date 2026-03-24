using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.UserLayoutViewComponents
{
    public class _UserLayoutHeadComponenPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
