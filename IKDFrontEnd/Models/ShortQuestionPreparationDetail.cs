using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class ShortQuestionPreparationDetail
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public int QuestionId { get; set; }
}
