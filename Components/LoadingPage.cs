using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQuanLyLamViecNhom.Components
{
    public class LoadingPage : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
