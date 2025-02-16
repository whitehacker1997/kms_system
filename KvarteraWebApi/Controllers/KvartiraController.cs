#region 16.02.2025 CURL

/*
 curl --location 'http://localhost:5065/Kvartira/QarzniTola' \
--header 'Content-Type: application/json' \
--data '[
    {
        "Name": "Aziz",
        "Outcomes": [
            {
                "Amount": 220000,
                "Except": [
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            },
            {
                "Amount": 190000,
                "Except": []
            }
        ]
    },
    {
        "Name": "Akbar",
        "Outcomes": [
            {
                "Amount": 279000,
                "Except": [
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            },            
            {
                "Amount": 64000
            }
        ]
    },
    {
        "Name": "Muhammadali",
        "Outcomes": [
            {
                "Amount": 175000,
                "Except": [
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            },            
            {
                "Amount": 240000
            }
        ]
    },
    {
        "Name": "Elyor",
        "Outcomes": [
            {
                "Amount": 701000,
                "Except": [
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            }
        ]
    },
    {
        "Name": "Ahadulla",
        "Outcomes": [
            {
                "Amount": 512000,
                "Except": [
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            },
            {
                "Amount": 148000,
                "Except": [
                    {
                        "Name": "Elyor"
                    },
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            },            
            {
                "Amount": 237000
            }
        ]
    },
    {
        "Name": "Hojimurod",
        "Outcomes": [
            {
                "Amount": 96000,
                "Except": [
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            },
            {
                "Amount": 245000,
                "Except": [
                    {
                        "Name": "Elyor"
                    },
                    {
                        "Name": "Muhammadqodir"
                    }
                ]
            }
        ]
    },
    {
        "Name": "Muhammadqodir",
        "Outcomes": [
            {
                "Amount": 162000
            }
        ]
    }
]'
 */

#endregion

using KvarteraWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KvarteraWebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class KvartiraController : ControllerBase
{
    private readonly KvarteraService _kvarteraService;

    public KvartiraController(KvarteraService kvarteraService)
    {
        _kvarteraService = kvarteraService;
    }
    [HttpPost]
    public ActionResult<List<MonthlyRent>> QarzniTola([FromBody] List<MonthlyOutcome> monthlyOutcome)
    {
        return Ok(_kvarteraService.Calculate(monthlyOutcome));
    }




}

public class KvarteraService
{
    public List<MonthlyRent> Calculate(List<MonthlyOutcome> monthlyOutcome)
    {
        int memberCount = monthlyOutcome.Count();

        var result = monthlyOutcome
            .Select(mo => new MonthlyRent
            {
                Name = mo.Name,
            }).ToList();

        foreach (var memberOutcome in monthlyOutcome)
        {
            foreach (var outcome in memberOutcome.Outcomes)
            {
                int availMemCount = monthlyOutcome.Count() - outcome.Except.Count();

                decimal initialAmount = outcome.Amount / availMemCount;

                var otherMembers = result.Where(r => r.Name != memberOutcome.Name &&
                                                              !outcome.Except.Any(e => e.Name == r.Name));


                foreach (var otherMember in otherMembers)
                {
                    var alreadyExistsMember = otherMember.Rests.FirstOrDefault(r => r.Name == otherMember.Name);

                    if (alreadyExistsMember != null)
                        alreadyExistsMember.Amount += initialAmount;
                    else
                        otherMember.Rests.Add(new Rest
                        {
                            Amount = initialAmount,
                            Name = memberOutcome.Name
                        });
                }
            }
        }


        foreach (var member in result)
        {
            member.Rests = member.Rests.Where(r => r.Amount != 0)
                .GroupBy(r => r.Name)
                .Select(gr => new Rest
                {
                    Name = gr.Key,
                    Amount = Math.Round(gr.Sum(g => g.Amount))
                }).ToList();
        }

        foreach (var member in result)
        {
            foreach (var otherMember in member.Rests)
            {
                var res = result
                    .FirstOrDefault(r => r.Name == otherMember.Name);

                var memberRestFromOther = res!.Rests
                    .FirstOrDefault(r => r.Name == member.Name);

                if (memberRestFromOther != null)
                {
                    if (memberRestFromOther.Amount > otherMember.Amount)
                    {
                        memberRestFromOther.Amount -= otherMember.Amount;
                        otherMember.Amount = 0;
                    }
                    else
                    {
                        otherMember.Amount -= memberRestFromOther.Amount;
                        memberRestFromOther.Amount = 0;
                    }
                }
            }
        }

        foreach (var member in result)
        {
            member.Rests = member.Rests.Where(r => r.Amount != 0)
                .GroupBy(r => r.Name)
                .Select(gr => new Rest
                {
                    Name = gr.Key,
                    Amount = Math.Round(gr.Sum(g => g.Amount))
                }).ToList();
        }

        return result;
    }
}

