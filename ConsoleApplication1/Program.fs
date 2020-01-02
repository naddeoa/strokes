// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
module Program

open System.Windows.Forms
open EventManager
open ConfigurationFormat
open UI.TrayIcon
        
[<EntryPoint>]
let main argv = 
    setupLLEventHandler <| getConfig getConfigPath
    Application.Run(new StrokesApplicationContext())
    0 
