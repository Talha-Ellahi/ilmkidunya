using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IKDFrontEnd.ViewModels
{
    public class HostelHomeViewModel
    {
        public List<SelectListItem> CitySelectList { get; set; } = new();
        public List<CityWithImage> CitiesWithImages { get; set; } = new();
        public List<HostelCard> LatestHostels { get; set; } = new();
    }

    public class CityWithImage
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string ImagePath { get; set; }
    }

    public class HostelCard
    {
        public int HostelId { get; set; }
        public string HostelName { get; set; }
        public string CityName { get; set; }
        public string? MainImage { get; set; }
        public decimal? Price { get; set; }
        public string? HostelUrl { get; internal set; }
    }

    public class HostelDetailViewModel
    {
        public string HostelName { get; set; } = "";
        public string? FullAddress { get; set; }
        public string? CityName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public List<string> ImagePaths { get; set; } = new();
        public List<HostelFeature> Features { get; set; } = new();
        public List<RoomType> RoomTypes { get; set; } = new();
        public bool? Gender { get; internal set; }
        public List<CityWithImage> CityList { get; internal set; }
    }

    public class HostelFeature
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; } = "";
        public string? IconPath { get; set; }
    }

    public class RoomType
    {
        public string? RoomTypeName { get; set; } 
        public string? Details { get; set; }
        public decimal? Cost { get; set; }
    }



    public class HostelRegistrationViewModel
    {
        // Hostel Info
        public string HostelName { get; set; }
        public int CityId { get; set; }
        public int GenderType { get; set; }
        public int AreaId { get; set; } = 0;
        public string FullAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ZoomLevel { get; set; }
        public string? HostelDetails { get; set; }
        public List<int> SelectedFeatures { get; set; } = new List<int>();

        // Room Info
        public List<string> RoomCategories { get; set; } = new();
        public List<string> RoomDetails { get; set; } = new();
        public List<decimal?> RoomCosts { get; set; } = new();

        // Account Info
        public string OwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public decimal MemberId { get; internal set; }
        public int Gender { get; internal set; }
        public decimal RoomTypes { get; internal set; }
        public List<SelectListItem>? Cities { get; internal set; }
        public List<HostelFeature>? Features { get; internal set; } = null;
    }


}