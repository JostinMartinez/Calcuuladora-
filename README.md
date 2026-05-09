# Tarea 2 — Calculadora Genérica con Delegados y Control de Excepciones

> **Asignatura:** C# Intermedio  
> **Instituto Tecnológico de las Américas (ITLA)**  
> **Lenguaje:** C# (.NET 9+)

---

## Descripción General

Esta aplicación de consola permite al usuario **gestionar una lista de números y realizar operaciones matemáticas** sobre ella (suma, resta, multiplicación y división). Está construida sobre tres pilares fundamentales del lenguaje C#:

- **Genéricos:** la lista de números puede trabajar con cualquier tipo numérico (`int`, `double`, `float`, `decimal`) sin duplicar código.
- **Delegados:** cada operación matemática se pasa como argumento a través de un delegado, desacoplando la lógica de la operación de la lógica de iteración.
- **Control de excepciones:** todos los errores previsibles (entrada inválida, lista vacía, división por cero) se capturan y comunican al usuario de forma clara.

El programa interactúa con el usuario mediante un menú de consola numerado y muestra mensajes descriptivos ante cualquier error.

---

## Estructura del Proyecto

```
tarea2/
├── Program.cs       ← Toda la lógica de la aplicación
└── README.md        ← Esta documentación
```

Todas las clases, el delegado y el punto de entrada `Main` se encuentran en un único archivo `Program.cs` bajo el namespace `tarea2`.

---

## Cómo Usar el Programa

1. Clona el repositorio y entra a la carpeta:
   ```bash
   git clone https://github.com/tu-usuario/tarea2.git
   cd tarea2
   ```
2. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```
3. Sigue el menú interactivo:
   - Agrega al menos **2 números** a la lista (opción 1).
   - Elige la operación matemática que deseas aplicar (opciones 3–6).
   - Usa la opción 7 para limpiar la lista y empezar de nuevo.

---

## Explicación de los Componentes

### Delegado `OperacionMatematica<T>`

```csharp
delegate T OperacionMatematica<T>(T a, T b);
```

Define la **firma común** que deben tener todas las operaciones matemáticas: reciben dos valores del tipo `T` y devuelven un valor del mismo tipo. Esto permite pasar cualquier operación como argumento sin importar cuál sea concretamente, aplicando el principio de **programación basada en comportamiento**.

---

### Clase Genérica `ListaNumerica<T>`

```csharp
class ListaNumerica<T> where T : struct, IConvertible, IComparable<T>
```

Clase central de la aplicación. La restricción `where T : struct, IConvertible` garantiza que solo se puedan usar tipos de valor numéricos.

| Método | Descripción |
|---|---|
| `Agregar(T numero)` | Agrega un número al final de la lista interna y confirma la acción al usuario. |
| `MostrarLista()` | Imprime todos los elementos actuales de la lista en consola. Si está vacía, lo informa. |
| `Limpiar()` | Elimina todos los elementos de la lista. |
| `EjecutarOperacion(OperacionMatematica<T>, string)` | Recorre la lista aplicando el delegado recibido de forma acumulada (resultado = op(resultado, siguiente)). Lanza `InvalidOperationException` si hay menos de 2 elementos. |

---

### Clase Estática `Operaciones`

Contiene los cuatro métodos que se pasan como delegados. Todos tienen la misma firma `(double, double) → double`.

| Método | Comportamiento |
|---|---|
| `Sumar(a, b)` | Retorna `a + b`. |
| `Restar(a, b)` | Retorna `a - b`. |
| `Multiplicar(a, b)` | Retorna `a * b`. |
| `Dividir(a, b)` | Retorna `a / b`. Lanza `DivideByZeroException` si `b == 0`. |

---

### Clase `Program` — Métodos de Interfaz

| Método | Descripción |
|---|---|
| `Main(string[] args)` | Punto de entrada. Contiene el bucle principal del menú y el `switch` que despacha cada opción. |
| `EjecutarYMostrar(lista, operacion, nombreOp)` | Muestra la lista, llama a `EjecutarOperacion` con el delegado correspondiente y captura las excepciones de la operación. |
| `MostrarBienvenida()` | Imprime el encabezado decorativo al iniciar el programa. |
| `MostrarMenu()` | Imprime las opciones disponibles antes de cada turno del usuario. |

---

## Uso de Genéricos

La clase `ListaNumerica<T>` es genérica: el tipo `T` se define en el momento de crear la instancia.

```csharp
// En Main — usando double para mayor precisión
ListaNumerica<double> lista = new ListaNumerica<double>();
```

Si en el futuro se necesitara trabajar con enteros, bastaría con cambiar a:

```csharp
ListaNumerica<int> lista = new ListaNumerica<int>();
```

Sin modificar ningún otro código. El método `EjecutarOperacion` también es genérico por herencia, lo que evita duplicar la lógica de iteración para cada tipo numérico.

---

## Uso de Delegados

El delegado `OperacionMatematica<double>` permite tratar las operaciones matemáticas como **valores que se pueden pasar como argumentos**:

```csharp
// El método Operaciones.Sumar cumple con la firma del delegado
EjecutarYMostrar(lista, Operaciones.Sumar, "Suma");

// Internamente, EjecutarOperacion lo usa así:
resultado = operacion(resultado, _numeros[i]);
```

Esto evita tener cuatro métodos de iteración distintos (uno por operación). En cambio, existe un único método de iteración que recibe el comportamiento desde afuera, siguiendo el patrón de **inyección de comportamiento**.

---

## Excepciones Manejadas

| Excepción | Cuándo ocurre | Dónde se captura |
|---|---|---|
| `FormatException` | El usuario escribe texto en lugar de un número (ej. `"abc"`). | Bloque `catch` en `Main`, opción 1. |
| `OverflowException` | El número ingresado excede el rango del tipo `double`. | Bloque `catch` en `Main`, opción 1. |
| `InvalidOperationException` | Se intenta operar con una lista que tiene menos de 2 elementos. | Método `EjecutarYMostrar`. |
| `DivideByZeroException` | El segundo número (o resultado intermedio) en una división es cero. | Método `EjecutarYMostrar`. |

Todos los errores muestran un mensaje descriptivo con el símbolo `✘` y el programa **continúa ejecutándose** sin cerrarse abruptamente.

---

## Ejemplo de Ejecución

```
╔══════════════════════════════════════════════╗
║     CALCULADORA GENÉRICA CON DELEGADOS       ║
║       Generics · Delegates · Exceptions       ║
╚══════════════════════════════════════════════╝

  Tipo de dato activo: double (doble precisión)

┌─────────────────────────────────────┐
│           MENÚ PRINCIPAL             │
├─────────────────────────────────────┤
│  1. Agregar número a la lista        │
│  2. Ver lista actual                 │
├─────────────────────────────────────┤
│  3. Sumar todos los números          │
│  4. Restar (secuencial)              │
│  5. Multiplicar todos los números    │
│  6. Dividir (secuencial)             │
├─────────────────────────────────────┤
│  7. Limpiar lista                    │
│  8. Salir                            │
└─────────────────────────────────────┘
  Seleccione una opción: 1

  Ingrese un número: 10
  ✔ Número 10 agregado. Total en lista: 1

  Seleccione una opción: 1

  Ingrese un número: hola
  ✘ Error de formato: 'hola' no es un número válido.

  Seleccione una opción: 1

  Ingrese un número: 5
  ✔ Número 5 agregado. Total en lista: 2

  Seleccione una opción: 3

  Lista actual: [ 10 5 ]
  ✔ Resultado de la Suma: 15

  Seleccione una opción: 6

  Lista actual: [ 10 5 ]
  ✔ Resultado de la División: 2
```

---

## Tecnologías

- **Lenguaje:** C# 10+
- **Framework:** .NET 9 o superior
- **Paradigma:** Programación orientada a objetos + genéricos + delegados
