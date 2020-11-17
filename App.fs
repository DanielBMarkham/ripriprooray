module App
open Util
open System
open Hopac
open Logary
open Logary.Message
open Logary.Configuration
open Logary.Targets
open System.Threading
open Types
open AppRRRTypes

// Tag-list for the logger is namespace, project name, file name
let moduleLogger = logary.getLogger (PointName [| "ripriprooray"; "App"; "NewProgram" |])
logEvent LogLevel.Verbose "Module enter...." moduleLogger


//loadEAConfigFromCommandLine argv incomingStream |> inputStuff |> doStuff |> outputStuff


let getAppConfiguration:GetAppConfigurationFunction =
  (fun (args,incomingStream)->defaultRRRConfig)
let getIncomingStream:GetIncomingStreamFunction=
  (fun (appConfig)->(appConfig,emptyInterAppDataTransfer))
let transformIncomingStreamToIncomingData:TransformIncomingStreamToIncomingDataFunction =
  (fun (appConfig,interAppDataTransfer)->(appConfig,()))
let processIncomingData:ProcessIncomingDataFunction =
  (fun (appConfig,incomingData)->(appConfig,()))
let performIncomingDataTransforms:PerformIncomingDataTransformsFunction =
  (fun (appConfig,processedData)->(appConfig,()))
let generateOutgoingData:GenerateOutgoingDataFunction=
  (fun (appConfig,transformedData)->(appConfig,()))
let outputData:OutputDataFunction=
  (fun (appConfig,outgoingData)->0)
// Allows apps to be joined and tested by the supercompiler without any IO
let outpuDataToStreams:OutpuDataToStreamsFunction=
  (fun (appConfig,outgoingData)->emptyInterAppDataTransfer)

// app Flow -- CURRENTLY NOT WIRED UP
let ret =
  getAppConfiguration(argv,incomingStream)
  |> getIncomingStream
  |> transformIncomingStreamToIncomingData
  |> processIncomingData
  |> performIncomingDataTransforms
  |> generateOutgoingData
  |> outputData

let getRRRConfigFromCommandLine:GetRRRProgramConfigType =
  (
    fun (args)->defaultRRRConfig
  )


let newMain (argv:string[]) (compilerCancelationToken:System.Threading.CancellationTokenSource) (manualResetEvent:System.Threading.ManualResetEventSlim) (incomingStream:seq<string>) (ret:int byref) =
    try
      logEvent LogLevel.Verbose "Method newMain beginning....." moduleLogger
      //logEvent Logary.Debug ("Method newMain incomingStuff lineCount = " + (incomingStream |> Seq.length).ToString()) moduleLogger

      // Error is the new Out. Write here so user can pipe places
      //Console.Error.WriteLine "I am here. yes."
     // incomingStream |> Seq.iter(fun x->Console.Error.Write(x))
      let mutable ret=0//loadEAConfigFromCommandLine argv incomingStream |> inputStuff |> doStuff |> outputStuff
      // I'm done (since I'm a single-threaded function, I know this)
      // take a few seconds to catch up, then you may run until you quit
      logEvent LogLevel.Verbose "..... Method newMain ending. Normal Path." moduleLogger
      compilerCancelationToken.Token.WaitHandle.WaitOne(3000) |> ignore
      manualResetEvent.Set()
      ()
    with
        | :? System.NotSupportedException as nse->
            logEvent Logary.Debug ("..... Method newMain ending. NOT SUPPORTED EXCEPTION = " + nse.Message) moduleLogger
            ()
        | :? UserNeedsHelp as hex ->
            defaultRRRBaseOptions.PrintThis
            logEvent Logary.Debug "..... Method newMain ending. User requested help." moduleLogger
            manualResetEvent.Set()
            ()
        | ex ->
            logEvent LogLevel.Error "..... Method newMain ending. Exception." moduleLogger
            logEvent LogLevel.Error ("Program terminated abnormally " + ex.Message) moduleLogger
            logEvent LogLevel.Error (ex.StackTrace) moduleLogger
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
logEvent LogLevel.Verbose "....Module exit" moduleLogger


