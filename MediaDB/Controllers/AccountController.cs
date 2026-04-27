using System.Linq;
using System.Web.Mvc;
using MediaDB.Models;

public class AccountController : Controller
{
    private readonly AccountService accountService = new AccountService();

    public ActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = GetFirstModelError();
            return View(model);
        }

        var user = accountService.AuthenticateUser(model.user_id, model.user_password);

        if (user != null)
        {
            var displayName = string.IsNullOrWhiteSpace(user.Username) ? user.UserId : user.Username;

            Session["user_id"] = user.UserId;
            Session["UserId"] = user.UserId;
            Session["Username"] = displayName;
            Session["UserEmail"] = user.Email;
            Session["UserPk"] = user.Id;
            Session["UserCreatedAt"] = user.CreatedAt;
            Session["UserUpdatedAt"] = user.UpdatedAt;

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Please check your ID or password.";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public ActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new
            {
                success = false,
                message = GetFirstModelError()
            });
        }

        var result = accountService.RegisterUser(model.user_id, model.user_password, model.confirm_user_password, model.username, model.email);

        return Json(new
        {
            success = result.Success,
            message = result.Message
        });
    }

    private string GetFirstModelError()
    {
        return ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message))
            ?? "Please check the submitted values.";
    }
}
