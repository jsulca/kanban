using Azure.Storage.Blobs;

namespace Kanban.WebApp.Commons;

public class BlobStorage
{
    public static string UploadFile(Stream fileStream, string fileName)
    {
        try
        {
            var containerClient =
                new BlobContainerClient(AppSettings.AZURE_BLOB_CONNECTION, AppSettings.AZURE_BLOB_CONTAINER);
            var blobClient = containerClient.GetBlobClient(fileName);

            blobClient.Upload(fileStream);

            return blobClient.Uri.AbsoluteUri;
        }
        catch (Exception)
        {
            throw new Exception("Error al subir el archivo al blob storage");
        }
    }

    public static Stream DownloadFile(string fileName)
    {
        try
        {
            var containerClient =
                new BlobContainerClient(AppSettings.AZURE_BLOB_CONNECTION, AppSettings.AZURE_BLOB_CONTAINER);
            var blobClient = containerClient.GetBlobClient(fileName);

            var result = blobClient.Download();
            return result.Value.Content;
        }
        catch (Exception)
        {
            throw new Exception("Error al descargar el archivo al blob storage");
        }
    }

    public async Task UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            var containerClient =
                new BlobContainerClient(AppSettings.AZURE_BLOB_CONNECTION, AppSettings.AZURE_BLOB_CONTAINER);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream);
        }
        catch (Exception)
        {
            throw new Exception("Error al subir el archivo al blob storage");
        }
    }
}
