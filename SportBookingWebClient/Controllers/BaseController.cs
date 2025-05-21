using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SportBookingWebClient.Controllers
{
	public class BaseController : Controller
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var path = context.HttpContext.Request.Path.ToString().ToLower();

			// Bỏ qua kiểm tra nếu đang ở trang login
			if (path.Contains("Auth/Login"))
			{
				base.OnActionExecuting(context);
				return;
			}

			// Kiểm tra session
			var token = context.HttpContext.Session.GetString("Token");
			if (string.IsNullOrEmpty(token))
			{
				context.Result = new RedirectToRouteResult(new { controller = "Login", action = "Login" });
			}

			base.OnActionExecuting(context);
		}
	}
}
