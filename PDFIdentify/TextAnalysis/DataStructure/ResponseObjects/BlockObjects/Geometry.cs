using System.Collections.Generic;
using Newtonsoft.Json;
using PDFIdentify.DataStructure.ResponseObjects.BlockObjects.GeometryObjects;

namespace PDFIdentify.DataStructure.ResponseObjects.BlockObjects
{
    public class Geometry
    {
        [JsonProperty("BoundingBox")]
        public BoundingBox BoundingBox { get; set; }
        
        [JsonProperty("Polygon")]
        public List<Coordinate> Polygon { get; set; }
    }
}