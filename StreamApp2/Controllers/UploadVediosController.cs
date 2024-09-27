using Application.Services;
using Core.DTOs.rating_vedioDTO;
using Core.DTOs.SearchDTO;
using Core.DTOs.WatchHistoryDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Validaitor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Security.Cryptography;
using static Core.Models.VedioMetaData;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadVediosController(IVedioUploadService vedioUploadService) : ControllerBase
    {
        [HttpPost("StoreVideo")]
        //[Authorize]
        [RequestSizeLimit(1073741824)]
        public async Task<ActionResult<APIResponse>> StoreImagesByType(IFormFile video, Guid uuid, [FromForm] string name,
        [FromForm] Category category, [FromForm] string genre)
        {
            var filename = video.FileName;
            var response = await vedioUploadService.StoreVedio(video.OpenReadStream(), uuid, name, category, genre, filename);
            return response;
        }

        [HttpPost("Store-url")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> StoreURL(string filename, string url)
        {
            var response = await vedioUploadService.Storeurl(filename, url);
            return response;
        }
        [HttpPost("watch-history-add")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> WatchHistoryAdd(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO)
        {
            var response = await vedioUploadService.WatchHistoryAdd(userWatchHistoryRequestDTO);
            return response;
        }
        [HttpPost("watch-history-Update")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> WatchHistoryUpdate(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO)
        {
            var response = await vedioUploadService.WatchHistoryUpdate(userWatchHistoryRequestDTO);
            return response;
        }
        [HttpPost("rate-vedio")]
        public async Task<ActionResult<APIResponse>> RatingAdd(RatingVedioDTO request)
        {
            var validator = new RatingVedioValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage,
                });

                return BadRequest(new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = errorMessages.FirstOrDefault()?.Message,
                    Data = null
                });
            }
            var response = await vedioUploadService.RatingVedio(request);
            return response;
        }

        [HttpGet("average-rating")]
        public async Task<ActionResult<APIResponse>> GetAverageRating(string VedioId)
        {
            var response = await vedioUploadService.GetAverageRating(VedioId);
            return response;
        }

        [HttpGet("get-vedio-by-id")]
        public async Task<ActionResult<APIResponse>> GetVedioById(string id)
        {
            var response = await vedioUploadService.GetVedioById(id);
            return response;
        }
        [HttpGet("Search")]
        public async Task<ActionResult<APIResponse>> VedioSearch([FromQuery] VideoSearchRequest videoSearchRequest)
        {
            var response = await vedioUploadService.VedioSearch(videoSearchRequest);
            return response;
        }
        [HttpGet("show-vedio-list")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> ShowVedioList(string UserId)
        {
            var response = await vedioUploadService.ShowVedioList(UserId);
            return response;
        }

        [HttpGet("get-top-rated-movies")]
        public async Task<ActionResult<APIResponse>> GetTopRatedMovies()
        {
            var response = await vedioUploadService.GetTopRatedMovies();
            return response;
        }
        [HttpGet("get-top-rated-tvshows")]
        public async Task<ActionResult<APIResponse>> GetTopRatedTvshows()
        {
            var response = await vedioUploadService.GetTopRatedTvshows();
            return response;
        }
    }
}
