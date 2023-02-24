using System.Runtime.CompilerServices;

List<PersonModel> people = new List<PersonModel>
{
    new PersonModel {FirstName="Héctor",LastName="Sandoval",Email="hector@hola.com"},
    new PersonModel {FirstName="Heck",LastName="Alvarado",Email="claudia@hola.com"},
    new PersonModel {FirstName="Manuel",LastName="Landázuri",Email="manuel@hola.com"}
};

List<CarModel> cars = new List<CarModel>
{
    new CarModel {Manufacturer="Toyota",Model="Corolla"},
    new CarModel {Manufacturer="Nissan",Model="Sentra"},
    new CarModel {Manufacturer="Ford",Model="Darn"}
};

DataAccess<PersonModel> peopleData = new DataAccess<PersonModel>();
DataAccess<CarModel> carData = new DataAccess<CarModel>();
peopleData.BadEntryFound += PeopleData_BadEntryFound;
carData.BadEntryFound += CarData_BadEntryFound;

void CarData_BadEntryFound(object? sender, CarModel e)
{
    Console.WriteLine($"Bad Entry Found for {e.Manufacturer} {e.Model}");
}

void PeopleData_BadEntryFound(object? sender, PersonModel e)
{
    Console.WriteLine($"Bad Entry Found for {e.FirstName} {e.LastName}");
}

peopleData.SaveToCSV(people,@"c:\cnf\people.csv");
carData.SaveToCSV(cars,@"c:\cnf\cars.csv");

public class DataAccess<T> where T : new()
{
    public void SaveToCSV(List<T> items,string filePath) 
    {
        List<string> rows =new List<string>();
        T entry = new T();
        var cols=entry.GetType().GetProperties();
        string row = "";
        foreach (var col in cols)
        {
            row += $",{col.Name}";
        }
        row = row.Substring(1);
        rows.Add(row);
        foreach (var item in items)
        {
            row = "";
            bool badWordDetected=false;
            foreach (var col in cols)
            {
                string val = col.GetValue(item, null).ToString();
                badWordDetected = BadWordDetector(val);
                if (badWordDetected)
                {
                    BadEntryFound?.Invoke(this, item);
                    break;
                }
                row += $",{val}";
            } 
            if (!badWordDetected)
            {
                row = row.Substring(1);
                rows.Add(row);
            }
        }

        File.WriteAllLines(filePath, rows);
    }

    public bool BadWordDetector (string stringToText)
    {
        bool output=false;
        string lowerCaseTest = stringToText.ToLower();
        if (lowerCaseTest == "darn" || lowerCaseTest == "heck")
        {
            output = true;
        }
        return output;
    }

    public event EventHandler<T> BadEntryFound;


}


