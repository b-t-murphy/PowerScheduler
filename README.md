# PowerScheduler
A Windows service project configured using **Topshelf**. It's purpose is to retrieve day ahead power trades at a given time interval, aggregate them into hourly positions and output the result to a CSV file. Both the time interval (in minutes) and the directory for the output CSV file are set within the _PowerScheduler.exe.config_ config file.

## Project Structure
The service is constructed within a console application (targeting .NET 4.5), whereby the service class _PositionService_ is wired-up within the console's Program.Main entry point using Topshelf's fluent API. As such, the service can be run as a console app in Debug mode locally without having to attach a debugger to the installed service.

### Nuget package dependencies
* **Topshelf** v3.3.1
* **Topshelf.Nlog** v3.3.1 (includes **Nlog** v4.2.2)
* **CsvHelper** v12.1.2

### Installing

1. Open the Command Prompt in Administrator mode
1. Navigate to the bin\Debug folder within the PowerScheduler source folder, then enter "PowerScheduler install". For example:
``` Example
D:\Development\CSharp\PowerScheduler\PowerScheduler\bin\Debug>PowerScheduler install
```
Topshelf will then install the service; it will display as **Power Position** in _Services (Local)_

### Un-installing

1. Open the Command Prompt in Administrator mode
1. Navigate to the bin\Debug folder within the PowerScheduler source folder, then enter "PowerScheduler uninstall"
```
D:\Development\CSharp\PowerScheduler\PowerScheduler\bin\Debug>PowerScheduler uninstall
```
Topshelf will then uninstall the service

### Configuration

#### Time Interval
In the _PowerScheduler.exe.config_ config file, set the value for **"RefreshInterval"** to the desired number of **whole** minutes. If the value cannot be processed at run-time (e.g. not an integer > 0), a default value of 1 minute is used.

#### CSV output location
In the _PowerScheduler.exe.config_ config file, set the value for **"CsvFileLocation"** to the desired folder. This is currently set to be the same folder as the service executable

### Logging
Nlog is configured within the _PowerScheduler.exe.config_ config file. It is set to log to both a text file _PositionServiceLog.txt_ within the same folder as the service executable, and to Console.  Both targets can be modified in the config file as required.
