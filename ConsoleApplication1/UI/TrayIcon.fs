module UI.TrayIcon

open System.Windows.Forms
open System.ComponentModel
open System.Drawing
open UI.EventViewerForm

type StrokesApplicationContext() = 
    inherit ApplicationContext()
    let container = new Container()
    let icon = new NotifyIcon(container)
    let Exit = fun sender e -> 
        // Need to make this invisible when the app is closed as well otherwise the
        // icon hangs around until the mouse hovers over it
        icon.Visible <- false
        Application.Exit()

    let showEventViewer () =
        let form = new EventViewerForm()
        form.Show()

    let exitItem = new MenuItem("Exit", Exit)
    let keyCodesItem = new MenuItem("Event Viewer", fun sender e -> showEventViewer())
    let items = [| exitItem; keyCodesItem  |]
    let menu = new ContextMenu(items)

    do 
        icon.Icon <- new Icon("Resources\icon.ico")
        icon.Text <- "Strokes Keybinder"
        icon.Visible <- true
        icon.ContextMenu <- menu

