using System.Web.Mvc;
using MediaDB.Models;

public class AccountController : Controller
{
    private readonly AccountService accountService = new AccountService();

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        if (accountService.ValidateUser(username, password))
        {
            Session["Username"] = username;
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Error = "아이디 또는 패스워드를 확인해주세요.";
            return View();
        }
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public JsonResult Register(string Username, string Password, string ConfirmPassword)
    {
        var result = accountService.RegisterUser(Username, Password, ConfirmPassword);
        return Json(new { success = result.Success, message = result.Message });
    }
}
