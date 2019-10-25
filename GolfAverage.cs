#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(GolfData golfData, ILogger log)
{   
    if(golfData == null)
    {
        return new BadRequestObjectResult("Please pass an instance of GolfData.");
    }
    if(golfData.Name == null)
    {
        return new BadRequestObjectResult("Please pass a Name.");
    }
    if(golfData.Scores == null)
    {
        return new BadRequestObjectResult("Please pass a comma separated list of Scores.");
    }

    List<int> scores = null;
    try
    {
        scores = ParseScores(golfData.Scores);

    }catch(ArgumentException e)
    {
        return new BadRequestObjectResult($"Could not parse {e.Message}. Please pass a comma separated list of integers.");
    }

    var average = scores.Average();

   return (ActionResult)new OkObjectResult($"{golfData.Name}\t{golfData.Scores}\tAverage:{average}");

}

public class GolfData {
     public string Name {get; set;}
     public string Scores {get;set;}
}

public static List<int> ParseScores(string Scores)
{   
   var splitScores = Scores.Split(",");
    var scores = new List<int>();
    foreach(var splitScore in splitScores)
    {
        if(int.TryParse(splitScore, out int score))
        {
            scores.Add(score);
        }
        else
        {
            throw new ArgumentException(splitScore);
        }
    }
    return scores;
}

