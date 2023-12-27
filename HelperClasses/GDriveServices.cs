using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.StaticFiles;

namespace WebsiteQuanLyLamViecNhom.HelperClasses
{
    public class GDriveServices : DriveService
    {
        /// <summary>
        /// Get Mime type from file's name
        /// </summary>
        /// <param name="fileName"></param> 
        /// <returns></returns>
        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var mimeType))
            {
                mimeType = "application/octet-stream"; // Default MIME type if not found
            }
            return mimeType;
        }
        /// <summary>
        /// Get the already provided user account to do these actions
        /// </summary>
        /// <returns></returns>
        public DriveService GetService()
        {
            ClientSecrets clientSecrets = new ClientSecrets
            {
                ClientId = "183810282546-np7kpca3673n8htal3908acknfmafftc.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            /* set where to save access token. The token stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time. */
            string tokenPath = Path.GetTempPath();

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
                ApplicationName = "GroupProjectManagement",
            });
            return service;
        }
        /// <summary>
        /// To act like as a bot account to do these actions
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// For sending file into the designated google drive
        /// </summary>
        /// 
        /// <param name="fileName"></param> string defining the file's name on drive
        /// <param name="data"></param> the stream contatining the file's data
        /// <param name="destinationFolderId"></param> Folder id
        /// (Example:https://drive.google.com/drive/u/0/folders/1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP.
        /// Then the Folder Id would be "1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP").
        /// 
        /// FolderIds:
        /// 1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP: Teacher profile picture
        /// 
        /// <returns>A string the the file's ID which can be used for loading the pic</returns>
        public Object UploadFile(string fileName, byte[] data, string destinationFolderId)
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

                var existingFile = _driveClient.Files.List().Execute().Files.FirstOrDefault(f => f.Name == fileName);
                if (existingFile != null)
                {
                    // If a file with the same name exists, delete then remake it
                    FilesResource.DeleteRequest deleteRequest = _driveClient.Files.Delete(existingFile.Id);
                    deleteRequest.Fields = "*";
                    deleteRequest.Execute();

                    FilesResource.CreateMediaUpload createRequest = _driveClient.Files.Create(metadata, stream, mimetype);
                    createRequest.Fields = "*";
                    createRequest.Upload();

                    uploadedFile = createRequest.ResponseBody;
                    return new { FileId = uploadedFile?.Id, Updated = true, FileName = fileName };
                }
                else
                {
                    FilesResource.CreateMediaUpload createRequest = _driveClient.Files.Create(metadata, stream, mimetype);
                    createRequest.Fields = "*";
                    createRequest.Upload();

                    uploadedFile = createRequest.ResponseBody;
                    return new {FileId = uploadedFile?.Id, Updated = false, FileName = fileName };
                }
            }

            return new { FileId = uploadedFile?.Id, Updated = false };
        }
        //Returns a download link that can be use by <img src="link"/>
        public string? GetDownloadLink(string fileId)
        {
            Google.Apis.Drive.v3.Data.File fileResponse = null;

            DriveService _driveClient = GetClient();

            if (_driveClient != null && !string.IsNullOrWhiteSpace(fileId))
            {
                FilesResource.GetRequest getRequest = _driveClient.Files.Get(fileId);
                getRequest.Fields = "webContentLink, mimeType"; // Request both webContentLink and mimeType

                fileResponse = getRequest.Execute();
            }

            string downloadLink = "no file attached";

            if (fileResponse != null)
            {
                // Extract extension from mimeType
                string extension = fileResponse?.MimeType?.Split('/')[1];

                // Construct download link with extension
                downloadLink = fileResponse?.WebContentLink + "?e=" + extension;
            }

            return downloadLink;
        }
    }
}
