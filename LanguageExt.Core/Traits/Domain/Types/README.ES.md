# Domain / Types

Esta carpeta contiene las abstracciones que modelan los tipos del dominio.

Aquí definimos qué es un valor dentro del dominio, cuáles son sus propiedades fundamentales y cómo se comporta al ser compuesto con otros valores.

Si las factories representan **cómo se crean los valores**, los types representan **qué son esos valores**.

## Propósito

El objetivo de esta carpeta es encapsular significado.

Un tipo de dominio no es solo una estructura de datos. Es una representación explícita de una idea del dominio con reglas, forma y comportamiento definidos.

Esto nos permite:

- evitar el uso de primitivos sin contexto
- hacer explícitas las invariantes del dominio
- componer valores de forma segura
- reducir errores en runtime moviendo reglas al modelo

## Traits principales

Los traits en esta carpeta definen capacidades que un tipo puede tener. En lugar de usar herencia, componemos comportamiento a través de estos traits.

### DomainType

Es la base conceptual de todos los tipos de dominio.

Define que un valor:
- tiene una representación subyacente
- pertenece al dominio
- puede ser tratado como una unidad significativa

No define cómo se crea (eso vive en las factories), sino qué representa.

### Identifier

Representa valores cuya identidad es lo importante.

Se usa para modelar identificadores dentro del dominio.

Ejemplos conceptuales:
- identificadores de usuario
- identificadores de entidades
- claves únicas

Su principal característica es que no tienen comportamiento más allá de identificar.

### Magnitude

Representa valores que tienen una cantidad o medida.

Se usa cuando el valor expresa “cuánto” de algo existe.

Ejemplos conceptuales:
- dinero
- peso
- cantidad
- duración

Estos valores suelen tener reglas como no negatividad, límites o precisión.

### RefinedType

Representa valores que restringen un tipo base mediante reglas adicionales.

Es una forma de tomar un valor primitivo o simple y refinarlo con invariantes del dominio.

Ejemplos conceptuales:
- string no vacío
- texto con longitud máxima
- número dentro de un rango
- formato específico

Permite expresar validaciones como parte del tipo en lugar de lógica dispersa.

### DerivedType

Representa valores que se derivan de otros valores.

No son independientes, sino que existen como resultado de una transformación o composición.

Ejemplos conceptuales:
- un total calculado
- un valor normalizado
- una proyección de otro tipo

### Maintainer

Representa conjuntos cerrados (con identidad) de valores de un dominio.

Señala tipos de dominio que tienen un conjunto limitado de valores, funcionando como una union discriminada, pero a nivel de valores, no tipo.

Ejemplos conceptuales:
- estados posibles
- categorías
- opciones válidas

### Espacios algebraicos

Estos traits modelan estructuras matemáticas que permiten composición segura y expresiva.

> Muchos de estos traits sirven de base para los traits de dominio

#### DiscreteSpace

Representa conjuntos abiertos (con identidad) de valores válidos dentro del dominio.

Ejemplos conceptuales:
- SKU de un producto
- Slug de un libro

#### VectorSpace

Representa valores que pueden combinarse y escalarse.

Ejemplos conceptuales:
- cantidades acumulables
- valores que pueden sumarse y multiplicarse por un factor

#### AffineSpace

Representa valores donde tiene sentido medir distancias entre puntos.

Se usa cuando distinguimos entre “puntos” y “desplazamientos”.

Ejemplos conceptuales:
- posiciones en el tiempo
- ubicaciones
- estados con transición

## Extensions

Algunos tipos incluyen extensiones que agregan operaciones derivadas.

Estas extensiones no definen la identidad del tipo, pero sí mejoran su ergonomía y capacidad de composición.

## Relación con Factories

Los types definen qué es válido.

Las factories definen cómo se llega a algo válido.

Un type no debería preocuparse por validar inputs externos directamente. Esa responsabilidad vive en las factories.

Esto permite mantener el modelo limpio y enfocado en significado, mientras que la construcción se mantiene controlada y explícita.

## Ejemplos conceptuales

### Email

Un tipo que representa un correo válido.

No es simplemente texto: es texto que cumple reglas específicas del dominio.

### UserId

Un identificador único dentro del sistema.

No importa su valor interno, sino su capacidad de identificar de forma inequívoca.

### Money

Un valor que representa una cantidad monetaria.

Tiene reglas propias como no negatividad, precisión y operaciones válidas.

### Slug

Un texto normalizado que representa una forma segura de identificar recursos en URLs.

## Criterio de uso

Usa estos traits para expresar intención, no por completar una jerarquía.

Un tipo puede implementar múltiples traits si eso refleja correctamente su naturaleza.

Prefiere composición sobre herencia.

Prefiere modelar restricciones en el tipo antes que validarlas repetidamente fuera de él.

## Filosofía

Esta carpeta es el corazón del dominio.

Aquí definimos el lenguaje con el que el sistema piensa.

Mientras más precisos sean los tipos, menos ambigüedad existe en el resto del sistema.

Tipos expresivos llevan a código más claro, menos validaciones repetidas y mayor seguridad en tiempo de compilación.