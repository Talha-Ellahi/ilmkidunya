using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class Admin
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? LoginId { get; set; }

    public string? Password { get; set; }

    public bool? IsSuper { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsLogin { get; set; }

    public string? MacAddress { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Pictures { get; set; }

    public virtual ICollection<LongQuestionCriterion> LongQuestionCriteria { get; set; } = new List<LongQuestionCriterion>();

    public virtual ICollection<NewsPaper> NewsPapers { get; set; } = new List<NewsPaper>();

    public virtual ICollection<TblAdmission> TblAdmissions { get; set; } = new List<TblAdmission>();

    public virtual ICollection<TblArticle> TblArticles { get; set; } = new List<TblArticle>();

    public virtual ICollection<TblLongQuestion> TblLongQuestions { get; set; } = new List<TblLongQuestion>();

    public virtual ICollection<TblMainNews> TblMainNews { get; set; } = new List<TblMainNews>();

    public virtual ICollection<TblOtsTestMcq> TblOtsTestMcqs { get; set; } = new List<TblOtsTestMcq>();
}
