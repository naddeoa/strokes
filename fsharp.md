


# Inferences

F# has a ton of syntax with a lot of subtle differences. These examples aren't actually the same in some way.

```f#
type Foo = inherit ApplicationContext
type Foo = inherit ApplicationContext()
type Foo() = inherit ApplicationContext()


// You can do generics either of the following ways apparently. Is there an actual difference? 

	type BiMap<'K, 'V when 'K: comparison and 'V: comparison>  =
		private BiMap of Map<'K, 'V> * Map<'V, 'K>

	// Or after the >
	type BiMap<'K, 'V> when 'K: comparison and 'V: comparison =
		private BiMap of Map<'K, 'V> * Map<'V, 'K>
```
