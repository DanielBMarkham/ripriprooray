module Util
open System
open Hopac
open Logary
open Logary.Message
open Logary.Configuration
open Logary.Targets
open System.Threading
open Types

// Logging all the things!
// Logging code must come before anything else in order to use logging
// let incomingStuff:string=pipedStreamIncoming()
// Need to know this when logging
let oldStdout=System.Console.Out
let oldStdErr=System.Console.Error
let mutable CommandLineArgumentsHaveBeenProcessed=false
type LogEventParms=LogLevel*string*Logary.Logger
let loggingBacklog = new System.Collections.Generic.Queue<LogEventParms>(4096)
let logary =
    Logary.Configuration.Config.create "EA.Logs" "localhost"
    |> Logary.Configuration.Config.targets [ Logary.Targets.LiterateConsole.create Logary.Targets.LiterateConsole.empty "console" ]
    |> Logary.Configuration.Config.loggerMinLevel "" Logary.LogLevel.Debug
    |> Logary.Configuration.Config.processing (Logary.Configuration.Events.events |> Logary.Configuration.Events.sink ["console";])
    |> Logary.Configuration.Config.build
    |> Hopac.Hopac.run
// Tag-list for the logger is namespace, project name, file name
let moduleLogger = logary.getLogger (PointName [| "EA"; "Types"; "EATypeExtensions" |])

/// ErrorLevel, Message to display, and logger to send it to
let logEvent (lvl:LogLevel) msg lggr =
    if CommandLineArgumentsHaveBeenProcessed
        then Logary.Message.eventFormat (lvl, msg)|> Logger.logSimple lggr
        else loggingBacklog.Enqueue(lvl, msg, lggr)
let turnOnLogging() =
    CommandLineArgumentsHaveBeenProcessed<-true
    System.Console.SetOut oldStdErr
    System.Console.SetError oldStdout
    loggingBacklog|>Seq.iter(fun x-> 
        let lvl, msg, lggr = x
        logEvent lvl msg lggr)
logEvent Verbose "Module enter...." moduleLogger


let newMain (argv:string[]) (compilerCancelationToken:System.Threading.CancellationTokenSource) (manualResetEvent:System.Threading.ManualResetEventSlim) (incomingStream:seq<string>) (ret:int byref) =
    try
      logEvent Verbose "Method newMain beginning....." moduleLogger
      //logEvent Logary.Debug ("Method newMain incomingStuff lineCount = " + (incomingStream |> Seq.length).ToString()) moduleLogger

      // Error is the new Out. Write here so user can pipe places
      //Console.Error.WriteLine "I am here. yes."
     // incomingStream |> Seq.iter(fun x->Console.Error.Write(x))
      let mutable ret=0//loadEAConfigFromCommandLine argv incomingStream |> inputStuff |> doStuff |> outputStuff
      // I'm done (since I'm a single-threaded function, I know this)
      // take a few seconds to catch up, then you may run until you quit
      logEvent Verbose "..... Method newMain ending. Normal Path." moduleLogger
      compilerCancelationToken.Token.WaitHandle.WaitOne(3000) |> ignore
      manualResetEvent.Set()
      ()
    with
        | :? System.NotSupportedException as nse->
            logEvent Logary.Debug ("..... Method newMain ending. NOT SUPPORTED EXCEPTION = " + nse.Message) moduleLogger
            ()
(*        | :? UserNeedsHelp as hex ->
            defaultEARBaseOptions.PrintThis
            logEvent Logary.Debug "..... Method newMain ending. User requested help." moduleLogger
            manualResetEvent.Set()
            ()*)
        | ex ->
            logEvent Error "..... Method newMain ending. Exception." moduleLogger
            logEvent Error ("Program terminated abnormally " + ex.Message) moduleLogger
            logEvent Error (ex.StackTrace) moduleLogger
            if ex.InnerException = null
                then
                    Logary.Message.eventFormat (Logary.Debug, "newMain Exit System Exception")|> Logger.logSimple moduleLogger
                    ret<-1
                    manualResetEvent.Set()
                    ()
                else
                    System.Console.WriteLine("---   Inner Exception   ---")
                    System.Console.WriteLine (ex.InnerException.Message)
                    System.Console.WriteLine (ex.InnerException.StackTrace)
                    Logary.Message.eventFormat (Logary.Debug, "newMain Exit System Exception")|> Logger.logSimple moduleLogger
                    ret<-1
                    manualResetEvent.Set()
                    ()
// For folks on anal mode, log the module being exited.  NounVerb Proper Case
logEvent Verbose "....Module exit" moduleLogger
