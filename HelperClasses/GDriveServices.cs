using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.StaticFiles;

namespace WebsiteQuanLyLamViecNhom.HelperClasses
{
    public class GDriveServices : DriveService
    {

        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var mimeType))
            {
                mimeType = "application/octet-stream"; // Default MIME type if not found
            }
            return mimeType;
        }

        public DriveService GetService()
        {
            ClientSecrets clientSecrets = new ClientSecrets
            {
                ClientId = "183810282546-np7kpca3673n8htal3908acknfmafftc.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            /* set where to save access token. The token stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time. */
            string tokenPath = Path.GetTempPath(); // <- change

            string[] scopes = { Scope.Drive };

            UserCredential authorizedUserCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
              clientSecrets,
              scopes,
              "user",
              CancellationToken.None,
              new FileDataStore(tokenPath, true)
            ).Result;

            DriveService service = new DriveService(new Initializer()
            {
                HttpClientInitializer = authorizedUserCredential,
                ApplicationName = "Name_of_your_application", // <- change
            });
            return service;
        }
        public DriveService? GetClient()
        {
            string credenitalsJSONPath
                = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceAccountJSON", "nckh2324-de4686629f17.json");

            using (var stream = new FileStream(credenitalsJSONPath, FileMode.Open, FileAccess.Read))
            {
                var credentials = GoogleCredential.FromStream(stream);

                if (credentials != null)
                {
                    if (credentials.IsCreateScopedRequired)
                    {
                        string[] scopes = { Scope.Drive };
                        credentials = credentials.CreateScoped(scopes);
                    }

                    var service = new DriveService(new Initializer()
                    {
                        HttpClientInitializer = credentials,
                        ApplicationName = "GroupProjectManagement",
                    });

                    return service;
                }
            }
            return null;
        }
        public string? UploadFile(string fileName, byte[] data, string destinationFolderId)
        {
            Google.Apis.Drive.v3.Data.File uploadedFile = null;

            DriveService _driveClient = GetClient();

            if (_driveClient != null)
            {
                Google.Apis.Drive.v3.Data.File metadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    Parents = new List<string>()
                };

                if (!string.IsNullOrWhiteSpace(destinationFolderId))
                {
                    metadata.Parents.Add(destinationFolderId);
                }

                var stream = new MemoryStream(data);
                var mimetype = GetMimeType(fileName);

                FilesResource.CreateMediaUpload request = _driveClient.Files.Create(metadata, stream, mimetype);
                request.Fields = "id";
                request.Upload();

                uploadedFile = request.ResponseBody;
            }

            return uploadedFile?.Id;
        }
        public string? GetDownloadLink(string fileId)
        {
            Google.Apis.Drive.v3.Data.File fileResponse = null;

            DriveService _driveClient = GetClient();

            if (_driveClient != null && !string.IsNullOrWhiteSpace(fileId))
            {
                FilesResource.GetRequest getRequest = _driveClient.Files.Get(fileId);
                getRequest.Fields = "webContentLink";

                fileResponse = getRequest.Execute();
            }

            return fileResponse?.WebContentLink;
        }
    }
}
