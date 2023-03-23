# 1 - Query Storage metrics from Azure Monitor

```powershell

$subscriptionId = "xxx"

az login
az account set --subscription $subscriptionId


$resourceGroup = "xxx-RG"
$server = "xxx"

$startTime = "2023-02-13 20:00:00"
$endTime = "2023-03-13 20:00:00"
$interval = "PT24H"

$databases = (az sql db list --server $server --resource-group $resourceGroup | convertfrom-json)

new-item metrics -type directory

foreach($database in $databases) {

    if (test-path "metrics/$($database.name).json") {
        write-output "metrics already collected for $($database.name)"
    } else {
        write-output "collecting metrics from $($database.name)"
        az monitor metrics list --resource $database.id --metric "storage" --start-time $startTime --end-time $endTime --interval $interval | out-file "metrics/$($database.name).json"        
    }
}

```

# 2 - Extract and aggregate data

```powershell
dotnet run "<path_to_metrics_folder>"
```

A CSV file will be created in the root folder of the git repository.