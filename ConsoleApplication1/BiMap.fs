namespace BiMap


module BiMap =

    type BiMap<'K, 'V when 'K: comparison and 'V: comparison>  =
        private BiMap of Map<'K, 'V> * Map<'V, 'K>

    let empty<'K , 'V when 'K:comparison and 'V: comparison> = BiMap(Map.empty<'K, 'V>, Map.empty)
        
    let add (key: 'K) (value: 'V) (bimap: BiMap<'K, 'V>) =
        match bimap with
        | BiMap(map, reverseMap) ->
            let newMap = Map.add key value map
            let newReverseMap = Map.add value key reverseMap
            BiMap(newMap, newReverseMap)
       
    let remove (key: 'K) (bimap: BiMap<'K, 'V>) =
        match bimap with
        | BiMap(map, reverseMap) ->
            match Map.tryFind key map with
            | Some(currentValue) -> 
                let newMap = Map.remove key map
                let newReverseMap = Map.remove currentValue reverseMap
                BiMap(newMap, newReverseMap)
            | None -> bimap

    let mappings (bimap: BiMap<'K, 'V>) =
        let (BiMap (map, reverseMap)) = bimap
        map

    let reverseMappings (bimap: BiMap<'K, 'V>) =
        let (BiMap (map, reverseMap)) = bimap
        reverseMap

