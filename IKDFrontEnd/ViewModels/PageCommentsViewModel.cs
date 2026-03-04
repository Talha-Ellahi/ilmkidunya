using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class PageCommentsViewModel
    {
        public string PageUrl { get; set; } = null!;
       
        public TblComments2 NewComment { get; set; } = new();
        public List<CommentWithRepliesViewModel> Comments { get; set; } = new List<CommentWithRepliesViewModel>();
    }

    public class CommentWithRepliesViewModel
    {
        public TblComments2 Comment { get; set; } = new();
        public List<TblCommentsChild> Replies { get; set; } = new();
        public int CommentId { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public DateTime? DatePosted { get; set; }
        public int? LikeCmt { get; set; }
    }
}
