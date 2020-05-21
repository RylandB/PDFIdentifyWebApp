﻿using System;
using System.Collections.Generic;
using System.Drawing;
 using System.IO;
 using Microsoft.AspNetCore.Http;
 using Newtonsoft.Json;
 using PDFIdentify.DataStructure;
using PDFIdentify.DataStructure.ResponseObjects;

namespace SampleWebApp
{
    public static class IndexOperations
    {
        public static VisualData OnPostActions(IFormFile imageFile)
        {
            //Create Image from File
            Image image = CreateImage(imageFile);
            
            /*
            //Upload DataStream to S3
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                PDFIdentify.S3Upload.Upload.UploadDocument("pdfidentify", "112233.png", memoryStream, RegionEndpoint.USEast2);
            }

            //Run Textract Operations
            //Preserve tables bounding boxes & response.blocks
            TextractResponse response = Analyze.AnalyzeFile("112233.png", "https://localhost:5003").Result;
            */

            TextractResponse response =
                JsonConvert.DeserializeObject<TextractResponse>(
                    File.ReadAllText(@"D:\OCR\PDFIdentify\Python\test.json"));


            List<BoundingBoxIdentifier> bBoxIdens = new List<BoundingBoxIdentifier>();
            foreach (var table in response.FilterType("TABLE"))
            {
                //Convert relative coordinate values to abs pixel values
                bBoxIdens.Add(CreateBBox(table, image));
            }
            

            //Draw on Image
            var visualData = new VisualData
            {
                Colors = drawBBOnImage(image, bBoxIdens), DataUrl = GetImageDataUrl(image), TextractResponse = response
            };

            //Return Image DataURL & Options for Dropdown Menu
            return visualData;
        }
        
        public static string GetImageDataUrl(Image image)
        {
            Byte[] imageBytes = GetImageBytes(image);
            return AssembleDataUrl(image, imageBytes);
        }

        private static Image CreateImage(IFormFile imageFile)
        {
            using var memoryStream = new MemoryStream();
            imageFile.CopyTo(memoryStream);
            
            return Image.FromStream(memoryStream);
        }

        private static Byte[] GetImageBytes(Image image)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, image.RawFormat);
            
            return memoryStream.ToArray();
        }

        private static string AssembleDataUrl(Image image, Byte[] imageBytes)
        {
            return "data:image/"
                   + image.RawFormat.ToString().Replace(".", "")
                   + ";base64,"
                   + Convert.ToBase64String(imageBytes);
        }

        private static BoundingBoxIdentifier CreateBBox(Block table, Image image)
        {
            int left = (int) (image.Width * table.Geometry.BoundingBox.Left);
            int top = (int) (image.Height * table.Geometry.BoundingBox.Top);
            int width = (int) (image.Width * table.Geometry.BoundingBox.Width);
            int height = (int) (image.Height * table.Geometry.BoundingBox.Height);
            string label = table.Id;
            string id = table.Id;

            Console.Out.WriteLine(id);
            Console.Out.WriteLine("Left: " + left);
            Console.Out.WriteLine("Top: " + top);
            Console.Out.WriteLine("Width: " + width);
            Console.Out.WriteLine("Height: " + height);
            Console.Out.WriteLine();
            Console.Out.WriteLine();
            
            return new BoundingBoxIdentifier(left, top, width, height, label, id);
        }

        private static Dictionary<string, string> drawBBOnImage(Image image, IEnumerable<BoundingBoxIdentifier> bBoxIdens)
        {
            List<Color> colors = new List<Color>()
            {
                Color.Blue,
                Color.Red,
                Color.Green,
                Color.Yellow,
                Color.Purple,
                Color.Orange,
                Color.HotPink,
                Color.Aqua,
                Color.LightPink,
                Color.SpringGreen
            };
            Dictionary<string, string> usedColors = new Dictionary<string, string>();
            
            using var graphics = Graphics.FromImage(image);
            Pen pen = new Pen(Color.Black, 3);
            int i = 0;
            foreach (var box in bBoxIdens )
            {
                if (i > 10){break;}
               
                Color color = colors[i % colors.Count];
                pen.Color = color;
                
                graphics.DrawRectangle(pen,
                    box.Coord[0],
                    box.Coord[1],
                    box.Coord[2],
                    box.Coord[3]);
                
                usedColors[color.Name] = box.Id;
                i++;
            }
            
            return usedColors;
        }
    }

    public class VisualData
    {
        public string DataUrl { get; set; }
        
        public Dictionary<string, string> Colors { get; set; }
        
        public TextractResponse TextractResponse { get; set; }
    }
}