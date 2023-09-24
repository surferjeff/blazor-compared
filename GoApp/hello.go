package main

import (
	"fmt"
	"html/template"
	"log"

	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/template/html/v2"
)

type MyViews struct {
	templates map[string]*template.Template
}

func (v *MyViews) Load() error {
	v.templates = make(map[string]*template.Template)

	parsePage := func(templates ...string) error {
		name := templates[0]
		templates = append(templates, "MainLayout", "NavMenu", "_Layout")
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

	return nil
}

func main() {
	engine := html.New("./templates", ".html")
	app := fiber.New(fiber.Config{
		Views: engine,
	})

	app.Static("/", "./wwwroot")

	app.Get("/", func(c *fiber.Ctx) error {
		return c.Render("mainarticle", fiber.Map{})
	})

	log.Fatal(app.Listen(":3000"))
}
