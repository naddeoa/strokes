


# Inferences

F# has a ton of syntax with a lot of subtle differences. These examples aren't actually the same in some way.

```f#
type Foo = inherit ApplicationContext
type Foo = inherit ApplicationContext()
type Foo() = inherit ApplicationContext()
```
