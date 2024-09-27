using Core.DTOs.rating_vedioDTO;
using Core.DTOs.SearchDTO;
using Core.DTOs.WatchHistoryDTOs;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IVedioUploadRepository
    {
        public Task<APIResponse> VedioUpload(Stream vedio,Guid uuid, string name, Category category, string canvas, string filename);
        public Task<APIResponse> VedioUrl(string uuid, string Url);
        public Task<APIResponse> VedioSearch(VideoSearchRequest videoSearchRequest);
        public Task<APIResponse> WatchHistoryAdd(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO);
        public Task<APIResponse> WatchHistoryUpdate(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO);
        public Task<APIResponse> RatingVedio(RatingVedioDTO request);
        public Task<APIResponse> ShowVedioList(string userID);
        public Task<APIResponse> GetTopRatedMovies();
        public Task<APIResponse> GetTopRatedTvshows();
        public Task<APIResponse> GetAverageRating(string vedioId);
        public Task<APIResponse> GetVedioById(string vedioId);
    }
}
