
namespace IKDFrontEnd.ViewModels
{
    public class SlidersViewModel
    {

    }
    public class SliderHome
    {
        public string? Slidertitle { get; internal set; }
        public int ID { get; internal set; }
        public string? Categoryname { get; internal set; }
        public string? Image { get; internal set; }
        public DateTime? Date { get; internal set; }
    }


    public class SliderDetails
    {
        public string SliderName { get; set; }
        public string MainSliderImage { get; set; }
        public string Slidertitle { get; set; }
        public int ID { get; set; }
        public string SliderDesc { get; set; }
        public string Image { get; set; }
        public DateTime? Date { get; set; }
        public int? SortOrder { get; set; }
        public int? SliderId { get; set; }
        public string AuthorInfo { get; set; }
    }


    public class SliderDetailViewModel
    {
        public List<SliderDetails> Slides { get; set; }
        public List<SliderHome> Sliders { get; set; }
    }
}
