using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Helpers.Security;
using TalentsApi.Models;
using TalentsApi.Models.AppSettings;
using TalentsApi.Models.Auth;

namespace TalentsApi.Services
{
    public class FilesService
    {
        private readonly AppSettingsModel _appSettingsModel;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserClaims _userClaims;

        public FilesService(AppSettingsModel appSettingsModel,
            IHttpContextAccessor httpContext,
            ILogger<EmailService> logger)
        {
            _logger = logger;
            _appSettingsModel = appSettingsModel;
            _httpContext = httpContext;
            _userClaims = (UserClaims)_httpContext.HttpContext.Items["UserClaims"];
        }

        public async Task<ResultModel<string>> SaveToDisk(IFormFile file, string filePath)
        {
            try
            {
                if (file.Length <= 0)
                    return new ResultModel<string> { Message = "Invalid file length" };
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var fullFilePath = $"{filePath}\\{file.FileName}";
                using (Stream fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var success = File.Exists(fullFilePath);
                    var encryptedPath = string.Empty;
                    if (success)
                        encryptedPath = Encryption.OpenSSLEncrypt(fullFilePath, _appSettingsModel.Config.EncryptionSalt);
                    return new ResultModel<string> { IsSuccess = success, Data = encryptedPath };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string>() { Message = "An error occured" };
            }
        }

        public async Task<ResultModel<string>> SaveToDisk(IFormFile file, int companyId, int userId)
        {
            var filePath = Path.Combine(_appSettingsModel.Config.FilesPath, $"{companyId}\\{userId}");
            return await SaveToDisk(file, filePath);
        }

        public async Task<ResultModel<string>> SaveToDisk(IFormFile file, int userId)
        {
            var filePath = Path.Combine(_appSettingsModel.Config.FilesPath, $"{_userClaims.CompanyId}\\{userId}");
            return await SaveToDisk(file, filePath);
        }

        public byte[] Get(string token)
        {
            try
            {
                var decrypted = Encryption.OpenSSLDecrypt(token, _appSettingsModel.Config.EncryptionSalt);
                if (File.Exists(decrypted))
                    return File.ReadAllBytes(decrypted);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return null;
            }
        }

        public ResultModel<string> ValidateImageExtension(IFormFile file)
        {
            var extensions = new string[] { ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(file.FileName);
            if (!extensions.Contains(extension.ToLower()))
                return new ResultModel<string>() { Message = "Invalid file format" };

            return new ResultModel<string> { IsSuccess = true };
        }

        public ResultModel<string> ValidateImageSize(IFormFile file, double maxSizeInMb = 5)
        {
            var size = maxSizeInMb * 1024 * 1024;
            if (file.Length > size)
                return new ResultModel<string> { Message = $"Allowed max file size is { maxSizeInMb }Mb" };

            return new ResultModel<string> { IsSuccess = true };
        }


    }
}
