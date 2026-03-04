namespace IKDFrontEnd.ViewModels
{
	public class PostReplyDto
	{
		public int ParentCommentId { get; set; }   // maps to C_Id in TblCommentsChild
		public string Comment { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string IpAddress { get; set; }
		public string Source { get; set; }
	}
}
