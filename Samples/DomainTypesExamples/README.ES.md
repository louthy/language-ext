## Descripción del ejemplo

Este ejemplo modela un pequeño dominio de **seguimiento de jornada laboral**.

El objetivo es demostrar cómo los tipos de dominio, factories, reglas, traits algebraicos y capacidades con efectos pueden trabajar juntos para construir un modelo de dominio seguro y componible.

El ejemplo incluye usuarios, jornadas de trabajo, bloques de trabajo, duraciones, fechas y generación de datos en tiempo de ejecución. Muestra cómo valores externos o primitivos se transforman en valores de dominio con significado, cómo se aplican invariantes durante la construcción y cómo luego estos valores pueden componerse de forma segura.

---

### Qué modela el ejemplo

El flujo representado es el siguiente:

* Se crea un `User` a partir de un nombre
* El usuario recibe un identificador generado
* La fecha de creación se obtiene desde el reloj del sistema
* Se generan jornadas de trabajo
* Cada jornada contiene bloques de trabajo como trabajo efectivo, almuerzo y descanso
* El sistema calcula tiempo total, tiempo efectivo y posibles excesos

Esto demuestra tanto construcción pura como construcción con efectos (tiempo, random, secuencias).

---

### Traits utilizados

#### `DomainType`

Se utiliza para representar valores de dominio con significado y una representación subyacente.

Ejemplos:

* `User`
* `WorkBlock`
* `WorkBlockKind`
* `NonFutureDate`
* `WorkDayHistory`

`User` expone su representación como `(int Id, string Name, DateOnly CreatedAt, Seq<WorkDay> WorkDays)`, manteniendo el modelo tipado internamente.

---

#### `DomainTypeFactory`

Se utiliza cuando un tipo puede construirse de forma pura desde su representación.

Ejemplos:

* `WorkDay`
* `HourValue`

`WorkDay` valida que:

* exista al menos un bloque
* la duración total no supere las 12 horas

---

#### `DomainFactoryM`

Se utiliza cuando la construcción requiere efectos o contexto.

Ejemplos:

* `User.Factory<RT>`
* `NonFutureDate.Factory<RT>`

`User.Factory<RT>` depende de capacidades como tiempo, random o secuencias para construir el usuario correctamente.

---

#### `RefinedTypeFactory`

Se utiliza para refinar un tipo base válido con reglas adicionales.

Ejemplo:

* `WorkDuration`

`WorkDuration` refina `HourOnly` asegurando que esté dentro de un rango válido.

---

#### `Magnitude`

Representa valores medibles.

Ejemplo:

* `WorkDuration`

Permite comparar, sumar, restar y ordenar duraciones sin usar primitivos directamente.

---

#### `VectorSpace`

Permite operaciones algebraicas como suma, resta y escalado.

Ejemplo:

* `HourValue`

Esto permite trabajar con valores numéricos del dominio manteniendo el tipado fuerte.

---

#### `AffineSpace`

Distingue entre posición y desplazamiento.

Ejemplo:

* `WorkBlock`

Un `WorkBlock` tiene una posición en el tiempo (inicio) y una duración.
Permite operaciones como:

* inicio + duración → nuevo bloque
* bloque - bloque → duración

---

#### `DomainSet`

Representa conjuntos cerrados de valores válidos.

Ejemplo:

* `WorkBlockKind`

Define tipos de bloque como trabajo efectivo, almuerzo o descanso.

---

#### Reglas reutilizables (`RuleK`)

Permiten definir validaciones reutilizables.

Ejemplos:

* `NonEmptyWorkBlocks`
* `DailyBlocksWithinTwelveHours`

Se utilizan en `WorkDay` para validar listas de bloques antes de permitir su construcción.

---

### Capacidades utilizadas

El ejemplo incluye módulos effectful:

* `Time`
* `Random`
* `Sequences`

Estas capacidades se exponen mediante traits como:

* `HasTime<RT>`
* `HasRandom<RT>`
* `HasSequences<RT>`

Esto mantiene los efectos explícitos y evita dependencias ocultas.

---

### Qué demuestra este ejemplo

Este ejemplo muestra cómo:

* evitar primitivos sin contexto
* construir valores de forma segura mediante factories
* modelar errores explícitamente (`Fin`, `FinT`)
* combinar construcción pura y con efectos
* reutilizar reglas de validación
* modelar valores medibles con comportamiento algebraico
* diferenciar posición y desplazamiento (`AffineSpace`)
* evolucionar el dominio sin dispersar lógica

---

## En resumen

El ejemplo demuestra cómo construir un dominio pequeño pero realista utilizando traits componibles en lugar de jerarquías rígidas, manteniendo seguridad, claridad y expresividad en todo el modelo.
