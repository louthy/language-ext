# Domain / Factories

Esta carpeta contiene las abstracciones encargadas de construir valores de dominio.

La idea principal es separar dos responsabilidades que suelen mezclarse:

- qué es un tipo de dominio
- cómo se crea un valor válido de ese tipo.

Un tipo de dominio no debería ser simplemente un envoltorio sobre un `string`, `Guid`, `int` o `decimal`. También debe proteger sus invariantes. Por eso la construcción de valores vive en traits especializados: las factories.

Una factory representa el punto de entrada oficial para transformar una representación externa o primitiva en un valor de dominio válido.

## Propósito

Las factories existen para centralizar la creación de valores de dominio y evitar que las reglas de validación queden dispersas por la aplicación.

Esto permite que el dominio sea más expresivo, más seguro y más fácil de componer.

Por ejemplo, en vez de tratar un correo electrónico, un identificador, un monto de dinero o un nombre de usuario como valores primitivos sueltos, podemos modelarlos como tipos de dominio con reglas propias.

La factory se encarga de decidir si una representación puede convertirse en ese tipo de dominio.

## Traits principales

### DomainFactory

Representa una factory pura.

Se usa cuando la creación de un valor de dominio solo depende del valor de entrada y de reglas deterministas del dominio.

Por ejemplo:

- validar que un correo tenga formato correcto
- validar que un nombre no esté vacío
- validar que una cantidad sea positiva
- validar que un identificador tenga una forma esperada

Una factory pura no necesita acceder a configuración, repositorios, servicios externos, reloj del sistema, IO ni estado mutable.

Su resultado expresa explícitamente si la construcción fue exitosa o fallida.

### DomainFactoryM

Representa una factory effectful, es decir, una factory que construye valores dentro de un contexto monádico.

Se usa cuando crear el valor requiere algún tipo de contexto adicional o efecto controlado.

Por ejemplo:

- generar un identificador usando una fuente de aleatoriedad
- crear un valor usando la hora actual
- validar contra configuración
- consultar un repositorio
- depender de un ambiente de ejecución
- componer validaciones que viven dentro de `IO`, `Eff` u otro contexto monádico

La diferencia importante es que `DomainFactoryM` no rompe la pureza del modelo: encapsula el efecto en el tipo, en vez de esconderlo dentro de la implementación.

## Safe vs Unsafe

La carpeta separa dos estilos de construcción:

### Safe

La construcción safe devuelve un resultado que puede fallar.

Este es el camino recomendado para la lógica de dominio y aplicación, porque obliga a tratar el error como parte explícita del flujo.

Debe usarse cuando el input viene desde fuera del sistema o puede no cumplir las reglas del dominio.

Ejemplos típicos:

- datos recibidos desde una API;
- valores leídos desde base de datos;
- input de usuario;
- datos importados desde archivos;
- parámetros de configuración.

### Unsafe

La construcción unsafe intenta construir el valor directamente y falla lanzando una excepción si el valor no cumple las reglas.

Este camino existe por ergonomía, interoperabilidad o escenarios donde ya existe una garantía externa fuerte.

Debe usarse con cuidado.

Ejemplos razonables:

- tests;
- seeds;
- fixtures;
- migraciones controladas;
- código interno donde el valor ya fue validado previamente;
- integración con APIs que esperan errores por excepción.

La regla general es: safe por defecto, unsafe solo cuando la precondición está garantizada.

## Relación con DomainType

`DomainType` describe qué representa un valor de dominio.

`DomainFactory` describe cómo se construye.

Esta separación permite que un tipo pueda mantener una representación clara sin acoplarse a una única estrategia de construcción.

Por ejemplo, un tipo puede representar un correo electrónico como texto, pero su factory define qué textos son aceptables como correos válidos.

## Ejemplos conceptuales

### Email

Un `Email` puede estar respaldado por texto.

Su factory valida que el texto tenga una forma aceptable antes de permitir crear el valor.

Un texto vacío, mal formado o incompleto produce un fallo explícito.

### UserId

Un `UserId` puede estar respaldado por un `Guid`.

Su factory puede validar que el identificador no sea vacío.

Si el identificador viene desde una API, usamos construcción safe.

Si el identificador se genera internamente y sabemos que es válido, podría usarse construcción unsafe en un punto controlado.

### Money

Un `Money` puede estar respaldado por un decimal.

Su factory puede validar que el monto no sea negativo y que respete las reglas propias del dominio financiero.

Esto evita que el resto del sistema tenga que repetir esas validaciones cada vez que recibe un monto.

### Slug

Un `Slug` puede estar respaldado por texto.

Su factory puede validar que solo contenga caracteres permitidos, que no tenga espacios y que esté normalizado.

Así, cualquier función que reciba un `Slug` puede asumir que ya es válido.

## Criterio de uso

Usa `DomainFactory` cuando la construcción sea pura.

Usa `DomainFactoryM` cuando la construcción necesite un contexto monádico o un efecto controlado.

Usa construcción safe como opción principal.

Usa construcción unsafe solo en bordes controlados, pruebas o puntos donde la validez ya esté garantizada.

## Filosofía

Esta carpeta existe para que el dominio sea el lugar donde nacen los valores válidos.

No buscamos crear wrappers por crear wrappers.

Buscamos que cada tipo de dominio tenga una puerta de entrada clara, composable y segura.

Mientras más reglas podamos mover al borde de construcción del valor, menos errores tendremos circulando en runtime y más confianza tendremos al componer sobre el dominio.