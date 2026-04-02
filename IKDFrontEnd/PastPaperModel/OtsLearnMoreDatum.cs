using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class OtsLearnMoreDatum
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public int? AnsOpt1Id { get; set; }

    public int? AnsOpt2Id { get; set; }

    public int? AnsOpt3Id { get; set; }

    public int? AnsOpt4Id { get; set; }

    public string LearnMoreData { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
