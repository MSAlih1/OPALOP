using System.Web.Http;

namespace Api.Controllers
{
    public interface IActionController
    {
        IHttpActionResult DownloadInstaPhotos();
        IHttpActionResult GetInstaPhotos();
        IHttpActionResult ImageFormats();
        IHttpActionResult ImageGenerate();
        IHttpActionResult ImageGet();
        IHttpActionResult ImageRemove();
        IHttpActionResult ImagesShow();
        IHttpActionResult SelectedInstaPhotos([FromBody] string ls);
        void UploadResource();
        bool UsersLogged();
    }
}