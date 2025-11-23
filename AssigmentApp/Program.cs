// See https://aka.ms/new-console-template for more information

using AssigmentApp.Core;
using AssigmentApp.IO;
using AssigmentApp.Parsing;
using FluentResults;

AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    Console.WriteLine("Fatal error:");
    Console.WriteLine(e.ExceptionObject);
    Environment.Exit(100);
};

var parseResult = ArgumentsParser.Parse(args);
if (parseResult.IsFailed) Fail(parseResult);


var stateResult = DataLoader.LoadData(parseResult.Value);
if (stateResult.IsFailed) Fail(stateResult);

CommandLoop.Run(Console.In, Console.Out, stateResult.Value);
return;

static void Fail(ResultBase r)
{
    foreach (var e in r.Errors)
        Console.WriteLine(e.Message);
    Environment.Exit(1);
}