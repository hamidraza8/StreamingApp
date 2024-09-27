using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Helper;
using Core.DTOs.rating_vedioDTO;
using Core.DTOs.SearchDTO;
using Core.DTOs.WatchHistoryDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Models.VedioMetaData;

namespace Infrastructure.Repositories
{
    public class VedioUploadRepository(UMSDbContext _context, ICurrentUserService currentUser) : IVedioUploadRepository
    {
        public async Task<APIResponse> VedioUpload(Stream vedio, Guid uuid, string name, Category category, string genre, string filename)
        {
            try
            {

                uuid = Guid.NewGuid();
                var key = $"{uuid}_{filename}";
                var url = await UploadVideoAsync(vedio, key);
                if (url.ApiCode == 0)
                {
                    // Save metadata to database
                    var videoMetadata = new VideoMetadata
                    {
                        UUID = uuid,
                        Name = name,
                        Category = category.ToString(),
                        Genre = genre,
                        Filename = key,
                        Url = "" // or null if you're updating later via notification API
                    };

                    _context.VideoMetadatas.Add(videoMetadata);
                    await _context.SaveChangesAsync();
                    return new APIResponse
                    {
                        ApiCode = 0,
                        DisplayMessage = "Video uploaded successfully!",
                        Data = videoMetadata
                    };
                }
                else
                {
                    return new APIResponse
                    {
                        ApiCode = url.ApiCode,
                        DisplayMessage = url.DisplayMessage,
                        Data = ""
                    };
                }
            }
            catch (Exception ex)
            {

                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = " Error while uploading video.",
                    Data = ex.Message
                };
            }

        }
        public async Task<APIResponse> UploadVideoAsync(Stream videoStream, string key)
        {
            try
            {
                // Convert Stream to Base64 string
                string base64Video;
                using (var memoryStream = new MemoryStream())
                {
                    await videoStream.CopyToAsync(memoryStream);
                    base64Video = Convert.ToBase64String(memoryStream.ToArray());
                }

                // Prepare HttpClient
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("filename", key);

                    var content = new StringContent($"\"{base64Video}\"", Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://gii566xzjux7mtcqpfh5gdcyn40ylvsx.lambda-url.us-east-1.on.aws/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return new APIResponse
                        {
                            ApiCode = 0,
                            DisplayMessage = "Video uploaded successfully!",
                            Data = responseContent // assuming this returns the URL or some other data
                        };
                    }
                    else
                    {
                        return new APIResponse
                        {
                            ApiCode = 99,
                            DisplayMessage = response.ReasonPhrase,
                            Data = response.ReasonPhrase
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while uploading video.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> VedioUrl(string filename, string url)
        {
            try
            {
                var uuid = filename.Split('/')[0];
                var vedios = await _context.VideoMetadatas.FindAsync(Guid.Parse(uuid));
                if (vedios != null)
                {
                    vedios.Url = url;
                    _context.VideoMetadatas.Update(vedios);
                    await _context.SaveChangesAsync();
                    return new APIResponse
                    {
                        ApiCode = 0,
                        DisplayMessage = "Video Url Updated Successfully",
                        Data = vedios
                    };
                }
                else
                {
                    return new APIResponse
                    {
                        ApiCode = 99,
                        DisplayMessage = "Video Not Found",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Video Not Found",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> WatchHistoryAdd(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO)
        {
            try
            {
                var watchHistory = new UserWatchHistory
                {
                    Id = Guid.NewGuid(), // Assign a new ID
                    UserId = userWatchHistoryRequestDTO.UserId,
                    VideoId = userWatchHistoryRequestDTO.VideoId,
                    WatchedOn = userWatchHistoryRequestDTO.WatchedOn,
                    WatchDuration = userWatchHistoryRequestDTO.WatchDuration,
                    IsCompleted = userWatchHistoryRequestDTO.IsCompleted
                };
                _context.UserWatchHistories.Add(watchHistory);
                watchHistory.Created = DateTime.Now;
                watchHistory.Updated = DateTime.Now;
                watchHistory.CreatedBy = currentUser.Name;
                watchHistory.UpdatedBy = currentUser.Name;
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Watch history added successfully.",
                    Data = watchHistory
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = $"An error occurred while adding watch history: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIResponse> WatchHistoryUpdate(UserWatchHistoryRequestDTO userWatchHistoryRequestDTO)
        {
            try
            {
                // Find the existing watch history by its Id
                var existingWatchHistory = await _context.UserWatchHistories.FindAsync(userWatchHistoryRequestDTO.Id);

                if (existingWatchHistory == null)
                {
                    return new APIResponse
                    {
                        ApiCode = 1,
                        DisplayMessage = "Watch history not found.",
                        Data = null
                    };
                }

                // Update the watch history fields
                existingWatchHistory.UserId = userWatchHistoryRequestDTO.UserId;
                existingWatchHistory.VideoId = userWatchHistoryRequestDTO.VideoId;
                existingWatchHistory.WatchedOn = userWatchHistoryRequestDTO.WatchedOn;
                existingWatchHistory.WatchDuration = userWatchHistoryRequestDTO.WatchDuration;
                existingWatchHistory.IsCompleted = userWatchHistoryRequestDTO.IsCompleted;
                existingWatchHistory.Updated = DateTime.Now;
                existingWatchHistory.UpdatedBy = currentUser.Name;
                // Save changes to the database
                _context.UserWatchHistories.Update(existingWatchHistory);
                await _context.SaveChangesAsync();

                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Watch history updated successfully.",
                    Data = existingWatchHistory
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = $"An error occurred while updating watch history: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIResponse> RatingVedio(RatingVedioDTO request)
        {
            try
            {


                var existingRating = await _context.RatingVedios
                    .FirstOrDefaultAsync(vr => vr.VideoId == request.VideoId && vr.UserId == request.UserId);
                if (existingRating != null)
                {
                    existingRating.Rating = request.Rating;
                    existingRating.RatedAt = DateTime.UtcNow;
                }
                else
                {
                    var newRating = new RatingVedio
                    {
                        VideoId = request.VideoId,
                        UserId = request.UserId,
                        Rating = request.Rating,
                        RatedAt = DateTime.UtcNow
                    };
                    _context.RatingVedios.Add(newRating);
                }
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Rating submitted successfully.",
                    Data = null
                };

            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while submitting rating.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> GetAverageRating(string vedioId)
        {
            try
            {
                Guid videoGuid = Guid.Parse(vedioId);

                // Check if any ratings exist for the video
                bool hasRatings = await _context.RatingVedios
                    .AnyAsync(vr => vr.VideoId == videoGuid);

                if (!hasRatings)
                {
                    return new APIResponse
                    {
                        ApiCode = 1,
                        DisplayMessage = "No ratings found for the specified video.",
                        Data = null
                    };
                }

                // Calculate the average rating
                var averageRating = await _context.RatingVedios
                    .Where(vr => vr.VideoId == videoGuid)
                    .AverageAsync(vr => vr.Rating);

                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Average rating retrieved successfully.",
                    Data = averageRating
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while retrieving average rating.",
                    Data = ex.Message
                };
            }
        }


        public async Task<APIResponse> GetVedioById(string vedioId)
        {
            try
            {


                var video = await _context.VideoMetadatas
               .FirstOrDefaultAsync(v => v.UUID == Guid.Parse(vedioId));

                if (video == null)
                {
                    return new APIResponse
                    {
                        ApiCode = 1,  // Use a non-zero value to indicate an error
                        DisplayMessage = "Video not found.",
                        Data = null
                    };
                }
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Video retrieved successfully.",
                    Data = video
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while retrieving video.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> VedioSearch(VideoSearchRequest request)
        {
            try
            {
                var query = _context.VideoMetadatas.AsQueryable();

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(v => v.Name.Contains(request.Name));
                }

                if (!string.IsNullOrEmpty(request.Genre))
                {
                    query = query.Where(v => v.Genre.Contains(request.Genre));
                }

                if (!string.IsNullOrEmpty(request.Category))
                {
                    query = query.Where(v => v.Category.Contains(request.Category));
                }

                var videos = await query.ToListAsync();

                if (videos.Count == 0)
                {
                    return new APIResponse
                    {
                        ApiCode = 1,
                        DisplayMessage = "No videos found matching the search criteria.",
                        Data = null
                    };
                }

                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Videos retrieved successfully.",
                    Data = videos
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while retrieving videos.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> ShowVedioList(string userID)
        {
            Guid userId = Guid.Parse(userID); // Parse the user ID
            try
            {
                // Step 1: Get the most watched genres/categories from the user's watch history
                var userInterests = await _context.UserWatchHistories
                    .Where(uw => uw.UserId == userId)
                    .GroupBy(uw => new { uw.Video.Genre, uw.Video.Category })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new
                    {
                        g.Key.Genre,
                        g.Key.Category,
                        WatchCount = g.Count()
                    })
                    .Take(20)  // Get top 20 interests
                    .ToListAsync();  // Materialize the user interests in memory

                // Step 2: Extract the user's preferred genres and categories
                var genres = userInterests.Select(ui => ui.Genre).ToList();
                var categories = userInterests.Select(ui => ui.Category).ToList();

                // Step 3: Suggest videos based on the user's interests, excluding already watched videos
                var suggestedVideos = await _context.VideoMetadatas
                    .Where(vm => (genres.Contains(vm.Genre) || categories.Contains(vm.Category))
                                 && !_context.UserWatchHistories.Any(uw => uw.UserId == userId && uw.VideoId == vm.UUID))  // Exclude already watched videos
                    .ToListAsync();

                // Step 4: If no suggestions are found, retrieve the top uploaded videos
                if (!suggestedVideos.Any())
                {
                    // Get top uploaded videos based on the creation date
                    var topUploadedVideos = await _context.VideoMetadatas
                        .OrderByDescending(vm => vm.Created) // Assuming 'Created' is the upload date
                        .Take(20) // Get top 20 most recent uploads
                        .ToListAsync();

                    if (!topUploadedVideos.Any())
                    {
                        return new APIResponse
                        {
                            ApiCode = 1,
                            DisplayMessage = "No videos available at the moment.",
                            Data = null
                        };
                    }

                    return new APIResponse
                    {
                        ApiCode = 0,
                        DisplayMessage = "Top uploaded videos retrieved successfully.",
                        Data = topUploadedVideos
                    };
                }

                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Video suggestions retrieved successfully.",
                    Data = suggestedVideos
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while retrieving video suggestions.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> GetTopRatedMovies()
        {
            try
            {
                // Query to get top-rated movies
                var topRatedMovies = await _context.VideoMetadatas
                    .Where(vm => vm.Category == "Movies") // Filter by "Movies"
                    .Join(_context.RatingVedios,
                          vm => vm.UUID,
                          rv => rv.VideoId,
                          (vm, rv) => new { vm, rv.Rating })
                    .GroupBy(x => new { x.vm.UUID, x.vm.Name, x.vm.Url }) // Group by video metadata
                    .Select(g => new
                    {
                        g.Key.UUID,
                        g.Key.Name,
                        g.Key.Url,
                        AverageRating = g.Average(x => x.Rating) // Calculate average rating
                    })
                    .OrderByDescending(x => x.AverageRating) // Sort by highest rating
                    .Take(20) // Limit to top 20
                    .ToListAsync();

                // If no top-rated movies exist, return latest uploaded movies
                if (!topRatedMovies.Any())
                {
                    var latestMovies = await _context.VideoMetadatas
                        .Where(vm => vm.Category == "Movies")
                        .OrderByDescending(vm => vm.Created) // Sort by upload date (Created)
                        .Take(20) // Limit to top 20
                        .ToListAsync();

                    if (!latestMovies.Any())
                    {
                        return new APIResponse
                        {
                            ApiCode = 1,
                            DisplayMessage = "No movies available at the moment.",
                            Data = null
                        };
                    }

                    return new APIResponse
                    {
                        ApiCode = 0,
                        DisplayMessage = "No top-rated movies found. Showing latest uploaded movies.",
                        Data = latestMovies
                    };
                }

                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Top-rated movies retrieved successfully.",
                    Data = topRatedMovies
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while retrieving movies.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> GetTopRatedTvshows()
        {
            try
            {
                // Query to get top-rated TV shows
                var topRatedTVShows = await _context.VideoMetadatas
                    .Where(vm => vm.Category == "TVShows") // Filter by "TVShows"
                    .Join(_context.RatingVedios,
                          vm => vm.UUID,
                          rv => rv.VideoId,
                          (vm, rv) => new { vm, rv.Rating })
                    .GroupBy(x => new { x.vm.UUID, x.vm.Name, x.vm.Url }) // Group by video metadata
                    .Select(g => new
                    {
                        g.Key.UUID,
                        g.Key.Name,
                        g.Key.Url,
                        AverageRating = g.Average(x => x.Rating) // Calculate average rating
                    })
                    .OrderByDescending(x => x.AverageRating) // Sort by highest rating
                    .Take(20) // Limit to top 20
                    .ToListAsync();

                // If no top-rated TV shows exist, return latest uploaded TV shows
                if (!topRatedTVShows.Any())
                {
                    var latestTVShows = await _context.VideoMetadatas
                        .Where(vm => vm.Category == "TVShows")
                        .OrderByDescending(vm => vm.Created) // Sort by upload date (Created)
                        .Take(20) // Limit to top 20
                        .ToListAsync();

                    if (!latestTVShows.Any())
                    {
                        return new APIResponse
                        {
                            ApiCode = 1,
                            DisplayMessage = "No TV shows available at the moment.",
                            Data = null
                        };
                    }

                    return new APIResponse
                    {
                        ApiCode = 0,
                        DisplayMessage = "No top-rated TV shows found. Showing latest uploaded TV shows.",
                        Data = latestTVShows
                    };
                }

                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "Top-rated TV shows retrieved successfully.",
                    Data = topRatedTVShows
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Exception while retrieving TV shows.",
                    Data = ex.Message
                };
            }
        }



    }
}
