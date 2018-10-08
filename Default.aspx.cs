using System;
using System.Collections.Generic;
using System.Linq;
// using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections;
using System.IO;
// using System.Diagnostics;
using System.Text.RegularExpressions;
// using Ionic.Zip; // DotNetZip

using System.Web.UI.DataVisualization.Charting;
using System.Drawing; // for Color

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            loadAllStands();
            tb_fileDump.Text = "**NOTE: Processing individual trees may take up to 15 seconds";
        }

    }

    protected void ddl_select_by_change(object sender, EventArgs e)
    {
        clearFileViewer();
        ddl_select_class.Items.Clear();
        if (ddl_select_by.SelectedValue == "Geo_loc")
            loadGeoLocItems(ddl_select_class.Items, 4);
        else if (ddl_select_by.SelectedValue == "admin_unit")
            loadAdminUnitItems(ddl_select_class.Items);
        else if (ddl_select_by.SelectedValue == "study_focus")
            loadStudyFocusItems(ddl_select_class.Items);
        else if (ddl_select_by.SelectedValue == "all_stands")
            loadAllStands();
    }

protected void loadGeoLocItems(ListItemCollection uiListBox, int fieldNum)
    {
        string fileName;
        string[] fieldNames = { "" };
        ArrayList dataArray;
        string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics
        fileName = appPath + "\\vegData\\PSP_stand_info_subset_2015.csv";

        uiListBox.Add(new ListItem("Select location"));

        dataArray = readCSV(fileName, ref fieldNames);
        foreach (string[] dataLine in dataArray)
        {
            if (uiListBox.FindByValue(dataLine[fieldNum]) == null && dataLine[fieldNum] != "SQNP" && dataLine[fieldNum] != "RockyMtns")
                uiListBox.Add(new ListItem(getReadableText(dataLine[fieldNum]), dataLine[fieldNum]));
        }

        uiListBox.Add(new ListItem("Mount Rainier National Park", "MRRS"));

    }

    protected void loadAdminUnitItems(ListItemCollection uiListBox)
    {
        uiListBox.Add(new ListItem("Select location"));
        uiListBox.Add(new ListItem("Cascade Head Experimental Forest", "CHEF"));
        uiListBox.Add(new ListItem("HJ Andrews Exp. Forest Reference Stands", "HJRS"));
        uiListBox.Add(new ListItem("HJ Andrews Exp. Forest Watersheds", "HJWS"));
        uiListBox.Add(new ListItem("Wind River Experimental Forest", "WREF"));
        uiListBox.Add(new ListItem("Mount St. Helens", "MSHV"));
        uiListBox.Add(new ListItem("Olympic National Park", "OLYNP"));
        uiListBox.Add(new ListItem("Mount Rainier National Park", "MRRS"));
    }

    protected void loadStudyFocusItems(ListItemCollection uiListBox)
    {
        uiListBox.Add(new ListItem("Select location"));
        uiListBox.Add(new ListItem("Alder-Conifer Study", "ALCO"));
        uiListBox.Add(new ListItem("Douglas-fir Growth and Yield", "DFGY"));
        uiListBox.Add(new ListItem("Hemlock-Spruce Growth and Yield", "HSGY"));
        uiListBox.Add(new ListItem("Mountain Hemlock Growth and Yield", "MHGY"));
        uiListBox.Add(new ListItem("Noble Fir Growth and Yield", "NFGY"));
        uiListBox.Add(new ListItem("Ponderosa Pine Growth and Yield", "PPGY"));
    }

    protected void loadAllStands()
    {
        string fileName;
        string[] fieldNames = { "" };
        ArrayList dataArray;
        string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics
        fileName = appPath + "\\vegData\\PSP_stand_info_subset_2015.csv";
        string textToAdd;
        List<string> removeStands = new List<string>();
        removeStands = loadRemoveStands();

        dataArray = readCSV(fileName, ref fieldNames);

        cbList_stands.Items.Clear();
        foreach (string[] dataLine in dataArray)
        {
            textToAdd = dataLine[1] + " - [Study ID] " + dataLine[0] + " - [Location] " + dataLine[4];
            if (!removeStands.Contains(dataLine[1]))
                cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
        }

        ListItem[] sortItems = cbList_stands.Items.Cast<ListItem>().OrderBy(i => i.Text).ToArray();
        cbList_stands.Items.Clear();
        cbList_stands.Items.AddRange(sortItems);

        if (IsPostBack)
        {
            clearFileViewer();
            foreach (ListItem li in cbList_stands.Items)
            {
                li.Selected = true;
            }
        }
    }

    protected string getReadableText(string inText)
    {
        if (inText == "SQNP")
            return ("Sequoia National Park");
        else if (inText == "RockyMtns")
            return ("Rocky Mountains");
        else if (inText == "OR_Coast")
            return ("Oregon - Coast");
        else if (inText == "OR_WestCasc")
            return ("Oregon - West Cascades");
        else if (inText == "OR_EastCasc")
            return ("Oregon - East Cascades");
        else if (inText == "WA_WestCasc")
            return ("Washington - West Cascades");
        else if (inText == "WA_OlyPen")
            return ("Washington - Olympic Peninsula");
        else
            return (inText);
    }

    protected void ddl_select_class_SelectedIndexChanged(object sender, EventArgs e)
    {
        string fileName, textToAdd;
        string[] fieldNames = { "" };
        IList<string> standIDList_CHEF = new List<string>() { "CH01", "CH03", "CH04", "CH05", "CH06", "CH07", "CH08", "CH09", "CH10", "CH11", "CH12"
                                                            , "CH13", "CH14", "CH15", "CH17", "CH22", "CH23", "CH41", "CH42", "HS01", "NCNA" };
        IList<string> standIDList_WREF = new List<string>() { "MUN2", "MUNA", "WR02", "WR04", "WR05", "WR09", "WR90" };
        IList<string> standIDList_OLYNP = new List<string>() { "HR01", "HR02", "HR03", "HR04" };
        ArrayList dataArray;
        string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics
        fileName = appPath + "\\vegData\\PSP_stand_info_subset_2015.csv";
        List<string> removeStands = new List<string>();
        removeStands = loadRemoveStands();

        // tb1.Text = "";
        clearFileViewer();
        dataArray = readCSV(fileName, ref fieldNames);
        // tb1.Text += fieldNames[0] + "\t" + fieldNames[1] + "\t" + fieldNames[4] + "\t\t" + fieldNames[6].Substring(7, 6) + "\t" + fieldNames[8] + "\t" + fieldNames[9] + "\n";

        cbList_stands.Items.Clear();
        foreach (string[] dataLine in dataArray)
        {
            textToAdd = dataLine[1] + " - [Study ID] " + dataLine[0] + " - [Location] " + dataLine[4];
            if (ddl_select_by.SelectedValue == "Geo_loc")
            {
                if (ddl_select_class.SelectedValue == "MRRS")
                {
                    if (dataLine[0] == ddl_select_class.SelectedValue)
                    {
                        if (!removeStands.Contains(dataLine[1]))
                            cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
                    }
                }
                else
                {
                    if (dataLine[4] == ddl_select_class.SelectedValue)
                    {
                        // writeLineToTb1(dataLine);
                        if (!removeStands.Contains(dataLine[1]))
                            cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
                    }
                }
            }
            else if (ddl_select_by.SelectedValue == "admin_unit" | ddl_select_by.SelectedValue == "study_focus")
            {
                if (ddl_select_class.SelectedValue == "CHEF")
                {
                    if (standIDList_CHEF.Contains(dataLine[1]) && !removeStands.Contains(dataLine[1]))
                        cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
                }
                else if (ddl_select_class.SelectedValue == "WREF")
                {
                    if (standIDList_WREF.Contains(dataLine[1]) && !removeStands.Contains(dataLine[1]))
                        cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
                }
                else if (ddl_select_class.SelectedValue == "OLYNP")
                {
                    if (standIDList_OLYNP.Contains(dataLine[1]) && !removeStands.Contains(dataLine[1]))
                        cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
                }
                else if (dataLine[0] == ddl_select_class.SelectedValue)
                {
                    if (!removeStands.Contains(dataLine[1]))
                        cbList_stands.Items.Add(new ListItem(textToAdd, dataLine[1]));
                }
            }
        }

        ListItem[] sortItems = cbList_stands.Items.Cast<ListItem>().OrderBy(i => i.Text).ToArray();
        cbList_stands.Items.Clear();
        cbList_stands.Items.AddRange(sortItems);

        btn_selectAll_Click(sender, e);
    }

    protected void writeLineToTb1(string[] dataLine)
    {
        string pad;

        if (dataLine[4].Length < 9)
            pad = "     ";
        else
            pad = "";

        // tb1.Text += dataLine[0] + "\t\t" + dataLine[1] + "\t" + dataLine[4] + pad + "\t" + dataLine[6] + "\t" + dataLine[8] + "\t\t" + dataLine[9] + "\n";
    }

    protected void loadAndSubsetByStand(string fileName, int outModeInt, string outfileName)
    {
        string[] fieldNames = { "" };
        string headerLine = "";
        int rowCount = 0;
        List<string> standIDList = new List<string>();
        List<string> dataArray;
        string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics

        foreach (ListItem i in cbList_stands.Items)
        {
            if (i.Selected == true)
                standIDList.Add(i.Value);
        }

        dataArray = readCSVandSubset(appPath + fileName, ref fieldNames, standIDList, 2, 0, 4);

        // Get header line
        foreach (string fieldName in fieldNames)
        {
            if (fieldName == fieldNames[0])
                headerLine = fieldName;
            else
                headerLine += "," + fieldName;
        }

        if (outModeInt == 1)
        {
            tb_fileDump.Text = "Writing output file... please wait";

            // open new file handle

            using (StreamWriter sw = new StreamWriter(appPath + "exportFiles\\" + outfileName))
            {
                sw.WriteLine(headerLine);
                foreach (string dataLine in dataArray)
                {
                    sw.WriteLine(dataLine);
                }

                tb_fileDump.Text = "Export complete";

            }

            // send output file to client
            // Response.ContentType = "application/x-zip-compressed";
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + outfileName);
            Response.TransmitFile(appPath + "exportFiles\\" + outfileName);
            Response.End();
        }
        else
        {
            tb_fileDump.Text = headerLine + "\n";
            foreach (string dataLine in dataArray)
            {
                tb_fileDump.Text += dataLine + "\n";
                rowCount++;
                if (rowCount == 10)
                    break;
            }
        }
    }

    //protected void btn1_click(object sender, EventArgs e)
    //{
    //    string line;
    //    string fileName;
    //    string[] fieldNames = { "" };
    //    ArrayList dataArray;
    //    string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics
    //    fileName = appPath + "\\vegData\\PSP_stand_info_subset_2015.csv";

    //    tb1.Text = "";
    //    using (StreamReader sr = new StreamReader(fileName))
    //    {
    //        while ((line = sr.ReadLine()) != null)
    //        {
    //            tb1.Text += line + "\n";
    //        }
    //    }

    //    dataArray = readCSV(fileName, ref fieldNames);
    //    foreach (string[] dataLine in dataArray)
    //    {
    //        tb1.Text += dataLine[4] + "\n";
    //    }

    //}

    protected ArrayList readCSV(string fileNameText, ref string[] fieldNames)
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

    protected Dictionary<string, string> readCSVdict(string fileNameText, List<string> fieldNamesKey, List<string> fieldNamesValue)
    {
        char[] delimiterChars = { ',', '\t' };
        Regex rex = new Regex(@"[ ]");
        Regex rex2 = new Regex("\"");
        Regex rex3 = new Regex(",");
        string line;
        string[] fieldNames;
        string[] dataLine;
        Dictionary<string, string> dataDict = new Dictionary<string, string>();
        List<int> keyIndexList = new List<int>();
        List<int> valueIndexList = new List<int>();
        string keyString;
        string valueString;
        int checkNotesIndex;

        try
        {
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(fileNameText))
            {
                line = sr.ReadLine();
                // replace all occurances of [ ] with ""
                line = rex.Replace(line, "");
                fieldNames = line.Split(delimiterChars);
                checkNotesIndex = findStringListIndex(fieldNames, "check_notes");

                // find the index values for the important fields
                foreach (string fieldKey in fieldNamesKey)
                {
                    keyIndexList.Add(findStringListIndex(fieldNames, fieldKey));
                }
                foreach (string fieldValue in fieldNamesValue)
                {
                    valueIndexList.Add(findStringListIndex(fieldNames, fieldValue));
                }

                if (keyIndexList.Count() == 1 && valueIndexList.Count() == 1)
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        // replace all occurances of [ ] with ""
                        line = rex.Replace(line, "");
                        dataLine = line.Split(delimiterChars);

                        // add key and value pair to dictionary
                        if (!dataDict.ContainsKey(rex2.Replace(dataLine[keyIndexList[0]], "")))
                        {
                            dataDict.Add(rex2.Replace(dataLine[keyIndexList[0]], ""), rex2.Replace(dataLine[valueIndexList[0]], ""));
                        }
                    }
                }
                else
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        // replace all occurances of [ ] with ""
                        // line = rex.Replace(line, "");
                        dataLine = line.Split(delimiterChars);

                        // repair check_notes field since the original had commas
                        if (fieldNames.Count() != dataLine.Count())
                        {
                            for (int i = checkNotesIndex + 1; i <= checkNotesIndex + (dataLine.Count() - fieldNames.Count()); i++)
                            {
                                dataLine[checkNotesIndex] += "; " + dataLine[i];
                            }
                        }

                        // build key and value strings
                        keyString = dataLine[keyIndexList[0]];
                        for (int i = 1; i < keyIndexList.Count(); i++)
                        {
                            keyString += dataLine[keyIndexList[i]];
                        }
                        valueString = rex3.Replace(dataLine[valueIndexList[0]], ";");
                        for (int i = 1; i < valueIndexList.Count(); i++)
                        {
                            valueString += "," + rex3.Replace(dataLine[valueIndexList[i]], ";");
                        }

                        // add key and value pair to dictionary
                        if (!dataDict.ContainsKey(rex2.Replace(keyString, "")))
                        {
                            dataDict.Add(rex2.Replace(keyString, ""), rex2.Replace(valueString, ""));
                        }
                    }

                }
            }

            return dataDict;
        }
        catch
        {
            dataDict.Clear();
            return dataDict;
        }

    }

    protected int findStringListIndex(string[] stringList, string stringValue)
    {
        for (int i = 0; i < stringList.Count(); i++)
        {
            if (stringList[i] == stringValue)
                return (i);
        }

        return (999);
    }

    protected List<string> readCSVandSubset(string fileNameText, ref string[] fieldNames, List<string> standIDList, int fieldNum, int startSubset, int endSubset)
    {
        char[] delimiterChars = { ' ', ',', '\t' };
        Regex rex = new Regex(@"[ ]");
        string line;
        List<string> dataArray = new List<string>();
        string[] dataLine;
        Dictionary<string, string> dataDict = new Dictionary<string, string>();
        Dictionary<string, string> dataDict2 = new Dictionary<string, string>();
        string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics
        int plotIDIndex;
        int startStringLen;

        try
        {
            // Read the file and display it line by line.
            using (StreamReader sr = new StreamReader(fileNameText))
            {
                line = sr.ReadLine();
                // replace all occurances of [ ] with ""
                line = rex.Replace(line, "");
                fieldNames = line.Split(delimiterChars);

                if (fileNameText.Contains("tp00111"))
                {
                    dataDict = readCSVdict(appPath + "vegData\\tp00101_01-20-2016.csv", new List<string> { "treeid" }, new List<string> { "species" });
                    dataDict2 = readCSVdict(appPath + "vegData\\tp00102_01-20-2016.csv", new List<string> { "treeid", "year" }, new List<string> { "tag", "tree_status", "dbh", "check_notes" });

                    // enlarge fieldNames array for new fields
                    Array.Resize(ref fieldNames, fieldNames.Length + 5);
                    fieldNames[fieldNames.Length - 1] = "CHECK_NOTES";
                    fieldNames[fieldNames.Length - 2] = "DBH";
                    fieldNames[fieldNames.Length - 3] = "TREE_STATUS";
                    fieldNames[fieldNames.Length - 4] = "TAG";
                    fieldNames[fieldNames.Length - 5] = "SPECIES";

                    while ((line = sr.ReadLine()) != null)
                    {
                        // replace all occurances of [ ] with ""
                        line = rex.Replace(line, "");
                        dataLine = line.Split(delimiterChars);
                        if (standIDList.Contains(dataLine[fieldNum].Substring(startSubset, endSubset)))
                        {
                            if (dataDict.ContainsKey(dataLine[2]))
                            {
                                if (dataDict2.ContainsKey(dataLine[2] + dataLine[5]))
                                    dataArray.Add(line + "," + dataDict[dataLine[2]] + "," + dataDict2[dataLine[2] + dataLine[5]]);
                                else
                                    dataArray.Add(line + "," + dataDict[dataLine[2]] + ",**MISSING_DATA**");
                            }
                            else
                                dataArray.Add(line + ",**MISSING_DATA**");
                        }
                    }
                }
                else if (fileNameText.Contains("tp00108") || fileNameText.Contains("tp00109"))
                {
                    plotIDIndex = findStringListIndex(fieldNames, "PLOTID");

                    // enlarge fieldNames array for new fields
                    Array.Resize(ref fieldNames, fieldNames.Length + 1);
                    for (int i = fieldNames.Length - 1; i > 1; i--)
                    {
                        if (i == 2)
                        {
                            fieldNames[i] = "STANDID";
                        }
                        else
                        {
                            fieldNames[i] = fieldNames[i - 1];
                        }
                    }

//                    fieldNames[plotIDIndex - 1] = "STANDID";

                    while ((line = sr.ReadLine()) != null)
                    {
                        // replace all occurances of [ ] with ""
                        line = rex.Replace(line, "");
                        dataLine = line.Split(delimiterChars);
                        startStringLen = dataLine[0].Length + dataLine[1].Length + 2;
                        if (standIDList.Contains(dataLine[fieldNum].Substring(startSubset, endSubset)))
                            dataArray.Add(line.Substring(0,startStringLen) + dataLine[plotIDIndex].Substring(0, 4) + line.Substring(startStringLen - 1, line.Length - startStringLen + 1));
                    }
                }
                else if (fileNameText.Contains("PSP_stand_info"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        // replace all occurances of [ ] with ""
                        line = rex.Replace(line, "");
                        dataLine = line.Split(delimiterChars);
                        if (standIDList.Contains(dataLine[1]))
                            dataArray.Add(line);
                    }
                }
                else
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        // replace all occurances of [ ] with ""
                        line = rex.Replace(line, "");
                        dataLine = line.Split(delimiterChars);
                        if (standIDList.Contains(dataLine[fieldNum].Substring(startSubset, endSubset)))
                            dataArray.Add(line);
                    }
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

    protected void btn_selectNone_Click(object sender, EventArgs e)
    {
        cbList_stands.ClearSelection();
        clearFileViewer();
    }
    protected void btn_selectAll_Click(object sender, EventArgs e)
    {
        clearFileViewer();
        foreach (ListItem li in cbList_stands.Items)
        {
            li.Selected = true;
        }
    }
    protected void btn_downloadStandID_Click(object sender, EventArgs e)
    {
        int outModeInt = 1;
        loadAndSubsetByStand("\\vegData\\PSP_stand_info_subset_2015.csv", outModeInt, "PSP_stand_info_subset.csv");
    }

    //protected void zipOutputFiles()
    //{
    //    try
    //    {
    //        using (ZipFile outputZipFile = new ZipFile())
    //        {
    //            string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics

    //            // note: this does not recurse directories! 
    //            //string[] filenames = System.IO.Directory.GetFiles(userDirectory.Value + "\\OutputFiles", "*");
    //            outputZipFile.AddDirectory(userDirectory.Value, runName.Value);
    //            outputZipFile.Comment = "Added by the Forest Sector Carbon Calculator";

    //            // This is just a sample, provided to illustrate the DotNetZip interface.  
    //            // This logic does not recurse through sub-directories.
    //            // If you are zipping up a directory, you may want to see the AddDirectory() method, 
    //            // which operates recursively. 
    //            // foreach (String filename in filenames)
    //            // {
    //            // Console.WriteLine("Adding {0}...", filename);
    //            //    ZipEntry e = outputZipFile.AddFile(filename);
    //            //    e.Comment = "Added by the Forest Sector Carbon Calculator";
    //            // }

    //            // outputZipFile.Comment = String.Format("This zip archive was created by the CreateZip example application on machine '{0}'",
    //            //   System.Net.Dns.GetHostName());

    //            outputZipFile.Save(userDirectory.Value + "\\FSCC_runOutput.zip");
    //        }

    //    }
    //    catch (System.Exception ex1)
    //    {
    //        System.Console.Error.WriteLine("exception: " + ex1);
    //    }

    //}

    protected void chooseFileName(string outputMode)
    {
        int outModeInt = 0;

        if (outputMode == "writeToDisk")
            outModeInt = 1;

        if (radio_summaryMethod.SelectedValue == "individualTrees")
        {
            loadAndSubsetByStand("\\vegData\\tp00111_01-08-2016.csv", outModeInt, "PSP_exportIndividualTrees.csv");
        }
        else if (radio_summaryMethod.SelectedValue == "plotState")
        {
            loadAndSubsetByStand("\\vegData\\tp00108_01-20-2016.csv", outModeInt, "PSP_exportPlot_stateVariables.csv");
        }
        else if (radio_summaryMethod.SelectedValue == "plotChange")
        {
            loadAndSubsetByStand("\\vegData\\tp00109_01-20-2016.csv", outModeInt, "PSP_exportPlot_changeVariables.csv");
        }
        else if (radio_summaryMethod.SelectedValue == "standState")
        {
            loadAndSubsetByStand("\\vegData\\tp00106_02-11-2016.csv", outModeInt, "PSP_exportStand_stateVariables.csv");
        }
        else if (radio_summaryMethod.SelectedValue == "standChange")
        { 
            loadAndSubsetByStand("\\vegData\\tp00107_10-20-2016.csv", outModeInt, "PSP_exportStand_changeVariables.csv");
        }
    }

    protected void btn_download_Click(object sender, EventArgs e)
    {
        chooseFileName("writeToDisk");
    }

    protected void radio_summaryMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        clearFileViewer();
        chooseFileName("view");
    }

    protected void clearFileViewer()
    {
        tb_fileDump.Text = "**NOTE: Processing individual trees may take up to 15 seconds";
    }

    protected void setupChart()
    {
        System.Drawing.Color bgColor = System.Drawing.ColorTranslator.FromHtml("#e1f9d9");

        // Set Border Skin
        Chart1.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
        Chart1.BorderSkin.PageColor = bgColor;

        // Set Titles and title fonts
        // Chart1.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
        Chart1.Titles.Add("title0");
        Chart1.Titles[0].Name = "outChartTitle";
        Chart1.Titles["outChartTitle"].Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);

        Chart1.Titles.Add("title1");
        Chart1.Titles[1].Name = "chartFooter";
        Chart1.Titles["chartFooter"].Alignment = System.Drawing.ContentAlignment.BottomRight;
        Chart1.Titles["chartFooter"].Docking = System.Web.UI.DataVisualization.Charting.Docking.Bottom;
        Chart1.Titles["chartFooter"].IsDockedInsideChartArea = true;
        Chart1.Titles["chartFooter"].Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
        Chart1.Titles["chartFooter"].Text = "All data is from the Permanant Sample Plot Program (http://pnwpsp.forestry.oregonstate.edu/)";

        Chart1.ChartAreas["ChartArea1"].BackColor = Color.FloralWhite;
        Chart1.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        Chart1.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineColor = Color.Silver;
        Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.Silver;
        Chart1.ChartAreas["ChartArea1"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        Chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = true;

        // Create legend
        Chart1.Legends.Clear();
        Chart1.Legends.Add("outChartLegend");
        //   Chart1.Legends["outChartLegend"].DockedToChartArea = "ChartArea1";
        Chart1.Legends["outChartLegend"].LegendStyle = LegendStyle.Row;
        Chart1.Legends["outChartLegend"].Docking = Docking.Top;
        Chart1.Legends["outChartLegend"].Alignment = StringAlignment.Center;
        Chart1.Legends["outChartLegend"].Font = new Font("Microsoft Sans Serif", 10);

        //Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 100;
        //Chart1.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Number;
        //Chart1.ChartAreas["ChartArea1"].AxisX.IntervalOffset = 10;
        //Chart1.ChartAreas["ChartArea1"].AxisX.IntervalOffsetType = DateTimeIntervalType.Auto;

        Chart1.Series.Clear();
        //if (drawZeroLine)
        //{
        //    addChartSeries("seriesZeroLine", SeriesChartType.Line, "", Color.Brown);
        //    Chart1.Series["seriesZeroLine"].IsVisibleInLegend = false;
        //    Chart1.Series["seriesZeroLine"].BorderWidth = 2;
        //    Chart1.Series["seriesZeroLine"].Points.AddXY(Convert.ToDouble(TextBox_startYear.Text) + 1, 0.0);
        //    Chart1.Series["seriesZeroLine"].Points.AddXY(Convert.ToDouble(Page.Session["currentYear"]) + Convert.ToDouble(Page.Session["numSimYears"]), 0.0);
        //}
    }

    protected void drawChart_summary(int chartType, string chartTitle, string YaxisVar, string YaxisLabel)
    {
        readOutput_class readOutput = new readOutput_class();
        string appPath = Request.PhysicalApplicationPath; // requires System.Diagnostics
        int standCount = 0, speciesNum = 0;
        List<Color> seriesColor = new List<Color>();
        List<string> speciesList = new List<string>();

        //graphTitle_class graphTitle = new graphTitle_class();
        //int startYear = Convert.ToInt16(TextBox_startYear.Text);

        string fileName = "";
        SeriesChartType chartTypeName;
        seriesColor.Add(Color.DarkSeaGreen);
        seriesColor.Add(Color.DarkSlateBlue);
        seriesColor.Add(Color.Goldenrod);
        seriesColor.Add(Color.DarkMagenta);
        seriesColor.Add(Color.Black);
        seriesColor.Add(Color.Red);
        seriesColor.Add(Color.Pink);
        seriesColor.Add(Color.Purple);
        seriesColor.Add(Color.RosyBrown);
        seriesColor.Add(Color.Tan);

        setupChart();
        if (chartType <= 2 || chartType == 4 || chartType == 5)
            fileName = appPath + "vegData\\tp00106_02-11-2016.csv";
        else if (chartType == 3)
            fileName = appPath + "vegData\\tp00107_10-20-2016.csv";

        Chart1.ChartAreas["ChartArea1"].AxisY.Title = YaxisLabel;
        Chart1.Titles["outChartTitle"].Text = chartTitle;
        chartTypeName = SeriesChartType.Line;

        if (chartType <= 3)
        {
            // graph all selected (max 5) stands
            foreach (ListItem li in cbList_stands.Items)
            {
                if (li.Selected == true)
                {
                    standCount++;
                    addChartSeries("series" + standCount.ToString(), chartTypeName, li.Value, seriesColor[standCount - 1]);
                    Chart1.Series["series" + standCount.ToString()].BorderWidth = 3;
                    if (readOutput.readCSV2Series(fileName, Chart1.Series["series" + standCount.ToString()], YaxisVar, chartType, li.Value, "ALL"))
                        fileName = "Error";

                }

                if (standCount == 5)
                    break;
            }
        }
        else
        {
            // graph first stand that is selected by species
            // find all species that are in a given stand
            speciesList = readOutput.findSpeciesByStand(fileName, cbList_stands.SelectedValue);
            Chart1.Titles["outChartTitle"].Text += " for " + cbList_stands.SelectedValue;
            foreach (string speciesName in speciesList)
            {
                speciesNum++;
                addChartSeries("series_" + speciesName, chartTypeName, speciesName, seriesColor[speciesNum - 1]);
                Chart1.Series["series_" + speciesName].BorderWidth = 3;
                if (readOutput.readCSV2Series(fileName, Chart1.Series["series_" + speciesName], YaxisVar, chartType, cbList_stands.SelectedValue, speciesName))
                    fileName = "Error";

                if (speciesNum == 10)
                    break;
            }
        }
    }

    protected void addChartSeries(string seriesName, SeriesChartType chartTypeName, string legendName, Color seriesColor)
    {
        Chart1.Series.Add(seriesName);

        if (chartTypeName == SeriesChartType.Line)
            Chart1.Series[seriesName].BorderWidth = 2;
        if (chartTypeName == SeriesChartType.StackedColumn)
        {
            Chart1.Series[seriesName]["PointWidth"] = "10";
            Chart1.Series[seriesName]["DrawingStyle"] = "Emboss";
        }

        Chart1.Series[seriesName].ChartType = chartTypeName;
        Chart1.Series[seriesName].Color = seriesColor;
        Chart1.Series[seriesName].LegendText = legendName;
    }

    protected void btn_chartTPH_Click(object sender, EventArgs e)
    {
        drawChart_summary(1, "Total Live TPH", "TPH_NHA", "Trees per hectare");
    }

    protected void btn_chartBA_Click(object sender, EventArgs e)
    {
        drawChart_summary(1, "Total Live Basal Area", "BA_M2HA", "Sq meters per hectare");
    }

    protected void btn_chartLiveBio_Click(object sender, EventArgs e)
    {
        drawChart_summary(1, "Total Live Biomass", "BIO_MGHA", "Megagrams per hectare");
    }

    protected void btn_chartDeadBio_Click(object sender, EventArgs e)
    {
        drawChart_summary(2, "Total Snag Biomass", "BIO_MGHA", "Megagrams per hectare");
    }

    protected void btn_chartNPP_Click(object sender, EventArgs e)
    {
        drawChart_summary(3, "Net Primary Productivity (NPP)", "MEAN_ANNUAL_NPP", "NPP (Mg/ha/year)");
    }

    protected void btn_bySPP_TPH_Click(object sender, EventArgs e)
    {
        drawChart_summary(4, "Live TPH by species", "TPH_NHA", "Trees per hectare");
    }

    protected void btn_bySPP_BA_Click(object sender, EventArgs e)
    {
        drawChart_summary(4, "Live Basal Area by species", "BA_M2HA", "Sq meters per hectare");
    }

    protected void btn_bySPP_LiveBio_Click(object sender, EventArgs e)
    {
        drawChart_summary(4, "Live Biomass by species", "BIO_MGHA", "Megagrams per hectare");
    }

    protected void btn_bySPP_DeadBio_Click(object sender, EventArgs e)
    {
        drawChart_summary(5, "Snag Biomass by species", "BIO_MGHA", "Megagrams per hectare");
    }

    //protected void btn_bySPP_DBH_Click(object sender, EventArgs e)
    //{
    //    drawChart_stackedBar("DBH Distribution by species", "DBH", "Trees per hectare");
    //}

    protected List<string> loadRemoveStands()
    {
        List<string> removeStands = new List<string>();

        removeStands.Add("ABCO");
        removeStands.Add("ARNA");
        removeStands.Add("BLNF");
        removeStands.Add("CH09");
        removeStands.Add("FRA3");
        removeStands.Add("FRB2");
        removeStands.Add("FRC3");
        removeStands.Add("FRD1");
        removeStands.Add("FRD2");
        removeStands.Add("GP02");
        removeStands.Add("GP04");
        removeStands.Add("HS01");
        removeStands.Add("LMCC");
        removeStands.Add("MPNF");
        removeStands.Add("NWNF");
        removeStands.Add("PIJE");
        removeStands.Add("PILA");
        removeStands.Add("SI04");
        removeStands.Add("SI05");
        removeStands.Add("SI06");
        removeStands.Add("SI07");
        removeStands.Add("SI08");
        removeStands.Add("SI09");
        removeStands.Add("SI10");
        removeStands.Add("SI20");
        removeStands.Add("SN01");
        removeStands.Add("SN02");
        removeStands.Add("SUCR");
        removeStands.Add("TCNF");
        removeStands.Add("UCRS");
        removeStands.Add("WI05");
        removeStands.Add("WLNF");
        removeStands.Add("WR02");
        removeStands.Add("WR09");
        removeStands.Add("YSNF");

        return removeStands;
    }
}