# Domain / Rules

This folder contains the abstractions used to model **domain invariants**.

A Rule represents a condition that a value must satisfy to be considered valid within the domain.

Unlike factories, which construct values, rules focus on validating and composing constraints in a reusable way.

## Purpose

The goal of Rules is to decouple validation from construction, allowing:

- reuse of invariants across contexts
- declarative composition of rules
- avoidance of duplicated logic
- a more expressive and consistent domain model

Rules can be applied to:

- value objects
- aggregate roots
- refined primitive values
- more complex structures

## Types of Rules

There are four main variants, depending on the execution context.

### Rule

Represents a pure rule over a value.

- Evaluates direct values
- Does not depend on effects or external context
- Returns a result that may fail

Used when validation is fully deterministic.

### RuleK

Represents a rule over values in a higher-kinded context.

- Evaluates values inside a structure
- Preserves the surrounding context

Used when the value is wrapped and we want to validate without leaving that structure.

### RuleM

Represents a rule over values in a monadic context.

- Evaluates values with effects
- Works within contexts like IO, Eff, etc

Used when validation requires access to context or controlled effects.

### RuleT

Represents a rule in a transformed monadic context.

- Evaluates values inside more complex structures
- Combines context and structure

Used in scenarios involving both effects and containers.

## Modules and composition

Each Rule type includes a module with combinators that allow building more complex rules from simpler ones.

Key combinators:

- **All** → all rules must pass
- **Any** → at least one rule must pass
- **Not** → negates an existing rule
- **Lift** → lifts a rule into a higher context

This enables declarative validation without imperative control flow.

## Extensions

Extensions provide helpers to apply rules in a more fluent and expressive way.

They do not define new rules, only improve usability.

## Relationship with Factories

Rules do not create values—they validate them.

Factories can use rules to:

- validate inputs before constructing a type;
- compose multiple invariants;
- keep validation logic reusable.

This keeps construction clear while validation remains decoupled.

## Conceptual examples

- a rule that ensures a string is not empty
- a rule that checks a number is positive
- a rule that validates a specific format
- a rule that depends on configuration or context
- a composition of rules defining a set of invariants

## Usage guidelines

Use Rules when:

- a validation is reused in multiple places
- you need to compose multiple invariants
- you want to separate validation from construction
- you want to express rules as part of the domain

Avoid creating Rules for trivial validations that are not reused.

## Philosophy

Rules make domain invariants:

- explicit
- reusable
- composable

The clearer the rules, the more reliable the system becomes.

The goal is not to validate everywhere, but to define rules once and reuse them where needed.
