using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Enums
    {
        public enum UserRole
        {
            Encoder = 1,
            Administrator
        }

        public enum UserLinkedAccountTypeEnums
        {
            Gmail = 1,
            Facebook,
            Apple,
            Mobile,
            Email
        }

        public enum CategoryTypeEnum
        {
            Accommodation = 1,
            Attraction,
            Municipality,
            Recreation,
            Restaurant,
            Shop,
            NewsEvents,
            DirectoryEntry,
            TourGuides,
            DiveShops,
            Pages,
            Other,
            SearchResult,
            DRRMOOffice,
            Announcement

        }

        public enum PaymentOptionEnums
        {
            Cash = 2,
            Card = 4
        }
        public enum PlaceImagesType
        {
            All = 0,
            HomeThumbnail,
            DetailThumbnail,
            Gallery,
            YoutubeURL,
            AnnouncementWeb,
            AnnouncementMobile

        }

        public enum UserUploadFileTypeEnum
        {
            ID = 1,
            Vaccination_Card,
            Other
        }

        public enum AnnouncementEnum
        {
            New = 1,
            Deleted
        }
    }
}
