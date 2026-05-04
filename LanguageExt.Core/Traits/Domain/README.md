Inspired by: https://mmapped.blog/posts/25-domain-types.html

# Domain

This folder represents the core of the system.

It defines the language of the domain: the concepts, their rules, and how they can be safely constructed and composed.

## Purpose

The domain exists to model the problem space explicitly.

We are not just storing data, we are representing meaning.

This implies:

- expressing invariants as part of the model
- avoiding scattered logic across upper layers
- reducing ambiguity through explicit types
- favoring compile-time safety over runtime errors

## Structure

The domain is organized around two main pillars:

- **Types** → define what values are
- **Factories** → define how values are created
- **Rules** → define what values must satisfy

### Types

Contain the abstractions that model domain concepts.

A type is not just a data container, it is a unit of meaning.

Here we define:

- what a value represents
- its properties
- how it composes with other values

### Factories

Contain the abstractions responsible for constructing domain values.

A factory is the only valid entry point for transforming external data into domain values.

Here we define:

- how values are validated
- how construction errors are handled
- how effects are encapsulated when needed

### Rules
Contain the abstractions that model domain invariants.

A rule represents a condition that a value must satisfy to be considered valid.

Here we define:

- how to express reusable validation
- how to compose multiple invariants
- how to decouple validation from construction

Rules can be used by factories or directly in domain logic.

## Conceptual Flow

The natural flow in the system is:

1. External data enters the system (API, database, files, etc.).
2. Data flows through **Factories**
3. **Rules** are applied to validate invariants
4. If valid, it becomes **Types**
5. From that point on, everything operates on valid domain values

This reduces repeated validation and increases confidence when composing logic.

## Principles

### No contextless primitives

Avoid using raw primitives to represent domain concepts.

A `string` is not an email. A `decimal` is not money.

### Controlled construction

Values are not created freely.

They always go through factories that guarantee invariants.

### Composition over inheritance

Behavior is built by combining traits, not extending rigid hierarchies.

### Explicit errors

Construction may fail, and that failure is part of the type.

Exceptions are not used for flow control.

### No hidden effects

When a value requires effects to be created, those effects are explicitly modeled.

## Relationship with the rest of the system

The domain does not know about:

- infrastructure
- frameworks
- databases
- external APIs

Other layers depend on the domain and use it as a foundation.

This ensures:

- model stability
- testability
- infrastructure changes do not affect the core

## Conceptual Examples

- A valid email
- A unique identifier
- A monetary value with clear rules
- A state within an allowed set

These are values born in the domain that the rest of the system must respect.

## Philosophy

The domain is the source of truth of the system.

Types define meaning.  
Rules define constraints.  
Factories define construction.

The clearer this contract is, the less complexity leaks into other layers.

The goal is not to model everything upfront, but to enable progressive evolution without losing consistency.

Clear types + explicit rules + controlled construction = fewer errors and faster development.