Inspired by: https://mmapped.blog/posts/25-domain-types.html

# Domain

Esta carpeta representa el núcleo de la composición del dominio de un sistema.

Aquí definimos el lenguaje del dominio: los conceptos, sus reglas y cómo pueden ser construidos y compuestos de forma segura.

## Propósito

El dominio existe para modelar la realidad del problema de forma explícita.

No buscamos solo almacenar datos, sino representar significado.

Esto implica:

- expresar invariantes como parte del modelo
- evitar lógica dispersa en capas superiores
- reducir ambigüedad mediante tipos explícitos
- favorecer seguridad en tiempo de compilación sobre errores en runtime

## Estructura

El dominio se organiza en dos pilares principales:

- **Types** → definen qué son los valores
- **Factories** → definen cómo se crean
- **Rules** → definen qué deben cumplir

### Types

Contienen las abstracciones que modelan los conceptos del dominio.

Un tipo no es un contenedor de datos, sino una unidad de significado.

Aquí se define:

- qué representa un valor
- qué propiedades tiene
- cómo se puede componer con otros valores

### Factories

Contienen las abstracciones encargadas de construir valores de dominio.

Una factory es el único punto de entrada válido para transformar datos externos en valores del dominio.

Aquí se define:

- cómo validar un valor
- cómo manejar errores de construcción
- cómo encapsular efectos cuando son necesarios

### Rules

Contienen las abstracciones que modelan invariantes del dominio.

Una rule representa una condición que un valor debe cumplir para ser considerado válido.

Aquí se define:
- cómo expresar validaciones de forma reutilizable
- cómo componer múltiples invariantes
- cómo desacoplar validación de construcción

Las rules pueden ser utilizadas por factories o directamente en lógica de dominio.

## Flujo conceptual

El flujo natural dentro del sistema es:

1. Datos externos entran al sistema (API, base de datos, archivos, etc.)
2. Esos datos pasan por **Factories**
3. Se aplican **Rules** para validar invariantes
4. Si son válidos, se transforman en **Types**
5. A partir de ese punto, todo opera sobre valores de dominio válidos

Esto reduce la necesidad de validaciones repetidas y aumenta la confianza al componer lógica.

## Principios

### No primitivos sin contexto

Evitar el uso directo de tipos primitivos cuando representan conceptos del dominio.

Un `string` no es un email. Un `decimal` no es dinero.

### Construcción controlada

Los valores no se crean libremente.

Siempre pasan por factories que garantizan sus invariantes.

### Composición sobre herencia

El comportamiento se construye combinando traits, no extendiendo jerarquías rígidas.

### Errores explícitos

La construcción puede fallar y ese fallo es parte del tipo.

No usamos excepciones como control de flujo.

### Sin efectos ocultos

Cuando un valor requiere efectos para construirse, estos se modelan explícitamente.

## Relación con el resto del sistema

El dominio no conoce:

- infraestructura
- frameworks
- bases de datos
- APIs externas

Las demás capas sí conocen el dominio y lo utilizan como base.

Esto permite que:

- el modelo sea estable
- la lógica sea testeable
- los cambios de infraestructura no afecten el núcleo

## Ejemplos conceptuales

- Un correo electrónico válido
- Un identificador único
- Un monto de dinero con reglas claras
- Un estado dentro de un conjunto permitido

Todos estos son valores que nacen en el dominio y que el resto del sistema debe respetar.

## Filosofía

El dominio es la fuente de verdad del sistema.

Types definen significado.  
Rules definen restricciones.  
Factories definen construcción.

Mientras más expresivo y preciso sea, menos complejidad se filtra hacia otras capas.

El objetivo no es modelar todo desde el inicio, sino permitir evolución progresiva sin perder consistencia.

Tipos claros + construcción controlada = menos errores, más velocidad de desarrollo.