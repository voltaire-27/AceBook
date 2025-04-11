using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceBook.Includes
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService()
        {
            var account = new Account(
               "di1qr5ck1",
               "696237154229656",
               "x1LEQQfYnLsOdGUZfDZ2hxYgk_0"
                );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImage(FileResult file)
        {
            if (file == null)
                return null;
            await using var stream = await file.OpenReadAsync();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = Guid.NewGuid().ToString(),
                Overwrite = true,
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }


        public string GetImageUrl(string publicId)
        {
            return _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        }
    }
}
