# Migration from V4 to V5 notes

## Migrating `EffectsExamples` from the `Samples` folder

1. Replace `HasCancel<RT>` with `HasIO<RT>`

2. Refactored `startActivity` to use `Eff.use` for automatic tracking of the disposable.


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