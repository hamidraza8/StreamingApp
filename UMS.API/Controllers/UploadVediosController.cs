using Application.Services;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using static Core.Models.VedioMetaData;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadVediosController(IVedioUploadService vedioUploadService) : ControllerBase
    {
        [HttpPost("StoreVideo")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> StoreImagesByType(IFormFile video, Guid uuid, [FromForm] string name,
        [FromForm] Category category, [FromForm] string genre)
        {
           var filename = video.FileName;
            var response = await vedioUploadService.StoreVedio(video.OpenReadStream(), uuid, name, category, genre, filename);
            return response;
        }

        [HttpPost("Store-url")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> StoreURL( string filename ,string url)
        {
            var response = await vedioUploadService.Storeurl(filename,url);
            return response;
        }
    }
}
