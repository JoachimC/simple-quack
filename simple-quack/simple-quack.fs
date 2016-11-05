module simpleQuack
open System
open System.Net.Sockets
open System.Threading
open System.Diagnostics


let debug (x : byte) = 
    Debug.Write(x) 
    x

let consoleOut (x : string) = 
    Console.Write(x) 
    x

let asByte (x :int) = byte x
 
let rec asyncSendInput (stream : NetworkStream) =
    async {
 
        let outWithDebug (x : byte) = 
            Debug.WriteLine("->")
            stream.WriteByte(x)

        let filter (x : byte) =
            match x with
            | 10uy -> Debug.WriteLine(" x")
            | _ -> outWithDebug x
  
        Console.Read() |> asByte |> debug |> filter |> ignore
               
        return! asyncSendInput stream
    }

let rec asyncPrintResponse (stream : NetworkStream) =
    async {

        let newLineWhenNeeded (x :string) =
            match x with
            | "\r" -> Console.WriteLine()
            | _ -> ()

        stream.ReadByte() |> Char.ConvertFromUtf32
            |> consoleOut
            |> newLineWhenNeeded

        return! asyncPrintResponse stream
    }

[<EntryPoint>]
let main argv =
    let client = new System.Net.Sockets.TcpClient()
    client.Connect("10.0.0.69", 23)
    let stream = client.GetStream()
    asyncSendInput stream |> Async.Start
    asyncPrintResponse stream |> Async.RunSynchronously
    0 // return an integer exit code
