/* ------------------------------
 * Keith Olsen - 1 September 2018
 * Oregon State University
 * keith.a.olsen@gmail.com
 * ------------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.UI.DataVisualization.Charting;

// ******** Classes for the output graphs

public class readOutput_class
{
    public ArrayList readCSV(string fileNameText, ref string[] fieldNames)
    {
        char[] delimiterChars = { ' ', ',', '\t' };
        Regex rex = new Regex(@"[ ]");
        string line;
        ArrayList dataArray = new ArrayList();

        try
        {
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(fileNameText))
            {
                line = sr.ReadLine();
                // replace all occurances of [ ] with ""
                line = rex.Replace(line, "");
                fieldNames = line.Split(delimiterChars);

                while ((line = sr.ReadLine()) != null)
                {
                    // replace all occurances of [ ] with ""
                    line = rex.Replace(line, "");
                    dataArray.Add(line.Split(delimiterChars));
                }
            }

            return dataArray;
        }
        catch
        {
            dataArray.Clear();
            return dataArray;
        }

    }
    public void loadChartSeries(string fileName, Series outputChartSeries, ArrayList dataArray, string[] fieldNames, string outputVariable, int chartType, string standID, string speciesName)
    {
        double yearAsNum;
        int fieldNum = -1;
        int yearFieldNum = -1;
        int speciesFieldNum = -1;
        int portionFieldNum = -1;
        int standIDFieldNum = -1;

        for (int i = 0; i < fieldNames.Length; i++)
        {
            if (fieldNames[i] == outputVariable)
                fieldNum = i;

            if (chartType == 3)
            {
                if (fieldNames[i] == "YEAR_END")
                    yearFieldNum = i;
            }
            else
            {
                if (fieldNames[i] == "YEAR_AGG")
                    yearFieldNum = i;
            }

            if (fieldNames[i] == "SPECIES")
                speciesFieldNum = i;
            if (fieldNames[i] == "PORTION")
                portionFieldNum = i;
            if (fieldNames[i] == "STANDID")
                standIDFieldNum = i;
        }

        if (fieldNum == -1 || speciesFieldNum == -1 || standIDFieldNum == -1 || yearFieldNum == -1)
            outputChartSeries.LegendText = "Error finding field names " + outputVariable;
        else
        {
            if (chartType == 1 && portionFieldNum != -1)
                foreach (string[] dataLine in dataArray)
                {
                    yearAsNum = Convert.ToInt16(dataLine[yearFieldNum]);
                    if (dataLine[speciesFieldNum] == speciesName && dataLine[portionFieldNum] == "LIVE" && dataLine[standIDFieldNum] == standID)
                        outputChartSeries.Points.AddXY(yearAsNum, Convert.ToDouble(dataLine[fieldNum]));
                }
            else if (chartType == 2 && portionFieldNum != -1)
                foreach (string[] dataLine in dataArray)
                {
                    yearAsNum = Convert.ToInt16(dataLine[yearFieldNum]);
                    if (dataLine[speciesFieldNum] == speciesName && dataLine[portionFieldNum] == "MORTALITY" && dataLine[standIDFieldNum] == standID)
                        outputChartSeries.Points.AddXY(yearAsNum, Convert.ToDouble(dataLine[fieldNum]));
                }
            else if (chartType == 3)
                foreach (string[] dataLine in dataArray)
                {
                    yearAsNum = Convert.ToInt16(dataLine[yearFieldNum]);
                    if (dataLine[speciesFieldNum] == speciesName && dataLine[standIDFieldNum] == standID)
                        outputChartSeries.Points.AddXY(yearAsNum, Convert.ToDouble(dataLine[fieldNum]));
                }
            else if (chartType == 4 && portionFieldNum != -1)
                foreach (string[] dataLine in dataArray)
                {
                    yearAsNum = Convert.ToInt16(dataLine[yearFieldNum]);
                    if (dataLine[speciesFieldNum] == speciesName && dataLine[portionFieldNum] == "LIVE" && dataLine[standIDFieldNum] == standID)
                        outputChartSeries.Points.AddXY(yearAsNum, Convert.ToDouble(dataLine[fieldNum]));
                }
            else if (chartType == 5 && portionFieldNum != -1)
                foreach (string[] dataLine in dataArray)
                {
                    yearAsNum = Convert.ToInt16(dataLine[yearFieldNum]);
                    if (dataLine[speciesFieldNum] == speciesName && dataLine[portionFieldNum] == "MORTALITY" && dataLine[standIDFieldNum] == standID)
                        outputChartSeries.Points.AddXY(yearAsNum, Convert.ToDouble(dataLine[fieldNum]));
                }
            else
                outputChartSeries.LegendText = "Error: Unrecognised chart type " + chartType.ToString() + " or missing PORTION field";
        }

    }
    public bool readCSV2Series(string fileName, Series outputChartSeries, string outputVariable, int chartType, string standID, string speciesName)
    {
        ArrayList dataArray;
        string[] fieldNames = { "" };

        // read from landcarb output file
        dataArray = readCSV(fileName, ref fieldNames);

        // return error state
        if (dataArray.Count > 0)
        {
            loadChartSeries(fileName, outputChartSeries, dataArray, fieldNames, outputVariable, chartType, standID, speciesName);
            dataArray.Clear();
            return (false);
        }
        else
            return (true);
    }

    public List<string> findSpeciesByStand(string fileName, string standID)
    {
        ArrayList dataArray;
        List<string> speciesList = new List<string>();
        string[] fieldNames = { "" };
        int speciesFieldNum = -1;
        int standIDFieldNum = -1;


        // read from landcarb output file
        dataArray = readCSV(fileName, ref fieldNames);

        for (int i = 0; i < fieldNames.Length; i++)
        {
            if (fieldNames[i] == "SPECIES")
                speciesFieldNum = i;
            if (fieldNames[i] == "STANDID")
                standIDFieldNum = i;
        }

        if (standIDFieldNum == -1 || speciesFieldNum == -1)
            return null;
        else
        {

            foreach (string[] dataLine in dataArray)
            {
                if (dataLine[standIDFieldNum] == standID)
                    if (!speciesList.Contains(dataLine[speciesFieldNum]) && dataLine[speciesFieldNum] != "ALL")
                        speciesList.Add(dataLine[speciesFieldNum]);
            }
        }

        return speciesList;
    }
}

public class graphTitle_class
{
    public string lookupGraphTitle(string fileName)
    {
        if (fileName.EndsWith("Mass.csv"))
        {
            if (fileName.StartsWith("TotalForestSector"))
                return ("Total Stores for Forest Sector (Ecosystem + Products + Substitution)");
            else if (fileName.StartsWith("TotalForestEcosystem"))
                return ("Total Stores for Forest Ecosystem (Live + Dead + Stable Soils)");
            else if (fileName.StartsWith("TotalForestProduct"))
                return ("Total Stores Related to Forest Products (Use + Disposal)");
            else if (fileName.StartsWith("TotalSubstitution"))
                return ("Total Stores Related to Forest Substitution (Products + Biofuels)");
            else if (fileName.StartsWith("Biofuel"))
                return ("Cummulative Biofuel Offset by Manufacturing and Disposal");
            else if (fileName.StartsWith("Live"))
                return ("Total Stores for All Live Pools");
            else if (fileName.StartsWith("Dead"))
                return ("Total Stores for all Dead Pools");
            else if (fileName.StartsWith("Stable"))
                return ("Total Stores of Stable (Soil) Pools");
            else if (fileName.StartsWith("Disposal"))
                return ("Total Stores for Disposed Wood Products");
            else if (fileName.StartsWith("Harvest"))
                return ("Annual Amount of Carbon Harvested");
            else if (fileName.StartsWith("ProductsInUse"))
                return ("Total Stores for Wood Products in Use");
            else if (fileName.StartsWith("ProductSubstitution"))
                return ("Total Stores for Product Substitution");
        }
        else if (fileName.EndsWith("Balance.csv"))
        {
            if (fileName.StartsWith("TotalForestSector"))
                return ("Net Balance for Forest Sector (Ecosystem + Products + Substitution)");
            else if (fileName.StartsWith("TotalForestEcosystem"))
                return ("Net Balance for Forest Ecosystem (Live + Dead + Stable Soils)");
            else if (fileName.StartsWith("TotalForestProduct"))
                return ("Net Balance for Forest Products (Use + Disposal)");
            else if (fileName.StartsWith("TotalSubstitution"))
                return ("Net Balance Related to Forest Substitution (Products + Biofuels)");
            else if (fileName.StartsWith("Biofuel"))
                return ("Production of Biofuels in Manufacturing and Disposal");
            else if (fileName.StartsWith("Live"))
                return ("Net Balance for All Live Pools");
            else if (fileName.StartsWith("Dead"))
                return ("Net Balance for All Dead Pools");
            else if (fileName.StartsWith("Stable"))
                return ("Net Balance of Stable (Soil) Pools");
            else if (fileName.StartsWith("Disposal"))
                return ("Net Balance for Disposed Wood Products");
           else if (fileName.StartsWith("ProductsInUse"))
                return ("Net Balance for Wood Products in Use");
            else if (fileName.StartsWith("ProductSubstitution"))
                return ("Net Balance for Product Substitution");
        }
        else if (fileName.EndsWith("Areas.csv"))
        {
            if (fileName.StartsWith("HarvestEvent"))
                return ("Harvest Events");
            else if (fileName.StartsWith("PrescribedFireEvent"))
                return ("Prescribed Broadcast Burn Events");
            else if (fileName.StartsWith("BurnPileFireEvent"))
                return ("Prescribed Burn Pile Events");
            else if (fileName.StartsWith("WildFireEvent"))
                return ("Wildfire Events");
            else if (fileName.StartsWith("SalvageEvent"))
                return ("Salvage Events");
            else if (fileName.StartsWith("InsectEvent"))
                return ("Insect Events");
        }
        else
        {
            if (fileName.StartsWith("ForestNep"))
                return ("Net Ecosystem Production of Stand (NPP-Rh)");
            else if (fileName.StartsWith("ForestNpp"))
                return ("Net Primary Production of Stand (NPP)");
            else if (fileName.StartsWith("ForestRh"))
                return ("Heterotrophic Respiration of Stand (Rh)");
            else if (fileName.StartsWith("ManufacturingFlux"))
                return ("Output of Solid Forest Products via Manufacturing");
            else if (fileName.StartsWith("HarvestYearArea.txt"))
                return ("Percent of Harvest Target Achieved");
        }

        return ("No Title Found for " + fileName);

    }
}