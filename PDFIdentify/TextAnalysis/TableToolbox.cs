using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PDFIdentify.DataStructure.ResponseObjects;

namespace PDFIdentify
{
    public class TableToolbox
    {
        [JsonProperty("Blocks")] public List<Block> Blocks { get; set; }
        
        [JsonProperty("Table")] public Block[,] Table { get; set; }
        
        [JsonProperty("BlockLookup")] public Dictionary<string, Block> BlockLookup { get; set; }
        

        public TableToolbox()
        {
            Blocks = new List<Block>();
            Table = new Block[,]{};
            BlockLookup = new Dictionary<string, Block>();
        }

        public TableToolbox(List<Block> blocks)
        {
            Blocks = blocks;
            Table = new Block[,]{};
            BlockLookup = new Dictionary<string, Block>();
        }
        

        [JsonConstructor]
        public TableToolbox(List<Block> blocks, Block[,] table, Dictionary<string, Block> blockLookup)
        {
            Blocks = blocks;
            Table = table;
            BlockLookup = blockLookup;
        }

        public void FilterToChildren(string blockId)
        {
            Dictionary<string, Block> allBlocks = new Dictionary<string, Block>();
            foreach (var block in Blocks)
            {
                allBlocks.Add(block.Id, block);
            }
            List<Block> children = new List<Block>();
            Blocks = Analyze.GetChildrenRecursive(allBlocks, allBlocks[blockId], children);
        }

        public void ConstructTable(string tableId)
        {
            int column = 0;
            int row = 0;
            Dictionary<string, Block> allBlocks = new Dictionary<string, Block>();
            foreach (var block in Blocks)
            {
                allBlocks.Add(block.Id, block);
            }

            foreach (var relationship in allBlocks[tableId].Relationships)
            {
                if (relationship.Type.Equals("CHILD"))
                {
                    foreach (var id in relationship.Ids)
                    {
                        Block block = allBlocks[id];
                        column = Math.Max(column, block.ColumnIndex);
                        row = Math.Max(row, block.RowIndex);
                    }
                }
            }

            Block[,] table = new Block[column, row];
            foreach (var relationship in allBlocks[tableId].Relationships)
            {
                if (relationship.Type.Equals("CHILD"))
                {
                    foreach (var id in relationship.Ids)
                    {
                        Block block = allBlocks[id];
                        //Adjust so Column 1 maps to index 0
                        if (block.BlockType.Equals("CELL"))
                        {
                            table[block.ColumnIndex - 1, block.RowIndex - 1] = block;
                        }
                    }
                }
            }

            Table = table;
        }

        public int[] FindIndex(List<string> searchParams)
        {
            Dictionary<string, Block> allBlocks = new Dictionary<string, Block>();
            foreach (var block in Blocks)
            {
                allBlocks.Add(block.Id, block);
            }

            for (int y = 0; y < Table.GetLength(1); y++)
            {
                for (int x = 0; x < Table.GetLength(0); x++)
                {
                    //get a CELL in ROW 1
                    Block cell = Table[x, y];
                    if (!(cell.Relationships is null))
                    {
                        //check all of its children for the search terms
                        foreach (var relationship in cell.Relationships)
                        {
                            if (relationship.Type.Equals("CHILD"))
                            {
                                foreach (var id in relationship.Ids)
                                {
                                    if (searchParams.Contains(allBlocks[id].Text.ToLower()))
                                    {
                                        //return the column index
                                        return new[] {x, y};
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // add other search cases
        
        
            // if nothing found, return -1
            return new[]{ -1, -1 };
        }

        public void BuildBlockLookup()
        {
            BlockLookup = new Dictionary<string, Block>();
            foreach (var block in Blocks)
            {
                BlockLookup.Add(block.Id, block);
            }
        }


        public string GetAllTextFromCell(int columnIndex, int rowIndex)//Block cell, List<Block> fullList
        {
            //Locate the cell in the table based on coordinates
            Block cell = Table[columnIndex, rowIndex];

            //Instantiate a string to contain all text from cell
            string cellText = "";
            
            //If cell has no children, it has no text so return empty string
            if (cell.Relationships is null) { return cellText; }

            //Each relationship corresponds with a list of IDs of that type (i.e. {"CHILD":["id1", "id2", "id3"...]}
            foreach (var relationship in cell.Relationships)
            {
                //Only evaluate CHILD relationships
                if (relationship.Type.Equals("CHILD"))
                {
                    //evaluate each Child's type
                    foreach (var id in relationship.Ids)
                    {
                        //if Child is of "WORD" type, append its text
                        if (BlockLookup[id].BlockType.Equals("WORD"))
                        {
                            cellText += BlockLookup[id].Text;
                        }
                    }
                }
            }

            return cellText;
        }
    }
}