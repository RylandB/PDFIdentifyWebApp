using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using PDFIdentify.DataStructure;
using PDFIdentify.DataStructure.ResponseObjects;

namespace PDFIdentify
{
    public static class Analyze
    {
        public static async Task<TextractResponse> AnalyzeFile(string fileName, string serverAddress)
        {
            Console.Out.WriteLine(serverAddress.AppendPathSegment(fileName));
            var getResp = await (serverAddress.AppendPathSegment(fileName))
                .GetJsonAsync<TextractResponse>();

            return getResp;
        }
        
        public static List<Block> GetChildrenRecursive(Dictionary<string, Block> blockDictionary, Block block,
            List<Block> targetList)
        {
            //Add this block to the list
            targetList.Add(block);
            
            //for each child not already in the list, navigate to that block
            if (block.Relationships is null) return targetList;
            
            foreach (var relationship in block.Relationships)
            {
                if (!relationship.Type.Equals("CHILD")) continue;

                foreach (var id in relationship.Ids.Where(id => !targetList.Contains(blockDictionary[id])))
                {
                    targetList = GetChildrenRecursive(blockDictionary, blockDictionary[id], targetList);
                }
            }
            return targetList;
        }
    }
}
