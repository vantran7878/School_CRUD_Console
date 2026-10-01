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
using School_CRUD_console.Queue;

ILogger consoleLog = new ConsoleLogger();
ILogger JsonLog = new JSON_Logger();
CompositeLogger compositeLog = new(consoleLog, JsonLog);

IGPACalculator GPACalc = new GPA4Calculator();

PersistenceService persistence = new PersistenceService(compositeLog);

// Version 5: queue and worker
SchoolEventQueue queue = new SchoolEventQueue();
SchoolEventWorker worker = new SchoolEventWorker(queue, compositeLog);

// Cancellation token
CancellationTokenSource cts = new CancellationTokenSource();
// Background worker asynchronously run with other task
Task workerTask = Task.Run(() => worker.StartProcessingAsync(cts.Token));


School school = new School(compositeLog, GPACalc, persistence, queue);

bool hasData = await school.LoadSavedDataAsync();

if (!hasData)
{
    await school.InitializeDataAsync(20);
}


await school.StartSimulationAsync(2);


queue.Complete();

await workerTask;

cts.Cancel();

await school.SaveAllDataAsync();