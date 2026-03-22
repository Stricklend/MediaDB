using System.Web;
using System.Web.Mvc;
using MediaDB.Models;

namespace MediaDB.Controllers
{
    public class AlbumController : Controller
    {
        private readonly AlbumService albumService = new AlbumService();

        [HttpGet]
        public ActionResult AddAlbum()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAlbum(
        string Artist, string AlbumTitle, string Genre, string Label, string ReleaseDate,
        string AddedBy, string ModifiedBy, HttpPostedFileBase AlbumImage)
        {
            var result = albumService.AddAlbum(Artist, AlbumTitle, Genre, Label, ReleaseDate, AddedBy, ModifiedBy, AlbumImage, Server.MapPath);

            ViewBag.Message = result.Message;
            ViewBag.MessageColor = result.Success ? "blue" : "red";
            ViewBag.IsSuccess = result.Success;
            return View();
        }


        //20260301 add start
        [HttpGet]
        public ActionResult Detail(int id)
        {
            var album = albumService.GetAlbumDetail(id);
            if (album == null)
            {
                return HttpNotFound();
            }
        
            return View(album);
        }
        //20260301 add end

    }
}
