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
    var golfAnalysis = new GolfAnalysis
    {
        Name = golfData.Name
    };

    try
    {
        golfAnalysis.Scores = ParseScores(golfData.Scores);

    }catch(ArgumentException e)
    {
        return new BadRequestObjectResult($"Could not parse {e.Message}. Please pass a comma separated list of integers.");
    }
   

    golfAnalysis.Average = golfAnalysis.Scores.Average();

    try
    {
        golfAnalysis.Handicap = CalculateHandicap(golfAnalysis.Scores);

    }catch(ArgumentException e)
    {
        return new BadRequestObjectResult(e.Message);
    }

   return (ActionResult)new OkObjectResult(golfAnalysis);

}

public class GolfData {
     public string Name {get; set;}
     public string Scores {get;set;}
}

public class GolfAnalysis 
{
    public string Name {get;set;}
    public List<int> Scores {get;set;}
    public double Average {get;set;}
    public int Handicap {get;set;}
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

public static int CalculateHandicap(List<int> scores)
{
    const int HandicapMin = 0;
    const int HandicapMax = 22;
    const int MinNumberOfScores = 2;
    const int MaxNumberOfScores = 10;
    const int RemoveHighAndLowThreshold = 6;

    if(scores.Count < MinNumberOfScores)
    {
        throw new ArgumentException("Too few scores to calculate handicap.");
    }
    if(scores.Count > MaxNumberOfScores)
    {
        throw new ArgumentException("Too many scores to calculate handicap.");
    }
    if(scores.Count >= RemoveHighAndLowThreshold)
    {
        scores.Sort();
        scores.Remove(scores.Count);
        scores.Remove(0);
    }

    int handicap = (int)((scores.Average() - 36) * .9) ;

    if(handicap > HandicapMax)
    {
        handicap = HandicapMax;
    }
    if(handicap < HandicapMin)
    {
        handicap = HandicapMin;
    }
    
    return handicap;
}
