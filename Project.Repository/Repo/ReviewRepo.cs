using Project.Database.DbContext;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;
using System.Data.Entity;
using System.Data.Sql;

namespace Project.Repository.Repo
{
    public class ReviewRepo
    {
        public static ReviewDto ToDto(PlaceReview a)
        {
            if (a == null) return null;
            return new ReviewDto()
            {
                Id = a.Id,
                CreatedDate = a.CreatedDatte,
                ReviewStars = a.ReviewStars,
                ReviewText = a.ReviewText,
                Username = GetName(a.User) ?? a.Username,
                UserId = a.UserId,
                PlaceId = a.PlaceId
            };
        }
        public static List<ReviewDto> GetList(int placeId = 0, int limit = 0)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.PlaceReviews
                    .Where(a => a.PlaceId == placeId && !(a.IsDeleted ?? false))
                    .Include(s => s.User)
                    .Include(s => s.User.UserDetails)
                    .OrderByDescending(s => s.Id)
                    .AsEnumerable();

                if (limit > 0)
                    list = list.Take(limit);

                return list.ToList().Select(a => ToDto(a)).ToList();
            }
        }



        public static List<Tuple<PlaceDto, ReviewDto>> GetListWithPlaceDetail(int userId, int placeId = 0, int limit = 0)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var reviews = context.PlaceReviews.Where(a => a.UserId == userId && !(a.IsDeleted??false));

                if (placeId > 0)
                    reviews = reviews.Where(a => a.PlaceId == placeId);

                if (limit > 0)
                    reviews = reviews.Take(limit);

                var list = (from r in reviews.ToList()
                            join p in context.Places on r.PlaceId equals p.Id
                            select Tuple.Create(
                                new PlaceDto()
                                {
                                    Id = p.Id,
                                    Title = p.Title,
                                    HomeThumbnail = p.HomeThumbnail,
                                    ParentName = p.City?.Name ?? ""
                                },
                                new ReviewDto()
                                {
                                    Id = r.Id,
                                    CreatedDate = r.CreatedDatte,
                                    ReviewStars = r.ReviewStars,
                                    ReviewText = r.ReviewText,
                                    Username = r.Username,
                                    UserId = r.UserId,
                                    PlaceId = r.PlaceId
                                })
                            ).OrderByDescending(s => s.Item2.Id).ToList();

                return list;
            }
        }
        public static Tuple<PlaceDto, ReviewDto> Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var reviews = context.PlaceReviews.Where(a => a.Id == id);

                var list = (from r in reviews.ToList()
                            join p in context.Places on r.PlaceId equals p.Id
                            select Tuple.Create(
                                new PlaceDto()
                                {
                                    Id = p.Id,
                                    Title = p.Title,
                                    HomeThumbnail = p.HomeThumbnail,
                                    ParentName = p.City?.Name ?? ""
                                },
                                new ReviewDto()
                                {
                                    Id = r.Id,
                                    CreatedDate = r.CreatedDatte,
                                    ReviewStars = r.ReviewStars,
                                    ReviewText = r.ReviewText,
                                    Username = r.Username,
                                    UserId = r.UserId,
                                    PlaceId = r.PlaceId
                                })
                            ).OrderByDescending(s => s.Item2.Id).ToList();

                return list.FirstOrDefault();
            }
        }

        public static int GetReviewsCount(int placeId)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                return context.PlaceReviews.Where(a => a.PlaceId == placeId).Count();
            }
        }

        private static string GetName(User user)
        {
            var fullname = user?.UserDetails?.FirstOrDefault()?.FullName;
            if (fullname == null) return null;

            var names = fullname.Split(' ').ToList();

            if (names.Count() == 1)
                return names.FirstOrDefault();

            var second = names[1].FirstOrDefault().ToString();
            if (!string.IsNullOrEmpty(names[1]))
                second += ".";
            return $"{names[0]} {second}";
        }

        public static ReviewDto GetReview(int userId, int placeId)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var review = context.PlaceReviews.Include(s => s.User).Include(s => s.User.UserDetails).Where(a => a.PlaceId == placeId && userId == a.UserId).FirstOrDefault();

                return ToDto(review);
            }
        }

        public static ReturnValue Save(ReviewDto dto)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var message = "";
                //var checkexisitingreview = context.PlaceReviews.FirstOrDefault(a => a.UserId == dto.UserId && a.PlaceId == dto.PlaceId);
                var checkexisitingreview = context.PlaceReviews.FirstOrDefault(a => a.Id == dto.Id && !(a.IsDeleted ?? false));
                 if (checkexisitingreview != null && dto.UserId != null)
                {
                    checkexisitingreview.CreatedDatte = DateTime.Now;
                    checkexisitingreview.ReviewStars = dto.ReviewStars;
                    checkexisitingreview.ReviewText = dto.ReviewText ?? "";
                    message = "Your review was successfully edited. Thank you.";
                }
                else
                {
                    var dFrom = DateTime.Now.Date;
                    var dTo = new DateTime(dFrom.Year, dFrom.Month, dFrom.Day, 23, 59, 59);
                    var hasExistingReviewToday = context.PlaceReviews.FirstOrDefault(a => a.PlaceId == dto.PlaceId &&
                                    a.UserId == dto.UserId && a.CreatedDatte >= dFrom && a.CreatedDatte <= dTo &&
                                    !(a.IsDeleted ?? false));

                    if (hasExistingReviewToday == null)
                    {
                        var newReview = new PlaceReview()
                        {
                            PlaceId = dto.PlaceId,
                            CreatedDatte = DateTime.Now,
                            ReviewStars = dto.ReviewStars,
                            ReviewText = dto.ReviewText ?? "",
                            Username = dto.Username,
                            UserId = dto.UserId
                        };
                        context.PlaceReviews.Add(newReview);
                        message = "Your review was successfully submitted. Thank you.";
                    }
                    else
                    {
                        //hasExistingReviewToday.CreatedDatte = DateTime.Now;
                        //hasExistingReviewToday.ReviewStars = dto.ReviewStars;
                        //hasExistingReviewToday.ReviewText = dto.ReviewText ?? "";
                        return new ReturnValue { Message = "You have already submitted a review today." };
                       // message = "Unable to add review. Please try again later.";
                    }
                }

                if (context.SaveChanges() > 0)
                {
                    result = new ReturnValue(message, true);
                }
                else
                {
                    result = new ReturnValue("Unable to submit your review. Please try again later. Thank you.");
                }
            }

            return result;
        }

        public static ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var message = "";
                //var checkexisitingreview = context.PlaceReviews.FirstOrDefault(a => a.UserId == dto.UserId && a.PlaceId == dto.PlaceId);
                var checkexisitingreview = context.PlaceReviews.FirstOrDefault(a => a.Id == id);
                if (checkexisitingreview != null)
                {
                    checkexisitingreview.IsDeleted = true;
                    message = "Your review was successfully deleted. Thank you.";

                    if (context.SaveChanges() > 0)
                    {
                        result = new ReturnValue(message, true);
                    }
                    else
                    {
                        result = new ReturnValue("Unable to delete your review. Please try again later. Thank you.");
                    }
                }
                else
                {
                    message = "Unable to delete your review. Please try again later. Thank you.";
                }
            }

            return result;
        }
    }
}
