namespace BACKEND_CNC_IIoT_Predictive_Maintenance_Tool_.Models;

public class CNC
{
    public Guid Id { get; }
    public string Model { get; set; }
    public Bearing[] Bearings;
    public bool IsOn {get; set;}

    public CNC()
    {
        Id = Guid.NewGuid();
        
        IsOn = false;

        int count = Random.Shared.Next(2) == 0 ? 2 : 4;
        Bearings = new Bearing[count];

        for (int i = 0; i < count; i++)
        {
            Bearings[i] = new Bearing();
        }
    }

    new public string ToString()
    {
        return $"CNC ID: {Id}, Model: {Model}, IsOn: {IsOn}, Bearings: {Bearings.Length}";
    }
}