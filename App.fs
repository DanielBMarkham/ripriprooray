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
  (fun (appConfig,transformedData)->
  printfn "okey-dokey"
  (appConfig,()))
let outputData:OutputDataFunction=
  (fun (appConfig,outgoingData)->0)
// Allows apps to be joined and tested by the supercompiler without any IO
let outpuDataToStreams:OutpuDataToStreamsFunction=
  (fun (appConfig,outgoingData)->emptyInterAppDataTransfer)

let getRRRConfigFromCommandLine:GetRRRProgramConfigType =
  (
    fun (args)->defaultRRRConfig
  )

// app Flow
let run(argv, incomingStream) =
  getAppConfiguration(argv,incomingStream)
  |> getIncomingStream
  |> transformIncomingStreamToIncomingData
  |> processIncomingData
  |> performIncomingDataTransforms
  |> generateOutgoingData
  |> outputData


// For folks on anal mode, log the module being exited.  NounVerb Proper Case
logEvent LogLevel.Verbose "....Module exit" moduleLogger


