


# Inferences

1. F# has a ton of syntax with a lot of subtle differences. These examples aren't actually the same in some way.

```fsharp
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

1. Some syntax is counterintuitive. It seems as though you can only associate one type with a union type. If you
want more than one type then you have to make it into a tuple. I would have expcted the syntax for that tuple to
look like `(int, int)` but it actually looks like `int * int`.

```fsharp
type Tree<'T> =
    | Node of Tree<'T> * 'T * Tree<'T>
    | Leaf
```

1. Some weird choies. Not sure why `this` isn't automatically bound in classes even though it apparently part of the
syntax of defining class members.

```fsharp
// You have to say 'as this'
type EventViewerForm() as this =
    inherit Form()

    let flowLayout = new FlowLayoutPanel()
    let textBox = new TextBox()

    do
        flowLayout.Parent <- this // or this won't exist


    override this.Text = "Strokes Event Viewer"

```
