module EventManager

open API
open NativeBindings.Hooks
open ConfigurationFormat
open NativeBindings.SendInputBindings
open NativeBindings.Keys
open System.Threading
open System

// The EventManager is responsible for keeping track of state about key presses
// and mapping events from Windows to the higher level evnts that are used in this program.

type private State = {
    keyState: LowLevelKeyState;
}

// TODO make another one of these that is always passive but gets metadata about how the key press was handled
type EventAction =
    | ToKeyPress of int * list<int> * list<int>
    | NoAction

// Higher level handler that works on the types that I control
type EventHandler = {
    toAction: Event -> list<EventAction>;
}

let private getPressedKeys dict =
    Map.toList dict
        |> List.filter (fun (keyCode, state) -> state.keyState = KeyPress)
        |> List.map (fun (keyCode, state) -> keyCode)

let mutable private keyState: Map<int, State> = Map.empty

let mutable private handlers : list<EventHandler> = []

let setHandlers newHandlers =
    handlers <- newHandlers

let consumeLowLevelEvent (kbEvent: LowLevelKeyboardEvent) =
    let evnt = match kbEvent.keyState with
    | KeyPress -> 
        let pressedKeys = getPressedKeys keyState
        keyState <- keyState.Add(kbEvent.rawStruct.vkCode, {keyState = kbEvent.keyState})
        KeyPressEvent {currentlyPressedKeys = pressedKeys; key = kbEvent.rawStruct.vkCode }
    | KeyRelease -> 
        keyState <- keyState.Remove(kbEvent.rawStruct.vkCode) 
        let pressedKeys = getPressedKeys keyState
        KeyReleaseEvent {currentlyPressedKeys = pressedKeys; key = kbEvent.rawStruct.vkCode }
    printfn "updating key state to %A" keyState
    evnt



let private convertStringKeysToCode (keyStrings: string []) =
    let codes = keyStrings |> Array.map convertStringKeyToCode
    if Array.forall Option.isSome codes then
        Option.Some <| Array.choose id codes
    else
        Option.None


let private convertToToKeyCodes (manTo: ConfigProvider.To) =
    let key = convertStringKeyToCode  manTo.Key
    let modifiers = convertStringKeysToCode manTo.Modifiers
    match key with
    | None -> None
    | Some(keyCode) ->
        match modifiers with
        | None -> None
        | Some(modifierCodes) -> Some (keyCode, modifierCodes)

let private convertTosToKeyCodes (tos: ConfigProvider.To []) =
    let tos = tos |> Array.map convertToToKeyCodes
    if Array.forall Option.isSome tos then
        Option.Some <| Array.choose id tos 
    else
        Option.None

let manipulatorToHandler (manipulator: Manipulator) =
    let handler: EventHandler = {
        toAction = fun event -> 
            match event with
            | KeyPressEvent(event)  ->
                // Its a key, press, continue
                let keyOption = convertStringKeyToCode manipulator.From.Key 

                match keyOption with
                | Some(manipulatorKey) ->
                    // The manipulator's target key is recognized
                    if event.key = manipulatorKey then

                        match convertStringKeysToCode manipulator.From.Modifiers with
                        | Some(keyCodes) ->
                            // All of the manipulator's modifiers are recognized
                            let requiredModifiers = Set.ofList event.currentlyPressedKeys
                            let contained = (keyCodes
                                |> Array.map (fun key -> Set.contains key requiredModifiers))
                            
                            let noModifiersPressed = event.currentlyPressedKeys.Length = 0
                            let allModifiersMet = ((not (Array.isEmpty contained)) || noModifiersPressed ) && Array.forall (fun result -> result = true) contained
                            if allModifiersMet then
                                printfn "Found handler for %s + %A" manipulator.From.Key manipulator.From.Modifiers
                                manipulator.To
                                    |> Array.map (fun toBlock -> (toBlock, convertToToKeyCodes toBlock) ) 
                                    |> Array.map (fun (toBlock, optionalKeyCodes) -> 
                                        match optionalKeyCodes with
                                            | None ->
                                                // Some of the keys in the To rule are not recognized
                                                Console.WriteLine("Some keys not recognized in " + toBlock.ToString())
                                                NoAction
                                            | Some(key, modifiers) ->
                                                ToKeyPress(key, (List.ofArray modifiers), event.currentlyPressedKeys)
                                    )
                                    |> List.ofArray

                            else
                                // Some modifiers aren't being held down yet
                                [NoAction]
                        | None ->
                            // One or more of of the manipulator's modifiers are not recognized
                            Console.WriteLine("Don't know key code for some of " + manipulator.From.Modifiers.ToString())
                            [NoAction]
                    else
                        [NoAction]
                | None -> 
                    // The manipulator's target key is not recognized
                    Console.WriteLine("Don't know which key " + manipulator.From.Key + " is")
                    [NoAction]

            | KeyReleaseEvent(event) ->
                // Ignore key release events for now
                [NoAction]
    }

    handler
    

let handleAction (action: EventAction) =
    match action with
    | NoAction -> 
        ignore 0
        false
    | ToKeyPress(key, modifiers, pressedModifiers) ->
        printfn "Executing key press %i + %A" key modifiers

        // Release any modifiers that are currently pressed
        for pressedModifier in pressedModifiers do
            releaseKey pressedModifier

        for modifier in modifiers do
            pressKey modifier
        
        pressKey key
        releaseKey key

        for modifier in modifiers do
            releaseKey modifier

        // TODO hacky. It looks like repressing the modifier keys right away has some strange side effects. For example,
        // I have a keybind to switch one desktop to the left. Without this timeout, after switching, the window on that
        // desktop doesn't have focus when it should. I think this is because that extra key press is going to the currently
        // focused window which is on the virtual desktop that I'm coming from, and I end up with a race that results in the
        // window that I can't see retaining focus.
        Thread.Sleep 100

        // Re-press whatever was pressed before
        for pressedModifier in pressedModifiers do
            pressKey pressedModifier

        true


let private llEventHandler :LowLevelEventHandler =
    fun llEvent ->
        let processedEvent = consumeLowLevelEvent llEvent
        let shouldPropagate = (handlers 
            |> List.map (fun handler -> handler.toAction)
            |> List.collect (fun action -> action(processedEvent))
            |> List.map handleAction
            |> List.exists (fun it -> it = true))

        shouldPropagate


let setupLLEventHandler (config: Config) =
    handlers <-
        config.Rules 
            |> Array.collect (fun rule -> rule.Manipulators)
            |> Array.map manipulatorToHandler
            |> List.ofArray
    setupKeyListener llEventHandler
  
let addHandler handler =
    handlers <- List.append [handler] handlers

let removeHandler handler =
    handlers <- List.filter (fun it -> not <| LanguagePrimitives.PhysicalEquality it handler) handlers
