using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace PDFIdentify.S3Upload
{
    public static class Upload
    {
        private static IAmazonS3 _client;

        public static void UploadDocument(string bucketName, string keyName, Stream inputStream, RegionEndpoint bucketRegion)
        {
            _client = new AmazonS3Client(bucketRegion);
            PutObjectResponse response = WriteObject(bucketName, keyName, inputStream);
        }

        private static PutObjectResponse WriteObject(string bucketName, string keyName, Stream inputStream)
        {
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = inputStream,
                    ContentType = "media/image"
                };
                putRequest.Metadata.Add("x-amz-meta-src", "PDFIdentifyC");
                return _client.PutObjectAsync(putRequest).Result;
                
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'" + e.Message + "' when writing an object");
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'" + e.Message + "' when writing an object");
            }

            var getRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            var resp = _client.GetObjectAsync(getRequest).Result;

            return new PutObjectResponse();
        }
    }
}
