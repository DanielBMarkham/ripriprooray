module AppRRRTypes
open Types


type TransferItem =
  {
    ItemName:string
    ItemLines:string []
  }
type InterAppDataTransfer =
  {
    CreationAppName:string
    Version:string 
    StreamName:string
    TransferItems:TransferItem[]
  }
let emptyInterAppDataTransfer = 
  {
    CreationAppName=""
    Version=""
    StreamName=""
    TransferItems=[|{ItemName="";ItemLines=[||]}|]
  }

type RRRConfigType =
    {
    ConfigBase:ConfigBase
    }
    with member this.PrintThis() =()
        //testingLogger.info(
        //    eventX "EasyAMConfig Parameters Provided"
        //)
let defaultVerbosity  =
    {
        commandLineParameterSymbol="V"
        commandLineParameterName="Verbosity"
        parameterHelpText=[|"/V:[1-7]           -> Amount of trace info to report.";"1=Silent, 2=Fatal, 3=Error, 4=Warn, 5=Info, 6=Debug, 7=Verbose"|]
        parameterValue=Verbosity.Info
    }
let RRRProgramHelp = [|"RipRipRooray. Translate RSS feeds into TSV files."|]
//createNewBaseOptions programName programTagLine programHelpText verbose
let defaultRRRBaseOptions = createNewBaseOptions "rrr" "RSS Ripper" RRRProgramHelp defaultVerbosity
let defaultRRRConfig:RRRConfigType ={ConfigBase = {defaultRRRBaseOptions with Verbosity=defaultVerbosity}}


type AppConfig=RRRConfigType
type IncomingData=unit
type ProcessedData=unit
type TransformedData=unit
type OutgoingData=unit


type GetAppConfigurationFunction=string []*seq<string>->AppConfig
type GetIncomingStreamFunction=AppConfig->AppConfig*InterAppDataTransfer
type TransformIncomingStreamToIncomingDataFunction=AppConfig*InterAppDataTransfer->AppConfig*IncomingData
type ProcessIncomingDataFunction=AppConfig*IncomingData->AppConfig*ProcessedData
type PerformIncomingDataTransformsFunction=AppConfig*ProcessedData->AppConfig*TransformedData
type GenerateOutgoingDataFunction=AppConfig*TransformedData->AppConfig*OutgoingData
type OutputDataFunction=AppConfig*OutgoingData->int
// Allows apps to be joined and tested by the supercompiler without any IO
type OutpuDataToStreamsFunction=AppConfig*OutgoingData->InterAppDataTransfer

/// Process any args you can from the command line
/// Get rid of any junk
type GetRRRProgramConfigType=string [] ->RRRConfigType

/// Get incoming items to process
type GetRRRIncomingItems=RRRConfigType->RRRConfigType*InterAppDataTransfer

type RRRIncomingData=unit
/// Get incomingStuffFromItems
type GetIncomingStuffFromItems=RRRConfigType*InterAppDataTransfer->RRRConfigType*RRRIncomingData

type RRRIncomingStuff=unit
/// ProcessIncomingStuff
type ProcessIncomingStuff=RRRConfigType*RRRIncomingData->RRRConfigType*RRRIncomingStuff

type RRRProcessedStuff=unit
/// ProcessedStuff
type ProcessStuff=RRRConfigType*RRRIncomingStuff->RRRConfigType*RRRProcessedStuff

type RRROutputedStuff=int
type OutputStuff=RRRConfigType*RRRProcessedStuff->RRRConfigType*RRROutputedStuff
