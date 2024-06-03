using System; // standardowe klasy i typy danych 
using System.IO; // funkcje do obsługi operacji wejścia/wyjścia, czytanie i zapisywanie danych do plików.
using System.Runtime.Serialization.Formatters.Binary; // klasy do serializacji i deserializacji danych w formacie binarnym, BinaryFormatter-obiekt

// Interfejs mediatora
public interface IMediator // element jest dostępny z poziomu innych klas, zestaw metod i właściwości, które muszą być zaimplementowane przez dowolną klasę, IMediator interfejs, dwie metody
{
    void RealizujOperacje(IOperacjaFinansowa operacja); //  realizacja operacji finansowych
    void ZapiszDoPliku(string operacja); 
}

// Klasa mediatora Bank
public class Bank : IMediator // realizacja operacji mediatora, obiekt mediatora
{
    public void RealizujOperacje(IOperacjaFinansowa operacja) // część implementacji interfejsu, realizuje operacje finansowe i zapisuje je do pliku
    {
        operacja.Realizuj(); // parametr metody, część interfejsu IOperacjaFinansowa,implementacja konkretnej operacji finansowej
        ZapiszDoPliku(operacja.GetType().Name); // Zapisz operację do pliku, operacja wywołuje GetType
    }

    public void ZapiszDoPliku(string operacja)
    {
        string filePath = "operacje.txt"; // ścieżka do pliku, zapis operacji finansowej
        try // blok, obsługa wyjątków
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Append)) // otwiera plik w trybie dołączania, nowe dane będą dodawane na koniec pliku a nie nadpisując go
            {
                BinaryFormatter formatter = new BinaryFormatter(); // serializacja danych, op. finansowa jest serializowana do pliku za pomocą tego formatera
                formatter.Serialize(fs, operacja);
            }
        } // catch- blok,obsługuje różne rodzaje wyjątków, błąd-program przejdzie do odpowiedniego bloku catch, wypisze odpowiedni komunikat o błędzie na konsoli i kontynuuje wykonywanie programu
        catch (UnauthorizedAccessException ex) // wystąpi, gdy brak uprawnień do zapisu do pliku
        {
            Console.WriteLine("Brak uprawnień do zapisu do pliku: " + ex.Message);
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine("Katalog docelowy nie istnieje: " + ex.Message); //  wystąpi, gdy katalog w którym znajduje się plik nie istnieje
        }
        catch (Exception ex)
        {
            Console.WriteLine("Wystąpił nieoczekiwany błąd podczas zapisu do pliku: " + ex.Message); // inne nieprzewidziane błędy, ogólny komunikat o błędzie
        }


    }
}

// Abstrakcyjna klasa operacji finansowej
[Serializable] //  klasy mogą być przekształcone w strumienie bajtów
public abstract class IOperacjaFinansowa //  nie może być bezpośrednio instancjonowana, tylko dziedziczona
{
    protected IMediator mediator; // przechowanie referencji do obiektu mediatora, tylko dla klasy Imediator 

    public IOperacjaFinansowa(IMediator mediator)
    {
        this.mediator = mediator; //  konstruktor klasy, używany do rozróżnienia parametrów metody i polami klasy o tej samej nazwie, wywoływania konstruktorów z innych konstruktorów w klasie dostep innych metod i właściwości klasy
    }
    public abstract void Realizuj(); // musi być zaimplementowana przez klasy dziedziczące z IOperacjaFinansowa
}

// Konkretna operacja Wplata
[Serializable]
public class Wplata : IOperacjaFinansowa //  klasa,dziedziczy po abstrakcyjnej klasie IOperacjaFinansowa
{
    public Wplata(IMediator mediator) : base(mediator) { } //  konstruktor klasy Wplata, który wywołuje konstruktor klasy bazowej IOperacjaFinansowa, używany do inicjalizacji obiektu, 

    public override void Realizuj() // dostarcza konkretnej implementacji dla operacji wpłaty
    {
        Console.WriteLine("Wykonano operację wpłaty."); // wypisuje komunikat na konsoli informujący o wykonaniu operacji wpłaty
    }
}

// Konkretna operacja Wyplata
[Serializable]
public class Wyplata : IOperacjaFinansowa
{
    public Wyplata(IMediator mediator) : base(mediator) { } //  konstruktor base wywołanie IOperacjaFinansowa, przekazując mu obiekt mediatora, bo iof ma wlasny konstruktor, base - odwołanie się do konstruktora, metod, pól

    public override void Realizuj() // override- zmiana zachowania metody zdefiniowanej w klasie bazowej, pozwala na dodanie lub zmianę funkcjonalności, 
    {
        Console.WriteLine("Wykonano operację wypłaty.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        IMediator bank = new Bank(); //  nową instancja klasy Bank, która implementuje interfejs IMediator

        // Tworzenie operacji finansowych
        IOperacjaFinansowa wplata = new Wplata(bank);  
        IOperacjaFinansowa wyplata = new Wyplata(bank); // operacje finansowe są związane z obiektem mediatora bank

        // Wykonywanie operacji za pomocą mediatora
        bank.RealizujOperacje(wplata); // wywołanie metody Realizuj Operacje na obiekcie mediatora bank
        bank.RealizujOperacje(wyplata); //  wykonywanie operacji, realizując je poprzez odpowiednie wywołanie metod operacji finansowych, które następnie będą zapisywane do pliku
    }
}
