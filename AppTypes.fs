module AppTypes
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

  // DATA types
  type EAConfigType =
      {
      ConfigBase:ConfigBase
      FileListFromCommandLine:(string*System.IO.FileInfo)[]
      IncomingStream:seq<string>
      }
      with member this.PrintThis() =()
          //testingLogger.info(
          //    eventX "EasyAMConfig Parameters Provided"
          //)

  // FUNCTION TYPES
  /// Process any args you can from the command line
  /// Get rid of any junk
  type GetEAProgramConfigType=string [] -> seq<string>->EAConfigType

