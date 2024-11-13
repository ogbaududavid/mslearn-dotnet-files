using Newtonsoft.Json; 
using Newtonsoft.Json.Linq;
using System.Text.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);   

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

SalesSummary("stores");

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

// Completed Sales Summary Function
void SalesSummary(string folderName)
{
    var foundFiles = Directory.GetDirectories(storesDirectory);
    //Used existing CalculateSalesTotal function
    var salesTotal = CalculateSalesTotal(salesFiles);
    string salesSummaryFile = Path.Combine(Directory.GetCurrentDirectory(), "SalesSummary.txt");

    File.AppendAllText(salesSummaryFile, $"Sales Summary{Environment.NewLine}--------------------{Environment.NewLine}");

    foreach (var file in salesFiles)
    {
        // Check for the sales.json file in the store's folder
        if (Path.GetFileName(file).Equals("sales.json", StringComparison.OrdinalIgnoreCase))
        {
            // Get the name of the store (which is the name of the folder housing the sales data)
            var dirPath = Path.GetDirectoryName(file);
            var storeName = Path.GetFileName(dirPath);
            var storeSales = JObject.Parse(File.ReadAllText(file))["Total"]?.Value<decimal>() ?? 0.0m;
            string summaryData = $"Filename: {storeName}, sales: {storeSales.ToString("C")}{Environment.NewLine}";

            File.AppendAllText(salesSummaryFile, summaryData);
        }
    }

    File.AppendAllText(salesSummaryFile, $"Total Sales: {salesTotal.ToString("C")}");
}
record SalesData (double Total);