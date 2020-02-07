using System.IO;
using Azure.Storage.Blobs.Models;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    internal sealed class AzureBlobDocument : IDocument
    {
        private readonly BlobDownloadInfo downloadInfo;

        public AzureBlobDocument(BlobDownloadInfo downloadInfo) => this.downloadInfo = downloadInfo;

        public Stream Content => downloadInfo.Content;

        public string ContentType => downloadInfo.ContentType;
    }
}
