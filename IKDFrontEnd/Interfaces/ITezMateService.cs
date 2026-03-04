using IKDFrontEnd.ViewModels;

namespace IKDFrontEnd.Interfaces
{

    public interface ITezMateService
    {
        Task<TezMateResponse?> GetUpdatedContentAsync(TezMateRequest request);
        Task<TezMateMultiHtmlResponse?> GetUpdatedHtmlContentsAsync(TezMateMultiHtmlRequest request);
    }

}
