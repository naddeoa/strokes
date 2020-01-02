module UI.EventViewerForm

open System.Windows.Forms
open EventManager
open System.Drawing
open API
open NativeBindings
open System.ComponentModel

type EventViewerFormData(e: Event) =
    let (eventType, key, modifiers) = match e with
        | KeyPressEvent(data) -> ("PRESS", data.key, data.currentlyPressedKeys)
        | KeyReleaseEvent(data) -> ("RELEASE", data.key, data.currentlyPressedKeys) 

    let keyCode = Option.get (Keys.convertCodeToStringKey(key))
    let modifierCodes = (List.map (fun code -> Option.get (Keys.convertCodeToStringKey(code)))  modifiers)

    member this.EventType = eventType
    member this.Key = keyCode
    member this.Modifiers = modifierCodes

type EventViewerForm() =
    inherit Form()
    let maxSize = 1000
    let dataGrid = new DataGridView(Dock = DockStyle.Fill)
    let mutable eventData: BindingList<EventViewerFormData> = new BindingList<EventViewerFormData>()

    let keyDisplayHandler : EventHandler = {
        toAction = fun event -> 
            eventData.Add(new EventViewerFormData(event))
            if eventData.Count > maxSize then
                eventData.RemoveAt(eventData.Count - 1)

            dataGrid.FirstDisplayedScrollingRowIndex <- dataGrid.RowCount-1
            [NoAction]
    }

    // TODO is this the right place to do this?
    override this.OnShown args =
        do // Configure form
            this.Size <- new Size(1000, 400)
            this.Text <- "Strokes Event Viewer"

        do // Configure data grid
            this.Controls.Add(dataGrid)
            dataGrid.AutoGenerateColumns <- true
            dataGrid.DataSource <- eventData
            // It knows to have 3 because it uses reflection on the EventViewerFormData type fields
            dataGrid.Columns.[0].AutoSizeMode <- DataGridViewAutoSizeColumnMode.AllCells
            dataGrid.Columns.[1].AutoSizeMode <- DataGridViewAutoSizeColumnMode.AllCells
            dataGrid.Columns.[2].AutoSizeMode <- DataGridViewAutoSizeColumnMode.Fill

        do // Set up event handlers
            addHandler keyDisplayHandler 

    override this.OnClosed args =
        removeHandler keyDisplayHandler 

