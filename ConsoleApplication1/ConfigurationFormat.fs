module ConfigurationFormat

open System.IO
open FSharp.Data
open FSharp.Data.JsonExtensions
open System
open Newtonsoft.Json.Serialization
open NativeBindings.Hooks

let private sampleJsonConfig = """
{
    "rules": [
        {
            "description": "Windows DE switching",
            "manipulators": [
                {
                    "from": {
                        "key": "l",
                        "modifiers": [
                            "shift",
                            "control"
                        ]
                    },
                    "description": "Next desktop",
                    "to": [
                        {
                            "key": "right",
                            "modifiers": [
                                "control",
                                "win"
                            ],
                            "type": "keys"
                        }
                    ]
                }
            ]
        },
        {
            "description": "Firefox tab switching",
            "manipulators": [
                {
                    "conditions": [
                        {
                            "description": "firefox",
                            "type": "focused_application"
                        }
                    ],
                    "from": {
                        "key": "l",
                        "modifiers": [
                            "alt",
                            "control"
                        ]
                    },
                    "description": "tab next",
                    "to": [
                        {
                            "key": "tab",
                            "modifiers": [
                                "control"
                            ],
                            "type": "keys"
                        }
                    ]
                },
                {
                    "conditions": [
                        {
                            "description": "firefox",
                            "type": "focused_applicatnextion"
                        }
                    ],
                    "from": {
                        "key": "h",
                        "modifiers": [
                            "alt",
                            "control"
                        ]
                    },
                    "description": "tab previous",
                    "to": [
                        {
                            "key": "tab",
                            "modifiers": [
                                "control",
                                "shift"
                            ],
                            "type": "keys"
                        }
                    ]
                }
            ]
        }
    ]
}
"""

let getConfigPath =
    /// Custom operator for combining paths
    let (+/) path1 path2 = Path.Combine(path1, path2)
    let homePath = Environment.GetEnvironmentVariable("HOMEPATH")
    homePath +/ ".strokes.json"

// TODO what value should I actually use here? Do I need to have a sample deployed with the app?
type ConfigProvider = JsonProvider<"../.strokes.json">
type Config = ConfigProvider.Root
type Rule = ConfigProvider.Rule
type Manipulator = ConfigProvider.Manipulator

let getConfig configPath =
    let configFile = File.ReadAllText(configPath)
    ConfigProvider.Parse(configFile)



