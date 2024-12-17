using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication20.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maSV = context.HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSV))
            {
                context.Result = new RedirectToActionResult("DangNhap", "NguoiDung", null);
            }
        }
    }
}