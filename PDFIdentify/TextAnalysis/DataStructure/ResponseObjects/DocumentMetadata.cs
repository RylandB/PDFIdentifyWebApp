﻿﻿using Newtonsoft.Json;

  namespace PDFIdentify.DataStructure.ResponseObjects
{
    public class DocumentMetadataObject
    {
        [JsonProperty("Pages")]
        public int Pages { get; set; }

        public DocumentMetadataObject()
        {
            Pages = 0;
        }

        [JsonConstructor]
        public DocumentMetadataObject(int pages)
        {
            Pages = pages;
        }
    }
}