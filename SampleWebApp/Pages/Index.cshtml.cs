using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PDFIdentify.DataStructure.ResponseObjects;

namespace SampleWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public VisualData PhotoData { get; set; }
        public string PhotoDataUrl { get; set; }
        public bool Uploaded { get; set; }
        public bool OutputDisplay { get; set; }
        public string SelectedColor { get; set; }
        public List<Block> Blocks { get; set; }
        public IEnumerable<string> ColorList { get; set; }
        public string Output { get; set; }
        public Dictionary<string, string> Colors { get; set; }
        public string JsonDocument { get; set; }
        

        public void OnPostUpload(IFormFile file)
        {
            PhotoData = IndexOperations.OnPostActions(file);
            PhotoDataUrl = PhotoData.DataUrl;
            Blocks = PhotoData.TextractResponse.Blocks;
            JsonDocument = JsonConvert.SerializeObject(Blocks);
            Colors = PhotoData.Colors;
            ColorList = Colors.Keys;
            
            Uploaded = true;
        }

        public void OnPostConfirm(string confirmList)
        {
            SelectedColor = confirmList;
            Blocks = JsonConvert.DeserializeObject<List<Block>>(JsonDocument);
            Output = Colors[SelectedColor];
            OutputDisplay = true;
        }
    }
}