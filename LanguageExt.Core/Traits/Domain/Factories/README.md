# Domain / Factories

This folder contains the abstractions responsible for constructing domain values.

The core idea is to separate two responsibilities that are often mixed together:

- what a domain type is
- how a valid value of that type is created

A domain type is not just a wrapper over primitives like `string`, `Guid`, `int`, or `decimal`. It is also responsible for protecting its invariants. For that reason, value construction lives in specialized traits: the factories.

A factory represents the official entry point for transforming an external or primitive representation into a valid domain value.

## Purpose

Factories exist to centralize the creation of domain values and prevent validation rules from being scattered across the application.

This makes the domain more expressive, safer, and easier to compose.

Instead of treating things like emails, identifiers, monetary values, or usernames as loose primitives, we model them as domain types with their own rules.

The factory decides whether a given representation can become a valid domain value.

## Main Traits

### DomainFactory

Represents a pure factory.

It is used when the creation of a domain value depends only on the input and deterministic domain rules.

For example:

- validating an email format
- ensuring a name is not empty
- checking that a number is positive
- validating the structure of an identifier

A pure factory does not depend on configuration, repositories, external services, system time, IO, or mutable state.

Its result explicitly represents success or failure.

### DomainFactoryM

Represents an effectful factory, meaning a factory that builds values within a monadic context.

It is used when creating the value requires additional context or controlled effects.

For example:

- generating an identifier using randomness;
- using the current time;
- validating against configuration;
- querying a repository;
- depending on a runtime environment;
- composing validations that live inside `IO`, `Eff`, or similar contexts.

The important distinction is that `DomainFactoryM` does not hide effects—it models them explicitly in the type.

## Safe vs Unsafe

This folder separates two construction styles:

### Safe

Safe construction returns a result that may fail.

This is the recommended path for domain and application logic because it forces error handling to be explicit.

It should be used whenever input comes from outside the system or may violate domain rules.

Typical examples:

- API input;
- database values;
- user input;
- imported files;
- configuration values.

### Unsafe

Unsafe construction attempts to build the value directly and fails by throwing if the value is invalid.

This exists for ergonomics, interoperability, or scenarios where validity is already guaranteed.

It should be used carefully.

Reasonable examples:

- tests;
- seeds;
- fixtures;
- controlled migrations;
- internal code where validation has already happened;
- integrations that expect exceptions.

General rule: safe by default, unsafe only when preconditions are guaranteed.

## Relationship with DomainType

`DomainType` describes what a domain value represents.

`DomainFactory` describes how it is created.

This separation allows a type to remain clear and focused without being tied to a single construction strategy.

For example, a type may represent an email as text, but its factory defines which texts are valid emails.

## Conceptual Examples

### Email

An `Email` may be backed by text.

Its factory ensures the text has a valid structure before allowing construction.

Invalid formats result in explicit failure.

### UserId

A `UserId` may be backed by a `Guid`.

Its factory ensures the identifier is not empty.

If the value comes from an external source, safe construction is used.

If it is generated internally and guaranteed to be valid, unsafe construction may be used in a controlled place.

### Money

A `Money` value may be backed by a decimal.

Its factory ensures the amount is not negative and follows domain-specific rules.

This prevents validation from being repeated throughout the system.

### Slug

A `Slug` may be backed by text.

Its factory ensures it contains only allowed characters, has no spaces, and is normalized.

Any function receiving a `Slug` can assume it is already valid.

## Usage Guidelines

Use `DomainFactory` when construction is pure.

Use `DomainFactoryM` when construction requires a monadic context or controlled effects.

Use safe construction by default.

Use unsafe construction only at controlled boundaries or when validity is already guaranteed.

## Philosophy

This folder exists to ensure that valid domain values are created at the boundary.

We are not creating wrappers for the sake of it.

We are defining clear, composable, and safe entry points into the domain.

The more rules we push to the point of construction, the fewer runtime errors we propagate, and the more confidence we gain when composing on top of the domain.