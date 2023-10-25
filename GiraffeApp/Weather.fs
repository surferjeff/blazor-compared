module Weather

open System

type Forecast =
    { Date: DateTime
      TemperatureC: int
      TemperatureF: int
      Summary: string }

let summaries =
    [| "Freezing"; "Bracing"; "Chilly"; "Cool"; "Mild"; "Warm"; "Balmy"
       "Hot"; "Sweltering"; "Scorching" |]

let makeRandomForecasts (count: int) =
    let now = DateTime.Now
    let tempC = Random.Shared.Next(-20, 55)

    List.map
        (fun day ->
            { Date = now.AddDays(day)
              TemperatureC = tempC
              TemperatureF = tempC * 9 / 5 + 32
              Summary = summaries.[Random.Shared.Next(summaries.Length)] })
        [ 1..count ]
