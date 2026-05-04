# Domain / Rules

Esta carpeta contiene las abstracciones encargadas de modelar **invariantes del dominio**.

Una Rule representa una condición que un valor debe cumplir para ser considerado válido dentro del dominio.

A diferencia de las factories, que construyen valores, las rules se enfocan en validar y componer restricciones de forma reutilizable.

## Propósito

El objetivo de las Rules es desacoplar la validación del proceso de construcción, permitiendo:

- reutilizar invariantes en distintos contextos
- componer reglas de forma declarativa
- evitar duplicación de lógica
- mantener el dominio expresivo y consistente

Las rules pueden aplicarse a:

- value objects
- aggregate roots
- tipos primitivos refinados
- estructuras más complejas

## Tipos de Rules

Existen cuatro variantes principales, dependiendo del contexto donde se ejecutan.

### Rule

Representa una regla pura sobre un valor.

- Evalúa valores directos
- No depende de efectos ni contexto externo
- Retorna un resultado que puede fallar

Se usa cuando la validación es completamente determinista.

### RuleK

Representa una regla sobre valores en un contexto de tipo más abstracto (higher-kinded).

- Evalúa valores dentro de una estructura
- Permite validar sin salir del contexto

Se usa cuando el valor está envuelto en una estructura que queremos preservar.

### RuleM

Representa una regla sobre valores en un contexto monádico.

- Evalúa valores considerando efectos
- Permite validar dentro de contextos como IO, Eff, etc.

Se usa cuando la validación requiere acceso a contexto o efectos controlados.

### RuleT

Representa una regla en un contexto monádico transformado.

- Evalúa valores dentro de estructuras más complejas
- Permite combinar contexto + estructura

Se usa en escenarios donde hay composición de efectos y contenedores.

## Módulos y composición

Cada tipo de Rule incluye un módulo con combinadores que permiten construir reglas más complejas a partir de otras.

Entre los más importantes:

- **All** → todas las reglas deben cumplirse
- **Any** → al menos una regla debe cumplirse
- **Not** → niega una regla existente
- **Lift** → eleva una regla a un contexto más complejo

Esto permite construir validaciones de forma declarativa, sin necesidad de lógica imperativa.

## Extensions

Las extensiones agregan helpers para aplicar reglas de forma más fluida y expresiva.

No definen nuevas reglas, sino formas más ergonómicas de utilizarlas.

## Relación con Factories

Las rules no crean valores, solo validan.

Las factories pueden utilizar rules para:

- validar inputs antes de construir un tipo
- componer múltiples invariantes
- mantener la lógica de validación reutilizable

Esto permite que la construcción siga siendo clara, mientras que las reglas se mantienen desacopladas.

## Ejemplos conceptuales

- una regla que valida que un texto no esté vacío
- una regla que valida que un número sea positivo
- una regla que valida un formato específico
- una regla que depende de configuración o contexto
- una combinación de reglas que define un conjunto de invariantes

## Criterio de uso

Usa Rules cuando:

- una validación se repite en más de un lugar
- necesitas componer múltiples invariantes
- quieres separar validación de construcción
- necesitas expresar reglas como parte del dominio

Evita crear Rules innecesarias para validaciones triviales que no se reutilizan.

## Filosofía

Las Rules permiten que las invariantes del dominio sean:

- explícitas
- reutilizables
- composables

Mientras más claras sean las reglas, más confiable será el sistema.

El objetivo no es validar en todas partes, sino definir las reglas una vez y reutilizarlas donde sea necesario.