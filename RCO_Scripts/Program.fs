// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions
open System.Net.Sockets
open System.Text

type File_Info = {
    Stream : MemoryStream
    Rate : int
}

let writeLine (color : ConsoleColor) ( msg : string ) =
    Console.ForegroundColor <- color
    Console.WriteLine msg
    Console.ResetColor()

type Fetch<'a> =
    | Fetched of 'a
    | Not_Fetched

type Msg =
    | Initialize
    | Write_To_File

type Model = {
    Info : Fetch<File_Info>
}

let socket_write (msg : string ) =
    let serverIp = "localhost"
    let port = 3000
    try
        let sckt = new TcpClient()
        sckt.Connect(serverIp, port)

        let byte_count = Encoding.ASCII.GetByteCount msg 
        let mutable bytesArr = [|byte_count |> byte|]
        bytesArr <- Encoding.ASCII.GetBytes msg
        let stream = sckt.GetStream()
        stream.Write(bytesArr, 0, bytesArr.Length)

        stream.Close()
        sckt.Close()
    with
        | (ex : Exception) -> 
            ex.Message
            |> writeLine ConsoleColor.Red
    
type Variant =
    | Straight
    | Diagonal

let address_regex = new Regex(".*?(?=\s)")
let content_regex = new Regex("(?<=\s+.*?)(.|\n)*")

let generateStreamFromString ( content : string ) =
    let stream = new MemoryStream()
    let writer = new StreamWriter(stream)
    writer.Write content
    writer.Flush()
    stream.Position <- 0 |> int64

    stream

let getMatches ( regex : Regex ) str =
    let matches_all = regex.Matches(str)

    let result =
        [0..matches_all.Count]
        |> Seq.map (fun pos ->
                matches_all.[pos].Value
            )

    result

let writeProgress variant (prog : int) = 
    let mutable output = ""
    prog
    |> function
        | p when p >= 0 && p < 10 -> 
            output <- "\r[          ]"
        | p when p >= 10 && p < 20 -> 
            output <- "\r[|         ]"
        | p when p >= 20 && p < 30 -> 
            output <- "\r[||        ]"
        | p when p >= 30 && p < 40 -> 
            output <- "\r[|||       ]"
        | p when p >= 40 && p < 50 -> 
            output <- "\r[||||      ]"
        | p when p >= 50 && p < 60 ->
            output <- "\r[|||||     ]"
        | p when p >= 60 && p < 70 ->
            output <- "\r[||||||    ]"
        | p when p >= 70 && p < 80 -> 
            output <- "\r[|||||||   ]"
        | p when p >= 80 && p < 90 -> 
            output <- "\r[||||||||  ]"
        | p when p >= 90 && p < 100 -> 
            output <- "\r[||||||||| ]"
        | _ ->
            output <- "\r[||||||||||]"

    let last_index = output.LastIndexOf("|")

    match variant with
    | Straight ->
        Console.Write(output)
    | _ ->
        if last_index <> -1 
        then
            Console.Write(output.Replace(output.[last_index] |> string, "/"))
        else 
            Console.Write(output)

let rec update ( model : Model ) ( msg : Msg )  =
    match msg with
    | Initialize ->
        let allInfo =
            Environment.GetCommandLineArgs()

        let rate =
            allInfo
            |> Array.item 2

        let content = 
            allInfo
            |> Array.skip 3
            |> String.concat " "

        match content.Length with
        | 0 -> 
            Console.ForegroundColor <- ConsoleColor.Red
            Console.WriteLine("No content could be obtained!")
        | _ -> 
            match rate.Length with
            | 0 -> 
                Console.ForegroundColor <- ConsoleColor.Red
                Console.WriteLine ("No byte rate given. Example")
                Console.WriteLine("1000")
            | _ ->
                let mutable rate_int = 0
                let rate_span = new ReadOnlySpan<char>(rate |> Seq.toArray) 
                if Int32.TryParse(rate_span, &rate_int)
                then
                    let writeStream = 
                        content
                        |> generateStreamFromString

                    let info = {
                        Stream = writeStream
                        Rate = rate_int
                    }

                    let newModel = { model with Info = info |> Fetched}

                    Write_To_File
                    |> update newModel
                else
                   Console.ForegroundColor <- ConsoleColor.Red
                   Console.WriteLine ("Couldn't convert rate. Example")
                   Console.WriteLine("3") 

    | Write_To_File ->
        match model.Info with
        | Fetched info ->
            let fsOut = new FileStream("Test.txt", FileMode.Create)

            let bt =
                [|0..info.Rate|]
                |> Array.map (fun _ -> 1048756 |> byte)

            let mutable readByte : int = info.Stream.Read(bt, 0, bt.Length)
            let mutable variant = Diagonal

            while readByte > 0 do
                fsOut.Write(bt, 0, readByte)

                variant <-
                    match variant with
                    | Straight -> Diagonal
                    | _ -> Straight

                let percentage = (info.Stream.Position * (100 |> int64) / info.Stream.Length)
                Console.ForegroundColor <- ConsoleColor.Green
                
                percentage
                |> string
                |> fun str -> $"@message:{str}"
                |> socket_write

                percentage
                |> int
                |> writeProgress variant

                readByte <- info.Stream.Read(bt, 0, bt.Length)

            "finished!"
            |> fun str -> $"@finished:{str}"
            |> socket_write

            fsOut.Close()

            Console.ResetColor()

        | _ -> 
            Console.ForegroundColor <- ConsoleColor.Red
            Console.WriteLine("No file info fetched")

            "No file info fetched"
            |> fun str -> $"@finished:{str}"
            |> socket_write

    Console.ResetColor()
        
let init = {
    Info = Not_Fetched
}

[<EntryPoint>]
update init Initialize
Console.WriteLine ""
