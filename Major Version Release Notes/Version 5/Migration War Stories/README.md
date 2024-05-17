# Migration from V4 to V5 notes

## Migrating `EffectsExamples` from the `Samples` folder

### `Menu.cs`

* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Changed: `HasFile<RT>` constraint to `Has<Eff<RT>, FileIO>`
* Changed: `HasTextRead<RT>` constraint to `Has<Eff<RT>, TextReadIO>`
* Changed: `HasTime<RT>` constraint to `Has<Eff<RT>, TimeIO>`
* Changed: `HasConsole<RT>` constraint to `Has<Eff<RT>, ConsoleIO>`
* Search and replace: `Aff<RT, ` with `Eff<RT, ` 
* Search and replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Replace: `.Sequence` with `.Traverse` (and put `.As()` a the end)
* Added: `.As()` to the end of the first `from` in `clearConsole`
* Changed: `ToAff()` to `ToEff()`
* Added: `.As()` to the end of the first `from` in `logError`
* Changed: `Time<RT>` to `Time<Eff<RT>, RT>`
* Added: `using static LanguageExt.UnitsOfMeasure;`
* Added: `.As()` to the end of the first `from` in `showComplete`
* Changed: `Seq(...)` to `[...]`

### `CancelExample.cs`

* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole<RT>` constraint to `Has<Eff<RT>, ConsoleIO>`
* Search and replace: `Aff<RT, ` with `Eff<RT, ` 
* Search and replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Changed: `k.Key == ConsoleKey.Enter ? cancel<RT>() : unitEff` to `k.Key == ConsoleKey.Enter ? cancel : unitIO`

### `ErrorAndGuardExample.cs`

* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole` to `Has<Eff<RT>, ConsoleIO>`
* Search and replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Added: `using static LanguageExt.UnitsOfMeasure;`
* Replace: `@catch` with `@catchM` when catching Effs (and changed predicate to use `ex.Is<SystemException>()`)

### `FoldTest.cs`

* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole` to `Has<Eff<RT>, ConsoleIO>`
* Search and replace: `Aff<RT, ` with `Eff<RT, ` 
* Removed: `ToAff()` from guard, not needed any more
* Changed: `Effect<RT, Unit>` to `Effect<Eff<RT>, Unit>`
* Added: `.As()` to the end of the first `RunEffect()`

### `ForkCancelExample.cs`

* Search and replace: `Aff<RT, ` with `Eff<RT, ` 
* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole<RT>` constraint to `Has<Eff<RT>, ConsoleIO>`
* Changed: `HasTime<RT>` constraint to `Has<Eff<RT>, TimeIO>`
* Added: `using static LanguageExt.UnitsOfMeasure;`
* Search and replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Change: `fork` now returns `ForkIO`, so update to use `ForkIO.Cancel`


### `QueueExample.cs`

* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole` to `Has<Eff<RT>, ConsoleIO>`
* Replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Changed: `EnqueueEff` with `EnqueueM` and appended `.As()`
* Changed: `Queue<RT, *>` to `Queue<Eff<RT>, *>`
* Changed: `Consumer<RT, *, *>` to `Consumer<*, Eff<RT>, *>`
* Changed: `Pipe<RT, *, *, *>` to `Pipe<*, *, Eff<RT>, *>`
* Changed: `Producer<RT, *, *>` to `Producer<*, Eff<RT>, *>`
* Changed: `FailEff<Unit>(Errors.Cancelled)` with `Fail(Errors.Cancelled)`
* Added: `.As()` to `fork`

### `RetryExample.cs`

* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole` to `Has<Eff<RT>, ConsoleIO>`
* Replace: `Console<RT>` with `Console<Eff<RT>, RT>`

### `TextFileChunkStreamExample.cs`

* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole` to `Has<Eff<RT>, ConsoleIO>`
* Changed: `HasFile<RT>` to `Has<Eff<RT>, FileIO>`
* Changed: `HasTextRead<RT>` to `Has<Eff<RT>, TextReadIO>`
* Replace: `Aff<RT, ` with `Eff<RT, ` 
* Replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Changed: `Consumer<RT, *, *>` to `Consumer<*, Eff<RT>, *>`
* Changed: `Pipe<RT, *, *, *>` to `Pipe<*, *, Eff<RT>, *>`
* Changed: `Effect<RT, *>` to `Effect<Eff<RT>, *>`

### `TextFileLineStreamExample.cs`

* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Changed: `HasConsole` to `Has<Eff<RT>, ConsoleIO>`
* Changed: `HasFile<RT>` to `Has<Eff<RT>, FileIO>`
* Changed: `HasTextRead<RT>` to `Has<Eff<RT>, TextReadIO>`
* Replace: `Aff<RT, ` with `Eff<RT, ` 
* Replace: `Console<RT>` with `Console<Eff<RT>, RT>`
* Changed: `Consumer<RT, *, *>` to `Consumer<*, Eff<RT>, *>`
* Changed: `Pipe<RT, *, *, *>` to `Pipe<*, *, Eff<RT>, *>`
* Changed: `Effect<RT, *>` to `Effect<Eff<RT>, *>`

### `TimeExample.cs`

* Added: `using static LanguageExt.UnitsOfMeasure;`
* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Replace: `HasConsole<RT>` constraint to `Has<Eff<RT>, ConsoleIO>`
* Replace: `HasTime<RT>` constraint to `Has<Eff<RT>, TimeIO>`
* Replace: `Time<RT>.now` to `Time<Eff<RT>, RT>.now`
* Replace: `Console<RT>` with `Console<Eff<RT>, RT>`

### `TimeoutExample.cs`

* Added: `using static LanguageExt.UnitsOfMeasure;`
* Removed: `struct` constraint
* Removed: `HasCancel<RT>` constraint 
* Replace: `HasConsole<RT>` constraint to `Has<Eff<RT>, ConsoleIO>`
* Replace: `HasTime<RT>` constraint to `Has<Eff<RT>, TimeIO>`
* Replace: `Aff<RT, ` with `Eff<RT, ` 


## Migrating `LanguageExt.Sys` from the `Samples` folder

Then changed:
```c#
use(startActivity(name, activityKind, activityTags, activityLinks, startTime),
	act => localEff<RT, RT, TA>(rt => rt.SetActivity(act.Activity), operation));
```
to:
```c#
from a in startActivity(name, activityKind, activityTags, activityLinks, startTime)
from r in localEff<RT, RT, TA>(rt => rt.WithActivity(a), operation)
select r;
```

3. Changed `Aff` to `Eff`

4. Deleted duplicate methods (due to `Aff` being renamed to `Eff`)

5. Changed `Eff<RT, ...>(rt => ...` to `lift((RT rt) => ...`

6. Changed `cancelToken<RT>()` to `cancelToken` 

7. Changed `Producer<RT, OUT, A>` to `Producer<OUT, Eff.R<RT>, A>`
