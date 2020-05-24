﻿﻿using Newtonsoft.Json;

  namespace PDFIdentify.DataStructure.ResponseObjects.BlockObjects.GeometryObjects
{
    public class Coordinate
    {
        [JsonProperty("X")]
        public double X { get; set; }
        
        [JsonProperty("Y")]
        public double Y { get; set; }

        public Coordinate()
        {
            X = 0;
            Y = 0;
        }
        
        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}