/*
    CSVReader by Dock. (24/8/11)
    http://starfruitgames.com

    usage:
    CSVReader.SplitCsvGrid(textString)

    returns a 2D string array.

    Drag onto a gameobject for a demo of CSV parsing.
*/

using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class CSVReader : MonoBehaviour
{
    public TextAsset csvFile;

    public void Start()
    {
        string[,] grid = SplitCsvGrid(csvFile.text);
        Debug.Log("size = " + (1 + grid.GetUpperBound(0)) + "," + (1 + grid.GetUpperBound(1)));

        DebugOutputGrid(grid);
    }

    /// <summary>
    /// outputs the content of a 2D array, useful for checking the importer
    /// </summary>
    /// <param name="grid">String array</param>
    static public void DebugOutputGrid(string[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y < grid.GetUpperBound(1); y++)
        {
            for (int x = 0; x < grid.GetUpperBound(0); x++)
            {

                textOutput += grid[x, y];
                textOutput += "|";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }

    // splits a CSV file into a 2D string array
    static public string[,] SplitCsvGrid(string csvText)
    {
        string[] lines = csvText.Split("\n"[0]);

        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width, lines.Length];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output.
                // Include or edit it as you wish.
                //outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    // splits a CSV file into a 2D string array
    static public int[,] SplitCsvGridToInt(string csvText)
    {
        var grid = SplitCsvGrid(csvText);

        int[,] outputGrid = new int[grid.GetLength(0) - 1, grid.GetLength(1)];

        for (int i = 0; i < outputGrid.GetLength(0); i++)
        {
            for (int j = 0; j < outputGrid.GetLength(1); j++)
            {
                int variable = 0;
                int.TryParse(grid[i, j], out variable);
                outputGrid[i, j] = variable;
            }
        }

        return outputGrid;
    }

    static public int[,] SplitCsvGridTiled(string csvText)
    {
        var grid = SplitCsvGrid(csvText);

        int[,] outputGrid = new int[grid.GetLength(0) - 1, grid.GetLength(1) - 1];

        for (int i = 0; i < outputGrid.GetLength(0); i++)
        {
            for (int j = 0; j < outputGrid.GetLength(1); j++)
            {
                int variable = 0;
                int.TryParse(grid[i, j], out variable);
                outputGrid[i, j] = variable;
            }
        }

        return outputGrid;
    }

    // splits a CSV row
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
        @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
        System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}