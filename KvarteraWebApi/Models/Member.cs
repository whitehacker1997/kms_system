namespace KvarteraWebApi.Models;

public class Member
{
    public string Name { get; set; } = null!;
}

public class MonthlyOutcome : Member
{
    public List<Outcome> Outcomes { get; set; } = new();
}


public class Outcome
{ 
    public decimal Amount { get; set; }
    public List<Member> Except { get; set; } = new();
}


public class Rest : Member
{ 
    public decimal Amount { get; set; }
}

public class MonthlyRent : Member
{ 
    public List<Rest> Rests { get; set; } = new();  
}