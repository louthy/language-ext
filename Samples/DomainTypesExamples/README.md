## Sample overview

This sample models a small **workday tracking domain**.

The goal is to demonstrate how domain types, factories, rules, algebraic traits, and effectful capabilities can work together to build a safe and composable domain model.

The sample includes users, work days, work blocks, durations, dates, and generated runtime data. It shows how external or primitive values are converted into meaningful domain values, how invariants are enforced during construction, and how valid values can later be composed safely.

### What the sample models

The example represents a basic work tracking flow:

* A `User` is created from a name
* The user receives a generated identifier
* The creation date is obtained from the runtime clock
* Work days are generated
* Each work day contains work blocks such as effective work, lunch, and rest
* The system calculates tracked time, effective time, and overtime

This demonstrates both pure domain construction and effectful construction using runtime capabilities like time, random values, and sequences.

### Traits used

#### `DomainType`

Used to represent meaningful domain values with an underlying representation.

Examples:

* `User`
* `WorkBlock`
* `WorkBlockKind`
* `NonFutureDate`
* `WorkDayHistory`

`User` exposes its representation as `(int Id, string Name, DateOnly CreatedAt, Seq<WorkDay> WorkDays)`, while keeping the domain model strongly typed internally.

#### `DomainTypeFactory`

Used when a domain type can be created from its representation using pure validation.

Examples:

* `WorkDay`
* `HourValue`

`WorkDay` validates that it contains at least one block and that the total tracked duration does not exceed twelve hours.

#### `DomainFactoryM`

Used when constructing a domain value requires effects or runtime context.

Examples:

* `User.Factory<RT>`
* `NonFutureDate.Factory<RT>`

`User.Factory<RT>` depends on capabilities such as time and sequences/random generation to create a complete user safely.

#### `RefinedTypeFactory`

Used to refine an already valid base type with additional domain rules.

Example:

* `WorkDuration`

`WorkDuration` refines `HourOnly` by enforcing that the duration is within the allowed range for a work block.

#### `Magnitude`

Used for measurable domain values that can be ordered, compared, added, subtracted, and scaled.

Example:

* `WorkDuration`

This allows durations to behave like proper domain magnitudes instead of raw numbers.

#### `VectorSpace`

Used for values that support algebraic operations such as addition, subtraction, negation, and scaling.

Example:

* `HourValue`

This demonstrates how numeric-like domain values can remain strongly typed while still supporting useful mathematical operations.

#### `AffineSpace`

Used when the domain distinguishes between a position and a displacement.

Example:

* `WorkBlock`

A `WorkBlock` behaves like a positioned value in the workday timeline: it has a start time and a duration. Adding a duration transforms the block, and subtracting two blocks produces a duration.

#### `DomainSet`

Used to represent a closed set of valid domain values.

Example:

* `WorkBlockKind`

`WorkBlockKind` defines the allowed kinds of work blocks: effective work, lunch, and rest.

#### `RuleK` / reusable rules

Used to define reusable validation rules over higher-kinded values.

Examples:

* `NonEmptyWorkBlocks`
* `DailyBlocksWithinTwelveHours`

These rules are used by `WorkDay` to validate collections of work blocks before allowing construction.

### Capabilities used

The sample also includes effectful capabilities:

* `Time`
* `Random`
* `Sequences`

These capabilities are exposed through runtime traits such as:

* `HasTime<RT>`
* `HasRandom<RT>`
* `HasSequences<RT>`

This keeps effects explicit and avoids hidden dependencies.

### What this sample demonstrates

This sample shows how to:

* avoid contextless primitives
* construct domain values through safe factories
* model validation failures explicitly with `Fin` and `FinT`
* compose pure and effectful construction
* express domain rules as reusable traits
* model measurable values with algebraic behavior
* distinguish positions from distances using affine spaces
* evolve a domain model without scattering validation logic

In short, the sample demonstrates how to build a small but realistic domain using composable traits instead of inheritance-heavy object models.
