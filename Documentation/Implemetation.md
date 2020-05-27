# Implementing PDFIdentify into a project

PDFIdentify is designed to create a way to interact with the AWS Textract service in a C# environment.
As AWS does not yet support C#/.NET for the Textract service, this package utilizes a supplementary Python server that behaves as the middleman. The Python service receives the name of the file from the .NET application, gets the Textract analysis from AWS, and bounces it back to the .NET application as JSON.

PDFIdentify wraps up two AWS services into a package in two parts.

# S3Upload.Upload library
Upload utilizes AWS .NET packages to upload any DataStream to AWS S3. This section is essentially just a wrapper for the official AWS S3 library. If the developer prefers, they can circumvent this segment and use the official AWS S3 library directly. It is included primarily for clarity that the file must be uploaded to S3 first.

## Upload Document
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

## Textract Analyze File
```AnalyzeFile(string fileName, string serverAddress)```

### Description
Asynchronously run AWS Textract analysis on a file that is uploaded to the AWS S3 bucket

### Parameters
string fileName - Name of the file (with extension) to be analyzed by Textract
string serverAddress - IP Address of the Python middleman server (TextractMiddleman.py)

### Return Data
[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=netcore-2.1)<[TextractResponse]()> - Task object that results in a C# Class conversion of the Textract JSON response.

## Get Descendants of a Block
```GetChildrenRecursive(Dictionary<string, Block> blockDictionary, Block block, List<Block> targetList)```

### Description
Recursively traverse through CHILD relationships of each Textract block to acquire a list of all Child-descendant blocks of a specified block

### Parameters
[Dictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=netcore-2.1)<string, [Block]()> blockDictionary - dictionary containing all known blocks (all potential offspring of specified block)

[Block]() block - Block whose offpring is to be found (origin node)

[List](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netcore-2.1)<[Block]()> targetList - List to be used to store each descendant

### Return Data
[List](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netcore-2.1)<[Block]()> targetList - List containing each descendant of the specified block

# TableToolbox Object
Object designed to assemble, traverse, and use Textract table blocks and their children.

## TableToolbox Constructor
```TableToolbox(List<Block> blocks)```

### Description
Initialize a new TableToolbox with a list of relevant blocks, an empty Table as a 2D Array, and an empty dictionary for block lookup by Textract ID

### Parameters
[List](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netcore-2.1)<[Block]()> blocks - List of blocks including at least the table block and every descendant of it

### Return Data
TableToolbox Object

## Filter the block list to descendants only
```tableToolbox.FilterToChildren(string blockId)```

### Description
If the List<Block> used to construct the TableToolbox contains more than just the table block and its descendants, the list must be filtered to only include the necessary blocks with this function

### Parameters
string blockId - Textract ID string of the desired TABLE Block

### Return Data
Void (N/A)

## Build the BlockLookup Dictionary
```tableToolbox.BuildBlockLookup()```

### Description
Populate the block dictionary "BlockLookup" with the necessary blocks as (Key:Value -> string blockId:Block block)

### Parameters
N/A

### Return Data
Void (N/A)

## Construct the Table
```tableToolbox.ConstructTable```

### Description
Builds a table into the two-dimensional Array called "Table"

### Parameters
string tableId - Textract ID string of the TABLE block

### Return Data
Void (N/A)

## Find Index of a column
```tableToolbox.FindIndex(List<string> searchParams)```

### Description
Locate the index of a column containing a match from a list of strings. This is used to determine what column contains a desired type of information based typically on table headers

### Parameters
[List](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netcore-2.1)<string> searchParams - List of strings to consider as a match for an indicator of the desired header

### Return Data
int[] coordinate - {x,y} value for the first found match; {-1,-1} returned in the event of no match

## Get all text from a cell
```tableToolbox.GetAllTextFromCell(int columnIndex, int rowIndex)```

### Description
Assembles a string phrase containing all words from the CELL at (columnIndex, rowIndex) as (x,y) of the table

### Parameters
int columnIndex - column number or x-coordinate of the CELL (Left column is 0)
int rowIndex - row number or y-coordinate of the CELL (Top row is 0)

### Return Data
