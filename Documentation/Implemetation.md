# Implementing PDFIdentify into a project

PDFIdentify is designed to create a way to interact with the AWS Textract service in a C# environment.
As AWS does not yet support C#/.NET for the Textract service, this package utilizes a supplementary Python server that behaves as the middleman. The Python service receives the name of the file from the .NET application, gets the Textract analysis from AWS, and bounces it back to the .NET application as JSON.

PDFIdentify wraps up two AWS services into a package in two parts.

# S3Upload.Upload library
Upload utilizes AWS .NET packages to upload any DataStream to AWS S3. This section is essentially just a wrapper for the official AWS S3 library. If the developer prefers, they can circumvent this segment and use the official AWS S3 library directly. It is included primarily for clarity that the file must be uploaded to S3 first.

## Upload Document Function
```UploadDocument(string bucketName, string keyName, Stream inputStream, RegionEndpoint bucketRegion)```

### Description
Upload a DataStream to an S3 bucket

### Parameters
string bucketName - name of the target S3 bucket for the file upload

string keyName - filename (with extension) to be assigned to the file upon upload

[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream?view=netcore-2.1) inputStream - datastream containing the file 

[RegionEndpoint](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Amazon/TRegionEndpoint.html) bucketRegion - AWS region assigned to the target bucket (i.e. RegionEndpoint.USEast2)

### Return Data

Void (N/A)


# Analyze Library
Analyze utilizes the Python middleman server to interact with AWS and deserializes the JSON result into custom C# Class Objects
