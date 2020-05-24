using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
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
        public string BlockId { get; set; }
        public Dictionary<string, string> ColorToBlockMap { get; set; }
        public string JsonDoc { get; set; }
        public string Output { get; set; }

        public void OnGet()
        {
            ColorList = new List<string>(){"Hold"};
        }

        public void OnPostUpload(IFormFile file)
        {
            PhotoData = IndexOperations.UploadActions(file);
            PhotoDataUrl = PhotoData.DataUrl;
            JsonDoc = JsonConvert.SerializeObject(PhotoData.TextractResponse.Blocks);
            ColorToBlockMap = PhotoData.Colors;
            ColorList = ColorToBlockMap.Keys;

            //HTTP Save Session Data
            HttpContext.Session.SetString("JsonDoc",JsonDoc);
            HttpContext.Session.SetString("ColorToBlockMap", JsonConvert.SerializeObject(ColorToBlockMap));
            HttpContext.Session.SetString("Blocks", JsonConvert.SerializeObject(Blocks));

            Uploaded = true;
        }

        public void OnPostConfirm(string confirmList)
        {
            
            
            //HTTP Read Session Data
            JsonDoc = HttpContext.Session.GetString("JsonDoc");
            ColorToBlockMap = JsonConvert.DeserializeObject<Dictionary<string,string>>(HttpContext.Session.GetString("ColorToBlockMap"));
            Blocks = JsonConvert.DeserializeObject<List<Block>>(HttpContext.Session.GetString("Blocks"));
            
            OutputDisplay = true;
            SelectedColor = confirmList;
            Blocks = JsonConvert.DeserializeObject<List<Block>>(JsonDoc);
            BlockId = ColorToBlockMap[SelectedColor];

            List<AssetData> assets = IndexOperations.RetrieveAssetData(BlockId, Blocks);
            
            foreach (var asset in assets)
            {
                Output += asset.Name + " x " + asset.Quantity + "\n";
            }
        }
    }
}