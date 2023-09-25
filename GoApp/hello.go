package main

import (
	"errors"
	"fmt"
	"html/template"
	"io"
	"log"
	"strings"
	"time"

	"github.com/gofiber/fiber/v2"
)

type MyViews struct {
	templates map[string]*template.Template
}

func reverse(numbers []string) []string {
	for i := 0; i < len(numbers)/2; i++ {
		j := len(numbers) - i - 1
		numbers[i], numbers[j] = numbers[j], numbers[i]
	}
	return numbers
}

func (v *MyViews) Load() error {
	v.templates = make(map[string]*template.Template)

	parsePage := func(templates ...string) error {
		name := templates[0]
		templates = append(templates, "MainLayout", "NavMenu", "_Layout")
		templates = reverse(templates)
		for i := 0; i < len(templates); i++ {
			templates[i] = fmt.Sprintf("templates/%s.html", templates[i])
		}
		tmpl, error := template.ParseFiles(templates...)
		if tmpl != nil {
			v.templates[name] = tmpl
			return nil
		} else {
			return error
		}
	}

	error := parsePage("Index", "SurveyPrompt")
	if error != nil {
		return error
	}

	error = parsePage("Counter")
	if error != nil {
		return error
	}

	error = parsePage("FetchData")
	if error != nil {
		return error
	}

	tmpl, error := template.ParseFiles("templates/Forecasts.html")
	if error != nil {
		return error
	} else {
		v.templates["Forecasts"] = tmpl
	}

	return nil
}

func (v *MyViews) Render(w io.Writer, templateName string,
	data interface{}, _ignored ...string) error {
	tmpls := strings.Split(templateName, " ")
	if len(tmpls) == 1 {
		tmpl := v.templates[templateName]
		if tmpl == nil {
			return errors.New(fmt.Sprintf("No template named %s", templateName))
		}
		return tmpl.Execute(w, data)
	} else if len(tmpls) == 2 {
		tmpl := v.templates[tmpls[0]]
		if tmpl == nil {
			return errors.New(fmt.Sprintf("No template named %s", tmpls[0]))
		}
		return tmpl.ExecuteTemplate(w, tmpls[1], data)
	} else {
		return errors.New(fmt.Sprintf("Bad template name '%s'", templateName))
	}
}

func isBoosted(c *fiber.Ctx) bool {
	headers := c.GetReqHeaders()
	return headers["Hx-Boosted"] == "true"
}

func main() {
	app := fiber.New(fiber.Config{
		Views: new(MyViews),
	})

	app.Static("/", "./wwwroot")

	app.Get("/", func(c *fiber.Ctx) error {
		return c.Render("Index", fiber.Map{
			"HxBoosted": isBoosted(c),
		})
	})
	app.Get("/counter", func(c *fiber.Ctx) error {
		return c.Render("Counter", fiber.Map{
			"HxBoosted":    isBoosted(c),
			"CurrentCount": 0,
			"NextCount":    1,
		})
	})
	app.Get("/increment", func(c *fiber.Ctx) error {
		count := c.QueryInt("count", 0)
		return c.Render("Counter main-article", fiber.Map{
			"CurrentCount": count,
			"NextCount":    count + 1,
		})
	})
	app.Get("/fetchdata", func(c *fiber.Ctx) error {
		return c.Render("FetchData", fiber.Map{
			"HxBoosted": isBoosted(c),
		})
	})
	app.Post("/forecasts", func(c *fiber.Ctx) error {
		return c.Render("Forecasts", getForecasts(time.Now()))
	})

	log.Fatal(app.Listen(":3000"))
}
