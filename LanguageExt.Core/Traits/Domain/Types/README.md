# Domain / Types

This folder contains the abstractions that model domain types.

Here we define what a value is within the domain, its fundamental properties, and how it behaves when composed with other values.

If factories represent **how values are created**, types represent **what those values are**.

## Purpose

The goal of this folder is to encapsulate meaning.

A domain type is not just a data structure. It is an explicit representation of a domain concept with defined rules, shape, and behavior.

This allows us to:

- avoid contextless primitives
- make domain invariants explicit
- compose values safely
- reduce runtime errors by moving rules into the model

## Main Traits

Traits in this folder define capabilities that a type can have. Instead of inheritance, behavior is composed through traits.

### DomainType

The conceptual base for all domain types.

It defines that a value:
- has an underlying representation
- belongs to the domain
- can be treated as a meaningful unit

It does not define how values are created (that belongs to factories), only what they represent.

### Identifier

Represents values whose purpose is identity.

Used to model identifiers in the domain.

Conceptual examples:
- user identifiers
- entity identifiers
- unique keys

Their main characteristic is that they exist to identify, not to carry behavior.

### Magnitude

Represents values that express a quantity or measure.

Used when the value answers “how much”.

Conceptual examples:
- money
- weight
- quantity
- duration

These values typically include rules like non-negativity, bounds, or precision.

### RefinedType

Represents values that constrain a base type with additional rules.

It allows refining a primitive or simple value with domain invariants.

Conceptual examples:
- non-empty string
- bounded text
- number within a range
- structured formats

It moves validation into the type itself instead of scattering it across the system.

### DerivedType

Represents values derived from other values.

They are not independent but exist as a result of transformation or composition.

Conceptual examples:
- calculated totals
- normalized values
- projections
- 
### DomainSet

Represents open sets of valid values with identity defined by the domain.

Used when the domain defines a set with unique, generated values.

Conceptual examples:
- A product SKU
- A book Slug

### Maintainer

Represents closed sets of valid values with identity defined by the domain.

Used when the domain defines a set of related unique values. It works like a discrimnated union, but on a value level.

Conceptual examples:
- possible states
- categories
- valid options


### Algebraic spaces

These traits model mathematical structures that enable safe and expressive composition.

#### VectorSpace

Represents values that can be combined and scaled.

Conceptual examples:
- accumulable quantities
- values that support addition and scaling

#### AffineSpace

Represents values where distances between points are meaningful.

Used when distinguishing between points and displacements.

Conceptual examples:
- time positions
- locations
- state transitions

#### ComplexSpace

Represents more complex compositions of values.

Used when a type is not a single value but a structured combination of others.

## Extensions

Some types include extensions that provide derived operations.

These do not define the identity of the type but improve ergonomics and composability.

## Relationship with Factories

Types define what is valid.

Factories define how something becomes valid.

A type should not be responsible for validating external input directly. That responsibility belongs to factories.

This keeps the model focused on meaning, while construction remains controlled and explicit.

## Conceptual Examples

### Email

A type representing a valid email.

Not just text, but text that satisfies domain rules.

### UserId

A unique identifier within the system.

Its value matters less than its ability to uniquely identify.

### Money

A value representing a monetary amount.

Includes rules like non-negativity, precision, and valid operations.

### Slug

A normalized text used to safely identify resources in URLs.

## Usage Guidelines

Use these traits to express intent, not to complete a hierarchy.

A type may implement multiple traits if it accurately reflects its nature.

Prefer composition over inheritance.

Prefer modeling constraints in the type rather than validating them repeatedly elsewhere.

## Philosophy

This folder is the heart of the domain.

It defines the language the system uses to think.

The more precise the types are, the less ambiguity exists across the system.

Expressive types lead to clearer code, fewer repeated validations, and stronger compile-time guarantees.