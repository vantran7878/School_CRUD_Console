// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using School_CRUD_console.Models;
using School_CRUD_console.Services;
using School_CRUD_console.Interfaces;
using School_CRUD_console.Constant;
using School_CRUD_console.Logger;
using School_CRUD_console.GPACalc;

ILogger consoleLog = new ConsoleLogger();
ILogger JsonLog = new JSON_Logger();
CompositeLogger compositeLog = new(consoleLog, JsonLog);

IGPACalculator GPACalc = new GPA4Calculator();

School school = new School(compositeLog, GPACalc);

await school.InitializeDataAsync(20);

await school.StartSimulationAsync(2);