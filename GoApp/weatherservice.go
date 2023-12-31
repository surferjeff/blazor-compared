package main

import (
	"fmt"
	"math/rand"
	"time"
)

type Forecast struct {
	Date         string
	TemperatureC int
	TemperatureF int
	Summary      string
}

var summaries = []string{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
}

func getForecasts(startDate time.Time) []Forecast {
	forecasts := make([]Forecast, 5)
	for i := 0; i < len(forecasts); i++ {
		date := startDate.AddDate(0, 0, i)
		forecasts[i].Date = fmt.Sprintf("%d/%d/%d",
			date.Month(), date.Day(), date.Year())
		forecasts[i].TemperatureC = rand.Intn(75) - 20
		forecasts[i].TemperatureF = int(
			32.0 + float32(forecasts[i].TemperatureC)/0.5556)
		forecasts[i].Summary = summaries[rand.Intn(len(summaries))]
	}
	return forecasts
}
