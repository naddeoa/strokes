// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
module Program

open System.Windows.Forms
open System.ComponentModel
open EventManager
open ConfigurationFormat
open System.Drawing


type StrokesApplicationContext() = 
    inherit ApplicationContext()
    let container = new Container()
    let icon = new NotifyIcon(container)
    let Exit = fun sender e -> 
        // Need to make this invisible when the app is killed as well
        icon.Visible <- false
        Application.Exit()

    let menuItem = new MenuItem("Exit", Exit)
    let items = [| menuItem |]
    let menu = new ContextMenu(items)
    do 
        icon.Icon <- new Icon("Resources\icon.ico")
        icon.Text <- "Strokes Keybinder"
        icon.Visible <- true
        icon.ContextMenu <- menu



        
[<EntryPoint>]
let main argv = 
    setupLLEventHandler <| getConfig getConfigPath

    let context = new StrokesApplicationContext()

    //let form = new Form()
    //form.Text <- "Strokes"
    //form.ShowInTaskbar <- true

    Application.Run(context)
    0 
