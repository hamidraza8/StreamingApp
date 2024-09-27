using Core.DTOs.rating_vedioDTO;
using Core.DTOs.SearchDTO;
using Core.DTOs.WatchHistoryDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Services
{
    public interface IVedioUploadService
    {
        public Task<APIResponse> StoreVedio(Stream vedio,Guid uuid, string name, Category category, string canvas , string filename);
        public Task<APIResponse> Storeurl(string uuid, string Url);
        public Task<APIResponse> WatchHistoryAdd(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO);
        public Task<APIResponse> WatchHistoryUpdate(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO);
        public Task<APIResponse> RatingVedio(RatingVedioDTO request );
        public Task<APIResponse> GetAverageRating( string vedioId );
        public Task<APIResponse> GetVedioById( string vedioId );
        public Task<APIResponse> VedioSearch(VideoSearchRequest videoSearchRequest);
        public Task<APIResponse> ShowVedioList(string UserId);
        public Task<APIResponse> GetTopRatedTvshows();
        public Task<APIResponse> GetTopRatedMovies();
    }
    public class VedioUploadService(IVedioUploadRepository vedioUploadRepository) : IVedioUploadService
    {
        public Task<APIResponse> GetAverageRating(string vedioId)
        {
            return vedioUploadRepository.GetAverageRating(vedioId);
        }

        public Task<APIResponse> StoreVedio(Stream vedio, Guid uuid, string name, Category category, string canvas, string filename)
        {
            return vedioUploadRepository.VedioUpload(vedio, uuid, name, category, canvas, filename);
        }

        Task<APIResponse> IVedioUploadService.GetVedioById(string vedioId)
        {
            return vedioUploadRepository.GetVedioById(vedioId);
        }

        Task<APIResponse> IVedioUploadService.RatingVedio(RatingVedioDTO request)
        {
            return vedioUploadRepository.RatingVedio(request);
        }

        Task<APIResponse> IVedioUploadService.ShowVedioList(string UserId)
        {
            return vedioUploadRepository.ShowVedioList(UserId);
        }

        Task<APIResponse> IVedioUploadService.Storeurl(string uuid, string Url)
        {
            return vedioUploadRepository.VedioUrl(uuid, Url);
        }

        Task<APIResponse> IVedioUploadService.VedioSearch(VideoSearchRequest videoSearchRequest)
        {
            return vedioUploadRepository.VedioSearch(videoSearchRequest);
        }

        Task<APIResponse> IVedioUploadService.WatchHistoryAdd(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO)
        {
            return vedioUploadRepository.WatchHistoryAdd(userWatchHistoryRequestDTO);
        }

        Task<APIResponse> IVedioUploadService.WatchHistoryUpdate(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO)
        {
            return vedioUploadRepository.WatchHistoryUpdate(userWatchHistoryRequestDTO);
        }

        Task<APIResponse> IVedioUploadService.GetTopRatedTvshows()
        {
            return vedioUploadRepository.GetTopRatedTvshows();
        }
        Task<APIResponse> IVedioUploadService.GetTopRatedMovies()
        {
            return vedioUploadRepository.GetTopRatedMovies();
        }
    }
}
