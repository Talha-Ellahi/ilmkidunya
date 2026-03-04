namespace IKDFrontEnd.ViewModels
{
    public class TezMateViewModels
    {
    }



    public class TezMateRequest
    {
        public string loginCode { get; set; }
        public string externalId { get; set; } = "0";
        public string pageUrl { get; set; }

        // ✅ SINGLE field, not list
        public TezMateField field { get; set; }

        public int percentage { get; set; } = 100;
        public string additionalPrompt { get; set; }

        // ✅ REQUIRED by API
        public int maxLength { get; set; } = 0;
    }


    public class TezMateField
    {
        public string name { get; set; }
        public string type { get; set; } // html, json-ckeditor, plain-text, etc.
        public object currentValue { get; set; }
    }








    public class TezMateMeta
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class TezMateMultiHtmlRequest
    {
        public string loginCode { get; set; }
        public string pageUrl { get; set; }
        public List<TezMateHtmlField> htmlFields { get; set; }
        public int percentage { get; set; } = 100;
        public string additionalPrompt { get; set; }
        public int maxLength { get; set; } = 0;
        public bool generateMeta { get; set; } = false;
    }

    public class TezMateHtmlField
    {
        public string name { get; set; }
        public string type { get; set; } = "html";
        public object currentValue { get; set; }
        public string additionalPrompt { get; set; }
    }

    public class TezMateMultiHtmlResponse
    {
        public string PageUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<TezMateResultItem> Result { get; set; }
        public TezMateMeta Meta { get; set; }
    }

    // Update the existing TezMateResponse to include new method
    public class TezMateResponse
    {
        public string ExternalId { get; set; }
        public string PageUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string ContentAction { get; set; }
        public TezMateResultItem Result { get; set; }
        public TezMateMeta Meta { get; set; }
    }

    // Update the existing TezMateResultItem
    public class TezMateResultItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object UpdatedContent { get; set; }
    }



}
