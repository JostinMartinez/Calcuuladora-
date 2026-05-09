namespace tarea2;

// ─────────────────────────────────────────────────────────────────
//  DELEGADO — representa cualquier operación matemática binaria
// ─────────────────────────────────────────────────────────────────
delegate T OperacionMatematica<T>(T a, T b);

// ─────────────────────────────────────────────────────────────────
//  CLASE GENÉRICA — almacena y opera sobre la lista de números
// ─────────────────────────────────────────────────────────────────
class ListaNumerica<T> where T : struct, IConvertible, IComparable<T>
{
    private List<T> _numeros = new List<T>();

    public void Agregar(T numero)
    {
        _numeros.Add(numero);
        Console.WriteLine($"  ✔ Número {numero} agregado. Total en lista: {_numeros.Count}");
    }

    public void MostrarLista()
    {
        if (_numeros.Count == 0)
        {
            Console.WriteLine("  La lista está vacía.");
            return;
        }
        Console.Write("  Lista actual: [ ");
        foreach (var n in _numeros)
            Console.Write(n + " ");
        Console.WriteLine("]");
    }

    public void Limpiar()
    {
        _numeros.Clear();
        Console.WriteLine("  Lista vaciada.");
    }

    // Aplica el delegado en cascada sobre todos los elementos
    public T EjecutarOperacion(OperacionMatematica<T> operacion, string nombreOp)
    {
        if (_numeros.Count < 2)
            throw new InvalidOperationException(
                $"Se necesitan al menos 2 números para '{nombreOp}'. " +
                $"Actualmente hay {_numeros.Count} elemento(s).");

        T resultado = _numeros[0];
        for (int i = 1; i < _numeros.Count; i++)
            resultado = operacion(resultado, _numeros[i]);

        return resultado;
    }
}

// ─────────────────────────────────────────────────────────────────
//  OPERACIONES MATEMÁTICAS — métodos que encajan con el delegado
// ─────────────────────────────────────────────────────────────────
static class Operaciones
{
    public static double Sumar(double a, double b) => a + b;

    public static double Restar(double a, double b) => a - b;

    public static double Multiplicar(double a, double b) => a * b;

    public static double Dividir(double a, double b)
    {
        if (b == 0)
            throw new DivideByZeroException($"No se puede dividir {a} entre cero.");
        return a / b;
    }
}

// ─────────────────────────────────────────────────────────────────
//  PROGRAMA PRINCIPAL
// ─────────────────────────────────────────────────────────────────
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        ListaNumerica<double> lista = new ListaNumerica<double>();

        MostrarBienvenida();

        bool salir = false;

        while (!salir)
        {
            MostrarMenu();
            string opcion = Console.ReadLine()?.Trim();

            switch (opcion)
            {
                // ── 1. Agregar número ──────────────────────────
                case "1":
                    Console.Write("\n  Ingrese un número: ");
                    string entrada = Console.ReadLine();
                    try
                    {
                        double numero = double.Parse(entrada,
                            System.Globalization.CultureInfo.InvariantCulture);
                        lista.Agregar(numero);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"\n  ✘ Error de formato: '{entrada}' no es un número válido.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("\n  ✘ Error: el número es demasiado grande o pequeño.");
                    }
                    break;

                // ── 2. Ver lista ───────────────────────────────
                case "2":
                    Console.WriteLine();
                    lista.MostrarLista();
                    break;

                // ── 3. Suma ────────────────────────────────────
                case "3":
                    EjecutarYMostrar(lista, Operaciones.Sumar, "Suma");
                    break;

                // ── 4. Resta ───────────────────────────────────
                case "4":
                    EjecutarYMostrar(lista, Operaciones.Restar, "Resta");
                    break;

                // ── 5. Multiplicación ──────────────────────────
                case "5":
                    EjecutarYMostrar(lista, Operaciones.Multiplicar, "Multiplicación");
                    break;

                // ── 6. División ────────────────────────────────
                case "6":
                    EjecutarYMostrar(lista, Operaciones.Dividir, "División");
                    break;

                // ── 7. Limpiar lista ───────────────────────────
                case "7":
                    Console.WriteLine();
                    lista.Limpiar();
                    break;

                // ── 8. Salir ───────────────────────────────────
                case "8":
                    salir = true;
                    Console.WriteLine("\n  ¡Hasta luego!\n");
                    break;

                default:
                    Console.WriteLine("\n  ✘ Opción no válida. Intente de nuevo.");
                    break;
            }

            if (!salir)
            {
                Console.WriteLine("\n  Presione Enter para continuar...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    static void EjecutarYMostrar(
        ListaNumerica<double> lista,
        OperacionMatematica<double> operacion,
        string nombreOp)
    {
        Console.WriteLine();
        lista.MostrarLista();

        try
        {
            double resultado = lista.EjecutarOperacion(operacion, nombreOp);
            Console.WriteLine($"\n  ✔ Resultado de la {nombreOp}: {resultado:G}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n  ✘ Operación inválida: {ex.Message}");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"\n  ✘ División por cero: {ex.Message}");
        }
    }

    static void MostrarBienvenida()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║     CALCULADORA GENÉRICA CON DELEGADOS       ║");
        Console.WriteLine("║       Generics · Delegates · Exceptions       ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine("\n  Tipo de dato activo: double (doble precisión)\n");
    }

    static void MostrarMenu()
    {
        Console.WriteLine("┌─────────────────────────────────────┐");
        Console.WriteLine("│           MENÚ PRINCIPAL             │");
        Console.WriteLine("├─────────────────────────────────────┤");
        Console.WriteLine("│  1. Agregar número a la lista        │");
        Console.WriteLine("│  2. Ver lista actual                 │");
        Console.WriteLine("├─────────────────────────────────────┤");
        Console.WriteLine("│  3. Sumar todos los números          │");
        Console.WriteLine("│  4. Restar (secuencial)              │");
        Console.WriteLine("│  5. Multiplicar todos los números    │");
        Console.WriteLine("│  6. Dividir (secuencial)             │");
        Console.WriteLine("├─────────────────────────────────────┤");
        Console.WriteLine("│  7. Limpiar lista                    │");
        Console.WriteLine("│  8. Salir                            │");
        Console.WriteLine("└─────────────────────────────────────┘");
        Console.Write("  Seleccione una opción: ");
    }
}